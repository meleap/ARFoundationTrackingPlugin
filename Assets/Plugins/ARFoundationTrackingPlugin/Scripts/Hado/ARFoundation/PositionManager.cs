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
        public static PositionManager Instance { get; } = new PositionManager();

        public GameObject WorldAnchor { get; set; }

        public string LastDetectedAnchorName { get; set; }

        public Vector3 GetCalibratedPosition(Vector3 position)
        {
            return WorldAnchor.transform.InverseTransformPoint(position);
        }

        public Quaternion GetCalibratedRotation(Quaternion rotation)
        {
            return Quaternion.Inverse(WorldAnchor.transform.rotation) * rotation;
        }

        public PosRot GetRelativePositionAndRotationFromWorldAnchor(Vector3 receivedPosition, Quaternion receivedRotation)
        {
            if (WorldAnchor != null)
            {
                return new PosRot()
                {
                    position = GetCalibratedPosition(receivedPosition),
                    rotation = GetCalibratedRotation(receivedRotation)
                };
            }

            // Debug.LogError("WorldAnchor is not set yet.");
            return new PosRot()
            {
                position = receivedPosition,
                rotation = receivedRotation
            };
        }
    }
}

