using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARTrackedImageOffsetManager : MonoBehaviour
{
    public GameObject Anchor;

    public void SetAnchorTransform(ImageTargetOffset offset)
    {
        var m = Matrix4x4.TRS(offset.position, offset.rotation, Vector3.one).inverse;
        Anchor.transform.localPosition = m.MultiplyPoint3x4(Anchor.transform.localPosition);
        Anchor.transform.rotation = Anchor.transform.rotation * Quaternion.Inverse(offset.rotation);
    }
}
