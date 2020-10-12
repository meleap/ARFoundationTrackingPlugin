using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hado.ARFoundation
{
    public static class ImageTargetOffsetMaster
    {
        public static ImageTargetOffset[] ImageTargets;

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
}