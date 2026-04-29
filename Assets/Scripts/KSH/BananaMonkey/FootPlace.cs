using Bax.P0.Client.UnityWorld.MonkeyGame;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
public class FootPlace : MonoBehaviour
{
    //발판의  애니메이터
   [NonSerialized] public Animator animator;

    //트리번호
    public int TreebranchNum;

    //애니메이션 hash 값
    private int hash;

    private void Awake()
    {
        //애니메이터 컴포넌트찾기
        animator = GetComponentInParent<Animator>();
        //트리번호로 애니메이션 키값 hash값으로 저장
        hash = Animator.StringToHash(TreebranchNum.ToString());
    }
    
    public async void Down()
    {
        if(MonkeyMgr.Instance.stateClass.state == GameState.GamePlay && MonkeyMgr.Instance.monkeyMoveState == MonkeyMgr.MonkeyMoveState.Result)
        {
            MonkeyMgr.Instance.isDown = true;
            SoundMGR.Instance.SoundPlay("24.발판");
            //저장한 hash 값으로 발판 애니메이션 실행
            animator.SetTrigger(hash);
            
            // 자신이 선택한 트리에 원숭이가 있는지 찾음
            var Monkey = await findMonkey();

            //정답
            if (MonkeyMgr.Instance.monkeys[MonkeyMgr.Instance.banana] == Monkey)    //자신이 찾은 원숭이가 바나나를 가진 원숭이라면 
            {
                Debug.Log("정답");
                //정답 애니메이션 실행
                Monkey.anim.SetTrigger("success");
                //게임결과를 Success 로 설정 
                MonkeyMgr.Instance.stateClass.resultState = GameResult.Success;
                //게임상태를 Result 로 이동
                MonkeyMgr.Instance.zozo.Change(GameState.GameResult);
            }
            else
            {
                //틀림
                //초기화 
                foreach (var item in MonkeyMgr.Instance.monkeys)
                {
                    //원숭이가 다녀갈 트리 번호 초기화
                    item.branchTemp.Clear();
                    item.NextBranchNum = 0;
                }

                //종료
                if (++MonkeyMgr.Instance.reGameCnt > 3)
                {
                    //3번 연속 맞추지 못했을때 바나나를 가진 원숭이 애니메이션 실행
                    MonkeyMgr.Instance.monkeys[MonkeyMgr.Instance.banana].anim.SetTrigger("success");
                    //게임결과를 Fall
                    MonkeyMgr.Instance.stateClass.resultState = GameResult.Fail;
                    //게임상태를 Result
                    MonkeyMgr.Instance.zozo.Change(GameState.GameResult);
                }
                else
                {
                    //3회 기회 
                    //리겜
                    ActionProcess.Intro?.Invoke();
                    await UniTask.Delay(TimeSpan.FromSeconds(1));
                    //게임 상태를 다시 진행
                    MonkeyMgr.Instance.zozo.Change(GameState.GamePlay);
                }
            }

        }
    }


    private async UniTask<Monkey> findMonkey()
    {
        foreach (var item in MonkeyMgr.Instance.monkeys)
        {
            //원숭이의 마지막 트리번호가 자신의 트리번호와 같다면
            if (item.branchTemp[item.branchTemp.Count - 1] == TreebranchNum)
            {
                return item;
            }
        }
        return null;
    }
    

}
