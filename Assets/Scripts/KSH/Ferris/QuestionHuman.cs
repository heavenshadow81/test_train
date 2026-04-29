using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EPOOutline;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.UI;

public class QuestionHuman : MonoBehaviour
{
    public bool isDown = false;
    public HumanColor FindColor;

    [NonSerialized] public SpriteRenderer head;
    [NonSerialized] public SpriteRenderer body;
    [NonSerialized] public SpriteRenderer leftarm;
    [NonSerialized] public SpriteRenderer rightarm;
    [NonSerialized] public Outlinable outline;

    public static bool isFerrisRot = false;
    public static bool ferrisRotation_quetion = false;

    private void Awake()
    {
        //해당 스프라이트 랜더러 찾기
        head = transform.GetChild(0).GetComponent<SpriteRenderer>();
        body = transform.GetChild(1).GetComponent<SpriteRenderer>();
        leftarm = transform.GetChild(2).GetComponent<SpriteRenderer>();
        rightarm = transform.GetChild(3).GetComponent<SpriteRenderer>();
    }

    //맞췄을때
    public void Human_Success()
    {
        //정답 true 저장
        FerrisMgr.Instance.GameClear.Add(true);
        Debug.Log("정답");
        SoundMGR.Instance.SoundPlay("18.정답");
        //정답 원형UI이미지 생성
        var hightlight = FerrisMgr.Instance.hightlightPulling.GetObj();
        //이미지 컴포넌트 찾기
        Image img = hightlight.GetComponent<Image>();

        //위치를 자신의 위치로 조절
        hightlight.transform.position = transform.position;
        //스케일값 조절
        hightlight.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        //이미지컴포넌트의 fillAmount값 0 ~ 1 로 트윈 (원형으로)
        DOTween.To(() => img.fillAmount, x => img.fillAmount = x, 1, 1);
        //  go.ArmPivotLoop().Forget();
    }
    //틀렸을떄
    public void Human_Fall()
    {
        SoundMGR.Instance.SoundPlay("18.틀림");
        //틀림 false 저장
        FerrisMgr.Instance.GameClear.Add(false);
        //Color 값 Gray로 변경
        head.color = Color.gray;
        body.color = Color.gray;
        leftarm.color = Color.gray;
        rightarm.color = Color.gray;

        //X 이미지 생성
        var wrong = FerrisMgr.Instance.wrongPulling.GetObj();
        //위치를 자신의 위치로 조절
        wrong.transform.position = transform.position;
        //부모 세팅
        wrong.transform.SetParent(transform);
    }


    /// <summary>
    /// 터치 정답인지 아닌지 비교 
    /// </summary>
    public async void Down()
    {
        if (isDown == false && ferrisRotation_quetion == false)
        {
            isDown = true;
            ferrisRotation_quetion = true;

            //정답 맞추기 
            SoundMGR.Instance.SoundPlay("18.문제클릭");
            //첫번째 관람차 저장
            var go = FerrisMgr.Instance.ferrisList[0];
            //첫번째 리스트 제거
            FerrisMgr.Instance.ferrisList.RemoveAt(0);
            //저장한 관람차 맨마지막으로 저장
            FerrisMgr.Instance.ferrisList.Add(go);
            //저장했던 관람차 문 열림
            go.DoorOpen();
            //1초 후 
            await UniTask.Delay(TimeSpan.FromSeconds(1));

            //관람차가 가지고 있는 인간 컬러값과 
            //정답화면의 인간이 가지고있는 컬러값 같다면
            if (go.FindColor == FindColor)
            {
                //정답
                Human_Success();
            }
            else
            {
                //실패
                Human_Fall();
            }
            //2초후
            await UniTask.Delay(TimeSpan.FromSeconds(2));

            if (isFerrisRot == false)
            {
                isFerrisRot = true;
                //관람차들 60도 위치 이동
                FerrisMgr.Instance.FerrisMove(60,
                    async () =>
                    {
                        isFerrisRot = false;
                        ferrisRotation_quetion = false;         
                        //6명 다 골랐을때 
                        if (++FerrisMgr.Instance.SelectFrame.questionCnt >= 6)
                        {
                            //틀린값이 한개라도 있다면
                            if (FerrisMgr.Instance.GameClear.Contains(false)) FerrisMgr.Instance.stateClass.resultState = GameResult.Fail;
                            else FerrisMgr.Instance.stateClass.resultState = GameResult.Success;
                            //결과값 지우고 게임상태 result 로 이동
                            FerrisMgr.Instance.GameClear.Clear();
                            FerrisMgr.Instance.zozo.Change(GameState.GameResult);
                        }
                    }, 2);
            }
        }
    }

   

}
