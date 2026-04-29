using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 화살표 버튼 Click Event Class
public class ArrowButtonClick : MonoBehaviour
{

    // 화살표 종류
    [Tooltip("화살표 종류")]
    public TotalParameter.Arrow arrow;

    // 화살표 이미지
    Image arrowImg;

    // 선택한 화살표 리스트
    [Tooltip("선택한 화살표 이미지 넣을 Object Parent")]
    [SerializeField]
    GameObject selectedArrowsParent;

    // 선택한 화살표 리스트
    SelectedButtons selectedButtons;

    void Start()
    {
        // 현재 버튼의 화살표 이미지를 들고 온다
        arrowImg = GetComponent<Image>();

        selectedButtons = selectedArrowsParent.GetComponent<SelectedButtons>();
    }

    public void ArrowClick()
    {
        // 화살표 종류 개수 만큼 화살표 선택을 하지 않았다면
        if(selectedButtons.selectCount < TotalParameter.Instance.arrowTotalCount)
        {
            // 선택한 화살표 이미지 넣어주기
            selectedButtons.selectedArrowImgs[selectedButtons.selectCount].sprite = arrowImg.sprite;

            // 선택한 화살표 value 전달
            selectedButtons.arrow[selectedButtons.selectCount] = arrow;

            // 선택 화살표 Count 증가
            selectedButtons.selectCount++;
        }
        else
        {
            print("초과");
        }
    }
}
