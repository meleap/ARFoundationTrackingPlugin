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
    WorldAnchorManager _worldAnchorManager;

    void Awake()
    {
        _btnRedDetected.onClick.AddListener(DetectedRed);
        _btnBlueDetected.onClick.AddListener(DetectedBlue);

        _btnUndetectedRed.onClick.AddListener(UndetectedRed);
        _btnUndetectedBlue.onClick.AddListener(UndetectedBlue);

        _worldAnchorManager = GetComponent<WorldAnchorManager>();
    }

    public void DetectedRed()
    {
        _worldAnchorManager.DetectMarker(imageTargetNameRed, _redMarker);
        ChangeButtonStatus(imageTargetNameRed, _btnRedDetected);
    }

    public void DetectedBlue()
    {
        _worldAnchorManager.DetectMarker(imageTargetNameBlue, _blueMarker);
        ChangeButtonStatus(imageTargetNameBlue, _btnBlueDetected);
    }

    public void UndetectedRed()
    {
        _worldAnchorManager.UndetectedMarker(imageTargetNameRed);
        ChangeButtonStatus(imageTargetNameRed, _btnRedDetected);
    }

    public void UndetectedBlue()
    {
        _worldAnchorManager.UndetectedMarker(imageTargetNameBlue);
        ChangeButtonStatus(imageTargetNameBlue, _btnBlueDetected);
    }

    void ChangeButtonStatus(string imageTargetName, Button btn)
    {
        var isActive = !_worldAnchorManager.IsTracking(imageTargetName);
        btn.interactable = isActive;
    }
}
