using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.ARFoundation;

namespace Hado.ARFoundation
{
    public class ARSessionManager : MonoBehaviour
    {
        private const string DummyBlackCanvasName = "DummyBlackCanvas";
        [SerializeField] private GameObject xrOriginGameObject;
        [SerializeField] private ARInputManager arInputManager;
        [SerializeField] private ARTrackedImageManager arTrackedImageManager;
        [SerializeField] private ARSession arSession;
        [SerializeField] private TrackedPoseDriver trackedPoseDriver;
        [SerializeField] public ARCameraManager arCameraManager;
        [SerializeField] public ARTrackedImageEventManager arTrackedImageEventManager;
        [SerializeField] public Camera arCamera;
        [SerializeField] public AROcclusionManager arOcclusionManager;
        private GameObject _dummyBlackCanvas;

        public static ARSessionManager Instance { get; private set; }

        public bool AutoFocusRequested
        {
            set => arCameraManager.autoFocusRequested = value;
        }

        public bool EnabledPositionTracking
        {
            set
            {
                arInputManager.enabled = value;
                trackedPoseDriver.enabled = value;
            }
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

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
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
            trackedPoseDriver.enabled = false;
            arSession.enabled = false;
            arCameraManager.enabled = false;

            var go = Resources.Load<GameObject>(DummyBlackCanvasName);
            // ARCameraを起動したときに前回のラストフレームが一瞬描写される。それを隠すための黒キャンバス
            _dummyBlackCanvas = Instantiate(go, arCamera.transform);
            _dummyBlackCanvas.GetComponent<Canvas>().worldCamera = arCamera;
            _dummyBlackCanvas.GetComponent<Canvas>().planeDistance = 1f;
        }

        public async UniTask PowerOffAsync()
        {
            _dummyBlackCanvas.SetActive(true);

            arTrackedImageEventManager.Clear();
            arSession.Reset();
            trackedPoseDriver.enabled = false;
            EnabledPositionTracking = false;
            EnabledImageTracking = false;
            EnableOcclusion = false;
            arCamera.enabled = false;
            arCameraManager.enabled = false;
            arSession.enabled = false;

            await UniTask.NextFrame();
        }

        public async UniTask PowerOnAsync(bool enableCamera = true,
            bool autoFocus = false,
            int warmupDelay = 1000,
            bool enableImageTracking = true,
            bool enableOcclusion = false,
            bool enablePositionTracking = true,
            bool enableHighResolution = false,
            CancellationToken ct = default)
        {
            arTrackedImageManager.referenceLibrary = ARMarkerManager.Instance.CurrentReferenceLibrary;

            if (autoFocus)
                AutoFocusRequested = true;

            trackedPoseDriver.enabled = true;

            if (enableCamera)
                arCamera.enabled = true;

            arCameraManager.enabled = true;
            EnabledPositionTracking = enablePositionTracking;
            EnabledImageTracking = enableImageTracking;

            arSession.enabled = true;

            try
            {
                await UniTask.Delay(warmupDelay, cancellationToken: ct);
            }
            catch (OperationCanceledException e)
            {
                throw new OperationCanceledException(e.Message);
            }

            if (enableHighResolution)
            {
                // 利用可能なカメラ設定を取得します
                using var configs = arCameraManager.GetConfigurations(Allocator.Temp);

                // 利用可能な最初の 4K カメラ設定に切り替えます
                var fourKResolution = new Vector2(3840, 2160);
                foreach (var c in configs.Where(c => c.resolution == fourKResolution))
                {
                    arCameraManager.currentConfiguration = c;
                    break;
                }
            }

            _dummyBlackCanvas.SetActive(false);

            await UniTask.NextFrame(ct);

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

            if (arTrackedImageManager == null)
                throw new Exception("ARTrackedImageManager not found.");

            if (arTrackedImageEventManager == null)
                throw new Exception("ARTrackedImageEventManager not found.");

            if (arCamera == null)
                throw new Exception("Camera not found.");
        }
    }
}
