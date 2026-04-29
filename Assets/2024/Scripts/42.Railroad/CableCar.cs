using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Splines;

namespace Railroad
{
    public class CableCar : MonoBehaviour
    {
        public SplineAnimate splineAnimate;
        private float raycastDistance = 100f; // 레이캐스트를 쏘는 거리       

        public bool isSplineFinished = false; // 스플라인 애니메이션이 끝났는지 확인할 변수

        private MagicLife life;

        public GameObject trainCam;

        private Image fade;
        private GameManager gameManager;


        private void Awake()
        {
            life = FindObjectOfType<MagicLife>();

            // "Fade"라는 이름의 게임 오브젝트를 찾아서 Image 컴포넌트를 가져옴
            fade = GameObject.Find("Fade")?.GetComponent<Image>();
            gameManager = GameObject.Find("GameManager")?.GetComponent<GameManager>();


            if (fade == null)
            {
                Debug.LogError("Fade라는 이름의 Image 컴포넌트를 찾을 수 없습니다.");
            }
        }

        private void Update()
        {
            // 스플라인의 진행 상태를 NormalizedTime으로 확인
            if (splineAnimate.NormalizedTime >= 1.0f && !isSplineFinished)
            {
                // 애니메이션이 끝에 도달했을 때 (NormalizedTime이 1.0 이상일 때)
                isSplineFinished = true;
                CheckArrived();
            }
        }

        // 도착 체크 함수
        public void CheckArrived()
        {
            // 로컬 좌표계 기준으로 뒤로 100만큼 이동한 위치에서 Raycast 시작 위치 설정
            Vector3 localRayOrigin = new Vector3(1.5f, 0, 0); // 로컬 좌표계 기준 Z축 방향으로 뒤로 100
            Vector3 rayOrigin = transform.TransformPoint(localRayOrigin); // 로컬 좌표를 월드 좌표로 변환

            // 아래 방향으로 레이캐스트를 쏴서 감지
            RaycastHit hit;
            if (Physics.Raycast(rayOrigin, Vector3.down, out hit, raycastDistance))
            {
                // 히트된 객체의 태그가 "Point"인지 확인
                if (hit.collider.CompareTag("Finish"))
                {
                    SoundMGR.Instance.SoundStop("Train");

                    fade.DOFade(1, 1).OnComplete(() =>
                    {
                        trainCam.SetActive(false);

                        // 다음 스테이지 이동
                        gameManager.NextStage();
                    });
                }
                else
                {
                    life.LifeDelete();

                    gameManager.failIndex++;

                        fade.DOFade(1, 1).OnComplete(() =>
                        {
                            // 다음 스테이지 이동
                            gameManager.NextStage();
                        });
                    
                }
            }
            else
            {
                life.LifeDelete();

                gameManager.failIndex++;

                    fade.DOFade(1, 1).OnComplete(() =>
                {
                    // 다음 스테이지 이동
                    gameManager.NextStage();
                });
                
            }

            // 레이캐스트 시각화 (디버그 용도)
            //Debug.DrawRay(rayOrigin, Vector3.down * raycastDistance, Color.red, 100f);
        }

        public void EnableSplineAnimate()
        {
            splineAnimate.enabled = true;
        }
    }
}
