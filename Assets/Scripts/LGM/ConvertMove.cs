using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Curve
{
    public float limitTime;
    public AnimationCurve curve;
}
public class ConvertMove : MonoBehaviour
{
    private Vector3 pos;
    private float timer;

    public float shakeTime;
    public Curve xCurve;
    public Curve yCurve;

    public Vector2 shakePower;

    private void Start()
    {
        pos = transform.position;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        transform.position = ObjMove(timer, xCurve, yCurve) + pos;
    }

    /// <summary>
    /// 오브젝트 이동
    /// </summary>
    /// <param name="timer"></param> 누적된 시간
    /// <param name="limitTime"></param> 이동하는데 걸리는 시간
    /// <param name="x"></param> 움직일 위치 x
    /// <param name="y"></param> 움직일 위치 y
    /// <param name="z"></param> 움직일 위치 z
    /// <returns></returns>
    public Vector3 ObjMove(float timer,
        Curve x, Curve y)
    {
        // 오브젝트 이동할 위치
        Vector3 movePos = new Vector3(
            x.curve.Evaluate(timer / x.limitTime),
            y.curve.Evaluate(timer / y.limitTime));
        // 이동할 위치 + 기존 위치
        return movePos;
    }

    // 잠깐 흔딜기
    public IEnumerator IShake_Umbrella()
    {
        transform.DOShakePosition(shakeTime, new Vector2(shakePower.x, shakePower.y));
        yield return new WaitForSeconds(0.5f);
    }
}
