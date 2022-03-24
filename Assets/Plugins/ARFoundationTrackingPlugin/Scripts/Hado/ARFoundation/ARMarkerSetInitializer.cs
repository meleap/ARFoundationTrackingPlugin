using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Hado.ARFoundation
{
    public class ARMarkerSetInitializer : MonoBehaviour
    {
        [SerializeField] private ARMarkerSet[] arMarkers;

        private void Awake()
        {
            ARMarkerManager.Instance.Init(arMarkers);

            if (ARMarkerManager.Instance.ARMarkerSetList.Length == 0)
            {
                Debug.LogWarning("ARMarkerSet is not defined");
            }
        }

        private void Start()
        {
            // とりあえず先頭をデフォルトに設定
            ARSessionManager.Instance.ChangeMarkerSet(arMarkers[0].SetName).Forget();
        }
    }


    
}
