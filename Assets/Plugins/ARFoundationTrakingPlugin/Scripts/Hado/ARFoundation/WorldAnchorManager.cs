using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

namespace Hado.ARFoundation
{
    [RequireComponent(typeof(ARTrackedImageManager))]
    public class WorldAnchorManager : MonoBehaviour
    {

        ARTrackedImageManager m_TrackedImageManager;

        void Awake()
        {
            m_TrackedImageManager = GetComponent<ARTrackedImageManager>();
        }

        void OnEnable()
        {
            m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
        }

        void OnDisable()
        {
            m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        }


        void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
        {
            foreach (var trackedImage in eventArgs.added)
            {
                // 初回だけの処理はここに
                InitAnchorTransform(trackedImage.name, trackedImage.gameObject);
            }

            foreach (var trackedImage in eventArgs.updated)
            {
                // UpdateInfo(trackedImage);
            }
        }

        void InitAnchorTransform(string imageTargetName, GameObject trackedImageObject)
        {
            var anchor = trackedImageObject.GetComponentInChildren<Anchor>();
            var offset = ImageTargetOffsetMaster.FindItem(imageTargetName);
            var m = Matrix4x4.TRS(offset.position, offset.rotation, Vector3.one).inverse;

            var t = anchor.gameObject.transform;
            t.localPosition = m.MultiplyPoint3x4(t.transform.localPosition);
            t.rotation = t.rotation * Quaternion.Inverse(offset.rotation);
        }
    }
}