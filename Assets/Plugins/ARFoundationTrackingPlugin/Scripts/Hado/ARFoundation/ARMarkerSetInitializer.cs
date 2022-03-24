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
                return;
            }
            
            // とりあえず先頭をデフォルトに設定
            ARMarkerManager.Instance.ChangeMarkerSet(arMarkers[0].SetName);
        }
    }


    
}
