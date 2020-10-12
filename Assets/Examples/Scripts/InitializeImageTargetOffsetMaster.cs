using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hado.ARFoundation;

public class InitializeImageTargetOffsetMaster : MonoBehaviour
{
    void Awake()
    {
        ImageTargetOffsetMaster.ImageTargets = ImageTargetOffsetSampleData.ImageTargets;
    }
}
