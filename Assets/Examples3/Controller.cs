using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class Controller : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    [SerializeField] private GameObject Red;
    [SerializeField] private GameObject Blue;

    [SerializeField] private Button btnToRed;
    [SerializeField] private Button btnToBlue;

    [SerializeField] private GameObject other;

    private int movingPosId = 0;
    private int movingRotId = 0;

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

        LeanTween.move(other, Blue.transform, 10);
    }
}