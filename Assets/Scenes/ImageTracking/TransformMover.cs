using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Transformの位置と姿勢を、目的とする位置と姿勢まで時間をかけて変更するクラス
public class TransformMover : MonoBehaviour
{
    // 変更する方法
    public enum MoveMode
    {
        Simultaneous,     // 位置、姿勢同時
        PositionRotation, // 位置 → 姿勢の順
        RotationPosition  // 姿勢 → 位置の順
    }
    // 変更にかける時間
    public float moveTime = 1;
    // 変更する対象
    public Transform target;
    // 変更方法
    public MoveMode moveMode = MoveMode.Simultaneous;
    private bool _isMoving = false; // 現在変更中かどうか

    // 補間用変数
    private float _startTime;              // 変更開始時間
    private Vector3 _startPos, _endPos;    // 変更開始時の位置と変更終了時の位置（目標とする位置）
    private Quaternion _startRot, _endRot; // 変更開始時の姿勢と変更終了時の姿勢（目標とする姿勢）

    // 変更開始
    // 変更中に呼び出すと目標位置と回転は上書きされる
    // 経過時間はリセットされる
    public bool StartMove(Vector3 targetPos, Quaternion targetRot)
    {
        if (target == null)
        {
            return false;
        }

        if (moveTime <= 0f)
        {
            target.position = targetPos;
            target.rotation = targetRot;
            _isMoving = false;
            return true;
        }

        _startTime = Time.time;
        _startPos = target.position;
        _startRot = target.rotation;
        _endPos = targetPos;
        _endRot = targetRot;
        _isMoving = true;
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isMoving)
        {
            return;
        }

        // 変更終了
        var elapsedTime = Time.time - _startTime;
        // Debug.Log(elapsedTime);
        if (elapsedTime > moveTime)
        {
            target.position = _endPos;
            target.rotation = _endRot;
            _isMoving = false;
            return;
        }

        float fracComplete; // 経過時間の割合
        switch (moveMode)
        {
            case MoveMode.Simultaneous:
                // 位置と回転を同時に変更する（変な動きで修正される）
                fracComplete = elapsedTime / moveTime;
                target.position = Vector3.Lerp(_startPos, _endPos, fracComplete);
                target.rotation = Quaternion.Lerp(_startRot, _endRot, fracComplete);
                break;
            case MoveMode.PositionRotation:
                // 位置を修正してから回転を変更する
                if (elapsedTime < moveTime * 0.5)
                {
                    fracComplete = elapsedTime / (moveTime * 0.5f);
                    target.position = Vector3.Lerp(_startPos, _endPos, fracComplete);
                }
                else
                {
                    fracComplete = (elapsedTime - moveTime * 0.5f) / (moveTime * 0.5f);
                    target.rotation = Quaternion.Lerp(_startRot, _endRot, fracComplete);
                }
                break;
            case MoveMode.RotationPosition:
                // 回転を修正してから位置を変更する
                if (elapsedTime < moveTime * 0.5)
                {
                    fracComplete = elapsedTime / (moveTime * 0.5f);
                    target.rotation = Quaternion.Lerp(_startRot, _endRot, fracComplete);
                }
                else
                {
                    fracComplete = (elapsedTime - moveTime * 0.5f)/ (moveTime * 0.5f);
                    target.position = Vector3.Lerp(_startPos, _endPos, fracComplete);
                }
                break;
        }
    }
}
