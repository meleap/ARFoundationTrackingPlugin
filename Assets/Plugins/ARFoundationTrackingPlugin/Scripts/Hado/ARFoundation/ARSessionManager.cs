using System;
using Hado.ARFoundation;
using UniRx;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Hado.ARFoundation
{
    public class ARSessionManager
    {
        private GameObject _arSessionManagerGameObject;
        private GameObject _arSessionOriginGameObject;
        private GameObject _arSessionGameObject;

        private ARCameraManager _arCameraManager;
        private ARSession _arSession;
        private ARInputManager _arInputManager;
        private ARTrackedImageManager _arTrackedImageManager;
        
        public ARTrackedImageEventManager arTrackedImageEventManager;

        public Camera arCamera;

        public static ARSessionManager Instance { get; } = new ARSessionManager();

        public void Init(GameObject go)
        {
            FindObjectsAndComponents(go);
            arCamera.enabled = false;
            _arSessionManagerGameObject.SetActive(true);
        }

        public void PowerOff()
        {
            AutoFocusRequested = false;
            arCamera.enabled = false;

            Observable.NextFrame()
                .Subscribe(_ =>
                {
                    _arSession.enabled = false;
                });
        }

        public void PowerOn(bool enableCamera = true, bool autoFocus = false)
        {
            
            if (enableCamera)
                arCamera.enabled = true;

            _arCameraManager.enabled = true;
            _arSession.enabled = true;
            EnabledPositionTracking = true;
            EnabledImageTracking = true;
            
            Observable.NextFrame()
                .Subscribe(_ =>
                {
                    AutoFocusRequested = autoFocus;
                });
        }

        public bool AutoFocusRequested
        {
            set => _arCameraManager.autoFocusRequested = value;
        }

        public bool EnabledPositionTracking
        {
            set => _arInputManager.enabled = value;
        }

        public bool EnabledImageTracking
        {
            set => _arTrackedImageManager.enabled = value;
        }

        private void FindObjectsAndComponents(GameObject go)
        {
            _arSessionManagerGameObject = go;
            _arSessionOriginGameObject = go.transform.Find("AR Session Origin").gameObject;
            _arSessionGameObject = go.transform.Find("AR Session").gameObject;

            _arCameraManager = _arSessionManagerGameObject.GetComponentInChildren<ARCameraManager>();
            _arSession = _arSessionManagerGameObject.GetComponentInChildren<ARSession>();
            _arInputManager = _arSessionManagerGameObject.GetComponentInChildren<ARInputManager>();
            _arTrackedImageManager = _arSessionManagerGameObject.GetComponentInChildren<ARTrackedImageManager>();
            arTrackedImageEventManager = _arSessionManagerGameObject.GetComponentInChildren<ARTrackedImageEventManager>();
            
            arCamera = _arSessionManagerGameObject.GetComponentInChildren<Camera>();


            if (_arSessionOriginGameObject == null)
                throw new Exception("GameObject AR Session Origin not found.");
            if (_arSessionGameObject == null)
                throw new Exception("GameObject AR Session not found.");

            if (_arCameraManager == null)
                throw new Exception("ARCameraManager not found.");

            if (_arSession == null)
                throw new Exception("ARSession not found.");

            if (_arInputManager == null)
                throw new Exception("ARInputManager not found.");
            
            if(_arTrackedImageManager == null)
                throw new Exception("ARTrackedImageManager not found.");

            if (arCamera == null)
                throw new Exception("Camera not found.");
        }
    }
}