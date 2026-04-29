using Bax.P0.Client.UnityWorld.MonkeyGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class MonkeyAnimEvent : MonoBehaviour
{
    //바나나 숨기고 난 후 
    public async void MonkeysMove()
    {
        //원숭이 4마리 이동시킴
        //랜덤 0 ~ 3 
        int[] idxRandom = new int[4] {0,1,2,3};

        for (int i = 0; i < 5; i++)
        {
            int rest = UnityEngine.Random.Range(0, 4);
            int dest = UnityEngine.Random.Range(0, 4);

            int t = idxRandom[rest];
            idxRandom[rest] = idxRandom[dest];
            idxRandom[dest] = t;
        }

        //이동시작 함 
        for (int i = 0; i < MonkeyMgr.Instance.monkeys.Length; i++)
        {
            //랜덤한 원숭이 찾음
            Monkey monkey = MonkeyMgr.Instance.monkeys[idxRandom[i]];

            //원숭이가 이동할 다음위치 세팅
            monkey.target.position = await monkey.NextBranch();
            //스윙 애니메이션 
            monkey.anim.SetTrigger("swing");

            await UniTask.Delay(TimeSpan.FromSeconds(0.1f), DelayType.UnscaledDeltaTime);
        }
    }

    //스윙전 방향계산
    public async void MonkeyDirCal()
    {
        Monkey monkey = transform.parent.GetComponent<Monkey>();
        //원숭이가 이동할 다음위치 세팅
        monkey.target.position = await monkey.NextBranch();

        //가야할 방향이 오른쪽이라면 
        if (monkey.transform.position.x < monkey.target.position.x)
        {
            monkey.transform.localScale = new Vector3(1, 1, 1);
        }
        else //가야할 방향이 왼쪽이라면
        {
            monkey.transform.localScale = new Vector3(-1, 1, 1);
        }
    }
    //스윙 후 베지어 이동 
    public void MonkeyMove()
    {
        Monkey monkey = transform.parent.GetComponent<Monkey>();
        //원숭이의 베지어로 이동
        monkey.BezierMove().Forget();
    }

    //애니메이션 중간중간에 넣을 사운드
    public void SoundMethod(string soundName)
    { 
        SoundMGR.Instance.SoundPlay(soundName);
    }

    //원숭이가 바나나를 숨기기 전 좀 더 잘보일 수있도록 조정
    //배경 검은투명
    //약 3초간 대기 
    public async void ThiefMode()
    {
        //바나나를 가진 원숭이 찾기
        int banana = MonkeyMgr.Instance.banana;
        Monkey monkey = MonkeyMgr.Instance.monkeys[banana];

        //바나나를 가진 원숭이를 좀 더 잘 볼수 있게 페이드 아웃 후 3초 뒤 페이드인
       await  MonkeyMgr.Instance.ThiefActive(monkey.transform.GetChild(1).position);
    }

}
