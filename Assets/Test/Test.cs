using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField]
    GameObject ARTrackableGameObject;

    [SerializeField]
    GameObject BlueMarker;

    [SerializeField]
    GameObject RedMarker;

    Dictionary<string, GameObject> _detectedARTrackedGameObjects = new Dictionary<string, GameObject>();

    public void DetectedRed()
    {
        var imageTargetName = "PvP_Marker_3_0_4_Red";
        CreateARTrackedGameObject(imageTargetName, RedMarker);
    }

    public void DetectedBlue()
    {
        var imageTargetName = "PvP_Marker_2_0_1_Blue";
        CreateARTrackedGameObject(imageTargetName, BlueMarker);
    }

    void CreateARTrackedGameObject(string imageTargetName, GameObject marker)
    {
        // ARFの挙動としてはARTrackedGameObjectは初回マーカー認識時に１回だけInstantiateされるっぽいので
        if (!_detectedARTrackedGameObjects.ContainsKey(imageTargetName))
        {
            var go = Instantiate(ARTrackableGameObject, marker.transform.position, marker.transform.rotation);
            _detectedARTrackedGameObjects.Add(imageTargetName, go);
            _detectedARTrackedGameObjects[imageTargetName].transform.SetParent(marker.transform);

            SetAnchorTransform(imageTargetName, marker);
        }
    }

    void SetAnchorTransform(string imageTargetName, GameObject marker)
    {
        var trackedImageObject = _detectedARTrackedGameObjects[imageTargetName];
        var offsetManager = trackedImageObject.GetComponent<ARTrackedImageOffsetManager>();
        var offsetData = ImageTargetMaster.FindItem(imageTargetName);
        offsetManager.SetAnchorTransform(offsetData);
    }
}
