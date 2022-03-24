using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
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
        // 等速移動中の速度
        private const float LERP_SPEED = 2f;
    
        // ここまで移動したら速度落とす(0 ~ 1f)
        private const float SPEED_DOWN_POINT = 0.8f;
    
        // 減速値 = _lerpSpeed / SPEED_DOWN_VALUE
        private const float SPEED_DOWN_VALUE = 50f;
        
        /// フレーム間の移動距離がこの値より大きい場合はノイズとして捨てる
        [NonSerialized] public float MovingNoiseThreshold = 0.05f;

        /// MovingNoiseThresholdのチェックを何回ぶん行うか
        [NonSerialized] public int NoiseCheckSampleCount = 2;

        private readonly List<float> _noiseCheckSamples = new List<float>();

        public ReactiveProperty<MovingStatus> IsMoving { get; } = new ReactiveProperty<MovingStatus>(MovingStatus.None);

        private void Awake()
        {
            PositionManager.Instance.WorldAnchor = gameObject;
        }

        private void Start()
        {
            var moveStartTransform = gameObject.transform;
            
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

                    var moveEndRotation =
                        ARSessionManager.Instance.arTrackedImageEventManager.GetReferenceAnchor(PositionManager.Instance
                            .LastDetectedAnchorName).transform.rotation;
                    
                    IsMoving.Value = MovingStatus.Moving;
                    
                    MoveToX(moveStartTransform.position, moveStartTransform.rotation, positions[2], moveEndRotation);
                    
                }).AddTo(this);
        }

        private bool IsNoiseData(IList<Vector3> positions)
        {
            for (var i = 0; i < NoiseCheckSampleCount; i++)
            {
                _noiseCheckSamples.Add(Vector3.Distance(positions[i], positions[i + 1]));
                Debug.Log($"Noise check[{i}]: {Vector3.Distance(positions[i], positions[i + 1]):F6}");
            }
            
            Debug.Log($"Check: {_noiseCheckSamples.Any(x => x > MovingNoiseThreshold)}");

            return _noiseCheckSamples.Any(x => x > MovingNoiseThreshold);
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
        
        private async void MoveToX(Vector3 startPos, Quaternion startRot, Vector3 endPos, Quaternion endRot)
        {
            var lerpPoint = 0f;
            var lerpSpeed = LERP_SPEED; 
        
            while (IsMoving.Value == MovingStatus.Moving)
            {

                var x = Time.deltaTime;
                if (lerpPoint <= SPEED_DOWN_POINT)
                {
                    // ここまでは等速で動く
                    lerpPoint += lerpSpeed * x;
                }
                else
                {
                    // ここからは減速させる(0.1よりは下がらない）
                    if(lerpSpeed > 0.1)
                        lerpSpeed -= lerpSpeed / SPEED_DOWN_VALUE;
                    lerpPoint += lerpSpeed * x;
                }
                //Debug.Log($"speed={speed}:leapPoint={lerpPoint}");
            
                if (lerpPoint > 1)
                {
                    IsMoving.Value = MovingStatus.None;
                    lerpPoint = 1f;
                }
            
                gameObject.transform.position = Vector3.Lerp(startPos, endPos, lerpPoint);
                gameObject.transform.rotation = Quaternion.Lerp(startRot, endRot,lerpPoint);
            
                await UniTask.WaitForEndOfFrame();
            }
        }
    }
}