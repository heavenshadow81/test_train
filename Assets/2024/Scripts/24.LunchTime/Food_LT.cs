using UnityEngine;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class Food_LT : MonoBehaviour
{
    private float rotationDuration = 1.5f;
    private float moveAmount = 0.1f;
    private float moveDuration = 0.2f;
    private float pauseDuration = 0.5f;

    private Tweener rotationTween;

    public Action OnHit;
    public Action OnFoodChange;
    public Action OnFoodRotate;

    [SerializeField] GameObject embeddedTableWare;
    [SerializeField] List<GameObject> embeddedTableWares;

    [SerializeField] private PlayManager_LT playManager;

    [SerializeField] Sprite[] foodSprites;
    [SerializeField] GameObject effect;
    private Image foodImage;
    private ParticleSystem particleSys;

    [SerializeField] TableWareCountUI_LT countUI;

    private void Awake()
    {
        foodImage = GetComponent<Image>();
        particleSys = effect.GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        countUI.OnFullStack += PlayEffect;
        RotateFood();
        SetFood();
    }

    private void OnDisable()
    {
        countUI.OnFullStack -= PlayEffect;
    }

    private void SetFood()
    {
        int randIdx = UnityEngine.Random.Range(0, foodSprites.Length);
        foodImage.sprite = foodSprites[randIdx];
    }

    private void PlayEffect()
    {
        if (playManager.GetStack() <= 3)
        {
            DisappearFood();
            effect.SetActive(true);

            // 파티클 시스템이 재생이 끝날 때까지 기다린 후 SetFood 호출
            StartCoroutine(WaitForEffectToFinish());
        }
    }

    private IEnumerator WaitForEffectToFinish()
    {
        // 파티클 시스템이 재생되는 동안 대기
        while (particleSys.isPlaying)
        {
            yield return null;
        }

        // 이펙트 비활성화
        effect.SetActive(false);
        ShowFood();

        // SetFood() 호출
        SetFood();
        OnFoodChange?.Invoke();
    }

    private void ShowFood()
    {
        foodImage.enabled = true;
        rotationTween.Play();
    }

    private void DisappearFood()
    {
        SoundMGR.Instance.SoundPlay("PlayGround_RightAnswer");
        rotationTween.Pause();
        foodImage.enabled = false;

        foreach (var tableWare in embeddedTableWares)
        {
            Destroy(tableWare.gameObject);
        }
        embeddedTableWares.Clear();
    }

    private void RotateFood()
    {
        rotationTween = transform.DORotate(new Vector3(0, 0, 360), rotationDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Green") || collision.gameObject.CompareTag("Orange"))
        {
            PlayHitAnimation();

            TableWare_LT tableWare = collision.gameObject.GetComponent<TableWare_LT>();
            tableWare.StopShootTableWare();
            tableWare.ResetPos();

            // 현재 Food 오브젝트의 위치를 기준으로 Y축으로 -130만큼 떨어진 위치에 생성
            Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y - 1.3f, transform.position.z);
            embeddedTableWare.GetComponent<Image>().sprite = collision.gameObject.GetComponent<Image>().sprite;
            GameObject spawnedTableWare = Instantiate(embeddedTableWare, spawnPosition, Quaternion.identity, transform);
            embeddedTableWares.Add(spawnedTableWare);

            OnHit?.Invoke();
        }
    }

    public void PlayHitAnimation()
    {
        SoundMGR.Instance.SoundPlay("PlayGround_Moving");

        rotationTween.Pause();

        transform.DOMoveY(transform.position.y + moveAmount, moveDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                transform.DOMoveY(transform.position.y - moveAmount, moveDuration)
                    .SetEase(Ease.InQuad)
                    .OnComplete(() =>
                    {
                        DOVirtual.DelayedCall(pauseDuration, () =>
                        {
                            rotationTween.Play();
                            if (!countUI.isLastStack)
                            {
                                OnFoodRotate?.Invoke();
                            }
                        });
                    });
            });
    }
}
