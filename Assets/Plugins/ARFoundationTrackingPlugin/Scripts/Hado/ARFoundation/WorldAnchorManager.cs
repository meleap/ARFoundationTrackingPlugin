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

        private bool _isTrackedOnce = false;

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
            _positionAndRotation
                .Subscribe(x => _transform.SetPositionAndRotation(x.Item1, x.Item2))
                .AddTo(this);

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
                .Select(l => l.Last()) // 最新のデータを取得
                .Subscribe(end =>
                {
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource.Dispose();
                    _cancellationTokenSource = new CancellationTokenSource();
                    var start = _positionAndRotation.Value;
                    MoveAsync(start, end, _cancellationTokenSource.Token).Forget();
                }).AddTo(this);
        }

        private async UniTask MoveAsync((Vector3, Quaternion) start, (Vector3, Quaternion) end,
            CancellationToken cancellationToken)
        {
            Debug.Log($"WorldAnchorManager MoveAsync: diff {Vector3.Distance(end.Item1, start.Item1):F3}, {Quaternion.Angle(start.Item2, end.Item2):F3}");
            _isMoving.Value = MovingStatus.Moving;
            try
            {
                if (!_isTrackedOnce)
                {
                    Debug.Log("WorldAnchorManager MoveFirst");
                    _positionAndRotation.Value = (end.Item1, end.Item2); // 初めてトラッキングしたときは即座に移動させる
                    // MoveTime の間移動したことにして、ImageTrackingの頻度を変えないようにします
                    await UniTask.Delay(TimeSpan.FromSeconds(MoveTime), cancellationToken: cancellationToken);
                }
                else if (Vector3.Distance(end.Item1, start.Item1) < 0.05f &&
                         Quaternion.Angle(start.Item2, end.Item2) < 1.5f)
                {
                    // トラッキングが安定している場合はキャリブレーションが不要と判断して移動させないようにします
                    // 物理的なカメラの位置が固定のときに小さな移動を繰り返すと揺れが目立ってしまうため、移動を抑制します
                    // 例えば角度が1度ずれると、8m先では0.14m程度ずれます
                    Debug.Log("WorldAnchorManager MoveSkip");
                    // MoveTime の間移動したことにして、ImageTrackingの頻度を変えないようにします
                    await UniTask.Delay(TimeSpan.FromSeconds(MoveTime), cancellationToken: cancellationToken);
                }
                else
                {
                    Debug.Log("WorldAnchorManager MoveCore");
                    await MoveCoreAsync(start, end, cancellationToken);
                }

                _isTrackedOnce = true;
            }
            finally
            {
                _isMoving.Value = MovingStatus.None;
            }
        }

        private async UniTask MoveCoreAsync((Vector3, Quaternion) start, (Vector3, Quaternion) end,
            CancellationToken cancellationToken)
        {
            var t = 0f; // 0~1 正規化した時間
            while (!cancellationToken.IsCancellationRequested)
            {
                t += Time.deltaTime / MoveTime;
                var lerpPoint = Mathf.Clamp01(1 - Mathf.Pow(1 - t, 5)); // easeOutQuint
                var pos = Vector3.Lerp(start.Item1, end.Item1, lerpPoint);
                var rot = Quaternion.Lerp(start.Item2, end.Item2, lerpPoint);
                _positionAndRotation.Value = (pos, rot);
                if (t >= 1f) break;
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
            for (var i = 0; i < positionAndRotations.Count - 1; i++)
            {
                var d = Vector3.SqrMagnitude(positionAndRotations[i].Item1 - positionAndRotations[i + 1].Item1);
                if (d > threshold) return true;
            }

            return false;
        }

        public IDisposable RegisterIntervalTracking(CancellationToken cancellationToken,
            int imageTrackingIntervalMils = 3000)
        {
            var compositeDisposable = new CompositeDisposable();
            cancellationToken.Register(() => compositeDisposable.Dispose());
            // WorldAnchorの移動中はトラッキングを無効にする
            _isMoving
                .SkipLatestValueOnSubscribe()
                .Where(s => s == MovingStatus.Moving)
                .Subscribe(_ => _arSessionManager.EnabledImageTracking = false)
                .AddTo(compositeDisposable);
            // WorldAnchorの移動が終わってしばらくしたらトラッキングを元に戻す
            _isMoving
                .SkipLatestValueOnSubscribe()
                .Where(s => s != MovingStatus.Moving)
                .Delay(TimeSpan.FromMilliseconds(imageTrackingIntervalMils))
                .Subscribe(_ =>
                    _arSessionManager.EnabledImageTracking = _arSessionManager.arCamera.enabled) // ARカメラの状態にあわせる
                .AddTo(compositeDisposable);
            return compositeDisposable;
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
