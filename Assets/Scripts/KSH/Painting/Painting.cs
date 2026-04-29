using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class Painting : MonoBehaviour,Game.IMyGameActions
{
    //저장할 월드 포지션
    Vector2 worldPoint;

    //뉴인풋
    public Game inputGame;
    //저장될 Hit
    RaycastHit2D hit2D;
    private void OnEnable()
    {
        inputGame = new Game();
        inputGame.Enable();
        inputGame.MyGame.AddCallbacks(this);

        EnhancedTouchSupport.Enable();
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown += touch_onFingerDown;
    }

    private void touch_onFingerDown(Finger finger)
    {
        Down(finger.currentTouch.screenPosition);
    }

    private void OnDisable()
    {
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown -= touch_onFingerDown;
        EnhancedTouchSupport.Disable();
        inputGame.Disable();
    }


    public void OnDown(InputAction.CallbackContext context)
    {
        if (Settings.instance.mouseToggle.isOn == false) return;
        if (context.ReadValue<float>() == 1f)
        {
            Down(Settings.instance.MousePos());
        }
    }

    public void OnTouch(InputAction.CallbackContext context)
    {
        //Vector3 endScale = ((Vector3.one) - (Vector3.one * 0.7f)) * Random.Range(1, 4);
        //worldPoint = Camera.main.ScreenToWorldPoint(touchInputs);
        ////RaycastHit2D hit2D = Physics2D.Raycast(worldPoint, Vector2.zero, 0);

        //var stamp = PaintMGR.Instance.stampPool.Get();
        //await PaintMGR.Instance.loadSprite.LoadSpriteData($"foot{Random.Range(1, 7)}", stamp.spriteRenderer);
        //stamp.transform.localScale = endScale;
        //stamp.transform.position = worldPoint;
        //stamp.spriteRenderer.color = PaintMGR.Instance.PaintColor;
        //PaintMGR.Instance.sorting += 1;
        //stamp.spriteRenderer.sortingOrder = PaintMGR.Instance.sorting;
        //PaintMGR.Instance.stampList.Add(stamp);
    }

    private async void Down(Vector2 fingerPos)
    {
        //게임 플레이중일때만
        if (PaintMGR.Instance.stateClass.state != GameState.GamePlay) return;

        //사운드 플레이
        SoundMGR.Instance.SoundPlay("21.발도장생성");
        
        //스케일값 조정
        Vector3 endScale = ((Vector3.one) - (Vector3.one * 0.7f)) * Random.Range(1, 4);
        //터치한 위치값 변환 스크린에서 월드
        worldPoint = Camera.main.ScreenToWorldPoint(fingerPos);
        //2D 레이케스팅
        hit2D = Physics2D.Raycast(worldPoint, Vector2.zero,0);
       
        if (hit2D)
        {   
            //페이퍼에 터치나 클릭이 됬다면
            if (hit2D.collider.gameObject.layer == LayerMask.NameToLayer("Paper"))
            {
                stamp(endScale);
            }
            //지우개이미지에 터치나 클릭이 됬다면
            else if (hit2D.collider.TryGetComponent<Eraser>(out var eraser))
            {
                eraser.DownProcess();
            }
            //색상이미지에 터치나 클릭이 됬다면
            else if (hit2D.collider.TryGetComponent<Paint>(out var paint))
            {
                paint.Down();
            }
        }
        
    }

    public void OnIsDown(InputAction.CallbackContext context) { }

    private void stamp(Vector3 endScale)
    {
        //발바닭 오브젝트 생성
        var stamp = PaintMGR.Instance.stampPool.Get();
        //발바닥 이미지 로드
        PaintMGR.Instance.loadSprite.LoadSpriteData($"foot{Random.Range(1, 7)}", stamp.spriteRenderer);
        //계산된 스케일값
        stamp.transform.localScale = endScale;
        //터치한위치의 월드 포지션값 
        stamp.transform.position = worldPoint;
        //저장된 컬러값
        stamp.spriteRenderer.color = PaintMGR.Instance.PaintColor;
        //랜덤방향
        stamp.transform.rotation = Quaternion.Euler(Vector3.forward * Random.Range(0, 359));
        //찍을때마다 솔팅 + 1
        PaintMGR.Instance.sorting += 1;
        stamp.spriteRenderer.sortingOrder = PaintMGR.Instance.sorting;
        //스탬프 리스트에 저장
        PaintMGR.Instance.stampList.Add(stamp);
    }
}
