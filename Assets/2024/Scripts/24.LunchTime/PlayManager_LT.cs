using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.SocialPlatforms.Impl;

public class PlayManager_LT : PlayManager_PlayGround
{
    [Header("게임 설정")]
    [SerializeField] int stage;
    [SerializeField] TableWareCountUI_LT countUI;
    [SerializeField] TableWare_LT tableWare;

    private void OnEnable()
    {
        countUI.OnFullStack += ChangeGauge;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        countUI.OnFullStack -= ChangeGauge;
    }

    public override void HandleInput(Vector2 inputPosition)
    {
        // 터치/마우스 위치를 월드 좌표로 변환
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(inputPosition);

        // 터치/마우스 위치에서 카드 찾기
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

        if (hit.collider != null)
        {
            // 터치한 카드가 어떤 태그를 가지고 있는지 확인
            if (hit.collider.CompareTag(TeamNameString))
            {
                //TableWare_LT tableWare = hit.collider.GetComponent<TableWare_LT>();
                isTouchable = false;
                tableWare.ShootTableWare();
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


    public override void CorrectAnswer(GameObject touched)
    {      
        isTouchable = true;
    }

    public override void WrongAnswer(GameObject touched)
    {

        isTouchable = true;
    }

    public int GetStack()
    {
        return stack;
    }

    public void DiableInput()
    {
        touchAction.Disable();
    }

    public void SetTouchable()
    {
        isTouchable = true;
    }
}
