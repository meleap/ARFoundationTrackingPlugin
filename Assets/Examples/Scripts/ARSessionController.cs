using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARSubsystems;
using Hado.ARFoundation;

public class ARSessionController : MonoBehaviour
{
    public void OnPressBack()
    {
        Debug.Log("===Back===");
        ARSessionManager.Instance.PowerOff();
        SceneManager.LoadScene("Ready", LoadSceneMode.Single);
    }
    
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

    public void ResetSession()
    {
        ARSessionManager.Instance.ResetSession();
    }
}