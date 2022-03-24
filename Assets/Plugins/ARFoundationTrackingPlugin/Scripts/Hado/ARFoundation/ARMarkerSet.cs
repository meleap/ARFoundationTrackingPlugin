using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace Hado.ARFoundation
{
    [CreateAssetMenu(fileName = "ARMarkerSet", menuName = "HADO/ARMarkerSet")]
    public class ARMarkerSet : ScriptableObject
    {
        public string SetName => setName;
        public XRReferenceImageLibrary Library => library;

        public List<ARMarkerOffset> Offsets => offsets.ToList();

        [SerializeField] public string setName;
        [SerializeField] private XRReferenceImageLibrary library;
        [SerializeField] private ARMarkerOffset[] offsets;
    }

    [Serializable]
    public class ARMarkerOffset
    {
        public string MarkerName => markerName;
        public Vector3 Position => position;
        public Quaternion Rotation => Quaternion.Euler(rotation);
        
        [SerializeField] private string markerName;
        [SerializeField] private Vector3 position;
        [SerializeField] private Vector3 rotation;
    }
}
