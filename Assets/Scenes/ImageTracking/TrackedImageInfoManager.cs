using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

/// This component listens for images detected by the <c>XRImageTrackingSubsystem</c>
/// and overlays some information as well as the source Texture2D on top of the
/// detected image.
/// </summary>
[RequireComponent(typeof(ARTrackedImageManager))]
public class TrackedImageInfoManager : MonoBehaviour
{

    [Tooltip("AR座標系の回転のうちY軸方向のみを修正するか,falseの場合は全ての軸で」調整を行う")]
    public bool isOnlyAdjustYAxis = true; // AR座標系の回転のうちY軸方向のみを修正するか

    // 前回の座標系調整時間
    private float _previousAdjustTime = 0; 

    [Tooltip("AR座標系の基準")]
    Transform _arOriginTransform = null;

    // マーカの名前からUnity座標系におけるマーカーのTransformを引くための辞書
    private Dictionary<string, Transform> _mDicMarkerNameToTransform;

    // 座標系を時間をかけて調整するためのクラス
    private TransformMover _arOriginMover;

    [SerializeField]
    [Tooltip("The camera to set on the world space UI canvas for each instantiated image info.")]
    Camera m_WorldSpaceCanvasCamera;

    /// <summary>
    /// The prefab has a world space UI canvas,
    /// which requires a camera to function properly.
    /// </summary>
    public Camera worldSpaceCanvasCamera
    {
        get { return m_WorldSpaceCanvasCamera; }
        set { m_WorldSpaceCanvasCamera = value; }
    }

    [SerializeField]
    [Tooltip("If an image is detected but no source texture can be found, this texture is used instead.")]
    Texture2D m_DefaultTexture;

    /// <summary>
    /// If an image is detected but no source texture can be found,
    /// this texture is used instead.
    /// </summary>
    public Texture2D defaultTexture
    {
        get { return m_DefaultTexture; }
        set { m_DefaultTexture = value; }
    }

    ARTrackedImageManager m_TrackedImageManager;

    void Awake()
    {
        m_TrackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable()
    {
        m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    // マーカTransform用のY軸回転角度計算
    // マーカは画像面から手前方向がY軸正のため、マーカを壁に貼るとマーカ上方はZ軸正となる
    float GetMarkerYAngle(Quaternion rotation)
    {
        var forward = rotation * Vector3.down;
        float yAngle = Mathf.Atan2(forward.x, forward.z);
        return yAngle;
    }

    // 検出されたマーカの相対位置に基づいて原点を調整する
    private void UpdateOrigin(ARTrackedImage trackedImage)
    {
        // トラッキングしていない画像は処理しない
        if (trackedImage.trackingState == TrackingState.None)
        {
            return;
        }

        // マーカに対応するTransformを取得
        if (!_mDicMarkerNameToTransform.ContainsKey(trackedImage.referenceImage.name))
        {
            Debug.LogError("not found name: " + trackedImage.referenceImage.name);
            return;
        }

        // 検出した画像の位置姿勢がUnity座標系の対応マーカーの位置姿勢と同じになるようにAROriginの位置姿勢を計算
        // スケールは調整しない
        var markerTransform = _mDicMarkerNameToTransform[trackedImage.referenceImage.name];
        var trackedImageTransform = trackedImage.transform;
        Quaternion rotation;
        Vector3 position;
        if (isOnlyAdjustYAxis)
        {
            // 垂直方向が変化しないようにY回転のみ調整する
            Debug.Log(GetMarkerYAngle(markerTransform.rotation) * Mathf.Rad2Deg + ", "  + GetMarkerYAngle(trackedImageTransform.localRotation) * Mathf.Rad2Deg);

            float correctYAngle = GetMarkerYAngle(markerTransform.rotation) - GetMarkerYAngle(trackedImageTransform.localRotation);

            rotation = Quaternion.AngleAxis(correctYAngle * Mathf.Rad2Deg, Vector3.up);
            position = markerTransform.position - rotation * trackedImageTransform.localPosition;
        }
        else
        {
            // 姿勢も完全にマーカ基準で調節する
            rotation = markerTransform.rotation * Quaternion.Inverse(trackedImageTransform.localRotation);
            position = markerTransform.position - _arOriginTransform.rotation * trackedImageTransform.localPosition;
        }

        // AROriginの調整開始
        _arOriginMover.StartMove(position, rotation);

        _previousAdjustTime = Time.time;

        //Debug.Log(
        //    "global position: " + trackedImageTransform.position + Environment.NewLine
        //    + "global rotation: " + trackedImageTransform.rotation + Environment.NewLine
        //    + "local position: " + trackedImageTransform.localPosition + Environment.NewLine
        //    + "local rotation: " + trackedImageTransform.localRotation + Environment.NewLine
        //    );
    }

    // 検出したマーカに関する情報をマーカに重畳表示する（デバッグ用）
    // 使用の際はARTrackedImageManagerのTrackedImagePrefabをセットすること
    void UpdateInfo(ARTrackedImage trackedImage)
    {
        // Set canvas camera
        var canvas = trackedImage.GetComponentInChildren<Canvas>();
        canvas.worldCamera = worldSpaceCanvasCamera;

        var trackedImageTransform = trackedImage.transform;

        // Update information about the tracked image
        var text = canvas.GetComponentInChildren<Text>();
        text.text = string.Format(
            "{0}\ntrackingState: {1}\nGUID: {2}\nReference size: {3} cm\nDetected size: {4} cm\nPosition(local): {5}\nRotation(local): {6}\nEuler angle(local): {7}\nPosition(global): {8}\nRotation(global): {9}\nEuler angle(global): {10}",
            trackedImage.referenceImage.name,
            trackedImage.trackingState,
            trackedImage.referenceImage.guid,
            trackedImage.referenceImage.size * 100f,
            trackedImage.size * 100f,
            trackedImageTransform.localPosition,
            trackedImageTransform.localRotation,
            trackedImageTransform.localRotation.eulerAngles,
            trackedImageTransform.position,
            trackedImageTransform.rotation,
            trackedImageTransform.rotation.eulerAngles
        );

        var planeParentGo = trackedImage.transform.GetChild(0).gameObject;
        var planeGo = planeParentGo.transform.GetChild(0).gameObject;

        // Disable the visual plane if it is not being tracked
        if (trackedImage.trackingState != TrackingState.None)
        {
            planeGo.SetActive(true);

            // The image extents is only valid when the image is being tracked
            trackedImage.transform.localScale = new Vector3(trackedImage.size.x, 1f, trackedImage.size.y);
        }
        else
        {
            planeGo.SetActive(false);
        }
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            // Give the initial image a reasonable default scale
            trackedImage.transform.localScale = new Vector3(0.01f, 1f, 0.01f);

            UpdateInfo(trackedImage);
        }

        foreach (var trackedImage in eventArgs.updated)
            UpdateInfo(trackedImage);
    }
}
