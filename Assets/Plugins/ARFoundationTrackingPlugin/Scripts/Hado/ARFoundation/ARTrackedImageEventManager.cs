﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
using UniRx;

namespace Hado.ARFoundation
{
    [RequireComponent(typeof(ARTrackedImageManager))]
    public class ARTrackedImageEventManager : MonoBehaviour
    {
        private readonly Subject<ARTrackedImage> _trackImagesChangedSubject = new Subject<ARTrackedImage>();

        public IObservable<ARTrackedImage> OnTrackedImagesChangedObservable => _trackImagesChangedSubject.AsObservable();

        private ARTrackedImageManager _mTrackedImageManager;

        private readonly Dictionary<string, GameObject> _detectedReferenceAnchors = new Dictionary<string, GameObject>();

        public GameObject GetReferenceAnchor(string imageName)
        {
            if (!_detectedReferenceAnchors.ContainsKey(imageName))
                throw new Exception("一度も認識していないか、存在しないマーカーです");

            return _detectedReferenceAnchors[imageName];
        }

        public void Clear()
        {
            _detectedReferenceAnchors.Clear();
        }

        private void Awake()
        {
            _mTrackedImageManager = GetComponent<ARTrackedImageManager>();
        }

        private void OnEnable()
        {
            Debug.Log("Register trackedImagesChanged event: OnTrackedImagesChanged");
            _mTrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
        }

        private void OnDisable()
        {
            Debug.Log("Unregister trackedImagesChanged event: OnTrackedImagesChanged");
            _mTrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        }

        private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
        {
            foreach (var trackedImage in eventArgs.added)
            {
                // 初回だけの処理はここに
                Debug.Log($"OnTrackedImagesChanged: add: {trackedImage.trackingState}");
                InitAnchorTransform(trackedImage);
                _trackImagesChangedSubject.OnNext(trackedImage);
            }

            foreach (var trackedImage in eventArgs.updated)
            {
                if (trackedImage.trackingState != TrackingState.Tracking) return;
                
                Debug.Log($"OnTrackedImagesChanged: updated: {trackedImage.trackingState}");
                _trackImagesChangedSubject.OnNext(trackedImage);
            }
        }

        private void InitAnchorTransform(ARTrackedImage trackedImage)
        {
            Debug.Log("InitAnchorTransform");
            var markerName = trackedImage.referenceImage.name;
            var anchor = trackedImage.GetComponentInChildren<Anchor>();
            anchor.Name = markerName;
            var offset = ImageTargetOffsetMaster.FindItem(markerName);
            var m = Matrix4x4.TRS(offset.position, offset.rotation, Vector3.one).inverse;

            var t = anchor.gameObject.transform;
            t.localPosition = m.MultiplyPoint3x4(t.localPosition);
            t.rotation *= Quaternion.Inverse(offset.rotation);

            _detectedReferenceAnchors.Add(markerName, anchor.gameObject);
        }
    }
}