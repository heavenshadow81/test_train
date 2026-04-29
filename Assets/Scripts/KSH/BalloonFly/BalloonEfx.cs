using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BalloonEfx : MonoBehaviour
{
    //efx 풀링 (인터페이스)
    //efx 생성시 참조
    public IObjectPool<BalloonEfx> Ipool;


    //풀링에 집어넣음 
    public void ReturnObj()
    {
        Ipool.Release(this);
    }
}
