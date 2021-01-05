using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UnityEngine.XR.ARSubsystems;

namespace Hado.ARFoundation
{
    public class WorldAnchorManager : MonoBehaviour
    {
        /// <summary>
        /// プルつき防止のために、マーカー位置のズレがMovingThreshold以下の場合はWorldAnchorを動かさない
        /// </summary>
        [NonSerialized]
        public float MovingStartThreshold = 0.05f;
        
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
        
        
        private GameObject _anchor;

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
                .Where(_ => !_isMoving)
                .Subscribe(t =>
                {
                    // TODO: 今認識してる1つのマーカーの情報だけで補正してるので、もっと頭いいロジックにしたい

                    _anchor =
                        ARSessionManager.Instance.arTrackedImageEventManager.GetReferenceAnchor(t.referenceImage.name);
                    _moveEndPosition = _anchor.transform.position;
                    _moveEndRotation = _anchor.transform.rotation;

                    PositionManager.Instance.LastDetectedAnchorName = t.referenceImage.name;
                    Debug.Log($"{t.referenceImage.name} detected");
                    
                    if (Vector3.Distance(_transform.position, _moveEndPosition) > MovingStartThreshold)
                    {
                        Debug.Log($"Over MovingStartThreshold");
                        _isMoving = true;
                    }
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
    }
}