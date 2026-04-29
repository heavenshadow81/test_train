using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.EventSystems;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public enum TeamName
{
    Green,
    Orange
}

public abstract class PlayManager_PlayGround : MonoBehaviour, IPlayManager
{
    [Header("정답 클릭")]
    [SerializeField] protected GameObject effect;
    [SerializeField] protected GameObject perfectEffect;
    [SerializeField] protected Transform perfectEffectPos;
    protected bool isPerfect = false;
    protected bool isTouchable = false;

    [Header("인풋시스템")]
    [SerializeField] InputActionAsset actionAsset;
    private InputActionMap actionMap;
    protected InputAction touchAction;

    [Header("게이지UI")]
    [SerializeField] Image[] gauges;
    [SerializeField] Sprite[] gaugeSprites;
    protected int stack = 0;
    [SerializeField] protected int maxStack;

    [Header("게임승리")]
    [SerializeField] protected GameObject gameCanvas;
    [SerializeField] protected GameObject victoryUI;

    [Header("팀 이름")]
    [SerializeField] private TeamName teamName; // enum으로 팀 이름을 설정
    public string TeamNameString => teamName.ToString(); // 문자열로 변환

    private void Start()
    {
        GameManager_PlayGround.Instance.OnGameStart += Init;
    }

    protected virtual void Init()
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

    protected virtual void OnDisable()
    {
        GameManager_PlayGround.Instance.OnGameStart -= Init;

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

        isTouchable = false;

        HandleInput(Input.mousePosition); // 기존 마우스 포지션 처리
    }
//#else 
    private void OnTouch(Finger finger)
    {
        if (!isTouchable) return; // 터치가 비활성화된 경우 입력을 무시

        isTouchable = false;

        HandleInput(finger.screenPosition); // 기존 터치 포지션 처리
    }
//#endif

    // 추상 메서드 - 입력 처리 방식 구현
    public abstract void HandleInput(Vector2 inputPosition);
    public abstract void CorrectAnswer(GameObject touched);
    public abstract void WrongAnswer(GameObject touched);

    protected void ChangeGauge()
    {
        gauges[stack].sprite = gaugeSprites[stack];
        stack++;

        CheckVictory();
    }

    protected virtual void CheckVictory()
    {
        if (stack >= maxStack)
        {
            touchAction.Disable();
            SoundMGR.Instance.SoundPlay("PlayGround_Victory");
            gameCanvas.SetActive(false);
            victoryUI.SetActive(true);
        }
    }
}
