using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    [SerializeField]
    GameObject _arTrackableGameObject;

    [SerializeField]
    GameObject _redMarker;

    [SerializeField]
    GameObject _blueMarker;

    [SerializeField]
    Button _btnRedDetected;

    [SerializeField]
    Button _btnBlueDetected;

    [SerializeField]
    Button _btnUndetectedRed;

    [SerializeField]
    Button _btnUndetectedBlue;

    Dictionary<string, GameObject> _createdARTrackedGameObjects = new Dictionary<string, GameObject>();
    Dictionary<string, GameObject> _detectedARTrackedGameObjects = new Dictionary<string, GameObject>();

    readonly string imageTargetNameRed = "PvP_Marker_3_0_4_Red";
    readonly string imageTargetNameBlue = "PvP_Marker_2_0_1_Blue";


    void Awake()
    {
        _btnRedDetected.onClick.AddListener(DetectedRed);
        _btnBlueDetected.onClick.AddListener(DetectedBlue);

        _btnUndetectedRed.onClick.AddListener(UndetectedRed);
        _btnUndetectedBlue.onClick.AddListener(UndetectedBlue);
    }

    public void DetectedRed()
    {
        var o = GetARTrackedGameObject(imageTargetNameRed, _redMarker);
        _detectedARTrackedGameObjects.Add(imageTargetNameRed, o);

        ChangeButtonStatus(imageTargetNameRed, _btnRedDetected);
    }

    public void DetectedBlue()
    {
        var o = GetARTrackedGameObject(imageTargetNameBlue, _blueMarker);
        _detectedARTrackedGameObjects.Add(imageTargetNameBlue, o);

        ChangeButtonStatus(imageTargetNameBlue, _btnBlueDetected);
    }

    public void UndetectedRed()
    {
        _detectedARTrackedGameObjects.Remove(imageTargetNameRed);
        ChangeButtonStatus(imageTargetNameRed, _btnRedDetected);
    }

    public void UndetectedBlue()
    {
        _detectedARTrackedGameObjects.Remove(imageTargetNameBlue);
        ChangeButtonStatus(imageTargetNameBlue, _btnBlueDetected);
    }

    void ChangeButtonStatus(string imageTargetName, Button btn)
    {
        var isActive = !_detectedARTrackedGameObjects.ContainsKey(imageTargetName);
        btn.interactable = isActive;
    }

    /// <summary>
    /// ARFの挙動としてはARTrackedGameObjectは初回マーカー認識時に１回だけInstantiateされるっぽい
    /// すでに生成済みの場合はそのまま返す
    /// </summary>
    /// <param name="imageTargetName"></param>
    /// <param name="marker"></param>
    /// <returns>GameObject</returns>
    GameObject GetARTrackedGameObject(string imageTargetName, GameObject marker)
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
