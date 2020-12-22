using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hado.ARFoundation;

public static class ImageTargetOffsetSampleData
{
    public static ImageTargetOffset[] ImageTargets =
    {
        new ImageTargetOffset(
            "PvP_Marker_2_Blue",
            new Vector3(0, 1.33f/100, -5f/100),
            Quaternion.Euler(-90f, -180, 0)
        ),

        new ImageTargetOffset(
            "PvP_Marker_2_Red",
            new Vector3(0, 0, 0),
            Quaternion.Euler(-90f, 0, 0)
        ),

        new ImageTargetOffset(
            "PvP_Marker_3_Blue",
            new Vector3(0, 1.33f, -5f),
            Quaternion.Euler(-90f, -180, 0)
        ),

        new ImageTargetOffset(
            "PvP_Marker_3_Red",
            new Vector3(0, 1.33f, 5f),
            Quaternion.Euler(-90f, 0, 0)
        ),
    };
}
