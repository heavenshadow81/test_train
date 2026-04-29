using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Bax.P0.Client.UnityWorld.BalloonGame
{
    public class Balloon : MonoBehaviour
    {
        //풍선 이미지랜더러
        public SpriteRenderer balloonSpRender;

        //풍선 풀링 (인터페이스) 
        //풍선 생성할때 참조
        public IObjectPool<Balloon> Ipool;         
        private void Awake()
        {
            //자식한테 달려있는 스프라이트 랜더러 찾기
            balloonSpRender = GetComponentInChildren<SpriteRenderer>();
        }


        //오브젝트 풀에 집어넣음
        public void ReturnObj()
        {
            Ipool.Release(this);
        }
        public void ReturnObj(Balloon balloon)
        {
            Ipool.Release(balloon);
        }
    }
}