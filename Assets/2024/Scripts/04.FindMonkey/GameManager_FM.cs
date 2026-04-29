using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using DG.Tweening;

public class GameManager_FM : MonoBehaviour
{
    public static GameManager_FM Instance;

    public Sprite[] sprites; // 스프라이트 배열
    private Transform[] objectTransforms; // 게임 오브젝트의 Transform 배열
    private List<Sprite> answerSprites; // 정답 오브젝트의 Sprite 리스트

    public Action OnCorrect;
    public int answerIndex;

    public GameObject retryBtn;
    public AnswerUI_FM AnswerUI;
    public GameObject fialEffect;

    [Header("인풋시스템")]
    public InputActionAsset actionAsset;
    private InputActionMap actionMap;
    private InputAction touchAction;

    private void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 인풋시스템 등록
        actionMap = actionAsset.FindActionMap("Touch");
        touchAction = actionMap.FindAction("Touch");
        touchAction.Enable();
        touchAction.started += CheckAnswer;
    }

    void Start()
    {
        // 씬에 배치된 게임 오브젝트의 Transform 배열 생성
        objectTransforms = GameObject.FindGameObjectsWithTag("Spawn")
                          .Select(go => go.transform)
                          .ToArray();

        // sprites 배열 초기화
        sprites = new Sprite[objectTransforms.Length];

        // 오브젝트의 SpriteRenderer에서 스프라이트를 가져와 sprites 배열에 저장
        for (int i = 0; i < objectTransforms.Length; i++)
        {
            SpriteRenderer spriteRenderer = objectTransforms[i].GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                sprites[i] = spriteRenderer.sprite; // 현재 스프라이트를 sprites 배열에 할당
            }
        }

        SetAnswer();
    }

    public void SetAnswer()
    {
        // 슬라이더 초기화
        foreach (var t in objectTransforms)
        {
            Slider slider = t.GetComponentInChildren<Slider>();
            if (slider != null)
            {
                slider.value = 0f;
            }

            // 콜라이더 활성화
            Collider2D collider = t.GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = true;
            }
        }

        answerSprites = new List<Sprite>(); // 정답 스프라이트 리스트 초기화
        List<int> selectedIndices = new List<int>(); // 중복된 인덱스를 방지하기 위한 리스트 생성
        System.Random random = new System.Random();

        // 정답 오브젝트 5개 선택
        while (answerSprites.Count < 5)
        {
            int randomIndex = random.Next(sprites.Length); // 랜덤 인덱스 생성
            if (!selectedIndices.Contains(randomIndex)) // 이미 선택된 인덱스인지 확인
            {
                answerSprites.Add(sprites[randomIndex]); // 랜덤으로 선택된 스프라이트를 리스트에 추가
                selectedIndices.Add(randomIndex); // 선택된 인덱스를 추가
            }
        }
    }

    //public void Init()
    //{
    //    answerSprites.Clear();

    //    // 씬에 배치된 게임 오브젝트의 Transform 배열 생성
    //    objectTransforms = GameObject.FindGameObjectsWithTag("Spawn")
    //                      .Select(go => go.transform)
    //                      .ToArray();

    //    // 새로운 Random 객체 생성
    //    System.Random random = new System.Random();
    //    // objectTransforms 배열을 랜덤하게 재정렬
    //    objectTransforms = objectTransforms.OrderBy(t => random.Next()).ToArray();

    //    // 스프라이트 할당 및 슬라이더 초기화
    //    int index = 0;
    //    foreach (var t in objectTransforms)
    //    {
    //        t.GetComponent<SpriteRenderer>().sprite = sprites[index];
    //        index = (index + 1) % sprites.Length;

    //        Slider slider = t.GetComponentInChildren<Slider>();
    //        if (slider != null)
    //        {
    //            slider.value = 0f;
    //        }
    //    }

    //    // 정답 오브젝트 5개 선택
    //    answerSprites = new List<Sprite>();
    //    List<int> selectedIndices = new List<int>();    // 중복된 인덱스를 방지하기 위한 리스트 생성
    //    while (answerSprites.Count < 5)
    //    {
    //        int randomIndex = random.Next(objectTransforms.Length);
    //        if (!selectedIndices.Contains(randomIndex))               // 이미 선택된 인덱스인지 확인
    //        {
    //            answerSprites.Add(objectTransforms[randomIndex].GetComponent<SpriteRenderer>().sprite);
    //            selectedIndices.Add(randomIndex);
    //        }
    //    }
    //}

    void CheckAnswer(InputAction.CallbackContext context)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        // Raycast를 통해 오브젝트 감지
        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag("Spawn"))
        {
            // 감지된 오브젝트의 스프라이트 가져오기
            Sprite clickedSprite = hit.collider.GetComponent<SpriteRenderer>().sprite;

            // 정답 스프라이트와 비교
            if (answerSprites.Contains(clickedSprite))
            {
                answerIndex = answerSprites.IndexOf(clickedSprite);
                OnCorrect?.Invoke();

                Slider slider = hit.collider.GetComponentInChildren<Slider>();
                if (slider != null)
                {
                    StartCoroutine(FillSlider(slider, 0.5f));
                    SoundMGR.Instance.SoundPlay("FindMonkey_Circle");
                    hit.collider.enabled = false; // 클릭한 오브젝트의 콜라이더를 비활성화
                }
            }
            else
            {
                // 뽀잉뽀잉 효과 추가
                Transform clickedTransform = hit.collider.transform;
                Sequence bounceSequence = DOTween.Sequence(); // 시퀀스 생성

                // 첫 번째 애니메이션
                bounceSequence.Append(clickedTransform.DOScale(new Vector3(1.2f, 1.2f, 1f), 0.2f))
                    .Append(clickedTransform.DOScale(new Vector3(1f, 1f, 1f), 0.2f)) // 원래 스케일로 돌아감
                    .Append(clickedTransform.DOScale(new Vector3(1.2f, 1.2f, 1f), 0.2f)) // 두 번째 애니메이션
                    .Append(clickedTransform.DOScale(new Vector3(1f, 1f, 1f), 0.2f)); // 다시 원래 스케일로 돌아감

                SoundMGR.Instance.SoundPlay("FindMonkey_Fail");

                // 이펙트 생성
                GameObject effectInstance = Instantiate(fialEffect, hit.transform);

                // ParticleSystem의 Renderer를 가져와서 오더 인 레이어 설정
                ParticleSystem particleSystem = effectInstance.GetComponent<ParticleSystem>();
                if (particleSystem != null)
                {
                    var renderer = particleSystem.GetComponent<ParticleSystemRenderer>();
                    if (renderer != null)
                    {
                        renderer.sortingOrder = 20; // 원하는 오더 인 레이어 값으로 설정
                    }
                }
            }
        }
    }

    IEnumerator FillSlider(Slider slider, float duration)
    {
        float elapsedTime = 0f;
        float startValue = slider.value;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            slider.value = Mathf.Lerp(startValue, 1f, elapsedTime / duration);
            yield return null; // 다음 프레임까지 대기
        }

        slider.value = 1f; // 마지막 값을 정확히 설정
    }

    public List<Sprite> GetAnswerSprites()
    {
        return answerSprites;
    }
}
