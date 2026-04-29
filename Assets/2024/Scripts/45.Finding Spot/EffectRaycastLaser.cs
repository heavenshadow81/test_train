using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

public class EffectRaycastLaser : MonoBehaviour
{
    [SerializeField] GameObject ScaleDistance;  // 레이저가 닿는 거리까지 늘어날 오브젝트
    [SerializeField] GameObject targetParent; //타겟 포지션의 부모 오브젝트
    Transform[] targetPosition;      // 사용자가 지정할 레이저가 도달할 위치

    public Transform currentTarget;   // 현재 목표 위치

    static List<int> availableTargets = new List<int>(); //남은 타겟 추적을 위한 리스트

    int moveCount = 0; //이동 카운트
    int maxMoves = 7; //최대 이동 횟수

    [SerializeField] GameObject magicCircle; //소환될 마법진 프리팹
    [SerializeField] FS_GameManager gameManager; //게임 매니저

    void Start()
    {
        //타겟 포지션의 수만큼 배열할당
        targetPosition = new Transform[targetParent.transform.childCount];

        //타겟 포지션의 위치값을 저장
        for (int i = 0; i < targetParent.transform.childCount; i++)
        {
            targetPosition[i] = targetParent.transform.GetChild(i);
        }

        InitializeAvailableTargets();
    }
    void LaserFiring() //레이저 발사 애니메이션
    {
        // 목표 위치까지의 거리 계산
        if (currentTarget != null)
        {
            float distance = Vector3.Distance(transform.position, currentTarget.position);

            // 레이저 본체의 위치와 방향을 부드럽게 설정
            ScaleDistance.transform.DOMove(transform.position, 1.0f).SetEase(Ease.Linear);
            ScaleDistance.transform.DORotateQuaternion(Quaternion.LookRotation(currentTarget.position - transform.position), 1.0f).SetEase(Ease.Linear);

            // 레이저 스케일을 Z축 방향으로 목표 지점까지 부드럽게 늘림
            ScaleDistance.transform.DOScale(new Vector3(1, 1, distance), 1.0f).SetEase(Ease.Linear);
        }
    }
    //남은 타겟 리스트 초기화
    void InitializeAvailableTargets()
    {
        if (availableTargets.Count == 0) //초기화되지 않았을 경우에만 실행
        {
            for (int i = 0; i < targetPosition.Length; i++)
            {
                availableTargets.Add(i); //모든 타겟 추가
            }
        }
    }

    //레이저를 1초동안 5번 랜덤으로 이동하면서 쏘는 코루틴
    public IEnumerator StartLaserSequence()
    {
        moveCount = 0; //이동 카운트 초기화

        while(moveCount<maxMoves)
        {
            InitializeAvailableTargets();
            SetNewTarget(); //새로운 타겟 설정
            LaserFiring();
            moveCount++;

            yield return new WaitForSeconds(1.0f); //1초 대기
        }

        ScaleDistance.transform.DOScale(new Vector3(1, 1, 1), 1.0f).SetEase(Ease.Linear);  // 1초 동안 스케일을 1로 줄임
        gameManager.QuizTime();
        yield return new WaitForSeconds(1.0f); //1초 대기
        SummonMagicCircle(); //마법진 생성 함수
    }

    //새로운 타겟 설정
    void SetNewTarget()
    {

        //사용 가능한 타겟 중 하나를 랜덤 선택
        int randomIndex = Random.Range(0, availableTargets.Count);
        int targetIndex = availableTargets[randomIndex];

        //타겟 설정
        currentTarget = targetPosition[targetIndex];

        //사용된 타겟을 리스트에서 제거
        availableTargets.RemoveAt(randomIndex);
    }
    
    void SummonMagicCircle()
    {
        //마법진 커런트타겟의 자식으로 생성
        Instantiate(magicCircle, currentTarget);
    }
}

