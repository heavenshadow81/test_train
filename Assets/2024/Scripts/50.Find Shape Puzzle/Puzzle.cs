using DG.Tweening;
using GuessNumber;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UILabel;

namespace FindShapePuzzle
{
    public class Puzzle : MonoBehaviour
    {
        [Header("회전 애니메이션 설정")]
        private float rotationAngle = 360f;  
        private Ease rotationEaseType = Ease.Linear;  

        [Header("이동 애니메이션 설정")]
        private float moveDistance = 2f;     
        private float moveDuration = 1f;     
        private Ease moveEaseType = Ease.OutQuad;

        [SerializeField] GameObject correctEffect;
        [SerializeField] GameObject wrongEffect;
        private Tween moveTween;             
        private Tween rotationTween;         
        private Rigidbody rigidBody;

        private Vector3 originPos;
        private Quaternion originRot;
        public ObjectFade Fade { get; private set; }

        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody>();
            Fade = GetComponent<ObjectFade>();
            originPos = transform.localPosition;
            originRot = transform.localRotation;
        }

        private void OnDisable()
        {
            StopRotationAndMove();
        }

        public Tween MoveAndRotate(float rotationDuration)
        {
            moveTween = transform.DOMoveY(transform.position.y + moveDistance, moveDuration)
                                 .SetEase(moveEaseType)  
                                 .OnComplete(() =>
                                 {
                                     RotateClockwise(rotationDuration);
                                 });

            return moveTween;
        }
       
        public void SetOriginTransform()
        {
            transform.localPosition = originPos;
            transform.localRotation = originRot;
        }
        public void RotateClockwise(float rotationDuration)
        {
            rotationTween = transform.DORotate(new Vector3(transform.rotation.x, rotationAngle, transform.rotation.z), rotationDuration, RotateMode.FastBeyond360)
                                      .SetEase(rotationEaseType) 
                                      .SetLoops(-1);  
        }

        public void StopRotation()
        {
            if (rotationTween != null && rotationTween.IsActive())
            {
                rotationTween.Kill();  // 현재 회전 트윈을 중지
                rotationTween = null;
            }

            // 회전을 초기 상태로 설정할 필요가 있다면 여기에 초기화 추가
            transform.rotation = Quaternion.identity; // 회전을 기본 상태로 초기화
        }

        public void MoveToAnswerZone(Vector3 targetPosition)
        {
            SoundMGR.Instance.SoundPlay("Answer");
            correctEffect.SetActive(true);

            transform.DOMoveY(transform.position.y + moveDistance, moveDuration)
                                .SetEase(moveEaseType).OnComplete(() =>
            {
                RotateClockwise(0.5f);
                SoundMGR.Instance.SoundPlay("PuzzleMove");
                transform.DOMove(targetPosition, moveDuration)
                                 .SetEase(Ease.Linear)  // Ease 타입 설정
                                 .OnComplete(() =>
                                 {
                                     SoundMGR.Instance.SoundPlay("Arrive");
                                     correctEffect.SetActive(false);
                                     Fade.FadeOut(1f, () =>
                                     {
                                         StopRotation();
                                         GameManager.Instance.SavePuzzleInFrame(GameManager.Instance.SpawnCandy);
                                     });
                                 });
            });
        }



        public void StopRotationAndMove()
        {
            if (rotationTween != null && rotationTween.IsActive())
            {
                rotationTween.Kill();
                rotationTween = null;
            }

            if (moveTween != null && moveTween.IsActive())
            {
                moveTween.Kill();
                moveTween = null; 
            }

            transform.localRotation = Quaternion.identity;
            transform.localPosition = Vector3.zero;
        }

        public void ToggleKinematic()
        {
            rigidBody.isKinematic = !rigidBody.isKinematic;
        }

        public void ToggleTag()
        {
            if (CompareTag("Puzzle"))
            {
                gameObject.tag = "Untagged";
            }
            else
            {
                gameObject.tag = "Puzzle";
            }
        }

        public void TouchWrongAnswer()
        {
            if (wrongEffect.activeSelf)
            {
                wrongEffect.SetActive(false); 
                wrongEffect.SetActive(true);  
            }
            else
            {
                wrongEffect.SetActive(true);  
            }

        }
    }
}
