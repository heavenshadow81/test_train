using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Stamp : MonoBehaviour
{
    //이미지 랜더
    public SpriteRenderer spriteRenderer;
    //오브젝트 풀용 인터페이스
    public IObjectPool<Stamp> Ipool;

    //스탬프 이미지 제거 (풀에 집어넣음)
    public void ReturnObj()
    {
        spriteRenderer.sprite = null;
        Ipool.Release(this);
    }
}
