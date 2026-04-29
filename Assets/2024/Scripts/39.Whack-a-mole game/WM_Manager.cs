using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;
using TMPro;

public class WM_Manager : TouchManager_3DTouch
{
    int score; //점수
    [SerializeField] TextMeshProUGUI[] scoreText; //점수텍스트
    [SerializeField] WM_Spawner spawner; //스포너 스크립트
    WM_Record record; //레코드 스크립트

    private void Awake()
    {
        record = gameObject.GetComponent<WM_Record>();
    }

    public override void HandleInput(Vector2 pos)
    {
        isTouchable = true;

        // 마우스 또는 터치 입력을 사용하여 Ray 생성
        Ray ray = Camera.main.ScreenPointToRay(pos);
        RaycastHit hit;

        // Raycast로 오브젝트 감지
        if (Physics.Raycast(ray, out hit))
        {
            string hitName = hit.collider.name.Replace("(Clone)", "").Trim();
            if (spawner.shapesName.Contains(hitName))
            {
                //파티클 생성 후 삭제
                GameObject particle = Instantiate(effect[0], hit.collider.transform.position, Quaternion.identity);
                Destroy(particle,1f);

                //파티클 생성 후 삭제
                GameObject Correctparticle = Instantiate(effect[2], spawner.CorrectShapes[spawner.random].transform.position, Quaternion.identity);
                Destroy(Correctparticle, 1f);

                //정답 오브젝트 삭제
                Destroy(hit.collider.gameObject);
                spawner.shapesName.Remove(hitName);

               //중복 선택 안되게 박스콜라이더 비활성화 하고 새로운 정답판 생성
               spawner.PoneFalse();
                spawner.CorrectExtra();

                //점수 상승
                score += 100;
                scoreText[0].text = string.Format("{0:#,##0}", score);
                scoreText[1].text = string.Format("{0:#,##0}", score);

                //점수 저장
                record.SaveRecord(score);   // 베스트 스코어 저장

                SoundMGR.Instance.SoundPlay("정답");
            }
            else
            {
                //파티클 생성 후 삭제
                GameObject particle = Instantiate(effect[1], hit.collider.transform.position, Quaternion.identity);
                Destroy(particle, 1f);

                //오답 오브젝트 삭제
                Destroy(hit.collider.gameObject);

                //중복 선택 안되게 박스콜라이더 비활성화
                spawner.PoneFalse();

                SoundMGR.Instance.SoundPlay("펑");
            }
        }
        else
        {
            //print("콜라이더없음");
        }
    }
}