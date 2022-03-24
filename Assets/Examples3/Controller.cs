using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;
using Debug = UnityEngine.Debug;

public class Controller : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    [SerializeField] private GameObject Red;
    [SerializeField] private GameObject Blue;

    [SerializeField] private Button btnToRed;
    [SerializeField] private Button btnToBlue;

    [SerializeField] private Button btnToRedNative;
    [SerializeField] private Button btnToBlueNative;

    [SerializeField] private GameObject other;

    private int movingPosId = 0;
    private int movingRotId = 0;
    
    
    // 移動時間
    private const float MOVE_TIME = 1.5f;
    
    private bool _isMove;

    private void Start()
    {
        btnToRed.OnClickAsObservable()
            .Do(_ => LeanTween.cancel(targetObject))
            .Subscribe(_ =>
            {
                LeanTween.move(targetObject, Red.transform, 1.5f).setEaseOutQuint().setOnComplete(() =>
                {
                    Debug.Log("end");
                });
                LeanTween.rotate(targetObject, Red.transform.rotation.eulerAngles, 1.5f).setEaseOutQuint();
            });

        btnToBlue.OnClickAsObservable()
            .Do(_ => LeanTween.cancel(targetObject))
            .Subscribe(_ =>
            {
                LeanTween.move(targetObject, Blue.transform.position, 1.5f).setEaseOutQuint();
                LeanTween.rotate(targetObject, Blue.transform.rotation.eulerAngles, 1.5f).setEaseOutQuint();
            });

        btnToRedNative.OnClickAsObservable()
            .Subscribe(_ =>
            {
                MoveToX(targetObject.transform.position, targetObject.transform.rotation, Red.transform.position,
                    Red.transform.rotation);
            });
        
        btnToBlueNative.OnClickAsObservable()
            .Subscribe(async _ =>
            {
                await MoveToX(targetObject.transform.position, targetObject.transform.rotation, Blue.transform.position,
                    Blue.transform.rotation);
            });
        
        LeanTween.move(other, Blue.transform, 10);
    }


    private async UniTask MoveToX(Vector3 startPos, Quaternion startRot, Vector3 endPos, Quaternion endRot)
    {
        _isMove = true;

        var x = 0f;

        while (_isMove)
        {
            x += Time.deltaTime / MOVE_TIME;

            var lerpPoint = (float)(1 - Math.Pow(1 - x, 5));
            
            Debug.Log($"{x}:{lerpPoint}");

            if (lerpPoint > 1)
            {
                _isMove = false;
                lerpPoint = 1f;
            }

            targetObject.transform.position = Vector3.Lerp(startPos, endPos, lerpPoint);
            targetObject.transform.rotation = Quaternion.Lerp(startRot, endRot, lerpPoint);

            await UniTask.WaitForEndOfFrame();
        }
    }
}