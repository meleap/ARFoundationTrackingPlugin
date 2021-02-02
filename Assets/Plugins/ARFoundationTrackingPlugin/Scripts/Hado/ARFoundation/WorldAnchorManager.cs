using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

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
        /// <summary>
        /// フレーム間の移動距離がこの値より大きい場合はノイズとして捨てる
        /// </summary>
        [NonSerialized] public float MovingNoiseThreshold = 0.05f;

        /// <summary>
        /// MovingNoiseThresholdのチェックを何回ぶん行うか
        /// </summary>
        [NonSerialized] public int NoiseCheckSampleCount = 2;

        /// <summary>
        /// 予定位置からの距離がこの値になったら移動を停止する
        /// </summary>
        [NonSerialized] public float MovingEndThreshold = 0.01f;

        /// <summary>
        /// SmoothDumpのSmoothTime, 1fなら1秒かける、というわけでもない（ちょっと長い）, 1f前後で調整推奨
        /// </summary>
        [NonSerialized] public float SmoothTime = 1f;

        private Vector3 _moveEndPosition;
        private Quaternion _moveEndRotation;

        private List<float> _noiseCheckSamples = new List<float>();

        public ReactiveProperty<MovingStatus> IsMoving = new ReactiveProperty<MovingStatus>(MovingStatus.None);

        private void Awake()
        {
            PositionManager.Instance.WorldAnchor = gameObject;
        }

        private void Start()
        {
            ARSessionManager.Instance.arTrackedImageEventManager.OnTrackedImagesChangedObservable
                .Where(_ => IsMoving.Value == MovingStatus.None) // 補正中は流さない
                .Do(t => PositionManager.Instance.LastDetectedAnchorName = t.referenceImage.name)
                .Do(t => Debug.Log($"{t.referenceImage.name} detected"))
                .Select(t =>
                    ARSessionManager.Instance.arTrackedImageEventManager.GetReferenceAnchor(t.referenceImage.name)
                        .transform.position)
                .Where(_ => ARSession.state >= ARSessionState.SessionTracking)
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
                    };

                    _moveEndPosition = positions[2];
                    _moveEndRotation =
                        ARSessionManager.Instance.arTrackedImageEventManager.GetReferenceAnchor(PositionManager.Instance
                            .LastDetectedAnchorName).transform.rotation;
                    
                    IsMoving.Value = MovingStatus.Moving;
                    
                    LeanTween.move(gameObject, _moveEndPosition, SmoothTime).setEaseOutQuint().setOnComplete(() => IsMoving.Value = MovingStatus.None );
                    LeanTween.rotate(gameObject, _moveEndRotation.eulerAngles, SmoothTime).setEaseOutQuint();
                    
                }).AddTo(this);
        }

        private bool IsNoiseData(IList<Vector3> positions)
        {
            for (var i = 0; i < NoiseCheckSampleCount; i++)
            {
                _noiseCheckSamples.Add(Vector3.Distance(positions[i], positions[i + 1]));
                Debug.Log($"Noise check[{i}]: {Vector3.Distance(positions[i], positions[i + 1]):F6}");
            }
            
            Debug.Log($"Check: {_noiseCheckSamples.Any(x => x > MovingEndThreshold)}");

            return _noiseCheckSamples.Any(x => x > MovingEndThreshold);
        }

        public IDisposable RegisterIntervalTracking(CancellationTokenSource cancellationTokenSource,
            int imageTrackingIntervalMils = 3000)
        {
            Debug.Log("RegisterIntervalTracking");
            return ARSessionManager.Instance.arTrackedImageEventManager.OnTrackedImagesChangedObservable
                .Where(_ => IsMoving.Value == MovingStatus.Moving) // 補正が始まったら発火
                .Subscribe(async _ =>
                    {
                        ARSessionManager.Instance.EnabledImageTracking = false;

                        await WaitForMoveEnd();

                        await UniTask.Delay(TimeSpan.FromMilliseconds(imageTrackingIntervalMils));

                        if (cancellationTokenSource.Token.IsCancellationRequested) return;

                        ARSessionManager.Instance.EnabledImageTracking = true;
                    }
                );
        }

        private async UniTask WaitForMoveEnd()
        {
            while (IsMoving.Value == MovingStatus.Moving)
            {
                await UniTask.NextFrame();
            }
        }
    }
}