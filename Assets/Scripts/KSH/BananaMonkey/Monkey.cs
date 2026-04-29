
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Bax.P0.Client.UnityWorld.MonkeyGame
{

    public class Monkey : MonoBehaviour
    {
        //다음 가지 위치 
        public Transform target;

        //비지어 커브 위치 
        public Transform p1;
        public Transform p2;
        
        //이동할 위치값 갯수
        public int lineCnt;

        
      
        //다음 가지번호
        public int nextBranchCnt;

        //애니메이터
        public Animator anim;

        //처음에 보여줄 이미지
        public Sprite DefultMonkey;
        //스프라이트 랜더러
        public SpriteRenderer render;

        //다음가지로 이동한 수
        public int NextBranchNum = 0;
        //이동할 가지 번호를 저장할 리스트
        public List<int> branchTemp = new List<int>();

        
        private bool goal;
        private void Awake()
        {
            anim = GetComponentInChildren<Animator>();
            render = GetComponentInChildren<SpriteRenderer>();
        }

        #region Bezier
        public float Bezier(float P0, float P1, float P2, float P3, float t)
        {
            return Mathf.Pow((1 - t), 3) * P0 + Mathf.Pow((1 - t), 2) * 3 * t * P1 + Mathf.Pow(t, 2) * 3 * (1 - t) * P2 +
                   Mathf.Pow(t, 3) * P3;
        }
        public Vector3 Bezier(Vector3 P0, Vector3 P1, Vector3 P2, Vector3 P3, float t)
        {
            Vector3 M0 = Vector3.Lerp(P0, P1, t);
            Vector3 M1 = Vector3.Lerp(P1, P2, t);
            Vector3 M2 = Vector3.Lerp(P2, P3, t);

            Vector3 B0 = Vector3.Lerp(M0, M1, t);
            Vector3 B1 = Vector3.Lerp(M1, M2, t);

            return Vector3.Lerp(B0, B1, t);
        }
        #endregion

        public async UniTask<Vector3> NextBranch()
        {
            //가야할 가지번호는 시작할때 세팅됨
            //가지번호를 받아 다음 가지번호로
            nextBranchCnt = branchTemp[ NextBranchNum ];
                //(curBranchCnt + 2) % 6; 
            await UniTask.Yield();
            
            //다음 가지의 위치값 가져옴
            return  MonkeyMgr.Instance.TreeBranchs[nextBranchCnt].position;
        }


        public async UniTask BezierMove()
        {
            //왼쪽에서 오른쪽으로 이동
            if (transform.position.x < target.position.x)
            {
                //이미지의 피봇위치가 중앙이 아니기때문에 스케일 X값으로 이미지Flip 조절
                transform.localScale = new Vector3(1, 1, 1);

                p1.transform.position = new Vector3(
                    transform.position.x ,
                    transform.position.y /*- 1.5f*/, 
                    0);
                p2.transform.position = new Vector3(
                    target.position.x - 0.5f,
                    target.position.y + 1.5f, 
                    0);
            }
            else //오른쪽에서 왼쪽으로 이동
            {
                //이미지의 피봇위치가 중앙이 아니기때문에 스케일 X값으로 이미지Flip 조절
                transform.localScale = new Vector3(-1, 1, 1);

                p1.transform.position = new Vector3(
                    transform.position.x ,
                    transform.position.y /*- 1.5f*/, 
                    0);
                p2.transform.position = new Vector3(
                    target.position.x + 0.5f,
                    target.position.y + 1.5f, 
                    0);
            }

            await UniTask.Yield(cancellationToken: MonkeyMgr.Instance.token.Token);

            //점프 
            anim.SetTrigger("jump");
            MonkeyMgr.Instance.monkeyMoveState = MonkeyMgr.MonkeyMoveState.move;

            for (int i = 0; i < lineCnt; i++)
            {
                float t;
                if (i == 0)
                {
                    t = 0;
                }
                else
                {
                    t = (float)i / (lineCnt - 1);
                }

                Vector3 bezier = Bezier(transform.position,
                                        p1.position,
                                        p2.position,
                                        target.transform.position, t);

                transform.position = bezier;

                await UniTask.Delay(TimeSpan.FromSeconds(0.01f), DelayType.UnscaledDeltaTime , cancellationToken: MonkeyMgr.Instance.token.Token);
            }
            //도착
            
            //다음 가지번호 가져올 값 증가
            ++NextBranchNum;
            anim.Play("Idle");
            render.sprite = DefultMonkey;

            await UniTask.Delay(TimeSpan.FromSeconds(1f), DelayType.UnscaledDeltaTime , cancellationToken: MonkeyMgr.Instance.token.Token);

            //원숭이가 7번 다 돌아다녔다면
            if (NextBranchNum >= branchTemp.Count)
            {
                //마지막 가지에 도착 
                goal = true;

                //원숭이가 4마리 다 7번 다 돌았다면 
                if (moneyGoal())
                { 
                    MonkeyMgr.Instance.monkeyMoveState = MonkeyMgr.MonkeyMoveState.Result;
                }
                //원숭이가 전부 도착을 했다면 버튼 클릭가능하게 변경
                //버튼 막기 해제 
                MonkeyMgr.Instance.isDown = false;
                
                return;
            }
            else //원숭이가 7번 돌기전이라면
            {
                goal = false;
                anim.SetTrigger("swing");
            }

        }

        /// <summary>
        /// 원숭이가 4마리 전부 도착했다면 true 아니면 false
        /// </summary>
        /// <returns></returns>
        private bool moneyGoal()
        {
            //원숭이가 한마리라도 도착을 안했다면 false
            foreach (var item in MonkeyMgr.Instance.monkeys)
            {
                if (item.goal == false) return false;
            }
            //전부 도착 했다면 true
            return true;
        }

    }
    
}