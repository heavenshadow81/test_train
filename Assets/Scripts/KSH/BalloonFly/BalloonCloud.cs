using Bax.P0.Client.UnityWorld.BalloonGame;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BalloonCloud : MonoBehaviour
{
    public List<Arrow> CloudArrow = new List<Arrow>();          //박혀있는 화살 저장용 리스트

    private float speed = 0.7f;
    // Update is called once per frame
    void Update()
    {
        transform.Translate(transform.up * Time.deltaTime * -speed);             //우측에서 좌측으로 이동

        if (transform.position.y >= 7)                                          //대략 7 까지 이동하면
        {
            speed = Random.Range(0.3f, 0.7f);                               //랜덤 속도 설정

            // 0 ~ 6 까지 저장된 수 랜덤셔플 큐  
            //큐에 저장된 데이터가 없으면 다시 저장
            if (BalloonMgr.instance.rndQueue.Count <= 0)
            {
                BalloonMgr.instance.CloudqueueSetting();
            }

            //큐에서 하나씩 꺼내서 구름 위치 세팅
            if (BalloonMgr.instance.rndQueue.TryDequeue(out int rnd))
            { 
                transform.position = new Vector3( 1.3f * rnd, -7, 0);
            }

            //구름이 화면밖에 나가면 구름에 박혀있는 화살들 풀링에 집어넣음.
            foreach (var item in CloudArrow)
            {
                //화살 오브젝트 풀로 집어넣음
                BalloonMgr.instance.bow.arrowPulling.ReturnObj(item);
            }

            //화살저장용 리스트 비워줌
            CloudArrow.Clear();
        }
    }
}
