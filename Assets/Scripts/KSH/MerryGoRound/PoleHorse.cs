using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

// 베이스클래스
public class HorseBase
{
    public PoleHorse Horse;
    public HorseBase(PoleHorse _Horse) { Horse = _Horse; }
    public virtual void Enter() { }
    public virtual void Excute() { }
    public virtual void Exit() { }
}

//말 앞쪽진행 클래스
public class HorseFor: HorseBase
{
    public HorseFor(PoleHorse _Horse) : base(_Horse) { }
    public override void Enter()
    {
        base.Enter();
        //앞쪽 말이미지 전환
        Horse.horseRender.sprite = MerryGoMGR.Instance.spriteData.ForHorse;
    }
    public override void Excute()
    {
        base.Excute();
        //말의 방향이 오른쪽일때 
        if (Horse.dirType == DirType.Right)
        {
            //먼저 회전상태로 변환
            Horse.StateChange(HorseMoveType.Rotate);   
        }
    }
}

public class HorseRot: HorseBase
{
    public HorseRot(PoleHorse _Horse) : base(_Horse) { }
    public override async void Enter()
    {
        base.Enter();
        //회전 상태로 왔다
        try
        {
            //회전후 방향이 왼쪽이다 (바깥)
            if (Horse.dirType == DirType.Left)
            {
                //0.2초 딜레이
                await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: Horse.source.Token);
                //말의 앞쪽으로 상태전환
                Horse.StateChange(HorseMoveType.Forword);
            }
            //회전후 방향이 오른쪽이다 (안쪽)
            else if (Horse.dirType == DirType.Right)
            {
                //말이미지 전환
                Horse.horseRender.sprite = MerryGoMGR.Instance.spriteData.LeftRotHorse;
                //0.2초간 딜레이
                await UniTask.Delay(TimeSpan.FromSeconds(0.2f),cancellationToken : Horse.source.Token);
                //말의 뒤쪽으로 상태전환
                Horse.StateChange(HorseMoveType.Backword);
            }
        }
        catch (Exception ex) 
        { }
       
    }
}

public class HorseBack : HorseBase
{
    public HorseBack(PoleHorse _Horse) : base(_Horse) { }
    //말이 뒤쪽이다
    public override void Enter()
    {
        base.Enter();
        //말의 이미지 전환
        Horse.horseRender.sprite = MerryGoMGR.Instance.spriteData.BackHorse;
    }
    public override void Excute()
    {
        base.Excute();
        if  (Horse.dirType == DirType.Left)
        {
            Horse.StateChange(HorseMoveType.Rotate);
        }
    }
}

//아래로 
public class HorseLower:HorseBase
{
    public HorseLower(PoleHorse _Horse) : base(_Horse) { }

    public async override void Enter()
    {
        base.Enter();
        //말 이미지 전환 
        Horse.horseRender.sprite = MerryGoMGR.Instance.spriteData.LowerHorse;
        // 약 0.7정도 내려갓다가
        await  Horse.horseRender.transform.DOLocalMoveY(-0.7f, 0.3f).WithCancellation(Horse.source.Token);
        //원래위치로 복귀
        await  Horse.horseRender.transform.DOLocalMoveY(0, 0.3f).WithCancellation(Horse.source.Token);

        //방향에 따라 상태전환 
        if (Horse.dirType == DirType.Left) Horse.StateChange(HorseMoveType.Forword);
        if (Horse.dirType == DirType.Right) Horse.StateChange(HorseMoveType.Backword);
    }
}

public class HorseUpper : HorseBase
{ 
    public HorseUpper(PoleHorse _Horse):base(_Horse) { }

    public override async void Enter()
    {
        base.Enter();
        //말이미지 전환
        Horse.horseRender.sprite = MerryGoMGR.Instance.spriteData.UpperHorse;
        //위치가 위로 0.7 정도 올라갓다가
        await  Horse.horseRender.transform.DOLocalMoveY(0.7f, 0.3f).WithCancellation(Horse.source.Token); 
        //위치를 원상태로 복귀
        await  Horse.horseRender.transform.DOLocalMoveY(0, 0.3f).WithCancellation(Horse.source.Token);
        //방향에 따라 상태전환 
        if (Horse.dirType == DirType.Left) Horse.StateChange(HorseMoveType.Forword);
        if (Horse.dirType == DirType.Right) Horse.StateChange(HorseMoveType.Backword);
    }
}

public class HorseLRot : HorseBase
{
    public HorseLRot(PoleHorse _Horse) : base(_Horse) { }
    public override async void Enter()
    {
        base.Enter();
        // 방향에 따라 상태전환
        if (Horse.dirType == DirType.Left) Horse.StateChange(HorseMoveType.Forword);
        if (Horse.dirType == DirType.Right) Horse.StateChange(HorseMoveType.Backword);

        // 회전 
        await  Horse.horseRender.transform.DORotate(new Vector3(0, -180f, 0), 0.3f).WithCancellation(Horse.source.Token); ;
        await  Horse.horseRender.transform.DORotate(new Vector3(0, 0, 0), 0.3f).WithCancellation(Horse.source.Token);
    }
}
public class HorseRRot : HorseBase
{
    public HorseRRot(PoleHorse _Horse) : base(_Horse) { }

    public override async void Enter()
    {
        base.Enter();
        // 방향에 따라 상태전환
        if (Horse.dirType == DirType.Left) Horse.StateChange(HorseMoveType.Forword);
        if (Horse.dirType == DirType.Right) Horse.StateChange(HorseMoveType.Backword);
        // 회전 
        await Horse.horseRender.transform.DORotate(new Vector3(0, 180f, 0), 0.3f).WithCancellation(Horse.source.Token);
        await  Horse.horseRender.transform.DORotate(new Vector3(0, 0, 0), 0.3f).WithCancellation(Horse.source.Token); 
    }
}

//말의 상태타입
public enum HorseMoveType
{ 
    Forword,Rotate,Backword , Lower , Upper , LRot,RRot
}

public class PoleHorse : MonoBehaviour
{
    //회전목마 봉 이미지랜더
    public SpriteRenderer poleRender;
    //회전목마 말 이미지랜더
    public SpriteRenderer horseRender;
    //말 방향 타입
    public DirType dirType = DirType.Left;
    //말 상태관리용 Base 변수
    public HorseBase horseBase;
    //말의 상태타입
    public HorseMoveType HorseType;

    //트윈 종료 용 토큰변수 
    public CancellationTokenSource source = new();
    private void OnEnable()
    {
        if(source != null) source.Dispose();
        source = new();
    }

    private void OnDisable()
    {
        source.Cancel();
    }
    private void OnDestroy()
    {
        source.Cancel();
        source.Dispose();
    }

    //랜더러 세팅
    //방향 세팅
    public void HorseSetting()
    {
        horseRender = transform.GetChild(0).GetComponent<SpriteRenderer>();
        poleRender = transform.GetChild(1).GetComponent<SpriteRenderer>();
        if (dirType == DirType.Left) StateChange(HorseMoveType.Forword);
        if (dirType == DirType.Right) StateChange(HorseMoveType.Backword);
    }

    //스테이트 변환
    public void StateChange(HorseMoveType type)
    { 
        if(horseBase != null)  horseBase.Exit(); 

        horseBase = instanceBase(type);
        horseBase.Enter();
    }

    //스테이트 생성
    private HorseBase instanceBase(HorseMoveType type)
    {
        HorseType = type;
        switch (type)
        {
            case HorseMoveType.Forword: return new HorseFor(this);
            case HorseMoveType.Rotate: return new HorseRot(this);
            case HorseMoveType.Backword: return new HorseBack(this);
            case HorseMoveType.Lower: return new HorseLower(this);
            case HorseMoveType.Upper: return new HorseUpper(this);
            case HorseMoveType.LRot:return new HorseLRot(this);
            case HorseMoveType.RRot:return new HorseRRot(this);
        }
        return null;
    }

    private float v = 0;

    private void Update()
    {
        //게임상태가 아닐때는 돌지말것
        if (MerryGoMGR.Instance.stateClass.state != GameState.GamePlay) return;
        //상태에 Excute가 오버라이드가 선언되어있을때 실행
        if(horseBase != null) horseBase.Excute();
        //이동
        transform.Translate(Vector3.right * Time.deltaTime * MerryGoMGR.Instance.DirSpeed * v);

        //왼쪽 끝으로가면 오른쪽으로
        if (transform.position.x <= -5f)
        {
            dirType = DirType.Right;
        }
        //오른쪽 끝으로 가면 왼쪽으로
        else if (transform.position.x >= 5f)
        {
            dirType = DirType.Left;
        }

        //안쪽으로
        if (dirType == DirType.Right)
        {
            v = 1;
            //스프라이트 마스크적용
            poleRender.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            //스프라이트 뒤로보내기 적용 
            poleRender.sortingOrder = -1;
            //스프라이트 마스크적용
            horseRender.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            //스프라이트 뒤로보내기 적용 
            horseRender.sortingOrder = -2;
        }
        //바깥으로
        else if (dirType == DirType.Left)
        {
            v = -1;
            //스프라이트마스크 미적용
            poleRender.maskInteraction = SpriteMaskInteraction.None;
            //스프라이트 앞으로 보내기
            poleRender.sortingOrder = 2;
            //스프라이트마스크 미적용
            horseRender.maskInteraction = SpriteMaskInteraction.None;
            //스프라이트 앞으로 보내기
            horseRender.sortingOrder = 3;
        }
    }
}
