using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TableWareCountUI_LT : MonoBehaviour
{
    [SerializeField] private Food_LT food;
    [SerializeField] private TableWare_LT tableWare;

    [SerializeField] private Image[] allImages = null;

    [SerializeField] private List<Image> activeImages = new List<Image>();
    [SerializeField] private Color opaqueColor;
    [SerializeField] private Color originColor;

    private int uiStack = 0;
    private int ActiveCount = 5;

    public Action OnFullStack;
    public bool isLastStack = false;
  
    private void OnEnable()
    {
        food.OnHit += AddScore;
        food.OnFoodChange += Init;
    }

    private void OnDisable()
    {
        food.OnHit -= AddScore;
        food.OnFoodChange -= Init;
    }

    void Start()
    {
        Init();
    }

    private void Init()
    {
        activeImages.Clear();
        SettingactiveImages();
        foreach (var image in activeImages)
        {
            image.color = opaqueColor;
            image.sprite = tableWare.selectedSprite;
            image.GetComponentInChildren<ParticleSystem>().Stop();
        }

        uiStack = 0;  // 초기화 시 uiStack도 0으로 설정
        isLastStack = false;  // 초기화 시 isLastStack도 false로 설정
    }

    private void SettingactiveImages()
    {
        ActiveCount = Mathf.Min(ActiveCount, allImages.Length); // ActiveCount가 allImages 크기를 넘지 않도록 제한
        for (int i = 0; i < ActiveCount; i++)
        {
            allImages[i].gameObject.SetActive(true);
        }

        // 자식들 중 활성화된 이미지들만 리스트에 추가
        foreach (Transform child in transform)
        {
            Image img = child.GetComponent<Image>();
            if (img != null && img.gameObject.activeInHierarchy)
            {
                activeImages.Add(img);
            }
        }
    }

    private void AddScore()
    {
        // 이미지의 색상을 변경하고 파티클 시스템을 재생
        activeImages[uiStack].color = originColor;
        activeImages[uiStack].GetComponentInChildren<ParticleSystem>().Play();
        uiStack++;
        isLastStack = false;

        // 스택이 가득 찬 경우 처리
        if (uiStack >= activeImages.Count)
        {
            isLastStack = true;
            uiStack = 0;

            // ActiveCount 증가 및 제한
            ActiveCount = Mathf.Min(ActiveCount + 1, allImages.Length);

            OnFullStack?.Invoke();
        }
    }
}
