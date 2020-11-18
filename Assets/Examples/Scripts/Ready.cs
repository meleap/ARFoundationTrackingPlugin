using System;
using System.Collections;
using System.Collections.Generic;
using Hado.ARFoundation;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ready : MonoBehaviour
{
    public void OnPressNoFocus()
    {
        Debug.Log("===OnPressNoFocus===");

        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);

        Observable.Timer(TimeSpan.FromMilliseconds(1000))
            .Subscribe(_ =>
            {
                ARSessionManager.Instance.PowerOn();
            });
    }

    public void OnPressAutoFocus()
    {
        Debug.Log("===OnPressAutoFocus===");

        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);

        Observable.Timer(TimeSpan.FromMilliseconds(1000))
            .Subscribe(_ =>
            {
                ARSessionManager.Instance.PowerOn(true, true);
            });
    }
}