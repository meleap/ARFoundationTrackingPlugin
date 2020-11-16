using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using Hado.ARFoundation;

public class ARSessionController : MonoBehaviour
{
    [SerializeField] private XRReferenceImageLibrary markerLibrary;
    
    
    public void ChangePositionTrackingState(bool state)
    {
        ARSessionManager.Instance.EnabledPositionTracking = state;
    }
    
    public void ChangeAutoFocusState(bool state)
    {
        ARSessionManager.Instance.AutoFocusRequested = state;
    }

    public void ChangeImageTrackingState(bool state)
    {
        ARSessionManager.Instance.EnabledImageTracking = state;
    }

    public void IgnoreRedMarkers()
    {
        IgnoreMarkers("red");
    }

    public void IgnoreBlueMarkers()
    {
        IgnoreMarkers("blue");
    }
    
    private void IgnoreMarkers(string ignoreWord)
    {
        var list = new List<string>();
        foreach (var imageTarget in markerLibrary)
        {
            if (imageTarget.name.ToLower().Contains(ignoreWord))
            {
                list.Add(imageTarget.name);
            }
        }

        PositionManager.Instance.WorldAnchor.GetComponent<WorldAnchorManager>().IgnoreMarkerNames = list.ToArray();
    }
}
