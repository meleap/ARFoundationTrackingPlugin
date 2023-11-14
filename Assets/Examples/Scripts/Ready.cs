using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Hado.ARFoundation;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Ready : MonoBehaviour
{
    [SerializeField] private Button btnNoFocus;
    [SerializeField] private Button btnAutoFocus;
    
    private void Start()
    {
        btnNoFocus.OnClickAsAsyncEnumerable()
           .ForEachAwaitAsync(async _ => await OnPressNoFocus());

        btnAutoFocus.OnClickAsAsyncEnumerable()
            .ForEachAwaitAsync(async _ => await OnPressAutoFocus());
    }


    private async UniTask OnPressNoFocus()
    {
        Debug.Log("===OnPressNoFocus===");
        await SceneManager.LoadSceneAsync("SampleScene", LoadSceneMode.Single);
        await ARSessionManager.Instance.PowerOnAsync();
    }

    private async UniTask OnPressAutoFocus()
    {
        Debug.Log("===OnPressAutoFocus===");

        SceneManager.LoadSceneAsync("SampleScene", LoadSceneMode.Single);
        await ARSessionManager.Instance.PowerOnAsync(autoFocus:true);
    }
}