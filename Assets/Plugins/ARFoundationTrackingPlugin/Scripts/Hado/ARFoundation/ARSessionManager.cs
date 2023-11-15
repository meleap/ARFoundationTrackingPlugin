using System;
using System.Threading;
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
        [SerializeField] public AROcclusionManager arOcclusionManager;

        public static ARSessionManager Instance { get; private set; }

        private const string DummyBlackCanvasName = "DummyBlackCanvas";
        private GameObject _dummyBlackCanvas;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            } else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            CheckComponents();
            
            Init();
        }

        private void Init()
        {
            arCamera.enabled = false;
            arInputManager.enabled = false;

            _dummyBlackCanvas = Resources.Load<GameObject>(DummyBlackCanvasName);
        }

        public async UniTask PowerOffAsync()
        {
            EnabledPositionTracking = false;
            EnabledImageTracking = false;
            EnableOcclusion = false;
            arTrackedImageEventManager.Clear();
            arCameraManager.enabled = false;
            arCamera.enabled = false;
            
            await ResetSessionAsync();
        }

        public async UniTask PowerOnAsync(bool enableCamera = true, bool autoFocus = false, int warmupDelay = 1000, bool enableImageTracking = true, bool enableOcclusion = false, CancellationToken ct = default)
        {
            arTrackedImageManager.referenceLibrary = ARMarkerManager.Instance.CurrentReferenceLibrary;
            
            // ARCameraを起動したときに前回のラストフレームが一瞬描写される。それを隠すための黒キャンバス
            var ui = Instantiate(_dummyBlackCanvas, arCamera.transform);
            
            ui.GetComponent<Canvas>().worldCamera = arCamera;
            ui.GetComponent<Canvas>().planeDistance = 1f;

            AutoFocusRequested = autoFocus;

            
            if (enableCamera)
                arCamera.enabled = true;

            arCameraManager.enabled = true;
            EnabledPositionTracking = true;
            EnabledImageTracking = enableImageTracking;

            try
            {
                await UniTask.Delay(warmupDelay, cancellationToken: ct);
            }
            catch (OperationCanceledException e)
            {
                Destroy(ui);
                throw new OperationCanceledException(e.Message);
            }
            
            Destroy(ui);

            await UniTask.NextFrame(cancellationToken: ct);

            EnableOcclusion = enableOcclusion;
        }

        public void ResetSession()
        {
            arTrackedImageEventManager.Clear();
            arSession.Reset();
        }

        public async UniTask ResetSessionAsync(CancellationToken ct = default)
        {
            EnabledImageTracking = false;
            EnabledPositionTracking = false;
            
            await UniTask.WaitWhile(() => ARSession.state != ARSessionState.SessionTracking, cancellationToken: ct);
            
            arTrackedImageEventManager.Clear();
            arSession.Reset();
            
            await UniTask.WaitWhile(() => ARSession.state != ARSessionState.SessionTracking, cancellationToken: ct);
            
            EnabledImageTracking = true;
            EnabledPositionTracking = true;
            await UniTask.WaitWhile(() => ARSession.state != ARSessionState.SessionTracking, cancellationToken: ct);
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
                Debug.Log($"Change EnabledImageTracking: {value}");
                arTrackedImageManager.enabled = value;
                arTrackedImageEventManager.enabled = value;
                
                if (value && ARMarkerManager.Instance.ARMarkerSetList.Length < 1)
                    throw new Exception("ARMarkerSetList is not set.");
            }
        }

        public bool EnableOcclusion
        {
            set => arOcclusionManager.enabled = value;
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
    }
}