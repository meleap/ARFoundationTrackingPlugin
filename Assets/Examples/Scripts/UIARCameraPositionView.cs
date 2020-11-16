using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hado.ARFoundation;
using UnityEngine.SceneManagement;

public class UIARCameraPositionView : MonoBehaviour
{
    [SerializeField]
    Text _uiText;

    PositionManager _pm;

    private Transform _arCamera;

    void Start()
    {
        _pm = PositionManager.Instance;
        _arCamera = ARSessionManager.Instance.arCamera.transform;
    }

    void Update()
    {

        var pr = _pm.GetRelativePositionAndRotationFromWorldAnchor(_arCamera.position, _arCamera.rotation);
        _uiText.text = $"pos: ({pr.position.x:f4}, {pr.position.y:f4}, {pr.position.z:f4})\n";

        var ea = pr.rotation.eulerAngles;
        _uiText.text += $"rot: ({ea.x:f4}, {ea.y:f4}, {ea.z:f4})\n";

        _uiText.text += $"current: {_pm.LastDetectedAnchorName}";
    }

    public void OnPressBack()
    {
        Debug.Log("===Back===");
        ARSessionManager.Instance.PowerOff();
        SceneManager.LoadScene("Ready", LoadSceneMode.Single);
    }

}
