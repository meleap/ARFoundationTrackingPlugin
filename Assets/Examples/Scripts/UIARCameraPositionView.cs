using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hado.ARFoundation;

public class UIARCameraPositionView : MonoBehaviour
{
    [SerializeField]
    Text _uiText;

    [SerializeField]
    Camera arCamera;

    PositionManager _pm;

    void Start()
    {
        _pm = PositionManager.Instance;
    }

    void Update()
    {
        var pr = _pm.GetRelativePositionAndRotationFromWorldAnchor(arCamera.transform.position, arCamera.transform.rotation);
        _uiText.text = $"pos: ({pr.position.x:f4}, {pr.position.y:f4}, {pr.position.z:f4})\n";

        var ea = pr.rotation.eulerAngles;
        _uiText.text += $"rot: ({ea.x:f4}, {ea.y:f4}, {ea.z:f4})";
    }
}
