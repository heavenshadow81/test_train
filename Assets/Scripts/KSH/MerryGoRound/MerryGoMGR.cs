using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class MerryGoMGR : MonoBehaviour, Game.IMyGameActions
{
   public static MerryGoMGR Instance;

    public EnumClass stateClass;
    public GameUI gameUI;
    public ScreenProsess screenProsess;

    public LoadSprite loadSprite;

   [System.NonSerialized] public Electronicdisplay display;

    public bool isDown;

    public spriteData spriteData;
    public bool b_Input = false;

    public PoleHorse[] poleHorse;

    //게이지 프로퍼티
    private float dirspeed;
    public float DirSpeed
    { 
        get => dirspeed;
        set
        {
            dirspeed = value;
           
            float angle = (dirspeed * -10f) <= -160f ? -160f : dirspeed * -10f;
            //각도 조절
            speedGaugePoint.transform.localRotation = Quaternion.Euler(0, 0, angle);
            //각도 스피드가 0이면 실패 상태로 변경
            if (dirspeed <= 0 && stateClass.state == GameState.GamePlay)
            {
                //게임결과상태를 패배상태
                stateClass.resultState = GameResult.Fail;
                //게임을 결과 상태로 이동
                zozo.Change(GameState.GameResult);
            }
        }
    }
    public GameObject speedGaugePoint;

    public ZoZoBasePatton<MerryGoMGR> zozo;
    public Game gameInput;
    private void Awake()
    {
        Instance = this;
        stateClass = new EnumClass();
        loadSprite = new LoadSprite("MerryGoRound");
        display = FindObjectOfType<Electronicdisplay>();

        DirSpeed = 16;
        
        display.QueueArrowAdd();

        foreach (var item in poleHorse)
        {
            //회전목마 말 세팅
            item.HorseSetting();
        }

        #region 공용 스테이트 패턴 
        ActionProcess.Enter_StateListener(null, null, null, null);
        zozo = new ZoZoBasePatton<MerryGoMGR>();
        zozo.Init(stateClass, screenProsess, new ReadyProcess(screenProsess), new ResultProcess(screenProsess));

        #endregion


    }
    public void SpeedUP(float sp)
    {
      //  DirSpeed = DirSpeed >= 8 ? 8 : DirSpeed + sp;
        DOTween.To(() => DirSpeed, x => DirSpeed = x, DirSpeed = DirSpeed >=16 ? 16 : DirSpeed + sp, 0.1f);
    }

    private void Update()
    {
        //Play 상태일때
        if (zozo != null) zozo.MGR.Excute(() => 
        {
            //speed 가 0아래로 떨어지면 
            if ((DirSpeed -= Time.deltaTime * 2f) <= 0)
            {
                DirSpeed = 0;
            }
        });
    }

    private void OnEnable()
    {
        gameInput = new();
        gameInput.Enable();
        gameInput.MyGame.SetCallbacks(this);

        EnhancedTouchSupport.Enable();
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown += onFingerDown;
    }
    private void onFingerDown(Finger finger)
    {
        var hit2D = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(finger.currentTouch.screenPosition), Vector2.zero);
        if (hit2D)
        {
            if (hit2D.collider.TryGetComponent<ArrowPanel>(out var arrow))
            {
                arrow.Down();
            }
        }
    }
    private void OnDisable()
    {
        ReadyProcess.sourceCancle?.Invoke();
        gameInput.Disable();

        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown -= onFingerDown;
        EnhancedTouchSupport.Disable();
    }
    private void OnDestroy()
    {
        ReadyProcess.sourceCancle?.Invoke();
        ReadyProcess.sourceDispone?.Invoke();
    }

    //pc에서 마우스로 터치했을때
    public void OnDown(InputAction.CallbackContext context)
    {
        if (Settings.instance.mouseToggle.isOn == false) return;
        if (context.ReadValue<float>() == 1f)
        {
            var hit2D = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Settings.instance.MousePos()), Vector2.zero);
            if (hit2D)
            {
                if (hit2D.collider.TryGetComponent<ArrowPanel>(out var arrow))
                {
                    arrow.Down();
                }
            }
        }
    }

    public void OnTouch(InputAction.CallbackContext context)
    {
        //var hit2D = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>()), Vector2.zero);
        //if (hit2D)
        //{
        //    if (hit2D.collider.GetComponent<ArrowPanel>() == this)
        //    {
        //        Down();
        //    }
        //}
    }

    public void OnIsDown(InputAction.CallbackContext context)
    {
    }

}
