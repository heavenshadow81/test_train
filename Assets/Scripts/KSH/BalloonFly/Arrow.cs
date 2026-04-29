using Bax.P0.Client.UnityWorld.BalloonGame;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//화살 상태 
public enum ArrowMove
{ 
    none,load , Shoot , End
}
public class Arrow : MonoBehaviour
{
    //화살 콜라이더
    public Collider2D Collider;
    //화살 상태
    public ArrowMove arrowMove  = ArrowMove.none;
    //바람 이펙트
    public ParticleSystem arrowWind;
    private void Awake()
    {
        //화살에 달려있는 컬라이더
        Collider = GetComponent<Collider2D>();
        //화살을 살짝 감싸고 있는 바람 이펙트
        arrowWind = GetComponentInChildren<ParticleSystem>();
    }

    private void OnEnable()
    {
        //활성화시 컬라이더 활성화
        Collider.enabled = true;
    }

    private void Update()
    {
        //화살이 발사중일때
        if (arrowMove == ArrowMove.Shoot)
        {
            //바람이펙트 실행
            arrowWind.Play();
            //right 방향으로 발사 (Position이동)
            transform.Translate(Vector3.right * Time.deltaTime * 20f);
        }
    }


    //구름에 충돌
    private void CloudEnter(Collider2D collision)
    {
        //구름일때
        if (collision.CompareTag("Cloud"))
        {
            //구름에 박힐때 소리사운드
            SoundMGR.Instance.SoundPlay("22.구름화살박힘");
            //구름의 컴포넌트 접근 
            if (collision.TryGetComponent<BalloonCloud>(out var cloud))
            {  //구름상태 End
                arrowMove = ArrowMove.End;
                //구름을 부모로 설정
                transform.SetParent(collision.transform);
                //컬라이더 비활성
                Collider.enabled = false;
                //구름에 박힌 녀석들만 저장
                cloud.CloudArrow.Add(this);
            }
        }
    }
    //맵 바깥 벽에 충돌
    private void wallEnter(Collider2D collision) 
    {
        //벽일때
        if (collision.gameObject.name == "Wall")
        {
            //화살 풀링에 넣음 
            BalloonMgr.instance.bow.arrowPulling.ReturnObj(this);
        }
    }

    //풍선에 충돌
    private async void balloonEnter(Collider2D collision) 
    {
        //풍선일때
        if (collision.CompareTag("Balloon"))
        {
            //생성된 풍선갯수 카운팅 -1
            --BalloonMgr.instance.balloonSetter.BalloonCnt;

            //풍선 터지는 사운드
            SoundMGR.Instance.SoundPlay("22.풍선터짐");
            //풍선의 컴포넌트에 접근
            if (collision.transform.parent.TryGetComponent<Balloon>(out var balloon))
            {
                //풍선 풀링에 넣음
                balloon.ReturnObj();
                //화살 풀링에 넣음
                BalloonMgr.instance.bow.arrowPulling.ReturnObj(this);

                //이펙트 풀링에서 꺼냄
                var efx = BalloonMgr.instance.efxPool.Get();
                //이펙트위치를 풍선의 위치로 조정
                efx.transform.position = balloon.transform.position;
                //1초후
                await UniTask.Delay(System.TimeSpan.FromSeconds(1));
                //이펙트 풀링에 넣음
                efx.ReturnObj();
            }
        }
    }

    //충돌 트리거 체크
    public void OnTriggerEnter2D(Collider2D collision)
    {
        //구름에 박혔을때
        CloudEnter(collision);

        //맵바깥에있는 벽에 박혔을때
        wallEnter(collision);

        //풍선에 박혔을때 
        balloonEnter(collision);
        
        //바람파티클 정지
        arrowWind.Stop();

        //게임 종료 조건
        BalloonMgr.instance.GameOver();
    }
}
