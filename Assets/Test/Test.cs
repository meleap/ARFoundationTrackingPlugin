using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField]
    GameObject worldAnchor;

    [SerializeField]
    GameObject Marker;

    void Start()
    {
        var imageTarget = ImageTargetMaster.FindItem("PvP_Marker_2_0_1_Blue");

        var baseTransform = gameObject.transform;


        var p1 = worldAnchor.transform.localPosition;

        // marker position
        //  (0, 1.33f, -5f),
        // -90f, -180, 0

        // origin position
        // (0, 5, -1.33)
        // (-90, 0, -180)

        // var originTransformationMatrix = Matrix4x4.TRS(imageTarget.Position, imageTarget.Rotation, Vector3.one);
        // // worldAnchor.transform.localRotation = imageTarget.Rotation * baseTransform.rotation;
        // worldAnchor.transform.localPosition = originTransformationMatrix.MultiplyPoint3x4(baseTransform.position);

        var originTransformationMatrix = Matrix4x4.TRS(imageTarget.Position, imageTarget.Rotation, Vector3.one);
        var inv_originTransformationMatrix = originTransformationMatrix.inverse;
        // Marker.transform.localPosition = originTransformationMatrix.MultiplyPoint3x4(Marker.transform.localPosition);
        // Marker.transform.localRotation = Marker.transform.rotation * imageTarget.Rotation;

        worldAnchor.transform.localPosition = inv_originTransformationMatrix.MultiplyPoint3x4(worldAnchor.transform.localPosition);
        worldAnchor.transform.rotation = worldAnchor.transform.rotation * imageTarget.Rotation;

    }


}
