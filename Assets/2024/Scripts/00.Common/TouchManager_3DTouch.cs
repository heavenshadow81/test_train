using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.EventSystems;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using vCatchStation;


public abstract class TouchManager_3DTouch : MonoBehaviour
{
    [Header("정답 클릭")]
    [SerializeField] protected GameObject[] effect;

    [Header("인풋시스템")]
    [SerializeField] InputActionAsset actionAsset;
    private InputActionMap actionMap;
    protected InputAction touchAction;

    [Header("게임승리")]
    [SerializeField] protected GameObject gameCanvas;
    [SerializeField] protected GameObject victoryUI;

    [Header("팀 이름")]
    [SerializeField] protected TeamName teamName; // enum으로 팀 이름을 설정 (레이 감지용 태그)
    public string TeamNameString => teamName.ToString(); // 문자열로 변환

    [SerializeField] protected bool isTouchable = false;

    protected virtual void Start()
    {
        // 인풋시스템 등록
        actionMap = actionAsset.FindActionMap("Touch");
        touchAction = actionMap.FindAction("Touch");
        touchAction.Enable();
        touchAction.started += OnClick;
        EnhancedTouchSupport.Enable();
        Touch.onFingerDown += OnTouch;

        isTouchable = true;
    }

    protected virtual void OnDisable()
    {
        if (touchAction != null)
        {
            touchAction.started -= OnClick;
        }

        if (EnhancedTouchSupport.enabled)
        {
            Touch.onFingerDown -= OnTouch;
        }
    }

    protected virtual void OnClick(InputAction.CallbackContext context)
    {
        if (!isTouchable) return; // 터치가 비활성화된 경우 입력을 무시
        isTouchable = false;
        HandleInput(Input.mousePosition); // Ray가 충돌한 3D 공간 좌표 전달
    }

    protected virtual void OnTouch(Finger finger)
    {
        if (!isTouchable) return; // 터치가 비활성화된 경우 입력을 무시

        isTouchable = false;

        HandleInput(finger.screenPosition); // Ray가 충돌한 3D 공간 좌표 전달
    }

    // 추상 메서드 - 입력 처리 방식 구현
    public abstract void HandleInput(Vector2 pos);

}
