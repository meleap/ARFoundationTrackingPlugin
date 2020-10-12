using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hado.ARFoundation
{
    public class PositionManager
    {
        static PositionManager instance = new PositionManager();

        public static PositionManager Instance
        {
            get
            {
                return instance;
            }
        }

        public GameObject WorldAnchor { get; set; }

        public Vector3 GetCalibratedPosition(Vector3 position)
        {
            return WorldAnchor.transform.InverseTransformPoint(position);
        }

        public Quaternion GetCalibratedRotation(Quaternion rotation)
        {
            return Quaternion.Inverse(WorldAnchor.transform.rotation) * rotation;
        }

        public void GetRelativePositionAndRotationFromWorldAnchor(Vector3 recievedPosition, Quaternion recievedRotation, out Vector3 calibratedPosition, out Quaternion calibratedRotation)
        {
            var originTransformationMatrix = Matrix4x4.TRS(WorldAnchor.transform.position, WorldAnchor.transform.rotation, WorldAnchor.transform.localScale);
            calibratedPosition = originTransformationMatrix.MultiplyPoint3x4(recievedPosition);
            calibratedRotation = WorldAnchor.transform.rotation * recievedRotation;
        }
    }
}

