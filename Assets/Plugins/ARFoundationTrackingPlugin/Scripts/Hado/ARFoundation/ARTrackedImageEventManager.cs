using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Hado.ARFoundation
{
    [RequireComponent(typeof(ARTrackedImageManager))]
    public class ARTrackedImageEventManager : MonoBehaviour
    {
        private readonly ARTrackedImageStabler _arTrackedImageStabler = new();

        public IObservable<(Vector3, Quaternion)> TrackedImagesChangedObservable =>
            _arTrackedImageStabler.TrackedImageObservable;

        private ARTrackedImageManager _mTrackedImageManager;

        public float MovingNoiseThreshold
        {
            get => _arTrackedImageStabler.MovingNoiseThreshold;
            set => _arTrackedImageStabler.MovingNoiseThreshold = value;
        }

        public int NoiseCheckSampleCount
        {
            get => _arTrackedImageStabler.NoiseCheckSampleCount;
            set => _arTrackedImageStabler.NoiseCheckSampleCount = value;
        }

        public void Clear()
        {
            _arTrackedImageStabler.Clear();
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
            // 安定するまでの間は何もしない
            if (ARSession.state < ARSessionState.SessionInitializing) return;

            foreach (var trackedImage in eventArgs.added)
            {
                // 初回だけの処理はここに
                Debug.Log($"OnTrackedImagesChanged: add: {trackedImage.trackingState}");
                _arTrackedImageStabler.TryInitAnchorTransformIfNotExists(trackedImage, InitAnchorTransform);
                _arTrackedImageStabler.OnTrackedImage(trackedImage);
            }

            foreach (var trackedImage in eventArgs.updated)
            {
                if (trackedImage.trackingState != TrackingState.Tracking) return;
                Debug.Log($"OnTrackedImagesChanged: updated: {trackedImage.trackingState}");
                //TODO: 稀に初回detectなのにupdateで渡されることがある
                _arTrackedImageStabler.TryInitAnchorTransformIfNotExists(trackedImage, InitAnchorTransform);
                _arTrackedImageStabler.OnTrackedImage(trackedImage);
            }
        }

        private static Anchor InitAnchorTransform(ARTrackedImage trackedImage)
        {
            Debug.Log($"InitAnchorTransform: {trackedImage.referenceImage.name}");
            var markerName = trackedImage.referenceImage.name;
            var anchor = trackedImage.GetComponentInChildren<Anchor>();
            anchor.Name = markerName;
            var offset = ARMarkerManager.Instance.GetOffsetByMarkerName(markerName);
            var m = Matrix4x4.TRS(offset.Position, offset.Rotation, Vector3.one).inverse;
            Debug.Log($"Anchor Offset: {offset.Position}, {offset.Rotation.eulerAngles}");

            var t = anchor.gameObject.transform;
            t.localPosition = m.MultiplyPoint3x4(t.localPosition);
            t.rotation *= Quaternion.Inverse(offset.Rotation);
            return anchor;
        }
    }
}
