﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using Hado.ARFoundation;

public class ARSessionController : MonoBehaviour
{
    public void ChangePositionTrackingState(bool state)
    {
        ARSessionManager.Instance.EnabledPositionTracking = state;
    }

    public void ChangeAutoFocusState(bool state)
    {
        ARSessionManager.Instance.AutoFocusRequested = state;
    }

    public void ChangeImageTrackingState(bool state)
    {
        ARSessionManager.Instance.EnabledImageTracking = state;
    }

    public void IgnoreRedMarkers()
    {
        IgnoreMarkers("red");
    }

    public void IgnoreBlueMarkers()
    {
        IgnoreMarkers("blue");
    }

    private void IgnoreMarkers(string ignoreWord)
    {
        var num = ARSessionManager.Instance.CurrentMarkerSetNumber;
        var markerLibrary = ARSessionManager.Instance.arTrackedImageReferenceManager.GetMarkerSet(num);
        var list = new List<string>();
        foreach (var imageTarget in markerLibrary)
        {
            if (imageTarget.name.ToLower().Contains(ignoreWord))
            {
                list.Add(imageTarget.name);
            }
        }

        PositionManager.Instance.WorldAnchor.GetComponent<WorldAnchorManager>().IgnoreMarkerNames = list.ToArray();
    }

    public void Power(bool isOn)
    {
        if (isOn)
            ARSessionManager.Instance.PowerOn();
        else
            ARSessionManager.Instance.PowerOff();
    }

    public void PowerAsync(bool isOn)
    {
        if (isOn)
            UniTask.Void(async () => await ARSessionManager.Instance.PowerOnAsync());
        else
            UniTask.Void(async () => await ARSessionManager.Instance.PowerOffAsync());
    }

    public void ChangeMarkerSet(int num)
    {
        UniTask.Void(async () => { await ARSessionManager.Instance.ChangeMarkerSet(num); });
    }
}