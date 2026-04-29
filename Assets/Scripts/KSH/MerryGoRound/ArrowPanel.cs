using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class ArrowPanel : MonoBehaviour 
{

    //스프라이트 렌더
    private SpriteRenderer spRender;

    //화살표 방향 타입
    public DisplayArrowTYPE displayType;    //화살표의 방향

    //방향마다 Down,Up 
    [SerializeField]
    private ArrowTYPE arrowType;            //눌렀을때 이미지 로드용

    //프로퍼티에 저장된 값에 따라 이미지 로드시킴
    //방향마다 Down,Up 
    public ArrowTYPE ArrowType
    { 
        get { return arrowType; }
        set 
        {
            arrowType = value;
            if (arrowType == ArrowTYPE.TOPDOWN) { setSprite("ArrowPanelUp1").Forget(); }
            if (arrowType == ArrowTYPE.RIGHTDOWN) { setSprite("ArrowPanelRight1").Forget(); }
            if (arrowType == ArrowTYPE.BOTTOMDOWN) { setSprite("ArrowPanelDown1").Forget(); }
            if (arrowType == ArrowTYPE.LEFTDOWN) { setSprite("ArrowPanelLeft1").Forget(); }

            if (arrowType == ArrowTYPE.TOPUP) { setSprite("ArrowPanelUp2").Forget(); }
            if (arrowType == ArrowTYPE.RIGHTUP) { setSprite("ArrowPanelRight2").Forget(); }
            if (arrowType == ArrowTYPE.BOTTOMUP) { setSprite("ArrowPanelDown2").Forget(); }
            if (arrowType == ArrowTYPE.LEFTUP) { setSprite("ArrowPanelLeft2").Forget(); }
        }
    }


    private int childIndx;

    private void Awake()
    {
        //발판 이미지랜더러
        spRender = GetComponentInChildren<SpriteRenderer>();
        //자신의 배열번호
        childIndx = transform.GetSiblingIndex();
    }

    private void Start()
    {
        //발판 방향 세팅
        ArrowType = (ArrowTYPE)childIndx;
        //화살표 방향 세팅
        displayType = (DisplayArrowTYPE)childIndx;
    }

    private async UniTask setSprite(string spriteName)
    {
        await MerryGoMGR.Instance.loadSprite.LoadSpriteData(spriteName, spRender);
    }

    public async void Down()
    {
        if (!MerryGoMGR.Instance.isDown && MerryGoMGR.Instance.stateClass.state == GameState.GamePlay)
        {
            MerryGoMGR.Instance.isDown = true;
            //자신의 배열번호 + 4 저장
            //  0 ~ 3 Down  4 ~ 7 Up
            ArrowType = (ArrowTYPE)(childIndx + 4);

            if (MerryGoMGR.Instance.display.displayQueue.TryPeek(out var queue)) 
            {
                //화살표의 타입이 발판의 방향 타입과 같다면
                if (queue.arrowTYPE == displayType)
                {
                    //발판 사운드 Play
                    SoundMGR.Instance.SoundPlay("16.정답발판");
                    //화살표를 회적으로 
                    queue.ArrowRender.color = Color.gray;
                    //말들의 스피드업
                    MerryGoMGR.Instance.SpeedUP(2f);
                    //화살표 큐 Array
                    var arr = MerryGoMGR.Instance.display.displayQueue.ToArray();
                    //첫번째를 끝으로
                    arr[0].ArrowRender.transform.position = MerryGoMGR.Instance.display.arrowPos[5];
                    
                    //끝으로 간 화살표는 랜덤으로 이미지 재 로딩시킴 
                    int rand = Random.Range(0, 4);
                    arr[0].arrowTYPE = (DisplayArrowTYPE)rand;
                    await MerryGoMGR.Instance.display.setImages(arr[0].arrowTYPE.ToString(), arr[0].ArrowRender);

                    //나머지 화살표 차례대로 위치 변환
                    arr[1].ArrowRender.transform.position = MerryGoMGR.Instance.display.arrowPos[0];
                    arr[2].ArrowRender.transform.position = MerryGoMGR.Instance.display.arrowPos[1];
                    arr[3].ArrowRender.transform.position = MerryGoMGR.Instance.display.arrowPos[2];
                    arr[4].ArrowRender.transform.position = MerryGoMGR.Instance.display.arrowPos[3];
                    arr[5].ArrowRender.transform.position = MerryGoMGR.Instance.display.arrowPos[4];

                    //해당 화살표 스케일 조정
                    queue.ArrowRender.transform.localScale = Vector3.one * 0.17f;

                    //큐에서 화살표랜더와 방향 뽑기
                    var displayArrow = MerryGoMGR.Instance.display.displayQueue.Dequeue();
                    //저장된 남은 화살표 중 맨앞 데이터 접근해서 스케일 및 컬러값 수정 
                    MerryGoMGR.Instance.display.displayQueue.Peek().ArrowRender.transform.localScale = Vector3.one * 0.3f;// = new Vector3(0.3115f, 0.3115f, 1);
                    MerryGoMGR.Instance.display.displayQueue.Peek().ArrowRender.color = Color.white;
                    //말 이미지 전환
                    horseSpriteChange().Forget();
                    //다시 넣기
                    MerryGoMGR.Instance.display.displayQueue.Enqueue(displayArrow);

                    /*foreach (var item in arr)
                    {
                        if (item.ArrowRender.transform.position.x <= -4.5f)
                        {
                            item.ArrowRender.transform.position = new Vector3(4.5f, 4, 0);

                            int rnd = Random.Range(0, 4);
                            await MerryGoMGR.Instance.display.setImages(((DisplayArrowTYPE)rnd).ToString(), item.ArrowRender);
                            item.arrowTYPE = ((DisplayArrowTYPE)rnd);
                        }
                    }*/
                }
                else
                {
                    SoundMGR.Instance.SoundPlay("16.틀린발판");
                }
            }
          
        }

        await UniTask.Delay(System.TimeSpan.FromSeconds(0.2f), DelayType.UnscaledDeltaTime);

        //발판 상태를 원래대로 복귀
        ArrowType = (ArrowTYPE)(childIndx);
        MerryGoMGR.Instance.isDown = false;
    }

    private async UniTask horseSpriteChange()
    {
        switch (displayType)
        {
            case DisplayArrowTYPE.TOP:
                    SoundMGR.Instance.SoundPlay("16.위로");
                foreach (var item in MerryGoMGR.Instance.poleHorse)
                {
                    //전체 위로 0.1초 딜레이
                    item.StateChange(HorseMoveType.Upper);
                    await UniTask.Delay(System.TimeSpan.FromSeconds(0.1f));
                }
                break;
            case DisplayArrowTYPE.BOTTOM:
                    SoundMGR.Instance.SoundPlay("16.아래로");
                foreach (var item in MerryGoMGR.Instance.poleHorse)
                {
                    //전체 아래로 0.1초 딜레이
                    item.StateChange(HorseMoveType.Lower);
                    await UniTask.Delay(System.TimeSpan.FromSeconds(0.1f));
                }
                break;
            case DisplayArrowTYPE.LEFT:
                SoundMGR.Instance.SoundPlay("16.회전");
                //전체 회전
                foreach (var item in MerryGoMGR.Instance.poleHorse)
                    item.StateChange(HorseMoveType.LRot);
                break;
            case DisplayArrowTYPE.RIGHT:
                SoundMGR.Instance.SoundPlay("16.회전");
                //전체 회전
                foreach (var item in MerryGoMGR.Instance.poleHorse)
                    item.StateChange(HorseMoveType.RRot);
                break;
        }

        await UniTask.Yield();
    }
     
    
}
