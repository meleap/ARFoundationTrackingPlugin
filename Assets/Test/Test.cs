using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField]
    GameObject worldAnchor;

    [SerializeField]
    GameObject BlueMarker;

    [SerializeField]
    GameObject RedMarker;

    void Start()
    {
        SetMarker("PvP_Marker_2_0_1_Blue", BlueMarker);
        //SetMarker("PvP_Marker_3_0_4_Red", RedMarker);
    }

    void SetMarker(string imageTargetName, GameObject marker)
    {
        worldAnchor.transform.SetParent(marker.transform);
        worldAnchor.transform.localPosition = Vector3.zero;
        worldAnchor.transform.localRotation = Quaternion.identity;

        var imageTarget = ImageTargetMaster.FindItem(imageTargetName);
        var m = Matrix4x4.TRS(imageTarget.position, imageTarget.rotation, Vector3.one).inverse;
        // Marker.transform.localPosition = originTransformationMatrix.MultiplyPoint3x4(Marker.transform.localPosition);
        // Marker.transform.localRotation = Marker.transform.rotation * imageTarget.Rotation;

        worldAnchor.transform.localPosition = m.MultiplyPoint3x4(worldAnchor.transform.localPosition);
        worldAnchor.transform.rotation = worldAnchor.transform.rotation * Quaternion.Inverse(imageTarget.rotation);
    }
}
