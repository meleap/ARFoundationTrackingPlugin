using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ImageTargetMaster
{
    public static ImageTargetOffset[] ImageTargets =
    {
        new ImageTargetOffset(
            "PvP_Marker_2_0_1_Blue",
            new Vector3(0, 1.33f, -5f),
            Quaternion.Euler(-90f, -180, 0)
        ),

        new ImageTargetOffset(
            "PvP_Marker_2_0_1_Red",
            new Vector3(0, 1.33f, 5f),
            Quaternion.Euler(-90f, 0, 0)
        ),

        new ImageTargetOffset(
            "PvP_Marker_3_0_4_Blue",
            new Vector3(0, 1.33f, -5f),
            Quaternion.Euler(-90f, -180, 0)
        ),

        new ImageTargetOffset(
            "PvP_Marker_3_0_4_Red",
            new Vector3(0, 1.33f, 5f),
            Quaternion.Euler(-90f, 0, 0)
        ),

    };

    public static ImageTargetOffset FindItem(string targetName)
    {
        return ImageTargets.First(x => x.Name == targetName);
    }
}

public class ImageTargetOffset
{
    public string Name { get; set; }
    public Vector3 position { get; set; }
    public Quaternion rotation { get; set; }

    public ImageTargetOffset(string name, Vector3 pos, Quaternion rot)
    {
        Name = name;
        position = pos;
        rotation = rot;
    }
}
