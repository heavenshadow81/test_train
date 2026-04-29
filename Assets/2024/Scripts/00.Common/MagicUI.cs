using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using Windows.Kinect;
using UnityEngine.Events;

public class MagicUI : MonoBehaviour
{
    [Header("게임시작")]
    [SerializeField] TextMeshProUGUI description; //설명 텍스트
    [SerializeField] string Text; //원하는 문구
    [SerializeField] GameObject[] doorObj; //움직일 오브젝트들

    [Header("게임종료")]
    [SerializeField] GameObject[] contents; //게임종료시 끌 콘텐츠전체
    [SerializeField] GameObject result; //게임종료시 뜰 팝업창

    [Header("카테고리")]
    [SerializeField] MagicTimer timerScript; //타이머 스크립트

    bool hasPlayed; //엔딩 사운드 체크

    [SerializeField] UnityEvent OnStart; //게임 시작 시 일어날 이벤트

    void Start()
    {

        //글씨 써지는 효과 3초동안 재생
        SoundMGR.Instance.SoundPlay("타자");
        description.DOText(Text, 3f).SetEase(Ease.Linear).OnComplete(() =>
        {
            //글씨가 다 써지면 문 양옆으로 이동
            doorObj[0].transform.DOLocalMoveX(-2000f, 3f).SetDelay(1);
            doorObj[1].transform.DOLocalMoveX(2000f, 3f).SetDelay(1);
            SoundMGR.Instance.SoundPlay("데구르르");

            //글자랑 프레임 1초 딜레이 후 2초동안 FadeOut 
            description.DOFade(0, 2f).SetDelay(1);
            doorObj[2].GetComponent<Image>().DOFade(0, 2f).SetDelay(1).OnComplete(() =>
            {
                timerScript?.ResumeTimer();
                //이벤트가 들어 있다면 실행
                OnStart?.Invoke();

                for (int i = 0; i < doorObj.Length; i++)
                {
                    //사라진 모든 오브젝트 비활성화
                    doorObj[i].SetActive(false);
                }
            });
        });
    }

    public void EndGame()
    {
        //콘텐츠 비활성화 및 팝업창 활성화
        for (int i = 0; i < contents.Length; i++)
        {
            if (contents[i] != null)
            {
                contents[i].SetActive(false);
            }
        }

        if (result != null)
        {
            result.SetActive(true);

            // 사운드가 한 번도 재생되지 않았으면
            if (!hasPlayed)
            {
                // PlayOneShot으로 사운드 재생
                SoundMGR.Instance.SoundPlay("lose");
                hasPlayed = true; // 재생 후 플래그 설정
            }
        }
    }
}
