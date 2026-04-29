using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Splines;
using DG.Tweening; // Spline 클래스를 사용하기 위한 네임스페이스

namespace Railroad
{
    public class RailManager : MonoBehaviour
    {

        [SerializeField] EmptyObject[] emptyObjects = null;
        [SerializeField] SplineManager splineManager;
        [SerializeField] CableCar cableCar;
        [SerializeField] GameObject floor;  // 생성될 프리팹 부모
        [SerializeField] GameObject buttons;
        [SerializeField] RotateStage stage;
        [SerializeField] GameManager gameManager;

        private Image fade;

        private int emptyIndex = 0;

        private void Awake()
        {
            // "Fade"라는 이름의 게임 오브젝트를 찾아서 Image 컴포넌트를 가져옴
            fade = GameObject.Find("Fade")?.GetComponent<Image>();

            if (fade == null)
            {
                Debug.LogError("Fade라는 이름의 Image 컴포넌트를 찾을 수 없습니다.");
            }

            stage = FindObjectOfType<RotateStage>();
        }

        private void Start()
        {
            SetEmptyBlinking(emptyIndex);
        }

        public void SpawnRail(GameObject railPrefab)
        {
            // 빈 오브젝트가 더 이상 존재하지 않으면 함수 종료
            if (emptyIndex >= emptyObjects.Length) return;

            stage.SpawnRailAnimation();
            SoundMGR.Instance.SoundPlay("Rail");

            // 지정된 위치에 프리팹을 생성
            emptyObjects[emptyIndex].DeActiveEmptyRail();
            railPrefab.SetActive(true);

            // SplineContainer를 가져와서 관리
            SplineContainer newSpline = railPrefab.GetComponentInChildren<SplineContainer>();
            splineManager.AddSplineContainerAtSecondLastIndex(newSpline);

            // 레이 쏴서 다음칸이 엠티칸인지 체크
            if(DetectedEmptyAtSplineEnd(newSpline))
            {
                // 다음 인덱스로 이동
                emptyIndex++;

                // 다음 빈칸 블링킹
                SetEmptyBlinking(emptyIndex);
            }
            else
            {
                // 버튼 안보이게 설정
                buttons.SetActive(false);

                if(DetectedFinishAtSplineEnd(newSpline))
                {
                    gameManager.SetScore();

                    SoundMGR.Instance.SoundPlay("Train");

                    // 페이드가 완료되면 작업을 실행하도록 설정
                    fade.DOFade(1, 1).OnComplete(() =>
                    {
                        // 스플라인 합병
                        splineManager.MergeAllSplinesIntoOne();

                        // 열차 카메라 활성화
                        //cableCar.trainCam.SetActive(true);

                        // 활성화된 모든 Rail 클래스를 찾아서 ChangeMat()과 SetProps() 메서드 실행
                        Rail[] activeRails = FindObjectsOfType<Rail>();  // 씬에서 활성화된 모든 Rail 객체를 찾음

                        foreach (Rail rail in activeRails)
                        {
                            if (rail.gameObject.activeInHierarchy) // Rail이 활성화된 상태인지 확인
                            {
                                rail.ChangeMat();  // Rail 클래스의 ChangeMat 메서드 실행
                                rail.SetProps();   // Rail 클래스의 SetProps 메서드 실행
                            }
                        }

                        fade.DOFade(0, 1).OnComplete(() =>
                        {
                            cableCar.splineAnimate.MaxSpeed = 500;
                            // 스플라인 애니메이션 플레이
                            cableCar.EnableSplineAnimate();
                        });
                    });
                }
                else
                {
                    // 폴스면 splineContainers의 마지막 인덱스 제거하고 진행
                    splineManager.splineContainers.RemoveAt(splineManager.splineContainers.Count - 1);
                    cableCar.splineAnimate.MaxSpeed = 1000;
                    splineManager.MergeAllSplinesIntoOne();
                    cableCar.EnableSplineAnimate();
                }
            }        
        }

        // 각 버튼에 등록될 함수 (프리팹을 전달하여 SpawnRail 호출)
        public void SpawnLeftRail()
        {
            SpawnRail(emptyObjects[emptyIndex].leftRail);
        }

        public void SpawnRightRail()
        {
            SpawnRail(emptyObjects[emptyIndex].rightRail);
        }

        public void SpawnStraightRail()
        {
            SpawnRail(emptyObjects[emptyIndex].straightRail);
        }

        // 스플라인 끝점에서 레이캐스트하여 태그가 "Point"인 오브젝트를 감지하는 함수
        private bool DetectedEmptyAtSplineEnd(SplineContainer splineContainer)
        {
            // 스플라인 가져오기
            UnityEngine.Splines.Spline spline = splineContainer.Spline;

            // 스플라인의 노드를 리스트로 변환하여 마지막 두 개의 노드를 가져옴
            List<BezierKnot> knots = new List<BezierKnot>(spline);

            int pointCount = knots.Count;

            if (pointCount < 2)
            {
                Debug.LogWarning("스플라인에 노드가 2개 미만입니다.");
                return false;
            }

            // 스플라인의 마지막 두 개의 노드를 가져옴
            BezierKnot lastKnot = knots[pointCount - 1];
            BezierKnot secondLastKnot = knots[pointCount - 2];

            // 로컬 좌표계에서 월드 좌표계로 변환
            Vector3 lastPointWorld = splineContainer.transform.TransformPoint(lastKnot.Position);
            Vector3 secondLastPointWorld = splineContainer.transform.TransformPoint(secondLastKnot.Position);

            // 스플라인이 흐르는 방향 계산 (마지막 두 노드의 벡터, 월드 좌표계 기준)
            Vector3 directionWorld = (lastPointWorld - secondLastPointWorld).normalized;

            // 레이 시작 위치를 스플라인 끝점에서 지정된 거리(50)만큼 이동한 지점으로 설정
            Vector3 forwardOffset = directionWorld * 100.0f; // 방향으로 100만큼 이동
            Vector3 rayStartPosition = lastPointWorld + forwardOffset;

            // 아래 방향으로 레이 쏠 때 사용할 벡터 (월드 좌표계 기준)
            Vector3 rayDirection = Vector3.down;

            // 레이 드로우: 레이를 시각적으로 확인하기 위해 DrawRay 사용 (월드 좌표계 기준)
            //Debug.DrawRay(rayStartPosition, rayDirection * 100f, Color.red, 100.0f);

            // 레이캐스트를 수행하여 태그가 "Point"인 오브젝트가 있는지 확인
            RaycastHit hit;
            float rayDistance = 100f; // 적절한 거리 설정

            // 레이캐스트 실행 (rayStartPosition에서 아래 방향으로)
            if (Physics.Raycast(rayStartPosition, rayDirection, out hit, rayDistance))
            {
                if (hit.collider.CompareTag("Point"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }


        // 스플라인 끝점에서 레이캐스트하여 태그가 "Finish"인 오브젝트를 감지하는 함수
        private bool DetectedFinishAtSplineEnd(SplineContainer splineContainer)
        {
            // 스플라인 가져오기
            UnityEngine.Splines.Spline spline = splineContainer.Spline;

            // 스플라인의 노드를 리스트로 변환하여 마지막 두 개의 노드를 가져옴
            List<BezierKnot> knots = new List<BezierKnot>(spline);

            int pointCount = knots.Count;

            if (pointCount < 2)
            {
                Debug.LogWarning("스플라인에 노드가 2개 미만입니다.");
                return false;
            }

            // 스플라인의 마지막 두 개의 노드를 가져옴
            BezierKnot lastKnot = knots[pointCount - 1];
            BezierKnot secondLastKnot = knots[pointCount - 2];

            // 로컬 좌표계에서 월드 좌표계로 변환
            Vector3 lastPointWorld = splineContainer.transform.TransformPoint(lastKnot.Position);
            Vector3 secondLastPointWorld = splineContainer.transform.TransformPoint(secondLastKnot.Position);

            // 스플라인이 흐르는 방향 계산 (마지막 두 노드의 벡터, 월드 좌표계 기준)
            Vector3 directionWorld = (lastPointWorld - secondLastPointWorld).normalized;

            // 레이 시작 위치를 스플라인 끝점에서 지정된 거리(50)만큼 이동한 지점으로 설정
            Vector3 forwardOffset = directionWorld * 100.0f; // 방향으로 100만큼 이동
            Vector3 rayStartPosition = lastPointWorld + forwardOffset;

            // 아래 방향으로 레이 쏠 때 사용할 벡터 (월드 좌표계 기준)
            Vector3 rayDirection = Vector3.down;

            // 레이 드로우: 레이를 시각적으로 확인하기 위해 DrawRay 사용 (월드 좌표계 기준)
            //Debug.DrawRay(rayStartPosition, rayDirection * 100f, Color.blue, 100.0f);

            // 레이캐스트를 수행하여 태그가 "Finish"인 오브젝트가 있는지 확인
            RaycastHit hit;
            float rayDistance = 100f; // 적절한 거리 설정

            // 레이캐스트 실행 (rayStartPosition에서 아래 방향으로)
            if (Physics.Raycast(rayStartPosition, rayDirection, out hit, rayDistance))
            {
                if (hit.collider.CompareTag("Finish"))
                {
                    //stage.StopRotation();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }



        public void SetEmptyBlinking(int blankIndex)
        {
            emptyObjects[blankIndex].GetComponent<BlinkingEffect>().enabled = true;
            emptyObjects[blankIndex].questionMark.SetActive(true);
        }
    }
}
