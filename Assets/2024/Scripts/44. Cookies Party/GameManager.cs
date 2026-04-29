using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CookiesParty
{
    public class GameManager : TouchManager_3DTouch
    {
        [Header("쿠키 머테리얼")]
        [SerializeField] private SaveCookie saveCookie;

        [Header("쿠키 스폰 위치")]
        [SerializeField] private Transform[] cookieSpawners = null;

        [Header("Hit Effect")]
        [SerializeField] private GameObject[] hitEffects = null;

        [Header("점수")]
        [SerializeField] private TextMeshProUGUI scoreText;
        private int redScore = 0;
        private int blueScore = 0;

        [Header("게임승리")]
        [SerializeField] GameObject[] victoryUIs;
        [SerializeField] GameObject[] basketEffects;
        [SerializeField] GameObject drawUI;
        [SerializeField] MagicTimer timer;

        [Header("카메라")]
        [SerializeField] private Camera cam;
        [SerializeField] Transform[] camTransform = null;

        [SerializeField] private Image fade;
        public Action OnGameStart;

        private void OnEnable()
        {
            timer.OnTimerEnd += CheckScore;

            fade.DOFade(0, 1f).OnComplete(() =>
            {
                fade.gameObject.SetActive(false);
                OnGameStart?.Invoke();
            });
        }

        protected override void OnDisable()
        {
            timer.OnTimerEnd -= CheckScore;
            base.OnDisable();
        }

        public override void HandleInput(Vector2 pos)
        {
            // 스크린 좌표로부터 레이 생성
            Ray ray = Camera.main.ScreenPointToRay(pos);
            RaycastHit hit;

            // 레이캐스트를 쏴서 충돌 여부 확인
            if (Physics.Raycast(ray, out hit))
            {
                // 충돌한 오브젝트의 태그에 따라 처리
                switch (hit.collider.tag)
                {
                    case "Green":
                        HandleTeamAction(hit, "Green", 0);
                        blueScore++;
                        SetScore();
                        break;
                    case "Orange":
                        HandleTeamAction(hit, "Orange", 1);
                        redScore++;
                        SetScore();
                        break;

                    default:
                        // 적절한 태그가 아닐 때 터치 가능 상태로 전환
                        isTouchable = true;
                        break;
                }
            }
            else
            {
                // 아무것도 충돌하지 않았을 경우 터치 가능 상태로 전환
                isTouchable = true;
            }
        }

        private void HandleTeamAction(RaycastHit hit, string teamTag, int idx)
        {
            isTouchable = true;

            // 레이가 충돌한 오브젝트 이름으로 Potion 객체를 가져옴
            Cookie touchCookie = hit.collider.gameObject.GetComponent<Cookie>();

            if (touchCookie != null)
            {
                SoundMGR.Instance.SoundPlay("Cookie");

                // 맞은 위치에 Hit Effect를 스폰
                GameObject hitEffect = Instantiate(hitEffects[idx], hit.point, Quaternion.identity);
                hitEffect.transform.localScale = Vector3.one * 200f;
                Destroy(hitEffect, 2f); // Hit Effect가 2초 후에 사라지도록 설정 (필요에 따라 조정 가능)

                saveCookie.SetCookieMat(touchCookie.GetCookieMat());
                touchCookie.gameObject.SetActive(false); // 버튼 안보이게 처리
                ObjectPooler.SpawnFromPool<BasketCookie>("BasketCookie", cookieSpawners[idx].position, cookieSpawners[idx].rotation);                
            }
        }

        private void SetScore()
        {
            scoreText.text = $"{blueScore}    |    {redScore}";
        }

        private void CheckScore()
        {
            if(blueScore > redScore)
            {
                // 블루팀 승리
                VictoryAnim(0);
                scoreText.text = blueScore.ToString();
            }
            else if(blueScore < redScore)
            {
                // 레드팀 승리
                VictoryAnim(1);
                scoreText.text = redScore.ToString();
            }
            else
            {
                // 무승부 처리
                if (drawUI != null)
                {
                    scoreText.transform.parent.gameObject.SetActive(false);
                    drawUI.SetActive(true);
                    SoundMGR.Instance.SoundPlay("lose");
                }
            }
        }

        private void VictoryAnim(int idx)
        {
            // 1. basketEffects 활성화
            SoundMGR.Instance.SoundPlay("Fanfare");
            basketEffects[idx].SetActive(true);

            // 2. basketEffects 활성화 후 CamMove 실행
            DOVirtual.DelayedCall(1f, () =>
            {         
                CamMove(camTransform[idx]);  // 1초 후 CamMove 실행

                // 3. CamMove 완료 후 victoryUIs 활성화
                DOVirtual.DelayedCall(3f, () =>
                {
                    scoreText.transform.parent.gameObject.SetActive(false);
                    victoryUIs[idx].SetActive(true);  // CamMove 2초 후 승리 UI 활성화
                    SoundMGR.Instance.SoundPlay("win");
                });
            });
        }

        private void CamMove(Transform target)
        {
            // 카메라의 위치를 타겟 위치로 부드럽게 이동
            cam.transform.DOMove(target.position, 2f).SetEase(Ease.InOutQuad); // 2초 동안 부드럽게 이동

            // 카메라의 회전도 타겟의 회전으로 부드럽게 변경
            cam.transform.DORotateQuaternion(target.rotation, 2f).SetEase(Ease.InOutQuad); // 2초 동안 회전도 자연스럽게 변화
        }
    }
}


