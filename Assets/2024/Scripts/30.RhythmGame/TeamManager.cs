using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.EventSystems;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using static UnityEngine.EventSystems.StandaloneInputModule;
using vCatchStation;

namespace RhythmGame
{
    public enum TeamName
    {
        Green,
        Orange
    }

    public class TeamManager : MonoBehaviour
    {
        [Header("게임 설정")]
        [SerializeField] bool isTouchable = false;
        [SerializeField] TimingManager timingManager;

        [Header("이펙트")]
        [SerializeField] protected GameObject effect;

        [Header("인풋시스템")]
        [SerializeField] InputActionAsset actionAsset;
        private InputActionMap actionMap;
        protected InputAction touchAction;

        [Header("게임승리")]
        [SerializeField] protected GameObject gameCanvas;
        [SerializeField] protected GameObject victoryUI;

        [Header("팀 이름")]
        [SerializeField] private TeamName teamName; // enum으로 팀 이름을 설정
        public string TeamNameString => teamName.ToString(); // 문자열로 변환

        private void OnEnable()
        {
            // 인풋시스템 등록
            actionMap = actionAsset.FindActionMap("Touch");
            touchAction = actionMap.FindAction("Touch");
            isTouchable = true;

//#if UNITY_EDITOR
            touchAction.Enable();
            touchAction.started += OnClick;
//#else
            EnhancedTouchSupport.Enable();
            Touch.onFingerDown += OnTouch;
//#endif
        }

        private void OnDisable()
        {
//#if UNITY_EDITOR
            if (touchAction != null)
            {
                touchAction.started -= OnClick;
            }
//#else
        if (EnhancedTouchSupport.enabled)
        {
            Touch.onFingerDown -= OnTouch;
        }
//#endif
        }

//#if UNITY_EDITOR
        private void OnClick(InputAction.CallbackContext context)
        {
            if (!isTouchable) return; // 터치가 비활성화된 경우 입력을 무시

            // 터치가 감지되었으므로 즉시 터치 비활성화
            isTouchable = false;

            HandleInput(Input.mousePosition); // 기존 마우스 포지션 처리
        }
//#else 
    private void OnTouch(Finger finger)
    {
        if (!isTouchable) return; // 터치가 비활성화된 경우 입력을 무시

            // 터치가 감지되었으므로 즉시 터치 비활성화
            isTouchable = false;

            HandleInput(finger.screenPosition); // 기존 터치 포지션 처리
    }
//#endif


        private void HandleInput(Vector2 inputPosition)
        {
            // 터치/마우스 위치를 월드 좌표로 변환
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(inputPosition);

            // 특정 레이어 무시: NoteLayer를 제외하기 위한 레이어 마스크 설정
            int layerMask = 1 << LayerMask.NameToLayer("Note");
            layerMask = ~layerMask; // NoteLayer를 제외하기 위해 NOT 연산자 적용

            float rayLength = 10f; // 레이 길이를 적절히 설정

            // 레이캐스트 시 특정 레이어를 제외
            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero, rayLength, layerMask);

            if (hit.collider != null)
            {
                // 터치한 카드가 어떤 태그를 가지고 있는지 확인
                if (hit.collider.CompareTag(TeamNameString))
                {
                    Base selectedSlot = hit.collider.GetComponent<Base>();
                    if (selectedSlot != null)
                    {
                        CheckAnswer(selectedSlot);
                        timingManager.CheckTiming(selectedSlot.GetLine());

                        // 터치 후 다시 활성화할 시간을 설정
                        StartCoroutine(ResetTouchDelay());
                    }
                }
                else
                {
                    // 터치한 오브젝트가 적합하지 않으면 즉시 다시 터치 가능
                    isTouchable = true;
                }
            }
            else
            {
                // 아무것도 맞지 않았을 경우 다시 터치 가능
                isTouchable = true;
            }
        }

        IEnumerator ResetTouchDelay()
        {
            yield return new WaitForSeconds(0.2f); // 짧은 대기 시간 후 다시 터치 가능
            isTouchable = true; // 다시 터치 활성화
        }

        void CheckAnswer(Base selectedSlot)
        {
            selectedSlot.PlayTouchAnim();
            // 매번 파티클을 인스턴스화하여 재생
            ParticleSystem newParticle = Instantiate(effect, selectedSlot.transform).GetComponent<ParticleSystem>();
            newParticle.Play();

            // 파티클이 재생이 끝나면 파괴
            Destroy(newParticle.gameObject, newParticle.main.duration); // 파티클의 수명에 맞춰 파괴
        }
    }
}

