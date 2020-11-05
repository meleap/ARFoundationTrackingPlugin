using UnityEngine;

namespace Hado.ARFoundation
{
    public class Anchor : MonoBehaviour
    {
        [SerializeField] private GameObject[] _debugObjects;
        
        public string Name { get; set; }
        
        private void Awake()
        {
            foreach (var obj in (_debugObjects))
            {
                obj.SetActive(Debug.isDebugBuild);
            }
        }
    }
}