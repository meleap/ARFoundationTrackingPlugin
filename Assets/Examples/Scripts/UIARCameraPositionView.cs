using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hado.ARFoundation;
using UniRx;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

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

        ARSession.stateChanged += UpdateARStateChangedText;

        ARSessionManager.Instance.arTrackedImageEventManager.OnTrackedImagesChangedObservable
            .Select(x => x.gameObject.transform.position)
            .Buffer(3)
            .Subscribe(UpdatePositionText);
    }

    private void Update()
    {
        //UpdatePositionText();
    }

    private void UpdateARStateChangedText(ARSessionStateChangedEventArgs args)
    {
        Debug.Log($"ARSession State: {args.state}");
        _uiText.text = $"{args.state}";
    }

    private void UpdatePositionText()
    {
        var pr = _pm.GetRelativePositionAndRotationFromWorldAnchor(_arCamera.position, _arCamera.rotation);
        _uiText.text = $"pos: ({pr.position.x:f4}, {pr.position.y:f4}, {pr.position.z:f4}) - ";

        var ea = pr.rotation.eulerAngles;
        _uiText.text += $"rot: ({ea.x:f4}, {ea.y:f4}, {ea.z:f4})\n";

        _uiText.text += $"current: {_pm.LastDetectedAnchorName}";
    }

    private void UpdatePositionText(GameObject go)
    {
        var tr = go.transform;
        _uiText.text = $"pos: ({tr.position.x:f6}, {tr.position.y:f6}, {tr.position.z:f6}) - ";

        var ea = tr.rotation.eulerAngles;
        _uiText.text += $"rot: ({ea.x:f4}, {ea.y:f4}, {ea.z:f4})\n";

        _uiText.text += $"current: {_pm.LastDetectedAnchorName}"; 
    }
    
    private void UpdatePositionText(IList<GameObject> go)
    {
        var tr = go[0].transform;
        var tr1 = go[1].transform;
        _uiText.text = $"pos: ({tr.position.x:f6}, {tr.position.y:f6}, {tr.position.z:f6})\n";

        _uiText.text += $"pos: ({tr1.position.x:f6}, {tr1.position.y:f6}, {tr1.position.z:f6})\n";

        _uiText.text += $"current: {_pm.LastDetectedAnchorName}"; 
    }
    
    private void UpdatePositionText(IList<Vector3> pos)
    {
        _uiText.text = $"pos: ({pos[0].x:f6}, {pos[0].y:f6}, {pos[0].z:f6})\n";

        _uiText.text += $"pos: ({pos[1].x:f6}, {pos[1].y:f6}, {pos[1].z:f6})\n";

        _uiText.text += $"current: {_pm.LastDetectedAnchorName}"; 
    }

    private void OnDestroy()
    {
        ARSession.stateChanged -= UpdateARStateChangedText;
    }
}
