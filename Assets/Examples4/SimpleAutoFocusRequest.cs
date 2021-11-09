using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Hado.ARFoundation;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UniRx;

public class SimpleAutoFocusRequest : MonoBehaviour
{
    [SerializeField] private Button focusOn;
    [SerializeField] private Button focusOff;
    [SerializeField] private Button arOn;
    [SerializeField] private Button arOff;
    [SerializeField] private Toggle toggleAutoFocus;

    [SerializeField] private ARCameraManager arCameraManager;

    [SerializeField] private GameObject arSessionManagerGameObject;
    
    private void Start()
    {
        ARSessionManager.Instance.Init(arSessionManagerGameObject);
        
        
        focusOn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                arCameraManager.autoFocusRequested = true;
            }).AddTo(this);
        
        focusOff.OnClickAsObservable()
            .Subscribe(_ =>
            {
                arCameraManager.autoFocusRequested = false;
            }).AddTo(this);

        arOn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                ARSessionManager.Instance.PowerOnAsync(autoFocus: toggleAutoFocus.isOn).Forget();
            }).AddTo(this);

        arOff.OnClickAsObservable()
            .Subscribe(_ =>
            {
                ARSessionManager.Instance.PowerOffAsync().Forget();
            }).AddTo(this);
    }
}
