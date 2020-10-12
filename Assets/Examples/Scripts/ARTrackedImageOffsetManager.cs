using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARTrackedImageOffsetManager : MonoBehaviour
{
    public GameObject Anchor;

    public void SetAnchorTransform(ImageTargetOffset offset)
    {
        // Anchor.transform.localPosition = Vector3.zero;
        // Anchor.transform.localRotation = Quaternion.identity;

        var m = Matrix4x4.TRS(offset.position, offset.rotation, Vector3.one).inverse;
        // // // Marker.transform.localPosition = originTransformationMatrix.MultiplyPoint3x4(Marker.transform.localPosition);
        // // // Marker.transform.localRotation = Marker.transform.rotation * imageTarget.Rotation;

        Anchor.transform.localPosition = m.MultiplyPoint3x4(Anchor.transform.localPosition);
        Anchor.transform.rotation = Anchor.transform.rotation * Quaternion.Inverse(offset.rotation);
    }
}
