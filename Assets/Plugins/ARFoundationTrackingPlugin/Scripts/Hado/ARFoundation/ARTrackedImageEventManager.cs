using System;
using UniRx;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Hado.ARFoundation
{
    [RequireComponent(typeof(ARTrackedImageManager))]
    public class ARTrackedImageEventManager : MonoBehaviour
    {
        private const string GroundMarkerName = "GroundMarker";
        private readonly ARTrackedImageStabler _arTrackedImageStabler = new();
        private readonly ReactiveProperty<bool> _groundMarkerTracked = new(false);

        public IObservable<(Vector3, Quaternion)> TrackedImagesChangedObservable =>
            _arTrackedImageStabler.TrackedImageObservable;

        public IReadOnlyReactiveProperty<bool> GroundMarkerTracked => _groundMarkerTracked;

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
            _groundMarkerTracked.Value = false;
        }

        private void Awake()
        {
            _mTrackedImageManager = GetComponent<ARTrackedImageManager>();
        }

        private void OnEnable()
        {
            _mTrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
        }

        private void OnDisable()
        {
            _mTrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        }

        private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
        {
            // 安定するまでの間は何もしない
            // SessionInitializingのときにも呼ばれる可能性がある
            if (ARSession.state != ARSessionState.SessionTracking) return;

            foreach (var trackedImage in eventArgs.added)
            {
                CheckGroundMarkerTracked(trackedImage);
                _arTrackedImageStabler.TryInitAnchorTransformIfNotExists(trackedImage, InitAnchorTransform);
                _arTrackedImageStabler.OnTrackedImage(trackedImage);
            }

            foreach (var trackedImage in eventArgs.updated)
            {
                if (trackedImage.trackingState != TrackingState.Tracking) return;
                CheckGroundMarkerTracked(trackedImage);
                _arTrackedImageStabler.TryInitAnchorTransformIfNotExists(trackedImage, InitAnchorTransform);
                _arTrackedImageStabler.OnTrackedImage(trackedImage);
            }
        }

        private void CheckGroundMarkerTracked(ARTrackedImage trackedImage)
        {
            if (trackedImage.referenceImage.name == GroundMarkerName)
            {
                _groundMarkerTracked.Value = true;
            }
        }

        private static Anchor InitAnchorTransform(ARTrackedImage trackedImage)
        {
            var markerName = trackedImage.referenceImage.name;
            var anchor = trackedImage.GetComponentInChildren<Anchor>();
            anchor.Name = markerName;
            var offset = ARMarkerManager.Instance.GetOffsetByMarkerName(markerName);
            var m = Matrix4x4.TRS(offset.Position, offset.Rotation, Vector3.one).inverse;

            var t = anchor.gameObject.transform;
            t.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            t.localPosition = m.MultiplyPoint3x4(t.localPosition);
            t.rotation *= Quaternion.Inverse(offset.Rotation);
            return anchor;
        }
    }
}
