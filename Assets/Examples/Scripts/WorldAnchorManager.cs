using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

public class WorldAnchorManager : MonoBehaviour
{
    [SerializeField]
    GameObject _arTrackableGameObject;

    Dictionary<string, GameObject> _createdARTrackedGameObjects = new Dictionary<string, GameObject>();
    Dictionary<string, GameObject> _detectedARTrackedGameObjects = new Dictionary<string, GameObject>();

    public void DetectMarker(string imageTargetName, GameObject marker)
    {
        // 初回認識時
        if (!_detectedARTrackedGameObjects.ContainsKey(imageTargetName))
        {
            // 作らせる
            var o = GetARTrackedGameObject(imageTargetName, marker);
        }
        // 認識中
        _detectedARTrackedGameObjects.Add(imageTargetName, _createdARTrackedGameObjects[imageTargetName]);
    }

    public void UndetectedMarker(string imageTargetName)
    {
        _detectedARTrackedGameObjects.Remove(imageTargetName);
    }

    public bool IsTracking(string imageTargetName)
    {
        return _detectedARTrackedGameObjects.ContainsKey(imageTargetName);
    }

    /// <summary>
    /// ARFの挙動としてはARTrackedGameObjectは初回マーカー認識時に１回だけInstantiateされるっぽい
    /// すでに生成済みの場合はそのまま返す
    /// </summary>
    /// <param name="imageTargetName"></param>
    /// <param name="marker"></param>
    /// <returns>GameObject</returns>
    public GameObject GetARTrackedGameObject(string imageTargetName, GameObject marker)
    {

        if (!_createdARTrackedGameObjects.ContainsKey(imageTargetName))
        {
            var go = Instantiate(_arTrackableGameObject, marker.transform.position, marker.transform.rotation);
            _createdARTrackedGameObjects.Add(imageTargetName, go);
            _createdARTrackedGameObjects[imageTargetName].transform.SetParent(marker.transform);

            InitAnchorTransform(imageTargetName, marker);
        }
        return _createdARTrackedGameObjects[imageTargetName];
    }

    void InitAnchorTransform(string imageTargetName, GameObject marker)
    {
        var trackedImageObject = _createdARTrackedGameObjects[imageTargetName];
        var offsetManager = trackedImageObject.GetComponent<ARTrackedImageOffsetManager>();
        var offsetData = ImageTargetMaster.FindItem(imageTargetName);
        offsetManager.SetAnchorTransform(offsetData);
    }
}
