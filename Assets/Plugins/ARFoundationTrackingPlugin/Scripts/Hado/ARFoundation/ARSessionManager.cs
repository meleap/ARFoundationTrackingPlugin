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

        private const string DummyBlackCanvasName = "DummyBlackCanvas";
        private GameObject _dummyBlackCanvas;

        public void Init(GameObject go)
        {
            FindObjectsAndComponents(go);
            arCamera.enabled = false;
            _arSessionManagerGameObject.SetActive(true);

            _dummyBlackCanvas = Resources.Load<GameObject>(DummyBlackCanvasName);
        }

        public void PowerOff()
        {
            AutoFocusRequested = false;
            arCamera.enabled = false;
            EnabledPositionTracking = false;
            EnabledImageTracking = false;

            Observable.NextFrame()
                .Subscribe(_ =>
                {
                    _arSession.enabled = false;
                });
        }

        public void PowerOn(bool enableCamera = true, bool autoFocus = false)
        {
            // ARCameraを起動したときに前回のラストフレームが一瞬描写される。それを隠すための黒キャンバス
            var ui = GameObject.Instantiate(_dummyBlackCanvas, arCamera.transform);
            
            if (enableCamera)
                arCamera.enabled = true;
            
            _arCameraManager.enabled = true;
            EnabledPositionTracking = true;
            EnabledImageTracking = true;
            _arSession.enabled = true;
            
            Observable.NextFrame()
                .Subscribe(_ =>
                {
                    AutoFocusRequested = autoFocus;
                    
                });

            Observable.Timer(TimeSpan.FromMilliseconds(1000))
                .Subscribe(_ =>
                {
                    GameObject.Destroy(ui);
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
            set
            {
                if (ImageTargetOffsetMaster.ImageTargets != null)
                {
                    _arTrackedImageManager.enabled = value;
                }
                else
                {
                    Debug.LogError("ImageTargetOffsetMaster.ImageTargets has no data");
                }
            }
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