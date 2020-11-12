﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.XR.ARSubsystems;

namespace Hado.ARFoundation
{
    public class WorldAnchorManager : MonoBehaviour
    {
        void Awake()
        {
            PositionManager.Instance.WorldAnchor = this.gameObject;
        }

        void Start()
        {
            ARSessionManager.Instance.arTrackedImageEventManager.OnTrackedImagesChangedObservable
                .Where(t => t.trackingState != TrackingState.None)
                .Subscribe(t =>
                {
                    // TODO: この処理をリニアではなくなんらかのスムージング入れると体感が良くなりそう
                    // TODO: 今認識してる1つのマーカーの情報だけで補正してるので、もっと頭いいロジックにしたい
                    var anchor =
                        ARSessionManager.Instance.arTrackedImageEventManager.GetReferenceAnchor(t.referenceImage.name);
                    PositionManager.Instance.WorldAnchor.transform.SetPositionAndRotation(
                        anchor.transform.position,
                        anchor.transform.rotation
                    );
                    PositionManager.Instance.LastDetectedAnchorName = t.referenceImage.name;
                }).AddTo(this);
        }
    }
}