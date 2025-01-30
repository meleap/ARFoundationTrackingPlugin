#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Hado.ARFoundation
{
    public enum MovingStatus
    {
        None,
        Detecting,
        Moving
    }

    public class WorldAnchorManager : MonoBehaviour
    {
        // 移動時間
        private const float MoveTime = 1.5f;

        private readonly List<float> _noiseCheckSamples = new();

        private CancellationTokenSource? _cancellationTokenSource;

        /// フレーム間の移動距離がこの値より大きい場合はノイズとして捨てる
        [NonSerialized] public float MovingNoiseThreshold = 0.05f;

        /// MovingNoiseThresholdのチェックを何回ぶん行うか
        [NonSerialized] public int NoiseCheckSampleCount = 2;

        private ReactiveProperty<MovingStatus> IsMoving { get; } = new(MovingStatus.None);
        
        //ARFoundationTrackingPlugin -> WorldAnchorInitializerSampleのためにpublicのものを用意する
        //TypeCだと不要
        public IReadOnlyReactiveProperty<MovingStatus> IsMovingProperty => IsMoving;

        private readonly ReactiveProperty<(Vector3, Quaternion)> _positionAndRotation =
            new((Vector3.zero, Quaternion.identity));

        public IReadOnlyReactiveProperty<(Vector3, Quaternion)> PositionAndRotation => _positionAndRotation;


        private Transform _transform = null!;

        private void Awake()
        {
            _transform = transform;
        }

        private void Start()
        {
            // WindowsEYEの場合は、ここまでにposition, rotationが更新されている
            _positionAndRotation.Value = (_transform.position, _transform.rotation);

            ARSessionManager.Instance.arTrackedImageEventManager.TrackedImagesChangedObservable
                .Where(_ => IsMoving.Value == MovingStatus.None) // 補正中は流さない
                .Do(t => PositionManager.Instance.LastDetectedAnchorName = t.referenceImage.name)
                .Select(t => ARSessionManager.Instance.arTrackedImageEventManager.GetReferenceAnchor(t.referenceImage
                    .name))
                .Where(x => x != null) // なぜnullがあるかはARTrackedImageEventManagerを参照
                .Select(x => x.transform.position)
                .Where(_ => ARSession.state >= ARSessionState.SessionInitializing)
                .Buffer(NoiseCheckSampleCount + 1)
                .Subscribe(positions =>
                {
                    // フレーム間の移動距離が大きすぎる場合はノイズとして捨てる
                    IsMoving.Value = MovingStatus.Detecting;
                    _noiseCheckSamples.Clear();
                    if (IsNoiseData(positions))
                    {
                        IsMoving.Value = MovingStatus.None;
                        return;
                    }

                    var moveEndRotation =
                        ARSessionManager.Instance.arTrackedImageEventManager.GetReferenceAnchor(PositionManager.Instance
                            .LastDetectedAnchorName).transform.rotation;

                    _cancellationTokenSource?.Cancel();
                    _cancellationTokenSource?.Dispose();
                    _cancellationTokenSource = new CancellationTokenSource();
                    MoveToX(_transform.position, _transform.rotation, positions[2], moveEndRotation,
                        _cancellationTokenSource.Token).Forget();
                }).AddTo(this);
        }

        public void CancelMove()
        {
            _cancellationTokenSource?.Cancel();
            IsMoving.Value = MovingStatus.None;
        }

        private bool IsNoiseData(IList<Vector3> positions)
        {
            for (var i = 0; i < NoiseCheckSampleCount; i++)
            {
                _noiseCheckSamples.Add(Vector3.Distance(positions[i], positions[i + 1]));
            }

            return _noiseCheckSamples.Any(x => x > MovingNoiseThreshold);
        }

        public IDisposable RegisterIntervalTracking(CancellationToken cancellationToken,
            int imageTrackingIntervalMils = 3000)
        {
            return ARSessionManager.Instance.arTrackedImageEventManager.TrackedImagesChangedObservable
                .Where(_ => IsMoving.Value == MovingStatus.Moving) // 補正が始まったら発火
                .Subscribe(_ => UniTask.Void(async () =>
                    {
                        try
                        {
                            ARSessionManager.Instance.EnabledImageTracking = false;
                            await UniTask.WaitWhile(() => IsMoving.Value == MovingStatus.Moving,
                                cancellationToken: cancellationToken);
                            await UniTask.Delay(TimeSpan.FromMilliseconds(imageTrackingIntervalMils),
                                cancellationToken: cancellationToken);
                            ARSessionManager.Instance.EnabledImageTracking = true;
                        }
                        finally
                        {
                            // arカメラの状態にあわせる
                            ARSessionManager.Instance.EnabledImageTracking = ARSessionManager.Instance.arCamera.enabled;
                        }
                    }
                ));
        }

        private async UniTask MoveToX(Vector3 startPos, Quaternion startRot, Vector3 endPos, Quaternion endRot,
            CancellationToken cancellationToken)
        {
            IsMoving.Value = MovingStatus.Moving;

            var x = 0f;

            try
            {
                while (IsMoving.Value == MovingStatus.Moving)
                {
                    x += Time.deltaTime / MoveTime;

                    var lerpPoint = (float)(1 - Math.Pow(1 - x, 5));

                    if (lerpPoint > 1)
                    {
                        IsMoving.Value = MovingStatus.None;
                        lerpPoint = 1f;
                    }

                    var targetPos = Vector3.Lerp(startPos, endPos, lerpPoint);
                    var targetRot = Quaternion.Lerp(startRot, endRot, lerpPoint);

                    _transform.SetPositionAndRotation(targetPos, targetRot);
                    _positionAndRotation.Value = (targetPos, targetRot);

                    await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, cancellationToken);
                }
            }
            finally
            {
                IsMoving.Value = MovingStatus.None;
            }
        }

        private void OnDestroy()
        {
            _positionAndRotation.Dispose();
        }

#if UNITY_EDITOR
        private void Update()
        {
            // デバッグ用に、Editorだったらインスペクタでposition, rotationが変更されたことを検知する
            var t = (_transform.position, _transform.rotation);
            if (_positionAndRotation.Value != t) _positionAndRotation.Value = t;
        }
#endif
    }
}
