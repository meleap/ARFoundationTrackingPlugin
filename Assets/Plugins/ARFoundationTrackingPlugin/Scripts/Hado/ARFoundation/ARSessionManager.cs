using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Cysharp.Threading.Tasks;
using UnityEngine.Serialization;

namespace Hado.ARFoundation
{
    public class ARSessionManager : MonoBehaviour
    {
        [SerializeField] private GameObject xrOriginGameObject;
        [SerializeField] private ARInputManager arInputManager;
        [SerializeField] private ARTrackedImageManager arTrackedImageManager;
        [SerializeField] private ARSession arSession;
        
        [SerializeField] public ARCameraManager arCameraManager;
        [SerializeField] public ARTrackedImageEventManager arTrackedImageEventManager;
        [SerializeField] public Camera arCamera;

        public static ARSessionManager Instance { get; private set; }

        private const string DummyBlackCanvasName = "DummyBlackCanvas";
        private GameObject _dummyBlackCanvas;

        private void Awake()
        {
            Instance = this;

            CheckComponents();
            
            Init();
        }

        private void Init()
        {
            arCamera.enabled = false;
            arSession.enabled = false;
            arInputManager.enabled = false;

            _dummyBlackCanvas = Resources.Load<GameObject>(DummyBlackCanvasName);
            arTrackedImageManager.referenceLibrary = ARMarkerManager.Instance.CurrentReferenceLibrary;

        }

        public async UniTask PowerOffAsync()
        {
            EnabledPositionTracking = false;
            EnabledImageTracking = false;
            arTrackedImageEventManager.Clear();

            await UniTask.Delay(300);
            arSession.Reset();
            await UniTask.Delay(300);
            arSession.enabled = false;
            arCamera.enabled = false;
        }

        public async UniTask PowerOnAsync(bool enableCamera = true, bool autoFocus = false, int warmupDelay = 1000, bool enableImageTracking = true)
        {
            // ARCameraを起動したときに前回のラストフレームが一瞬描写される。それを隠すための黒キャンバス
            var ui = GameObject.Instantiate(_dummyBlackCanvas, arCamera.transform);
            
            ui.GetComponent<Canvas>().worldCamera = arCamera;
            ui.GetComponent<Canvas>().planeDistance = 1f; 
            
            if (enableCamera)
                arCamera.enabled = true;

            arCameraManager.enabled = true;
            EnabledPositionTracking = true;
            EnabledImageTracking = enableImageTracking;
            arSession.enabled = true;

            await UniTask.Delay(warmupDelay);
            
            GameObject.Destroy(ui);

            await UniTask.NextFrame();

            if(autoFocus)
                AutoFocusRequested = true;
        }

        public void ResetSession()
        {
            arTrackedImageEventManager.Clear();
            arSession.Reset();
        }

        public async UniTask ResetSessionAsync()
        {
            EnabledImageTracking = false;
            EnabledPositionTracking = false;
            
            await WaitForARSessionReady();
            
            arTrackedImageEventManager.Clear();
            arSession.Reset();
            
            await WaitForARSessionReady();
            
            EnabledPositionTracking = true;
            
            await WaitForARSessionReady();
        }

        public bool AutoFocusRequested
        {
            set => arCameraManager.autoFocusRequested = value;
        }

        public bool EnabledPositionTracking
        {
            set => arInputManager.enabled = value;
        }

        public bool EnabledImageTracking
        {
            set
            {
                arTrackedImageManager.enabled = value;
                arTrackedImageEventManager.enabled = value;
                
                if (value && ARMarkerManager.Instance.ARMarkerSetList.Length < 1)
                    throw new Exception("ARMarkerSetList is not set.");
            }
        }

        public async UniTask ChangeMarkerSet(string markerSetName, bool restart = true)
        {
            ARMarkerManager.Instance.ChangeMarkerSet(arTrackedImageManager, markerSetName);

            if (restart)
            {
                await PowerOffAsync();
                await PowerOnAsync(true, arCameraManager.autoFocusEnabled);
            }
        }

        private void CheckComponents()
        {
            if (xrOriginGameObject == null)
                throw new Exception("GameObject XR Origin not found.");

            if (arSession == null)
                throw new Exception("ARSession not found.");

            if (arInputManager == null)
                throw new Exception("ARInputManager not found.");
            
            if(arTrackedImageManager == null)
                throw new Exception("ARTrackedImageManager not found.");
            
            if(arTrackedImageEventManager == null)
                throw new Exception("ARTrackedImageEventManager not found.");
            
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