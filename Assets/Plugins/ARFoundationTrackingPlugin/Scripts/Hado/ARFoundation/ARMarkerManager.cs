using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Hado.ARFoundation
{
    public class ARMarkerManager
    {
        public static ARMarkerManager Instance { get; } = new ARMarkerManager();

        public ARMarkerSet[] ARMarkerSetList { get; private set; } = { };

        private ARMarkerSet CurrentMarkerSet { get; set; }

        public XRReferenceImageLibrary CurrentReferenceLibrary => CurrentMarkerSet?.Library;
        
        public void Init(ARMarkerSet[] list)
        {
            ValidateARMarkerSet(list);
            ARMarkerSetList = list;
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
