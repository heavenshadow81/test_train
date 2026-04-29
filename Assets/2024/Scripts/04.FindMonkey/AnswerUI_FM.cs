using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
public class AnswerUI_FM : MonoBehaviour
{
    private Transform answerContainer;
    private Transform answerTemplate;
    public List<Transform> answerTransforms;
    public List<Image> Images;
    public GameManager_FM gameManager;
    public GameObject container;

    // Start is called before the first frame update
    void Start()
    {
        answerContainer = transform.Find("AnswerContainer");
        answerTemplate = answerContainer.Find("AnswerTemplate");
        //answerTemplate.gameObject.SetActive(false);
        //StartCoroutine(AnswerSetting());
    }

    public void Init()
    {
        StartCoroutine(AnswerSetting());
    }

    IEnumerator AnswerSetting()
    {
        yield return null;

        container.transform.localScale = Vector3.one;

        // 정답 UI 생성
        for(int i = 0; i< gameManager.GetAnswerSprites().Count; i++)
        {
            //CreateAnswerUI(gameManager.GetAnswerSprites()[i], answerContainer, answerTransforms);
            Images[i].sprite = gameManager.GetAnswerSprites()[i];
        }
    }

    private void OnEnable()
    {
        gameManager.OnCorrect += AnswerCheck; // 이벤트 핸들러 메서드 등록
    }

    private void OnDisable()
    {
        gameManager.OnCorrect -= AnswerCheck; // 이벤트 핸들러 메서드 제거
    }

    private void CreateAnswerUI(Sprite sprite, Transform container, List<Transform> transforms)
    {
        float templateHeight = 150f;

        Transform answerTransform = Instantiate(answerTemplate, container);
        RectTransform entryRectTransform = answerTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transforms.Count);
        answerTransform.gameObject.SetActive(true);

        // Set the sprite
        Image image = answerTransform.Find("AnswerSprite").GetComponent<Image>();
        if (image != null)
        {
            image.sprite = sprite;
            answerTransforms.Add(answerTransform);
        }
    }

    private void AnswerCheck()
    {
        Image answerImage = answerTransforms[gameManager.answerIndex].Find("AnswerSprite").GetComponent<Image>();
        answerImage.color = Color.gray;

        // 모든 정답 이미지의 색상이 변경되었는지 확인
        bool allAnswersChecked = true;
        foreach (Transform answerTransform in answerTransforms)
        {
            Image image = answerTransform.Find("AnswerSprite").GetComponent<Image>();
            if (image.color != Color.gray)
            {
                allAnswersChecked = false;
                break;
            }
        }

        if (allAnswersChecked)
        {
            StartCoroutine(WaitForDrawingCompletion(1f));
        }
    }

    IEnumerator WaitForDrawingCompletion(float duration)
    {
        yield return new WaitForSeconds(duration); // 원그리기 시간이 끝날 때까지 대기

        container.transform.localScale = Vector3.zero;

        ResetAnswer(); // 정답 초기화

        SoundMGR.Instance.SoundPlay("FindMonkey_Ending");

        gameManager.retryBtn.SetActive(true); // 리트라이 버튼 활성화
        Image[] images = gameManager.retryBtn.GetComponentsInChildren<Image>();

        foreach (var image in images)
        {
            image.DOFade(1, 1f).Play();
        }
    }

    private void ResetAnswer()
    {
        foreach (Transform answerTransform in answerTransforms)
        {
            Image image = answerTransform.Find("AnswerSprite").GetComponent<Image>();
            if (image != null)
            {
                image.sprite = null;
                image.color = Color.white;
            }
        }
    }
}
