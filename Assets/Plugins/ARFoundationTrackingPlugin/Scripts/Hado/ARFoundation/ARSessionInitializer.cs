using UnityEngine;

namespace Hado.ARFoundation
{
    public class ARSessionInitializer : MonoBehaviour
    {
        private async void Awake()
        {
            ARSessionManager.Instance.Init(gameObject);
            
            await ARSessionManager.Instance.PowerOnAsync(enableImageTracking:false);
            await ARSessionManager.Instance.PowerOffAsync();
        }
    }
}
