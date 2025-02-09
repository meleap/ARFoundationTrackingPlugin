#nullable enable
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Hado.ARFoundation
{
    public class WorldAnchorManager : MonoBehaviour
    {
        public TimeSpan MovingTime { get; set; } = TimeSpan.FromSeconds(1.5f);

        private readonly ReactiveProperty<bool> _isMoving = new(false);

        private readonly ReactiveProperty<bool> _calibrated = new(false);

        /// カメラ起動して初めてマーカーを認識してWorldAnchorを移動させたかどうか
        /// CancelMoveしたらfalseに戻ります
        public IReadOnlyReactiveProperty<bool> Calibrated => _calibrated;

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
                .Where(_ => !_isMoving.Value) // 補正中は流さない
                .Subscribe(end =>
                {
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource.Dispose();
                    _cancellationTokenSource = new CancellationTokenSource();
                    var start = _positionAndRotation.Value;
                    MoveAsync(start, end, _cancellationTokenSource.Token).Forget();
                }).AddTo(this);
        }

        public void ForceCalibrate()
        {
            _calibrated.Value = true;
        }

        private async UniTask MoveAsync((Vector3, Quaternion) start, (Vector3, Quaternion) end,
            CancellationToken cancellationToken)
        {
            _isMoving.Value = true;
            try
            {
                if (!_calibrated.Value)
                {
                    _positionAndRotation.Value = (end.Item1, end.Item2); // 初めてトラッキングしたときは即座に移動させる
                    // MoveTime の間移動したことにして、ImageTrackingの頻度を変えないようにします
                    await UniTask.Delay(MovingTime, cancellationToken: cancellationToken);
                }
                else if (Vector3.Distance(end.Item1, start.Item1) < 0.05f &&
                         Quaternion.Angle(start.Item2, end.Item2) < 1.5f)
                {
                    // トラッキングが安定している場合はキャリブレーションが不要と判断して移動させないようにします
                    // 物理的なカメラの位置が固定のときに小さな移動を繰り返すと揺れが目立ってしまうため、移動を抑制します
                    // 例えば角度が1度ずれると、8m先では0.14m程度ずれます
                    // MoveTime の間移動したことにして、ImageTrackingの頻度を変えないようにします
                    await UniTask.Delay(MovingTime, cancellationToken: cancellationToken);
                }
                else
                {
                    await MoveCoreAsync(start, end, cancellationToken);
                }

                _calibrated.Value = true;
            }
            finally
            {
                _isMoving.Value = false;
            }
        }

        private async UniTask MoveCoreAsync((Vector3, Quaternion) start, (Vector3, Quaternion) end,
            CancellationToken cancellationToken)
        {
            var t = 0f; // 0~1 正規化した時間
            while (!cancellationToken.IsCancellationRequested)
            {
                t += Time.deltaTime / (float)MovingTime.TotalSeconds;
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
            _isMoving.Value = false;
            _calibrated.Value = false;
        }

        public IDisposable RegisterIntervalTracking(CancellationToken cancellationToken, TimeSpan interval)
        {
            var compositeDisposable = new CompositeDisposable();
            cancellationToken.Register(() => compositeDisposable.Dispose());
            // WorldAnchorの移動中はトラッキングを無効にする
            _isMoving
                .SkipLatestValueOnSubscribe()
                .Where(isMoving => isMoving)
                .Subscribe(_ => _arSessionManager.EnabledImageTracking = false)
                .AddTo(compositeDisposable);
            // WorldAnchorの移動が終わってしばらくしたらトラッキングを元に戻す
            _isMoving
                .SkipLatestValueOnSubscribe()
                .Where(isMoving => !isMoving)
                .Delay(interval)
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
