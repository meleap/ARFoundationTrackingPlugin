using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using Cysharp.Threading.Tasks;
using UniRx;
using Hado.ARFoundation;

public class WorldAnchorInitializerSample : MonoBehaviour
{
    [SerializeField] private Button btn;
    [SerializeField] private Text txt;
    [SerializeField] private Text status;
    
    private CompositeDisposable _disposable;
    
    private CancellationTokenSource _ctsIntervalTracking;

    private int click = 0;

    private void Awake()
    {
        ARSessionManager.Instance.Init(gameObject);
        _disposable = new CompositeDisposable();
    }

    private void Update()
    {
        status.text = $"{SystemInfo.deviceModel}";
    }

    private async void Start()
    {
        ImageTargetOffsetMaster.ImageTargets = HogeData.ImageTargets;
        await ARSessionManager.Instance.PowerOnAsync();

        // 最初にマーカー認識してからは認識頻度を落とす
        SubscribeFirstAction();
        
        // ボタン操作によるARSessionのリセット
        btn.OnClickAsObservable()
            .Subscribe(async _ =>
            {
                btn.interactable = false;
                var nextMode = txt.text == "on" ? "off" : "on";
                if (nextMode == "on")
                {
                    _disposable.Clear();
                    _ctsIntervalTracking?.Cancel();

                    if (SystemInfo.deviceModel.StartsWith("iPhone10"))
                    {
                        await ARSessionManager.Instance.PowerOffAsync();
                        await ARSessionManager.Instance.PowerOnAsync(autoFocus: true);
                    }
                    else
                    {
                        await ARSessionManager.Instance.ResetSessionAsync();
                    }
                }
                else
                {
                    RegisterIntervalTracking();
                }

                txt.text = nextMode;
                btn.interactable = true;
            }).AddTo(this);
    }

    private void SubscribeFirstAction()
    {
        Debug.Log("SubscribeFirstAction");
        ARSessionManager.Instance.arTrackedImageEventManager.OnTrackedImagesChangedObservable
            .Take(1)
            .Subscribe(_ => RegisterIntervalTracking())
            .AddTo(this);
    }

    private void RegisterIntervalTracking()
    {
        Debug.Log("RegisterIntervalTracking");

        _ctsIntervalTracking = new CancellationTokenSource();
        
        ARSessionManager.Instance.arTrackedImageEventManager.OnTrackedImagesChangedObservable
            .Subscribe(async _ =>
            {
                ARSessionManager.Instance.EnabledImageTracking = false;

                await UniTask.Delay(TimeSpan.FromMilliseconds(5000));
                
                if (_ctsIntervalTracking.Token.IsCancellationRequested) return;

                ARSessionManager.Instance.EnabledImageTracking = true;
            })
            .AddTo(_disposable);
    }

    private async UniTask WaitForClicker()
    {
        while (3 > click)
        {
            await UniTask.NextFrame();
        }
    }

    private async UniTask WaitForARSessionReady()
    {
        Debug.Log($"Start WaitFor: {ARSession.state}");

        while (ARSession.state != ARSessionState.SessionTracking)
        {
            Debug.Log($"Waiting: {ARSession.state}");
            await UniTask.NextFrame();
        }
    }
}


public static class HogeData 
{
    public static ImageTargetOffset[] ImageTargets =
    {
        new ImageTargetOffset(
            "PvP_Marker_3_Red",
            new Vector3(0, 0, 0),
            Quaternion.Euler(0, 0, 0)
        ),
    };
}