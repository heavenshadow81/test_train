using Bax.P0.Client.UnityWorld.BalloonGame;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;


public class Foothold : MonoBehaviour, Game.IMyGameActions
{
    //발판 Down
    public bool isDown = false;
    //발판 스프라이트랜더러
    private SpriteRenderer foot_SpriteRenderer;

    //Unity용 newInputSystem 용 InputAction
    public Game inputGame;


    private void OnEnable()
    {
        //newInputSystem 세팅
        inputGame = new Game();
        inputGame.MyGame.Enable();
        inputGame.MyGame.SetCallbacks(this);

        //newInputSystem용 스크린멀티터치 이벤트 연결
        EnhancedTouchSupport.Enable();
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown += onFingerDown;
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerUp += onFingerUp;

    }

    //이벤트 연결용
    private void onFingerDown(Finger obj)
    {
        Down(obj.currentTouch.screenPosition);
    }
    //이벤트 연결용
    private void onFingerUp(Finger obj)
    {
        Up(obj.currentTouch.screenPosition);
    }

    private void OnDisable()
    {
        ////newInputSystem 해제
        inputGame.Disable();
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown -= onFingerDown;
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerUp -= onFingerUp;
        EnhancedTouchSupport.Disable();
    }

    private void Awake()
    {
        //시작시 랜더러 세팅 후 이미지 연결
        foot_SpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        foot_SpriteRenderer.sprite = BalloonMgr.instance.spriteData.footUp;
    }
    public void Down(Vector2 fingetPos)
    {
        if (isDown == false && BalloonMgr.instance.ArrowCurCount != 0 && BalloonMgr.instance.loadArrow && BalloonMgr.instance.stateClass.state == GameState.GamePlay)
        {
            //터치한 위치에 레이캐스트
            var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(fingetPos), Vector2.zero);

            //충돌체가 있고
            if (hit.collider != null)
            {
                //충돌한 녀석이 발판 이라면
                if (hit.collider.gameObject.TryGetComponent<Foothold>(out var foothold))
                {
                    
                    isDown = true;
                    //밣은 이미지 변경
                    foot_SpriteRenderer.sprite = BalloonMgr.instance.spriteData.footDown;

                    //화살 발사
                    BalloonMgr.instance.loadArrow.arrowMove = ArrowMove.Shoot;
                    //화살의 부모를 null
                    BalloonMgr.instance.loadArrow.transform.parent = null;
                    //load된 화살이 없음으로 변경
                    BalloonMgr.instance.loadArrow = null;

                    //남은 화살이 없다면 탈출
                    if (--BalloonMgr.instance.ArrowCurCount <= 0) return;
                    //다음 화살 장전
                    arrowReload().Forget();

                    SoundMGR.Instance.SoundPlay("바람소리");
                }
            }
        }
    }

    public void Up(Vector2 fingetPos)
    {
        if (isDown == true)
        {
            isDown = false;

            var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(fingetPos), Vector2.zero);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.TryGetComponent<Foothold>(out var foothold))
                    foot_SpriteRenderer.sprite = BalloonMgr.instance.spriteData.footUp;
            }
        }
    }
    public void Up()
    {
        if (isDown == true)
        {
            isDown = false;

            foot_SpriteRenderer.sprite = BalloonMgr.instance.spriteData.footUp;
        }
    }

    //화살 장전
    private async UniTask arrowReload()
    {
        //0.5초 후
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f), DelayType.UnscaledDeltaTime);
        //화살 갯수가 남아있다면 장전
        if (BalloonMgr.instance.ArrowCurCount > 0) BalloonMgr.instance.bow.BowNLoadArrow();
    }


    //NewInputSystem - InputAction 에 입력되있는 Down 을 자동실행 Down 1  Up 0 
    //Pc 용
    public void OnDown(InputAction.CallbackContext context)
    {
        if (Settings.instance.mouseToggle.isOn == false) return;

        if (context.ReadValue<float>() == 1f)
            Down(Settings.instance.MousePos());
        else
            Up();
    }

    public void OnTouch(InputAction.CallbackContext context){}

    public void OnIsDown(InputAction.CallbackContext context){}
   
    
}
