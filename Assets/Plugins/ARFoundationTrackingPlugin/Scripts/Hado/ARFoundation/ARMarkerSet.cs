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

        [SerializeField] private string setName;
        [SerializeField] private XRReferenceImageLibrary library;
        [SerializeField] private ARMarkerOffset[] offsets;
        
        public void ReplaceOffsetList(ARMarkerOffset[] newOffsets)
        {
            offsets = newOffsets;
        }
    }

    [Serializable]
    public class ARMarkerOffset
    {
        public string MarkerName => markerName;
        public Vector3 Position => position;
        public Quaternion Rotation => Quaternion.Euler(rotation);

        public ARMarkerOffset(string name, Vector3 pos, Vector3 rot)
        {
            markerName = name;
            position = pos;
            rotation = rot;
        }

        [SerializeField] private string markerName;
        [SerializeField] private Vector3 position;
        [SerializeField] private Vector3 rotation;
    }
}
