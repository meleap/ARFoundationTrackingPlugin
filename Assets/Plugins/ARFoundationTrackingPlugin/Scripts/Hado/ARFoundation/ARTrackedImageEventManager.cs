using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Hado.ARFoundation
{
    [RequireComponent(typeof(ARTrackedImageManager))]
    public class ARTrackedImageEventManager : MonoBehaviour
    {
        private readonly Subject<ARTrackedImage> _trackImagesChangedSubject = new();

        public IObservable<(Vector3, Quaternion)> TrackedImagesChangedObservable => _trackImagesChangedSubject
            .Select(trackedImage => GetOrNullAnchorWithClear(trackedImage.referenceImage.name))
            .Where(anchor => anchor != null)
            .Select(x =>
            {
                x.transform.GetPositionAndRotation(out var pos, out var rot);
                return (pos, rot);
            })
            .Buffer(NoiseCheckSampleCount + 1)
            .Where(l => !IsNoiseData(l))
            .Select(l => l.Last()); // 最新のデータを取得

        private ARTrackedImageManager _mTrackedImageManager;

        private readonly Dictionary<string, Anchor> _detectedReferenceAnchors = new();

        /// フレーム間の移動距離がこの値より大きい場合はノイズとして捨てる
        public float MovingNoiseThreshold { get; set; } = 0.05f;

        /// MovingNoiseThresholdのチェックを何回ぶん行うか
        public int NoiseCheckSampleCount { get; set; } = 2;

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

        private Anchor GetOrNullAnchorWithClear(string imageName)
        {
            // 初回マーカー認識後にNative側で"UnityARKit: Updating ARSession configuration"があると、keyはあるのにAnchorがnullという状態が発生する
            // その場合は一度クリアして再度Anchorを設定する
            var anchor = _detectedReferenceAnchors.GetValueOrDefault(imageName);
            if (anchor == null) Clear();
            return anchor;
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
            // 安定するまでの間は何もしない
            if (ARSession.state < ARSessionState.SessionInitializing) return;

            foreach (var trackedImage in eventArgs.added)
            {
                // 初回だけの処理はここに
                Debug.Log($"OnTrackedImagesChanged: add: {trackedImage.trackingState}");
                if (!_detectedReferenceAnchors.ContainsKey(trackedImage.referenceImage.name))
                    InitAnchorTransform(trackedImage);
                _trackImagesChangedSubject.OnNext(trackedImage);
            }

            foreach (var trackedImage in eventArgs.updated)
            {
                if (trackedImage.trackingState != TrackingState.Tracking) return;

                //TODO: 稀に初回detectなのにupdateで渡されることがある
                if (!_detectedReferenceAnchors.ContainsKey(trackedImage.referenceImage.name))
                    InitAnchorTransform(trackedImage);


                Debug.Log($"OnTrackedImagesChanged: updated: {trackedImage.trackingState}");
                _trackImagesChangedSubject.OnNext(trackedImage);
            }
        }

        private void InitAnchorTransform(ARTrackedImage trackedImage)
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

            _detectedReferenceAnchors.Add(markerName, anchor);
        }
    }
}
