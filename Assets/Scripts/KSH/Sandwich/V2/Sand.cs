using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
public class Sand : MonoBehaviour
{
    //스프라이트 랜더
    public SpriteRenderer sandRnder;
    //샌드위치 타입
    public SandKind SnadKind;
    //오브젝트 풀 (인터페이스)
    public IObjectPool<Sand> Ipool;

    //타입으로 이미지로드
    public async UniTask SpriteLoad(SandKind kind)
    {
       await SandWichManager.instance.loadSprite.LoadSpriteData(kind.ToString(), sandRnder);
    }

    /// <summary>
    /// 샌드위치 부속품 쌓기
    /// </summary>
    /// <param name="y"></param>
    /// <param name="time"></param>
    /// <param name="curve"></param>
    public void Raise(float y , float time,AnimationCurve curve)
    {
        //파라메터y값 만큼 오브젝트 y값 이동 
        transform.DOMoveY(y, time).SetEase(curve).OnComplete(() => {  });
    }

    //샌드위치 부속 위치 세팅 후 풀에 집어넣기
    public void Release()
    {
        transform.position = new Vector3(transform.position.x, 6, 0);
        Ipool.Release(this);
    }
    //샌드위치 부속 위치 세팅 후 풀에 집어넣기
    public void Release(Sand sand)
    {
        transform.position = new Vector3(transform.position.x, 6, 0);
        Ipool.Release(sand);
    }
}
