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
    public class WorldAnchorManager : MonoBehaviour
    {
        /// <summary>
        /// プルつき防止のために、マーカー位置のズレがMovingThreshold以下の場合はWorldAnchorを動かさない
        /// </summary>
        [NonSerialized] public float MovingStartThreshold = 0.05f;

        /// <summary>
        /// フレーム間の移動距離がこの値より大きい場合はノイズとして捨てる
        /// </summary>
        [NonSerialized] public float MovingNoiseThreshold = 0.05f;
        
        /// <summary>
        /// 予定位置からの距離がこの値になったら移動を停止する
        /// </summary>
        [NonSerialized]
        public float MovingEndThreshold = 0.01f;

        /// <summary>
        /// SmoothDumpのSmoothTime, 1fなら1秒かける、というわけでもない（ちょっと長い）, 0.2f前後で調整推奨
        /// </summary>
        [NonSerialized]
        public float SmoothTime = 0.2f;
        
        private Transform _transform;
        private Vector3 _moveEndPosition;
        private Quaternion _moveEndRotation;
        
        private bool _isMoving;
        private Vector3 _velocity = Vector3.zero;

        private void Awake()
        {
            PositionManager.Instance.WorldAnchor = gameObject;
            _transform = transform; //Transformは変数に入れたほうがパフォーマンスがいいらしい
        }

        private void Start()
        {
            ARSessionManager.Instance.arTrackedImageEventManager.OnTrackedImagesChangedObservable
                .Where(_ => !_isMoving) // 補正中は流さない
                .Do(t => PositionManager.Instance.LastDetectedAnchorName = t.referenceImage.name)
                .Do(t => Debug.Log($"{t.referenceImage.name} detected"))
                .Select(t =>  ARSessionManager.Instance.arTrackedImageEventManager.GetReferenceAnchor(t.referenceImage.name).transform.position)
                .Where(_ => ARSession.state >= ARSessionState.SessionTracking)
                .Buffer(3)
                .Subscribe(positions =>
                {
                    // フレーム間の移動距離が大きすぎる場合はノイズとして捨てる
                    if (IsNoiseData(positions)) return;
                    
                    _moveEndPosition = positions[2];
                    _moveEndRotation =
                        ARSessionManager.Instance.arTrackedImageEventManager.GetReferenceAnchor(PositionManager.Instance
                            .LastDetectedAnchorName).transform.rotation;

                    _isMoving = true;
                }).AddTo(this);
            
            // Transformのズレを補正
            Observable.EveryUpdate()
                .Where(_ => _isMoving)
                .Subscribe(_ =>
                {
                    _transform.position = Vector3.SmoothDamp(_transform.position, _moveEndPosition, ref _velocity, SmoothTime);

                    // まだMovingEndThresholdより距離があるので継続
                    if (Vector3.Distance(_transform.position, _moveEndPosition) > MovingEndThreshold) return;
                    
                    Debug.Log($"Moving finished");
                    _isMoving = false;
                    _velocity = Vector3.zero;
                    
                    // rotationはさほどずれることはない前提でなめさないで一気に入れる
                    _transform.SetPositionAndRotation(_transform.position, _moveEndRotation);
                    
                }).AddTo(this);
        }

        private bool IsNoiseData(IList<Vector3> positions)
        {
            var dist1 = Vector3.Distance(positions[0], positions[1]);
            var dist2 = Vector3.Distance(positions[1], positions[2]);
            Debug.Log($"MovingNoiseThreshold: {Math.Abs(dist1 - dist2):F6}");
            return Math.Abs(dist1 - dist2) > MovingNoiseThreshold;
        }
        
        public IDisposable RegisterIntervalTracking(CancellationTokenSource cancellationTokenSource, int imageTrackingIntervalMils = 3000)
        {
            Debug.Log("RegisterIntervalTracking");
            return ARSessionManager.Instance.arTrackedImageEventManager.OnTrackedImagesChangedObservable
                .Where(_ => _isMoving) // 補正が始まったら発火
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
            while (_isMoving)
            {
                await UniTask.NextFrame();
            }
        }

    }
}