using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class UIManager_PlayGround : MonoBehaviour
{
    [SerializeField] Button[] pageButtons;
    [SerializeField] List<GameObject> baseBoards;

    [SerializeField] GameObject fadeUI;

    public static int pageNum;

    private void Start()
    {
        //모든 베이스보드 비활성화
       for(int i = 0; i < baseBoards.Count; i++)
        {
            baseBoards[i].SetActive(false);
        }

       //페이지 넘버에 맞는 베이스보드만 활설화
        baseBoards[pageNum].SetActive(true);
    }

    // UIFade 메서드에 Action 콜백을 추가
    public void UIFade(Action onComplete)
    {
        fadeUI.SetActive(true);
        fadeUI.GetComponent<Image>().DOFade(1, 1f).OnComplete(() =>
        {
            onComplete?.Invoke(); // 페이드 아웃 완료 후 콜백 실행
        });
    }

    public void OnClick_NextPage()
    {
        if (baseBoards.Count <= 1) return;

        // 모든 버튼을 비활성화하여 연속 클릭 방지
        SetButtonsEnable(false);

        SoundMGR.Instance.SoundPlay("PlayGround_Page");

        GameObject currentBoard = baseBoards[pageNum];
        pageNum = (pageNum + 1) % baseBoards.Count; // 페이지 번호 증가 (순환)
        GameObject nextBoard = baseBoards[pageNum];

        // CanvasGroup을 사용하여 currentBoard와 자식들의 알파 값을 조절
        CanvasGroup canvasGroup = currentBoard.GetComponent<CanvasGroup>();

        // 페이드 아웃 애니메이션
        canvasGroup.DOFade(0, 1f).OnComplete(() =>
        {
            // 페이드 아웃이 완료된 후에 currentBoard를 비활성화
            currentBoard.SetActive(false);

            // 다음 페이지를 활성화하고, 페이드 인과 슬라이드 인 애니메이션
            nextBoard.SetActive(true);

            // nextBoard의 CanvasGroup을 사용하여 페이드 인 애니메이션 적용
            CanvasGroup nextCanvasGroup = nextBoard.GetComponent<CanvasGroup>();
            if (nextCanvasGroup == null)
            {
                nextCanvasGroup = nextBoard.AddComponent<CanvasGroup>();
            }

            nextCanvasGroup.alpha = 0;  // 페이드 인 시작 시 완전히 투명하게 설정
            nextCanvasGroup.DOFade(1, 0.5f);  // 페이드 인 애니메이션

            // 슬라이드 인 애니메이션
            nextBoard.transform.localPosition = new Vector3(300, nextBoard.transform.localPosition.y, nextBoard.transform.localPosition.z);
            nextBoard.transform.DOLocalMoveX(0, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                SetButtonsEnable(true); // 버튼 다시 활성화
            });
        });

        // 현재 페이지를 오른쪽으로 슬라이드 아웃 애니메이션
        currentBoard.transform.DOLocalMoveX(-800, 1f).SetEase(Ease.InBack);
    }

    public void OnClick_PrevPage()
    {
        if (baseBoards.Count <= 1) return;

        // 모든 버튼을 비활성화하여 연속 클릭 방지
        SetButtonsEnable(false);

        SoundMGR.Instance.SoundPlay("PlayGround_Page");

        GameObject currentBoard = baseBoards[pageNum];
        pageNum = (pageNum - 1 + baseBoards.Count) % baseBoards.Count; // 페이지 번호 감소 (순환)
        GameObject prevBoard = baseBoards[pageNum];

        // CanvasGroup을 사용하여 currentBoard와 자식들의 알파 값을 조절
        CanvasGroup canvasGroup = currentBoard.GetComponent<CanvasGroup>();

        // 페이드 아웃 애니메이션
        canvasGroup.DOFade(0, 1f).OnComplete(() =>
        {
            // 페이드 아웃이 완료된 후에 currentBoard를 비활성화
            currentBoard.SetActive(false);

            // 이전 페이지를 활성화하고, 페이드 인과 슬라이드 인 애니메이션
            prevBoard.SetActive(true);

            // prevBoard의 CanvasGroup을 사용하여 페이드 인 애니메이션 적용
            CanvasGroup prevCanvasGroup = prevBoard.GetComponent<CanvasGroup>();
            if (prevCanvasGroup == null)
            {
                prevCanvasGroup = prevBoard.AddComponent<CanvasGroup>();
            }

            prevCanvasGroup.alpha = 0;  // 페이드 인 시작 시 완전히 투명하게 설정
            prevCanvasGroup.DOFade(1, 0.5f);  // 페이드 인 애니메이션

            // 슬라이드 인 애니메이션
            prevBoard.transform.localPosition = new Vector3(-300, prevBoard.transform.localPosition.y, prevBoard.transform.localPosition.z);
            prevBoard.transform.DOLocalMoveX(0, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                SetButtonsEnable(true); // 버튼 다시 활성화
            });
        });

        // 현재 페이지를 왼쪽으로 슬라이드 아웃 애니메이션
        currentBoard.transform.DOLocalMoveX(800, 1f).SetEase(Ease.InBack);
    }

    private void SetButtonsEnable(bool enable)
    {
        // 하이어라키에 있는 모든 버튼을 찾아서 처리
        Button[] allButtons = FindObjectsOfType<Button>();

        foreach (Button button in allButtons)
        {
            button.enabled = enable;
        }
    }

    public void SetBaseBoardsList(PageArray newList)
    {
        // baseBoards 리스트 초기화
        baseBoards.Clear();

        // 페이지 숫자 초기화
        //pageNum = 0;

        // 각 PageArray의 pages 배열을 순차적으로 처리
        for (int i = 0; i < newList.pages.Length; i++)
        {
            baseBoards.Add(newList.pages[i]);
        }
    }
}
