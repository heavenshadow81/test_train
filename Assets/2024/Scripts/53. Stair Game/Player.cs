using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace StairGame
{
    public class Player : MonoBehaviour
    {
        public Rigidbody Rigid { get; private set; }
        public Collider Col { get; private set; }
        [SerializeField] Transform head;
        private Vector3 originHeadPosition;
        private Quaternion originHeadRotation;
        private float headTiltAngle = 30f; 
        private float headTiltDuration = 0.25f; 
        private float jumpPower = 5f;      
        private int jumpCount = 1;         
        private float jumpDuration = 0.5f; 
        
        public Vector3 CurrentPlayerPos { get; set; }
        public Vector3 PrevPlayerPos { get; set; }
        [SerializeField] CameraMove cam;
        [SerializeField] Dice dice;

        Sequence jumpSequence;

        private void Awake()
        {
            Rigid = GetComponent<Rigidbody>();
            Col = GetComponent<Collider>();

            DOTween.Init();
            DOTween.defaultUpdateType = UpdateType.Fixed;
        }

        private void Start()
        {
            cam.InitializeOffset(Rigid.position);
            dice.InitializeOffset(Rigid.position);
            GoToMagicCircle();

            originHeadPosition = head.localPosition;
            originHeadRotation = head.localRotation; 
        }

        // Head와 Body가 함께 점프하는 기본 점프 함수
        private Sequence FloorJump(Action onComplete = null)
        {
            float headYRotation = head.eulerAngles.y;

            // 캐릭터가 앞으로 점프할 위치 (body의 forward 방향으로 jumpDistance 만큼 이동)
            Vector3 jumpTargetPosition = Rigid.position + (-Rigid.transform.forward * 3.2f);

            // 점프 시작 시 머리 뒤로 젖히기 -> 점프 중 머리 유지 -> 점프 후 복귀
            jumpSequence = DOTween.Sequence();

            // 머리 뒤로 젖히기 (Y축 회전 고정)
            jumpSequence.Append(head.DORotate(new Vector3(headTiltAngle, headYRotation, 0), headTiltDuration).SetEase(Ease.OutQuad))
              .AppendCallback(() => SoundMGR.Instance.SoundPlay("Jump"))
              .Append(Rigid.transform.DOJump(jumpTargetPosition, jumpPower, jumpCount, jumpDuration).SetEase(Ease.OutQuad))
              .Join(head.DORotate(new Vector3(-headTiltAngle, headYRotation, 0), headTiltDuration).SetEase(Ease.OutQuad))
              .Append(head.DORotate(new Vector3(0, headYRotation, 0), headTiltDuration).SetEase(Ease.OutQuad))
              .OnComplete(() => onComplete?.Invoke());

            return jumpSequence;
        }

        private Sequence StairJump(Action onComplete = null)
        {
            float headYRotation = head.eulerAngles.y;

            // 캐릭터가 앞으로 점프할 위치 (body의 forward 방향으로 jumpDistance 만큼 이동)
            Vector3 jumpTargetPosition = Rigid.position + (-Rigid.transform.forward * 2.8f) + (Vector3.up * 0.8f);

            // 점프 시작 시 머리 뒤로 젖히기 -> 점프 중 머리 유지 -> 점프 후 복귀
            jumpSequence = DOTween.Sequence();

            // 머리 뒤로 젖히기 (Y축 회전 고정)
            jumpSequence.Append(head.DORotate(new Vector3(headTiltAngle, headYRotation, 0), headTiltDuration).SetEase(Ease.OutQuad))
              .AppendCallback(() => SoundMGR.Instance.SoundPlay("Jump"))
              .Append(Rigid.transform.DOJump(jumpTargetPosition, jumpPower, jumpCount, jumpDuration).SetEase(Ease.OutQuad))
              .Join(head.DORotate(new Vector3(-headTiltAngle, headYRotation, 0), headTiltDuration).SetEase(Ease.OutQuad))
              .Append(head.DORotate(new Vector3(0, headYRotation, 0), headTiltDuration).SetEase(Ease.OutQuad))
              .OnComplete(() => onComplete?.Invoke());

            return jumpSequence;
        }

        public void GoToMagicCircle()
        {
            Sequence magicCircleSequence = DOTween.Sequence();

            for (int i = 0; i < 3; i++)
            {
                magicCircleSequence.AppendCallback(() => FloorJump());
                magicCircleSequence.AppendInterval(jumpDuration + headTiltDuration * 2); // 점프와 머리 기울기 애니메이션의 총 지속시간을 고려하여 대기 시간 설정
            }
        }

        public void MoveCameraToTarget(Action onComplete = null)
        {
            cam.MoveCam(Rigid.position, onComplete);
            dice.MoveDice(Rigid.position);
        }

        public void StopJumpAnim()
        {
            if(jumpSequence != null && jumpSequence.IsPlaying())
            {
                jumpSequence.Kill(false);
                head.localPosition = originHeadPosition;
                head.localRotation = originHeadRotation;
            }
        }

        public void ClimbStairs()
        {
            ExecuteStairJump(GameManager.Instance.StairCount);
        }

        private void ExecuteStairJump(int remainingJumps)
        {
            if (remainingJumps > 0)
            {
                StairJump(() => ExecuteStairJump(remainingJumps - 1)); // 다음 점프 호출
            }
        }
    }
}
