using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Emulation : MonoBehaviour
{
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

    readonly string imageTargetNameRed = "PvP_Marker_3_0_4_Red";
    readonly string imageTargetNameBlue = "PvP_Marker_2_0_1_Blue";

    [SerializeField]
    GameObject _arTrackableGameObject;

    Dictionary<string, GameObject> _createdARTrackedGameObjects = new Dictionary<string, GameObject>();
    Dictionary<string, GameObject> _detectedARTrackedGameObjects = new Dictionary<string, GameObject>();

    void Awake()
    {
        _btnRedDetected.onClick.AddListener(DetectedRed);
        _btnBlueDetected.onClick.AddListener(DetectedBlue);

        _btnUndetectedRed.onClick.AddListener(UndetectedRed);
        _btnUndetectedBlue.onClick.AddListener(UndetectedBlue);
    }

    public void DetectedRed()
    {
        DetectMarker(imageTargetNameRed, _redMarker);
        ChangeButtonStatus(imageTargetNameRed, _btnRedDetected);
    }

    public void DetectedBlue()
    {
        DetectMarker(imageTargetNameBlue, _blueMarker);
        ChangeButtonStatus(imageTargetNameBlue, _btnBlueDetected);
    }

    public void UndetectedRed()
    {
        UndetectedMarker(imageTargetNameRed);
        ChangeButtonStatus(imageTargetNameRed, _btnRedDetected);
    }

    public void UndetectedBlue()
    {
        UndetectedMarker(imageTargetNameBlue);
        ChangeButtonStatus(imageTargetNameBlue, _btnBlueDetected);
    }

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
        ImageTargetOffsetMaster.ImageTargets = ImageTargetOffsetSampleData.ImageTargets;
        var trackedImageObject = _createdARTrackedGameObjects[imageTargetName];
        var offsetManager = trackedImageObject.GetComponent<ARTrackedImageOffsetManager>();
        var offsetData = ImageTargetOffsetMaster.FindItem(imageTargetName);
        offsetManager.SetAnchorTransform(offsetData);
    }
}
