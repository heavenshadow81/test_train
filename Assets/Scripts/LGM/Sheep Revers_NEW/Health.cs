using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGM
{
    namespace SheepRevers
    {
        public class Health : MonoBehaviour
        {
            public Transform hpBar; // Hp 상위 객체
            public float delayTime; // 혈압 상승 주기
            public List<GameObject> hp; // 혈압 수치
            public int Hp
            {
                get
                {
                    // hp에 아무것도 없을 시 hpBar의 자식 객체를 hp에 등록
                    if (hp == null)
                        hp.SetCheildObj(hpBar);
                    return hp.ACount(); // 활성화되있는 체력 반환
                }
                set
                {
                    // hp에 아무것도 없을 시 hpBar의 자식 객체를 hp에 등록
                    if (hp == null)
                        hp.SetCheildObj(hpBar);

                    // hp의 범위 (0~hp최대값)을 벗어나면 실행 X
                    if (value < 0 || value > hp.Count)
                    {
                        return;
                    }
                    if (value > hp.ACount())    // 체력이 늘어날 시 hpBar증가
                    {
                        hp[hp.ACount()].SetActive(true);
                    }
                    else if (value < hp.ACount())   // 체력이 감소할 시 hpBar감소
                    {
                        hp[hp.ACount() - 1].SetActive(false);
                    }
                }
            }
            private void Awake()
            {
                hp.SetCheildObj(hpBar); // 시작 시 자식오브젝트 등록
            }
            private void Start()
            {
                StartCoroutine(HealthTimer());  // 주기적 혈압 증가 코루틴 실행
            }

            // 시간에 따른 혈압 증가
            private IEnumerator HealthTimer()
            {
                // 시간이 지날 수록 혈압 상승
                while (Hp > 0)
                {
                    if (GameManager.Instance.stateClass.state == GameState.GameResult) break;

                    Hp += 1;
                    yield return new WaitForSeconds(delayTime);
                }
            }
        }
    }
}

