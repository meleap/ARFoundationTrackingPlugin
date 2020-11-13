using System.Collections;
using System.Collections.Generic;
using Hado.ARFoundation;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ready : MonoBehaviour
{
    public void OnPressNoFocus()
    {
        Debug.Log("===OnPressNoFocus===");
        ARSessionManager.Instance.PowerOn();

        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }

    public void OnPressAutoFocus()
    {
        Debug.Log("===OnPressAutoFocus===");
        ARSessionManager.Instance.PowerOn(true, true);
        
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }
}