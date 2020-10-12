using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

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
        var offsetManager = trackedImageObject.GetComponent<ARTrackedImageOffsetManager>();
        var offsetData = ImageTargetMaster.FindItem(imageTargetName);
        offsetManager.SetAnchorTransform(offsetData);
    }

}
