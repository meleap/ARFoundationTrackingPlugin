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

        /// フレーム間の移動距離がこの値より大きい場合はノイズとして捨てる
        [NonSerialized] public float MovingNoiseThreshold = 0.05f;

        /// MovingNoiseThresholdのチェックを何回ぶん行うか
        [NonSerialized] public int NoiseCheckSampleCount = 2;

        private readonly ReactiveProperty<MovingStatus> _isMoving = new(MovingStatus.None);

        //ARFoundationTrackingPlugin -> WorldAnchorInitializerSampleのためにpublicのものを用意する
        //TypeCだと不要
        public IReadOnlyReactiveProperty<MovingStatus> IsMovingProperty => _isMoving;

        private readonly ReactiveProperty<(Vector3, Quaternion)> _positionAndRotation =
            new((Vector3.zero, Quaternion.identity));

        public IReadOnlyReactiveProperty<(Vector3, Quaternion)> PositionAndRotation => _positionAndRotation;

        private CancellationTokenSource _cancellationTokenSource = new();

        private Transform _transform = null!;
        private ARSessionManager _arSessionManager = null!;
        private ARTrackedImageEventManager _arTrackedImageEventManager = null!;

        private void Awake()
        {
            _transform = transform;
            _arSessionManager = ARSessionManager.Instance;
            _arTrackedImageEventManager = _arSessionManager.arTrackedImageEventManager;
        }

        private void Start()
        {
            // WindowsEYEの場合は、ここまでにposition, rotationが更新されている
            _transform.GetPositionAndRotation(out var position, out var rotation);
            _positionAndRotation.Value = (position, rotation);

            _arTrackedImageEventManager.TrackedImagesChangedObservable
                .Where(_ => ARSession.state >= ARSessionState.SessionInitializing)
                .Where(_ => _isMoving.Value == MovingStatus.None) // 補正中は流さない
                .Select(t => _arTrackedImageEventManager.GetReferenceAnchor(t.referenceImage.name))
                .Where(x => x != null) // なぜnullがあるかはARTrackedImageEventManagerを参照
                .Select(x =>
                {
                    x.transform.GetPositionAndRotation(out var pos, out var rot);
                    return (pos, rot);
                })
                .Buffer(NoiseCheckSampleCount + 1)
                .Where(l => !IsNoiseData(l))
                .Subscribe(positionAndRotations =>
                {
                    var last = positionAndRotations.Last();
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource.Dispose();
                    _cancellationTokenSource = new CancellationTokenSource();
                    _transform.GetPositionAndRotation(out var pos, out var rot);
                    MoveAsync(pos, rot, last.pos, last.rot, _cancellationTokenSource.Token).Forget();
                }).AddTo(this);
        }

        private async UniTask MoveAsync(Vector3 startPos, Quaternion startRot, Vector3 endPos, Quaternion endRot,
            CancellationToken cancellationToken)
        {
            try
            {
                _isMoving.Value = MovingStatus.Moving;
                await MoveCoreAsync(startPos, startRot, endPos, endRot, cancellationToken);
            }
            finally
            {
                _isMoving.Value = MovingStatus.None;
            }
        }

        private async UniTask MoveCoreAsync(Vector3 startPos, Quaternion startRot, Vector3 endPos, Quaternion endRot,
            CancellationToken cancellationToken)
        {
            var x = 0f;
            while (_isMoving.Value == MovingStatus.Moving)
            {
                x += Time.deltaTime / MoveTime;

                var lerpPoint = (float)(1 - Math.Pow(1 - x, 5));
                if (lerpPoint > 1) lerpPoint = 1f;

                var targetPos = Vector3.Lerp(startPos, endPos, lerpPoint);
                var targetRot = Quaternion.Lerp(startRot, endRot, lerpPoint);

                _transform.SetPositionAndRotation(targetPos, targetRot);
                _positionAndRotation.Value = (targetPos, targetRot);
                if (lerpPoint >= 1) break;
                await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, cancellationToken);
            }
        }

        public void CancelMove()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
            _isMoving.Value = MovingStatus.None;
        }

        // フレーム間の移動距離が大きすぎる場合はノイズとして判定する
        private bool IsNoiseData(IList<(Vector3, Quaternion)> positionAndRotations)
        {
            var threshold = MovingNoiseThreshold * MovingNoiseThreshold;
            for (var i = 0; i < positionAndRotations.Count; i++)
            {
                var d = Vector3.SqrMagnitude(positionAndRotations[i].Item1 - positionAndRotations[i + 1].Item1);
                if (d > threshold) return true;
            }

            return false;
        }

        public IDisposable RegisterIntervalTracking(CancellationToken cancellationToken,
            int imageTrackingIntervalMils = 3000)
        {
            return _arTrackedImageEventManager.TrackedImagesChangedObservable
                .Where(_ => _isMoving.Value == MovingStatus.Moving) // 補正が始まったら発火
                .Subscribe(_ => UniTask.Void(async () =>
                    {
                        try
                        {
                            _arSessionManager.EnabledImageTracking = false;
                            await UniTask.WaitWhile(() => _isMoving.Value == MovingStatus.Moving,
                                cancellationToken: cancellationToken);
                            await UniTask.Delay(TimeSpan.FromMilliseconds(imageTrackingIntervalMils),
                                cancellationToken: cancellationToken);
                            _arSessionManager.EnabledImageTracking = true;
                        }
                        finally
                        {
                            // arカメラの状態にあわせる
                            _arSessionManager.EnabledImageTracking = _arSessionManager.arCamera.enabled;
                        }
                    }
                ));
        }

        private void OnDestroy()
        {
            _positionAndRotation.Dispose();
            _isMoving.Dispose();
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

#if UNITY_EDITOR
        private void Update()
        {
            // デバッグ用に、Editorだったらインスペクタでposition, rotationが変更されたことを検知する
            _transform.GetPositionAndRotation(out var pos, out var rot);
            _positionAndRotation.Value = (pos, rot);
        }
#endif
    }
}
