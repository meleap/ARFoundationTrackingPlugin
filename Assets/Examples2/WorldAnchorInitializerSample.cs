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

    private WorldAnchorManager _worldAnchorManager;

    private CompositeDisposable _disposable;

    private CancellationTokenSource _ctsIntervalTracking;

    private int click = 0;

    private void Awake()
    {
        ImageTargetOffsetMaster.ImageTargets = ImageTargetOffsetSampleData.ImageTargets;
        _worldAnchorManager = GetComponent<WorldAnchorManager>();

        _disposable = new CompositeDisposable();
    }

    private void Update()
    {
        //    status.text = $"{SystemInfo.deviceModel}";
    }

    private async void Start()
    {
        _worldAnchorManager.IsMoving
            .Buffer(2, 1)
            .Subscribe(data =>
            {
                status.text = $"{data[0]} => {data[1]}";
            });

        await ARSessionManager.Instance.PowerOnAsync();

        // 最初にマーカー認識してからは認識頻度を落とす
        SubscribeFirstAction();

        // ボタン操作によるARSessionのリセット
        btn.OnClickAsObservable()
            .Subscribe(async _ =>
            {
                btn.interactable = false;
                var nextMode = txt.text == "interval" ? "normal" : "interval";
                if (nextMode == "normal")
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
                    _ctsIntervalTracking = new CancellationTokenSource();
                    _disposable.Add(_worldAnchorManager.RegisterIntervalTracking(_ctsIntervalTracking));
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
            .Subscribe(_ =>
            {
                _ctsIntervalTracking = new CancellationTokenSource();
                _disposable.Add(_worldAnchorManager.RegisterIntervalTracking(_ctsIntervalTracking));
            })
            .AddTo(this);
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