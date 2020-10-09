using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

public class WorldAnchorManager : MonoBehaviour
{
    [SerializeField]
    public Text worldAnchorInfoText;

    [SerializeField]
    public GameObject WorldAnchor;

    /// <summary>
    /// マーカーの設定から逆算してゲーム世界の中心にWorldAnchorを移動させます
    /// </summary>
    /// <param name="trackedImage">認識しているマーカーの情報</param>
    public void SetOffset(ARTrackedImage trackedImage)
    {
        var targetData = ImageTargetMaster.FindItem(trackedImage.referenceImage.name);
        if (targetData == null)
            throw new Exception("マーカーの定義情報が見つかりませんでした");

        var originTransformationMatrix = Matrix4x4.TRS(targetData.position, targetData.rotation, Vector3.one).inverse;
        WorldAnchor.transform.localPosition = originTransformationMatrix.MultiplyPoint3x4(WorldAnchor.transform.localPosition);
        WorldAnchor.transform.rotation = WorldAnchor.transform.rotation * Quaternion.Inverse(targetData.rotation);
    }
}
