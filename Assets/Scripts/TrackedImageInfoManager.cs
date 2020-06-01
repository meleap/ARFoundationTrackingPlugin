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
    // マーカの位置リスト
    // マーカの名前を登録した画像と同じ名前にすること
    [Tooltip("unity座標系のマーカリスト（マーカオブジェクトの名前をリファレンス画像ライブラリに登録した名前にすること）")]
    [SerializeField]
    List<Transform> _MarkerTransformOnUnityCoordinate = null;
    
    [Tooltip("AR座標系の回転のうちY軸方向のみを修正するか,falseの場合は全ての軸で」調整を行う")]
    public bool isOnlyAdjustYAxis = true; // AR座標系の回転のうちY軸方向のみを修正するか

    [Header("Adjust method")]
    [Tooltip("座標系の調整にかける時間[sec]（0だとすぐに調整終了 ）")]
    public float moveTime = 1.0f;
    [Tooltip("座標系調整のインターバル.前回の座標調整からこの時間経過しないと座標調整が行われない")]
    public float minIntervalTime = 2.0f; // 座標系調整のインターバル.前回の座標調整からこの時間経過しないと座標調整が行われない
    private float _previousAdjustTime = 0; // 前回の座標系調整時間

    [Tooltip("AR座標系の基準")]
    Transform _arOriginTransform = null;

    // マーカの名前からUnity座標系におけるマーカーのTransformを引くための辞書
    private Dictionary<string, Transform> _mDicMarkerNameToTransform;

    ARTrackedImageManager _mTrackedImageManager;

    // 座標系を時間をかけて調整するためのクラス
    private TransformMover _arOriginMover;

    [Header("Debug")]
    public bool isDebug = false;
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
    GameObject trackedImagePrefab = null;

    //[SerializeField]
    //[Tooltip("If an image is detected but no source texture can be found, this texture is used instead.")]
    //Texture2D m_DefaultTexture;

    /// <summary>
    /// If an image is detected but no source texture can be found,
    /// this texture is used instead.
    /// </summary>
    //public Texture2D defaultTexture
    //{
    //    get { return m_DefaultTexture; }
    //    set { m_DefaultTexture = value; }
    //}

    void Awake()
    {
        Debug.Log("Screen width: " + Screen.width + ", Screen height: " + Screen.height);
        _previousAdjustTime = Time.time;
        _mTrackedImageManager = GetComponent<ARTrackedImageManager>();
        _mTrackedImageManager.trackedImagePrefab = trackedImagePrefab;

        _arOriginTransform = transform;
        _arOriginMover = gameObject.AddComponent<TransformMover>();
        _arOriginMover.target = _arOriginTransform;
        _arOriginMover.moveTime = moveTime;

        // マーカtransform取得用の辞書を作成
        _mDicMarkerNameToTransform = new Dictionary<string, Transform>();
        for (int i=0; i < _MarkerTransformOnUnityCoordinate.Count; i++)
        {
            var referenceImage = _mTrackedImageManager.referenceLibrary[i];
            var referenceImageName = referenceImage.name;
            // Debug.Log("Marker " + i + ", name: " + name);
            foreach (var t in _MarkerTransformOnUnityCoordinate)
            {
                if (t.gameObject.name == referenceImageName)
                {
                    _mDicMarkerNameToTransform.Add(referenceImageName, t);
                }
            }
        }
        // 辞書の内容確認
        // Debug.Log("DicMarkerNameToTransform: " + _mDicMarkerNameToTransform.Count);
        // foreach (var pair in _mDicMarkerNameToTransform)
        // {
        //     Debug.Log("Pair: " + pair.Key + "->" + pair.Value);
        // }
    }

    void OnEnable()
    {
        _mTrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        _mTrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
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

    // 検出されたマーカに対する処理
    // 複数のマーカが同時に検出されないことを前提にしている
    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // インターバルチェック
        if (Time.time - _previousAdjustTime < minIntervalTime)
        {
            return;
        }

        // Debug.Log("Added: " + eventArgs.added.Count);
        // Debug.Log("Updated: " + eventArgs.updated.Count);
        // Debug.Log("Removed: " + eventArgs.removed.Count);

        // このフレームで初めて検出されたマーカの処理
        foreach (var trackedImage in eventArgs.added)
        {
            UpdateOrigin(trackedImage);
            if (isDebug)
            {
                Debug.Log("Add " + trackedImage.referenceImage.name);
                UpdateInfo(trackedImage);
            }
            // Give the initial image a reasonable default scale
            // trackedImage.transform.localScale = new Vector3(0.01f, 1f, 0.01f);
        }

        // このフレームで更新されたマーカの処理
        foreach (var trackedImage in eventArgs.updated)
        {
            UpdateOrigin(trackedImage);
            if (isDebug)
            {
                Debug.Log("Update " + trackedImage.referenceImage.name);
                UpdateInfo(trackedImage);
            }
        }
    }
}
