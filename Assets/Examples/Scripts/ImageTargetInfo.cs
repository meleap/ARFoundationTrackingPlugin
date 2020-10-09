using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ImageTargetMaster
{
    public static ImageTargetTransform[] ImageTargets =
    {
        new ImageTargetTransform(
            "PvP_Marker_2_0_1_Blue",
            new Vector3(0, 1.33f, -5f),
            Quaternion.Euler(-90f, -180, 0)
        ),

        new ImageTargetTransform(
            "PvP_Marker_2_0_1_Red",
            new Vector3(0, 0.0133f, 0.05f),
            Quaternion.Euler(-90f, 0, 0)
        ),

        new ImageTargetTransform(
            "PvP_Marker_3_0_4_Red",
            new Vector3(0, 1.33f, 5f),
            Quaternion.Euler(-90f, 0, 0)
        ),

        new ImageTargetTransform(
            "PvP_Marker_3_0_4_Blue",
            new Vector3(0, 1.33f, 5f),
            Quaternion.Euler(-90f, -180, 0)
        ),
    };

    public static ImageTargetTransform FindItem(string targetName)
    {
        return ImageTargets.First(x => x.Name == targetName);
    }
}

public class ImageTargetTransform
{
    public string Name { get; set; }
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }

    public ImageTargetTransform(string name, Vector3 pos, Quaternion rot)
    {
        Name = name;
        Position = pos;
        Rotation = rot;
    }
}
