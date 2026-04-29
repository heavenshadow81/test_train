using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem;
public class Questionframe : MonoBehaviour, Game.IMyGameActions
{

    public QuestionHuman[] questionHumans;

    [NonSerialized] public int questionCnt;

    public Game inputGame;
    private void OnEnable()
    {
        inputGame = new();
        inputGame.Enable();
        inputGame.MyGame.SetCallbacks(this);

        EnhancedTouchSupport.Enable();
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown += onFingerDown;
    }
    private void OnDisable()
    {
        inputGame.Disable();
    }

    //Unity InputSystem 스크린용 
    private void onFingerDown(Finger fingerPos)
    {
        downProcess(fingerPos.currentTouch.screenPosition);
    }

    public void HumanSelectToSpriteLoad()
    {
        //섞기
        for (int x = 0; x < questionHumans.Length; x++)
        {
            int r = UnityEngine.Random.Range(0, questionHumans.Length);
            int d = UnityEngine.Random.Range(0, questionHumans.Length);
            var temp = questionHumans[r];
            questionHumans[r] = questionHumans[d];
            questionHumans[d] = temp;
        }

        //사람에게 자신의 컬러값을 가진 Enum 세팅
        int i = 0;
        foreach (var human in questionHumans) 
        {
            human.FindColor = (HumanColor)i;
            i++;
        }
        //이미지 로드
        childLoadImage().Forget();
    }

    //사람 6인 머리, 몸, 팔 이미지 로드
    private async UniTask childLoadImage()
    {
        foreach (var human in questionHumans)
        {
            await UniTask.WhenAll
            (
                //머리이미지 로드
                FerrisMgr.Instance.loadSprite.LoadSpriteData($"{human.FindColor.ToString()}HEAD", human.head),
                //몸이미지 로드
                FerrisMgr.Instance.loadSprite.LoadSpriteData($"{human.FindColor.ToString()}BODY", human.body),
                //왼팔이미지 로드
                FerrisMgr.Instance.loadSprite.LoadSpriteData($"{human.FindColor.ToString()}LEFTARM", human.leftarm),
                //오른팔 이미지 로드
                FerrisMgr.Instance.loadSprite.LoadSpriteData($"{human.FindColor.ToString()}RIGHTARM", human.rightarm)
            );
        }

    }

    //pc용 down
    public void OnDown(InputAction.CallbackContext context)
    {
        if (Settings.instance.mouseToggle.isOn == false) return;

        downProcess(Settings.instance.MousePos());
    }

    //Down
    private void downProcess(Vector2 pos)
    {
        var hit2D = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(pos), Vector2.zero);

        if (hit2D)
        {
            if (hit2D.collider.TryGetComponent<QuestionHuman>(out var human))
            {
                human.Down();
            }
        }
    }

    public void OnTouch(InputAction.CallbackContext context) { }

    public void OnIsDown(InputAction.CallbackContext context) { }
}
