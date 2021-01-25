using UnityEngine;

namespace Hado.ARFoundation
{
    public class ARSessionInitializer : MonoBehaviour
    {
        private void Awake()
        {
            ARSessionManager.Instance.Init(gameObject);
        }
    }
}
