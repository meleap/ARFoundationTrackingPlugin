using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace Hado.ARFoundation
{
    public class ARTrackedImageReferenceManager : MonoBehaviour
    {
        [SerializeField] public XRReferenceImageLibrary[] MarkerSets;

        private int _defaultMarkerSet = 0;
        
        void Awake()
        {
            if(MarkerSets.Length == 0)
                Debug.LogWarning("MarkerSets is not defined");
        }

        public XRReferenceImageLibrary GetMarkerSet(int num)
        {
            if(num > MarkerSets.Length)
                throw new Exception("invalid marker set number");
            return MarkerSets.Length == 0 ? null : MarkerSets[num];
        }
    }
}