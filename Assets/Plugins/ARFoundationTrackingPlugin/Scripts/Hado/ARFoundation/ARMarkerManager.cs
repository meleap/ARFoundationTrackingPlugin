using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Hado.ARFoundation
{
    public class ARMarkerManager : MonoBehaviour
    {
        [SerializeField] private ARMarkerSet[] arMarkers;
        
        public static ARMarkerManager Instance { get; private set; }

        public ARMarkerSet[] ARMarkerSetList { get; private set; } = { };

        private ARMarkerSet CurrentMarkerSet { get; set; }

        public XRReferenceImageLibrary CurrentReferenceLibrary => CurrentMarkerSet?.Library;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            } else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Init(arMarkers);
        }
        
        public void Init(ARMarkerSet[] list)
        {
            if (list.Length == 0)
            {
                Debug.LogWarning("ARMarkerSet is not defined");
            } else {
                ValidateARMarkerSet(list);
                ARMarkerSetList = list;
                CurrentMarkerSet = ARMarkerSetList.FirstOrDefault(x => list[0].SetName == x.SetName);
            }
        }

        public void ChangeMarkerSet(ARTrackedImageManager _arTrackedImageManager, string markerSetName)
        {
            Debug.Log($"Change MakerSet: {markerSetName}");
            CurrentMarkerSet = ARMarkerSetList.FirstOrDefault(x => markerSetName == x.SetName);
            _arTrackedImageManager.referenceLibrary = CurrentReferenceLibrary;
        }

        public ARMarkerOffset GetOffsetByMarkerName(string markerName)
        {
            return CurrentMarkerSet.Offsets.FirstOrDefault(x => markerName == x.MarkerName);
        }

        
        
        
        
        /// ここからはバリデーション用
        
        private void ValidateARMarkerSet(ARMarkerSet[] list)
        {
            Debug.Log($"MarkerSet Count: {list.Count()}");
            
            
            foreach (var (arMarkerSet, index) in list.Select((arMarkerSet, index) => (arMarkerSet, index)))
            {
                // MarkerSetName必須
                if(string.IsNullOrEmpty(arMarkerSet.SetName))
                    Debug.LogError($"MarkerSetName is required: {index}");
                
                Debug.Log($"MarkerSetName: {arMarkerSet.SetName}");
                
                // XRReferenceLibrary必須
                if(arMarkerSet.Library == null)
                    Debug.LogError($"Library is Required: {index}");
                
                Debug.Log($"MarkerSet[{arMarkerSet.SetName}] contains {arMarkerSet.Library.count} marker data");

                foreach (var library in arMarkerSet.Library)
                {
                    Debug.Log($"MarkerName: {library.name}");
                    
                    // ↑このマーカー名のオフセットがちゃんとあるか確認
                }
            }
        }

    }
}
