using System;
using UniRx;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Cysharp.Threading.Tasks;

namespace Hado.ARFoundation
{
    public class ARSessionManager
    {
        private GameObject _arSessionManagerGameObject;
        private GameObject _arSessionOriginGameObject;
        private GameObject _arSessionGameObject;

        public ARCameraManager arCameraManager;
        private ARSession _arSession;
        private ARInputManager _arInputManager;
        private ARTrackedImageManager _arTrackedImageManager;
        
        public ARTrackedImageEventManager arTrackedImageEventManager;
        public ARTrackedImageReferenceManager arTrackedImageReferenceManager;

        public Camera arCamera;

        public static ARSessionManager Instance { get; } = new ARSessionManager();

        private const string DummyBlackCanvasName = "DummyBlackCanvas";
        private GameObject _dummyBlackCanvas;

        public int CurrentMarkerSetNumber { get; set; } = 0;

        private bool IsUsingImageTracking => arTrackedImageReferenceManager.MarkerSets.Length > 0;

        public void Init(GameObject go)
        {
            FindObjectsAndComponents(go);
            _arSessionManagerGameObject.SetActive(true);
            
            arCamera.enabled = false;
            _arSession.enabled = false;
            _arInputManager.enabled = false;

            _dummyBlackCanvas = Resources.Load<GameObject>(DummyBlackCanvasName);

            _arTrackedImageManager.referenceLibrary = arTrackedImageReferenceManager.GetMarkerSet(0);
        }

        public void PowerOff()
        {
            AutoFocusRequested = false;
            arCamera.enabled = false;
            EnabledPositionTracking = false;
            EnabledImageTracking = false;
            arTrackedImageEventManager.Clear();
            
            Observable.Timer(TimeSpan.FromMilliseconds(300))
                .Do(_ => _arSession.Reset() )
                .Delay(TimeSpan.FromMilliseconds(300))
                .Subscribe(_ => _arSession.enabled = false);
        }

        public async UniTask PowerOffAsync()
        {
            AutoFocusRequested = false;
            arCamera.enabled = false;
            EnabledPositionTracking = false;
            EnabledImageTracking = false;
            arTrackedImageEventManager.Clear();

            await UniTask.Delay(300);
            _arSession.Reset();
            await UniTask.Delay(300);
            _arSession.enabled = false;
        }

        public void PowerOn(bool enableCamera = true, bool autoFocus = false, int warmupDelay = 1000)
        {
            // ARCameraを起動したときに前回のラストフレームが一瞬描写される。それを隠すための黒キャンバス
            var ui = GameObject.Instantiate(_dummyBlackCanvas, arCamera.transform);
            
            if (enableCamera)
                arCamera.enabled = true;
            
            arCameraManager.enabled = true;
            EnabledPositionTracking = true;
            EnabledImageTracking = IsUsingImageTracking;
            _arSession.enabled = true;
            
            Observable.NextFrame()
                .Subscribe(_ =>
                {
                    AutoFocusRequested = autoFocus;
                    
                });

            Observable.Timer(TimeSpan.FromMilliseconds(warmupDelay))
                .Subscribe(_ =>
                {
                    GameObject.Destroy(ui);
                });
        }

        public async UniTask PowerOnAsync(bool enableCamera = true, bool autoFocus = false, int warmupDelay = 1000)
        {
            // ARCameraを起動したときに前回のラストフレームが一瞬描写される。それを隠すための黒キャンバス
            var ui = GameObject.Instantiate(_dummyBlackCanvas, arCamera.transform);
            
            if (enableCamera)
                arCamera.enabled = true;

            arCameraManager.enabled = true;
            EnabledPositionTracking = true;
            EnabledImageTracking = IsUsingImageTracking;
            _arSession.enabled = true;

            await UniTask.Delay(warmupDelay);
            
            GameObject.Destroy(ui);

            await UniTask.NextFrame();

            AutoFocusRequested = autoFocus;
        }

        public void ResetSession()
        {
            arTrackedImageEventManager.Clear();
            _arSession.Reset();
        }

        public async UniTask ResetSessionAsync()
        {
            EnabledImageTracking = false;
            EnabledPositionTracking = false;
            
            await WaitForARSessionReady();
            
            arTrackedImageEventManager.Clear();
            _arSession.Reset();
            
            await WaitForARSessionReady();
            
            EnabledImageTracking = IsUsingImageTracking;
            EnabledPositionTracking = true;
            
            await WaitForARSessionReady();
        }

        public bool AutoFocusRequested
        {
            set => arCameraManager.autoFocusRequested = value;
        }

        public bool EnabledPositionTracking
        {
            set => _arInputManager.enabled = value;
        }

        public bool EnabledImageTracking
        {
            set
            {
                _arTrackedImageManager.enabled = value;
                arTrackedImageEventManager.enabled = value;
                
                if (value && ImageTargetOffsetMaster.ImageTargets == null)
                    throw new Exception("ImageTargetOffsetMaster.ImageTargets has no data");
            }
        }

        public async UniTask ChangeMarkerSet(int num, bool restart = true)
        {
            _arTrackedImageManager.referenceLibrary = arTrackedImageReferenceManager.GetMarkerSet(num);

            CurrentMarkerSetNumber = num;

            if (restart)
            {
                await PowerOffAsync();
                await PowerOnAsync(true, arCameraManager.autoFocusEnabled);
            }
        }

        private void FindObjectsAndComponents(GameObject go)
        {
            _arSessionManagerGameObject = go;
            _arSessionOriginGameObject = go.transform.Find("AR Session Origin").gameObject;
            _arSessionGameObject = go.transform.Find("AR Session").gameObject;

            arCameraManager = _arSessionManagerGameObject.GetComponentInChildren<ARCameraManager>();
            _arSession = _arSessionManagerGameObject.GetComponentInChildren<ARSession>();
            _arInputManager = _arSessionManagerGameObject.GetComponentInChildren<ARInputManager>();
            _arTrackedImageManager = _arSessionManagerGameObject.GetComponentInChildren<ARTrackedImageManager>();
            arTrackedImageEventManager = _arSessionManagerGameObject.GetComponentInChildren<ARTrackedImageEventManager>();
            arTrackedImageReferenceManager =
                _arSessionManagerGameObject.GetComponentInChildren<ARTrackedImageReferenceManager>();
            
            arCamera = _arSessionManagerGameObject.GetComponentInChildren<Camera>();


            if (_arSessionOriginGameObject == null)
                throw new Exception("GameObject AR Session Origin not found.");
            if (_arSessionGameObject == null)
                throw new Exception("GameObject AR Session not found.");

            if (arCameraManager == null)
                throw new Exception("ARCameraManager not found.");

            if (_arSession == null)
                throw new Exception("ARSession not found.");

            if (_arInputManager == null)
                throw new Exception("ARInputManager not found.");
            
            if(_arTrackedImageManager == null)
                throw new Exception("ARTrackedImageManager not found.");
            
            if(arTrackedImageEventManager == null)
                throw new Exception("ARTrackedImageEventManager not found.");
            
            if(arTrackedImageReferenceManager == null)
                throw new Exception("ARTrackedImageReferenceManager not found.");

            if (arCamera == null)
                throw new Exception("Camera not found.");
        }
        
        private async UniTask WaitForARSessionReady()
        {
            Debug.Log($"Start WaitFor: {ARSession.state}");

            while (ARSession.state != ARSessionState.SessionTracking)
            {
                Debug.Log($"Waiting: {ARSession.state}");
                await UniTask.NextFrame();
            }
        }
    }
}