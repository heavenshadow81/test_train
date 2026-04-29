using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 선택한 화살표를 이미지를 삽입 할 Object 정보를 들고 있는 Class
public class SelectedButtons : MonoBehaviour
{
    // 화살표 이미지를 삽입 할 오브젝트 리스트
    [Tooltip("선택 한 화살표 이미지 삽입 Object")]
    public Image[] selectedArrowImgs;

    [Tooltip("OK 버튼")]
    public Button okButton;

    // 선택 된 화살표 Count
    [Tooltip("선택 된 화살표 개수")]
    public int selectCount;

    [Tooltip("초기화 이미지")]
    [SerializeField]
    Sprite defualtImage;

    [Tooltip("알림 이미지")]
    [SerializeField]
    GameObject alertImage;

    [Tooltip("Center_1 Object")]
    [SerializeField]
    GameObject center_1_Object;

    [Tooltip("Bottom Object")]
    [SerializeField]
    GameObject bottomButtonObject;

    [Tooltip("playerCharacter")]
    [SerializeField]
    GameObject playerCharacter;

    public TotalParameter.Arrow[] arrow;

    void Start()
    {
        // 화살표 종류 개수 만큼 이미지 배열 길이 할당
        selectedArrowImgs = new Image[TotalParameter.Instance.arrowTotalCount];

        // 화살표 종류 개수 만큼 ArrowValue 길이 할당
        arrow = new TotalParameter.Arrow[TotalParameter.Instance.arrowTotalCount];

        // 화살표 종류 개수 만큼 이미지 Object 넣어 주기
        for (int i = 0; i < TotalParameter.Instance.arrowTotalCount; i++)
        {
            selectedArrowImgs[i] = transform.GetChild(i).GetComponent<Image>();
        }
    }

    // 재선택 버튼 클릭
    public void RetryButtonClick()
    {
        for (int i = 0; i < TotalParameter.Instance.arrowTotalCount; i++)
        {
            // 선택 된 화살표 이미지 초기화
            selectedArrowImgs[i].sprite = defualtImage;
            // 선택 된 화살표 값 초기화
            arrow[i] = 0;
            // 선택 된 화살표 개수 초기화
            selectCount = 0;
            
        }
    }

    // 선택 완료 버튼 클릭
    public void SelectCompleteClick()
    {
        // 화살표를 하나도 선택 하지 않았을 경우
        if (selectCount <= 0)
        {
            // 경고창 띄워주기
            StartCoroutine(AlertImageView());
        }
        else
        {
            // 화살표 선택 버튼 UI (Center_1 Object) false
            center_1_Object.SetActive(false);

            // 화살표 선택 버튼 UI (Bottom Object) false
            bottomButtonObject.SetActive(false);

            // OK 버튼
            okButton.enabled = false;

            // 이동 시작
            playerCharacter.transform.GetChild(0).GetComponent<CharacterMove>().CharacterMoving(transform.gameObject, arrow, selectCount);
            playerCharacter.transform.GetChild(0).GetComponent<CharacterMove>().selectedButtons = transform.GetComponent<SelectedButtons>();
        }
    }

    // 캐릭터 이동 후 처리 메소드
    public void CharacterMoveEnd()
    {
        // 재시작 버튼 클릭
        RetryButtonClick();

        // 화살표 선택 버튼 UI (Center_1 Object) false
        center_1_Object.SetActive(true);

        // 화살표 선택 버튼 UI (Bottom Object) false
        bottomButtonObject.SetActive(true);

        print("괜찮?");

        // OK 버튼
        okButton.enabled = true;
    }

    // 화살표를 하나도 선택 하지 않았을 경우 경고창을 띄워준다.
    IEnumerator AlertImageView()
    {
        alertImage.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        alertImage.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        alertImage.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        alertImage.SetActive(false);
    }

}
