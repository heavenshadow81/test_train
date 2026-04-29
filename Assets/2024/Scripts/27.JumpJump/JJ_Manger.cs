using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JJ_Manger : PlayManager_PlayGround
{
    [SerializeField] JJ_Rabbit rabbit; //토끼 오브젝트 스크립트

    [Header("UI Raycast")]
    public GraphicRaycaster raycaster; // Canvas에 부착된 GraphicRaycaster
    public EventSystem eventSystem;    // UI 이벤트 시스템
    

    protected override void Init()
    {
        base.Init();

        SettingSlot();
    }
    void SettingSlot()
    {
        
    }

    public override void HandleInput(Vector2 inputPosition)
    {
        // 터치 활성화
        isTouchable = true;

        // PointerEventData를 통해 입력 위치 설정
        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = inputPosition;

        // Raycast 결과를 저장할 리스트 생성
        List<RaycastResult> results = new List<RaycastResult>();

        // GraphicRaycaster로 UI 요소에 대한 Raycast 실행
        raycaster.Raycast(pointerData, results);

        if (results.Count > 0)
        {
            foreach (RaycastResult result in results)
            {
                // 특정 태그를 가진 UI 요소인지 확인
                if (result.gameObject.CompareTag(TeamNameString))
                {

                    SoundMGR.Instance.SoundPlay("PlayGround_Moving");
                    //토끼 오브젝트 점프 함수 실행
                    rabbit.Jump();
                }
            }
        }
    }

    public override void CorrectAnswer(GameObject touched)
    {
    }

    public override void WrongAnswer(GameObject touched)
    {
    }
}
