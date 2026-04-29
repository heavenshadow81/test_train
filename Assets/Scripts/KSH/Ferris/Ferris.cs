using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Ferris : MonoBehaviour
{
    //관람차에서보여줄 사람의 개인컬러
    public HumanColor FindColor;
    private Vector2 pos = Vector2.zero;

    //회전할 각도값
    public float angle;
    //회전 거리
    public float distance;

    //같이 회전할 뼈대
    public Transform center;

    //문
    [System.NonSerialized] public Transform door;

  

    //사람머리
    public SpriteRenderer Head;
    //사람 몸
    public SpriteRenderer Body;

    // Pivot 을 중심으로 팔 회전

    //사람 왼팔
    public SpriteRenderer LeftArm;
    //사람 왼팔Pivot
    public GameObject LeftArmPivot;
    //사람 오른팔
    public SpriteRenderer RightArm;
    //사람 오른팔 Pivot
    public GameObject RightArmPivot;

    private CancellationTokenSource sources = new();


    private void Awake()
    {
        //문은 관람차의 첫번째 자식
        door = transform.GetChild(0);
    }

    private void OnEnable()
    {
        if (sources != null) sources.Dispose();
        sources = new();
    }


    //비활성화시 Task정지
    private void OnDisable()
    {
        sources.Cancel();
    }

    //파괴시 Task정지 / Task 해제
    private void OnDestroy()
    {
        sources.Cancel();
        sources.Dispose();
    }


    private void Update()
    {
        //관람차가 원형으로 이동
        TriangleProcess(angle);
    }


    //    o 
    //o       o
    //
    //o       o
    //    o

    //삼각함수로 원형(각도)으로 위치변경
    public void TriangleProcess(float angle)
    { 
        pos.x = Mathf.Cos((angle - 90) * Mathf.Deg2Rad)  * distance;
        pos.y = Mathf.Sin((angle - 90) * Mathf.Deg2Rad)  * distance;

        transform.position = center.position + new Vector3( pos.x,pos.y ,0);
    }

    //문 열리고 (기능추가)
    public void DoorOpen(Action action = null)
    {
        //문열리는 소리
        SoundMGR.Instance.SoundPlay("18.문열림");
        //문만 좌측으로 트윈
        door.transform.DOLocalMoveX(-1.3f, 1).OnComplete(() => { action?.Invoke(); }).WithCancellation(sources.Token);
    }
    //문 닫히고 (기능추가)
    public void DoorClose(Action action = null)
    {
        //문닫힘 소리
        SoundMGR.Instance.SoundPlay("18.문닫힘");
        //문만 제자리로 트윈
        door.transform.DOLocalMoveX(0, 1).OnComplete(() => { action?.Invoke(); }).WithCancellation(sources.Token);
    }

    /// <summary>
    /// 안녕인사
    /// </summary>
    /// <returns></returns>
    public async UniTask ArmPivotLoop()
    {
        //팔을 앞으로 
        RightArm.sortingOrder = 3;
        //팔회전을 반복루프
        DOTween.Sequence().
            Append(RightArmPivot.transform.DORotate(new Vector3(0, 0, 80f), 0.7f)).
            Append(RightArmPivot.transform.DORotate(new Vector3(0, 0, 0f), 0.5f)).SetLoops(-1).WithCancellation(sources.Token); 
        
        await UniTask.Yield();
    }
}
