using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Hado.ARFoundation
{
    public class ARSessionManager : MonoBehaviour
    {
        private ARSession _arSession;

        private void Start()
        {
            _arSession = GetComponent<ARSession>();
        }

        private void OnDisable()
        {
            Debug.Log("Reset ARSession");
            _arSession.Reset();
        }
    }
}