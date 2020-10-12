using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hado.ARFoundation
{
    public struct PosRot
    {
        public Vector3 position;
        public Quaternion rotation;
    }


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

        public PosRot GetRelativePositionAndRotationFromWorldAnchor(Vector3 recievedPosition, Quaternion recievedRotation)
        {
            if (WorldAnchor != null)
            {
                var originTransformationMatrix = Matrix4x4.TRS(WorldAnchor.transform.position, WorldAnchor.transform.rotation, WorldAnchor.transform.localScale);
                return new PosRot()
                {
                    position = originTransformationMatrix.MultiplyPoint3x4(recievedPosition),
                    rotation = WorldAnchor.transform.rotation * recievedRotation
                };
            }
            else
            {
                Debug.LogError("WorldAnchor is not set yet.");
                return new PosRot()
                {
                    position = recievedPosition,
                    rotation = recievedRotation
                };
            }
        }
    }
}

