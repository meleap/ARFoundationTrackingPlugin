using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hado.ARFoundation;

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
    Dictionary<string, GameObject> _detectedAnchors = new Dictionary<string, GameObject>();

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
        var o = GetARTrackedGameObject(imageTargetName, marker);
        _detectedAnchors.Add(imageTargetName, o.GetComponentInChildren<Anchor>().gameObject);

        // 認識中
        PositionManager.Instance.WorldAnchor = _detectedAnchors[imageTargetName];
    }

    public void UndetectedMarker(string imageTargetName)
    {
        _detectedAnchors.Remove(imageTargetName);
    }

    void ChangeButtonStatus(string imageTargetName, Button btn)
    {
        var isActive = !_detectedAnchors.ContainsKey(imageTargetName);
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
        // 初回は作る
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

        var anchor = trackedImageObject.GetComponentInChildren<Anchor>();
        anchor.Name = imageTargetName;

        var offset = ImageTargetOffsetMaster.FindItem(imageTargetName);
        var m = Matrix4x4.TRS(offset.position, offset.rotation, Vector3.one).inverse;
        var t = anchor.gameObject.transform;
        t.localPosition = m.MultiplyPoint3x4(t.localPosition);
        t.rotation = t.rotation * Quaternion.Inverse(offset.rotation);
    }
}
