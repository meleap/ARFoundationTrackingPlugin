using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;



[RequireComponent(typeof(ARTrackedImageManager))]
public class WorldAnchorManager : MonoBehaviour
{
    ARTrackedImageManager _arTrackedImageManager;

    void Awake()
    {
        _arTrackedImageManager = GetComponent<ARTrackedImageManager>();

        var hoge = ImageTargetMaster.FindItem("PvP_Marker_2_0_1_Blue");
    }

    void OnEnable()
    {
        _arTrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        _arTrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void UpdateInfo(ARTrackedImage trackedImage)
    {
        // var wpos = trackedImage.gameObject.transform.position;
        // var lpos = trackedImage.gameObject.transform.localPosition;
        // Debug.Log($"worldPosition: ({wpos.x:f3}, {wpos.y:f3}, {wpos.z:f3})");
        // Debug.Log($"localPosition: ({lpos.x:f3}, {lpos.y:f3}, {lpos.z:f3})");
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            // 初回だけの処理はここに
            SetOffset(trackedImage);
            UpdateInfo(trackedImage);
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            UpdateInfo(trackedImage);
        }
    }

    void SetOffset(ARTrackedImage trackedImage)
    {
        var info = trackedImage.GetComponent<WorldAnchorInfo>();
        var targetData = ImageTargetMaster.FindItem(trackedImage.referenceImage.name);

        info.worldAnchorInfoText.text = $"pos: ({targetData.Position.x:f5}, {targetData.Position.y:f5}, {targetData.Position.z:f5})\n";
        info.worldAnchorInfoText.text += $"rot: ({targetData.Rotation.eulerAngles.x}, {targetData.Rotation.eulerAngles.y}, {targetData.Rotation.eulerAngles.z})\n";

        var originGo = trackedImage.transform.Find("Origin").gameObject;
        originGo.transform.localPosition = -1 * targetData.Position;
        // originGo.transform.localPosition = originGo.transform.InverseTransformPoint(targetData.Position);

        originGo.transform.localRotation = Quaternion.Inverse(originGo.transform.localRotation) * targetData.Rotation;
    }
}
