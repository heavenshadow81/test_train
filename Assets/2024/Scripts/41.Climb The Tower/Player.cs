using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using Cinemachine;


namespace ClimbTheTower
{
    public class Player : MonoBehaviour
    {
        [SerializeField] Transform head;  // Head 오브젝트
        [SerializeField] Transform body;  // Body 오브젝트
        [SerializeField] GameObject[] underPortals;
        [SerializeField] CinemachineVirtualCamera followCam;
        [SerializeField] CinemachineVirtualCamera towerCam;
        [SerializeField] GameManager gameManager;

        private float jumpPower = 5f;      // 점프 높이
        private int jumpCount = 1;         // 점프 횟수
        private float jumpDuration = 0.5f; // 점프 지속 시간
        private float turnDuration = 0.25f; // 회전 시간
        private float resetDuration = 0.5f; // 정면으로 회전하는 시간
        private float headTiltAngle = 30f; // 머리가 젖혀질 각도
        private float headTiltDuration = 0.25f; // 머리가 젖혀졌다가 돌아오는 시간
        private Vector3 originalRotation;
        private Vector3 towerPosition = new Vector3(0, 0, 150);
        private CapsuleCollider coll;
        private float moveDistance = 11f;

        private Vector3 startPosition;
        private Vector3 headPosition;
        private Vector3 bodyPosition;
        private float buttonDuration = 2f;

        private void Awake()
        {
            coll = GetComponent<CapsuleCollider>();
            coll.enabled = false;
     
            originalRotation = transform.eulerAngles;

            headPosition = head.localPosition;
            bodyPosition = body.localPosition;

            startPosition = transform.position;

            DOTween.Init();
            DOTween.defaultUpdateType = UpdateType.Fixed;
        }

        private void OnEnable()
        {
            towerCam.Priority = 5;
            towerCam.enabled = false;
            followCam.enabled = true;

            if(gameManager.GetTowerIndex() > 0)
            {
                StartCoroutine(ButtonRoutine());
            }
        }

        IEnumerator ButtonRoutine()
        {
            yield return new WaitForSeconds(buttonDuration);

            gameManager.SetButtonsEnable(true);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.name.Contains("Goal"))
            {
                SoundMGR.Instance.SoundPlay("Shout");

                gameManager.SetButtonsEnable(false);
                PerformArrived();

                if (gameManager.GetTowerIndex() >= gameManager.GetTowerLength())
                {
                    gameManager.PauseTimer();
                }
            }
        }

        private void PerformArrived()
        {
            coll.enabled = false;  // 충돌체 비활성화

            followCam.enabled = false;

            // DOTween Sequence 생성
            Sequence arrivedSequence = DOTween.Sequence();

            // 첫 번째 점프: 한 바퀴 돌면서 점프
            arrivedSequence.Append(transform.DOJump(transform.position, jumpPower, 2, 2).SetEase(Ease.OutQuad));

            // 두 번째 점프: 반대 방향으로 한 바퀴 돌면서 점프
            arrivedSequence.Append(transform.DOJump(transform.position, jumpPower, 1, 1).SetEase(Ease.OutQuad)
                .Join(transform.DORotate(new Vector3(0, -360, 0), jumpDuration, RotateMode.FastBeyond360)));

            // 점프가 끝나면 캐릭터를 안보이게 하고, 시작 위치로 이동
            arrivedSequence.OnComplete(() =>
            {
                // 시작 위치로 이동
                transform.position = startPosition;
                transform.rotation = Quaternion.identity;

                // 캐릭터를 비활성화
                gameObject.SetActive(false);

                towerCam.enabled = true;
                towerCam.Priority = 15;

                gameManager.ChangeTower();
            });
        }


        // 오른쪽 보고 점프하는 함수
        public void JumpRight()
        {
            gameManager.SetButtonsEnable(false);

            // 점프 중에는 카메라 회전을 비활성화
            followCam.enabled = false;

            originalRotation = transform.eulerAngles;

            // 오른쪽으로 회전 (Y축 값은 -90도로 고정)
            transform.DORotate(new Vector3(0, originalRotation.y - 90f, 0), turnDuration).OnComplete(() =>
            {
                // 오른쪽 방향을 보고 점프 후 다시 정면으로 회전
                Jump(Quaternion.Euler(0, originalRotation.y, 0)).OnComplete(() =>
                {
                    ResetRotation();
                });
            });
        }

        // 왼쪽 보고 점프하는 함수
        public void JumpLeft()
        {
            gameManager.SetButtonsEnable(false);

            // 점프 중에는 카메라 회전을 비활성화
            followCam.enabled = false;

            originalRotation = transform.eulerAngles;

            // 왼쪽으로 회전 (Y축 값은 90도로 고정)
            transform.DORotate(new Vector3(0, originalRotation.y + 90f, 0), turnDuration).OnComplete(() =>
            {
                // 왼쪽 방향을 보고 점프 후 다시 정면으로 회전
                Jump(Quaternion.Euler(0, originalRotation.y, 0)).OnComplete(() =>
                {
                    ResetRotation();
                });
            });
        }

        // Head와 Body가 함께 점프하는 기본 점프 함수
        private Sequence Jump(Quaternion headFinalRotation)
        {
            coll.enabled = false;
            float headYRotation = head.eulerAngles.y;

            // 점프 시작 시 머리 뒤로 젖히기 -> 점프 중 머리 유지 -> 점프 후 복귀
            Sequence jumpSequence = DOTween.Sequence();

            // 머리 뒤로 젖히기 (Y축 회전 고정)
            jumpSequence.Append(head.DORotate(new Vector3(headTiltAngle, headYRotation, 0), headTiltDuration).SetEase(Ease.OutQuad))
              .AppendCallback(() => SoundMGR.Instance.SoundPlay("Jump")) // 첫 회전 후 사운드 재생
              .Append(body.DOJump(body.position, jumpPower, jumpCount, jumpDuration).SetEase(Ease.OutQuad))
              .Join(head.DOJump(head.position, jumpPower * 0.8f, jumpCount, jumpDuration).SetEase(Ease.OutQuad))
              .Join(head.DORotate(new Vector3(-headTiltAngle, headYRotation, 0), headTiltDuration).SetEase(Ease.OutQuad))
              .Append(head.DORotate(new Vector3(0, headYRotation, 0), headTiltDuration).SetEase(Ease.OutQuad));

            return jumpSequence;
        }

        // 점프 후 정면을 보게 하는 함수
        private void ResetRotation()
        {
            transform.DORotate(originalRotation, resetDuration).OnComplete(() =>
            {
                head.localRotation = Quaternion.Euler(Vector3.zero);
                head.localPosition = headPosition;
                body.localPosition = bodyPosition;

                coll.enabled = true;

                // 점프가 끝나면 다시 카메라가 회전을 따르도록 설정
                followCam.enabled = true;

                gameManager.SetButtonsEnable(true);
            });
        }

        // 점프 애니메이션과 포탈 애니메이션을 순차적으로 실행하고 이동하는 함수
        public void PerformPortalTransition(Vector3 destination)
        {
            gameManager.SetButtonsEnable(false);

            coll.enabled = false;

            // 포탈 활성화
            underPortals[gameManager.GetTowerIndex()].gameObject.SetActive(true);

            // DOTween Sequence 생성
            Sequence portalSequence = DOTween.Sequence();

            // 포탈이 커지는 애니메이션
            portalSequence.Append(underPortals[gameManager.GetTowerIndex()].transform.DOScale(new Vector3(6f, 6f, 6f), 1f).SetEase(Ease.OutQuad));

            // 플레이어가 뒤로가는 애니메이션 (페이드 아웃 없이 이동만)
            portalSequence.Append(head.DOLocalMoveZ(head.localPosition.z + moveDistance, 1f).SetEase(Ease.OutQuad));
            portalSequence.Join(body.DOLocalMoveZ(body.localPosition.z + moveDistance, 1f).SetEase(Ease.OutQuad));

            // 포탈 애니메이션이 끝나면 캐릭터를 목적지로 이동시키고 도착 애니메이션 실행
            portalSequence.OnComplete(() =>
            {
                // 캐릭터를 목적지로 이동
                transform.position = destination;            

                // 타워를 바라보는 방향을 계산한 후, 반대로 회전 (Y축만)
                Vector3 directionToTower = (towerPosition - transform.position).normalized;

                // 현재 플레이어의 회전값에서 Y축만 변경
                Quaternion targetRotation = Quaternion.LookRotation(directionToTower);
                Vector3 currentRotation = transform.eulerAngles;

                // Y축만 타워를 등지는 방향으로 회전
                transform.DORotate(new Vector3(currentRotation.x, targetRotation.eulerAngles.y, currentRotation.z), 1f).OnComplete(() =>
                {
                    // 캐릭터가 앞으로 나오는 애니메이션
                    Sequence arrivalSequence = DOTween.Sequence();
                    arrivalSequence.Append(head.DOLocalMoveZ(head.localPosition.z - moveDistance, 1f).SetEase(Ease.OutQuad));
                    arrivalSequence.Join(body.DOLocalMoveZ(body.localPosition.z - moveDistance, 1f).SetEase(Ease.OutQuad));

                    // 도착 후 포탈이 다시 작아지면서 비활성화
                    arrivalSequence.OnComplete(() =>
                    {
                        // 포탈이 작아지는 애니메이션
                        Sequence portalCloseSequence = DOTween.Sequence();
                        portalCloseSequence.Append(underPortals[gameManager.GetTowerIndex()].transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InQuad));

                        // 포탈이 작아지면 비활성화
                        portalCloseSequence.OnComplete(() =>
                        {
                            underPortals[gameManager.GetTowerIndex()].gameObject.SetActive(false);
                            coll.enabled = true;
                            gameManager.SetButtonsEnable(true);
                        });
                    });
                });
            });
        }

        public float GetJumpReadyTime()
        {
            return turnDuration + headTiltDuration;
        }
    }
}
