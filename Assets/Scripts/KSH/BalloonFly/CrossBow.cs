using Bax.P0.Client.UnityWorld.BalloonGame;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossBow : MonoBehaviour
{

    private float angle = 30f;


    [SerializeField] private float angleZ;

    //화살 풀링
    public PullingMGR<Arrow> arrowPulling;
    //화살 프리팹
    public Arrow arrowPrefabs;

    public float AngleZ
    {
        get => angleZ;
        set 
        {
            angleZ = value;
            transform.localRotation = Quaternion.Euler(0, 0, angleZ);
        }
    }

    public void ArrowSet()
    {
        //풀링 객체 생성
        arrowPulling = new PullingMGR<Arrow>(arrowPrefabs, 10);
        //처음 화살세팅
        BowNLoadArrow();

        //초기각도 저장
        BalloonMgr.instance.saveBowAngle = angle;
        //발사하기 전 화살 화살대 각도
        BowRotate(0.5f);
    }

   
    //발사 하기 전 화살대 각도 Tween
    public void BowRotate(float time = 1)
    {
        // 지정한 각도까지 이동하면
        DOTween.To(() => AngleZ, x => AngleZ = x, BalloonMgr.instance.saveBowAngle, time).OnComplete(() =>
        {
            //반대값으로 전환
            BalloonMgr.instance.saveBowAngle *= -1;
            //재귀
            BowRotate();
        });
       
    }


    //남은 화살이 남았는지 검사 
    //남아있다면 다음 화살 세팅
    public void BowNLoadArrow()
    {
        if (BalloonMgr.instance.ArrowCurCount == 0)
        {
            Debug.Log("남아있는 Arrow가 없슴니다");
            return;
        }

        //남아있는 화살갯수가 있다면
        //화살 생성
        var arrow = arrowPulling.GetObj();
        //화살 부모설정
        arrow.transform.SetParent(transform);
        //로컬위치 설정
        arrow.transform.localPosition = new Vector3(0, 0, 0);
        arrow.transform.localRotation = Quaternion.identity;
        //상태 설정
        arrow.arrowMove = ArrowMove.load;
        //생성한 화살 저장
        BalloonMgr.instance.loadArrow = arrow;
    }

   
}
