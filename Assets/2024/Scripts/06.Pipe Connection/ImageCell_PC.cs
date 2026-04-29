using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ImageCell_PC : MonoBehaviour, IPointerDownHandler
{
    public GameObject[] pipes; // 각 칸에 들어갈 파이프 프리팹 배열
    private int currentPipeIndex = 0; // 현재 프리팹 인덱스
    private Image opaqueImage;

    public bool isTouchable = true; // 터치 가능 여부

    private void Awake()
    {
        opaqueImage = GetComponent<Image>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(isTouchable)
        {
            WaterFlowManager_PC.Instance.IsFlowable = true;
            OnClick_Cell();
        }
    }

    void OnClick_Cell()
    {
        // 현재 프리팹 제거
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // 클릭한 셀의 렉트 트랜스폼값 받아오기
        Vector2 pipePos = gameObject.GetComponent<RectTransform>().anchoredPosition;

        // 새로운 프리팹 생성
        SoundMGR.Instance.SoundPlay("배관 돌리는소리");
        GameObject newPipe = Instantiate(pipes[currentPipeIndex], transform.position, Quaternion.identity, transform);
        // 프리팹의 Order in Layer값 받아와서 새로 생성된 파이프의  Order값으로 오버라이딩 하기
        newPipe.GetComponentInChildren<Canvas>().overrideSorting = true;

        // 인덱스 증가 및 순환
        currentPipeIndex = (currentPipeIndex + 1) % pipes.Length;
    }

    public void UnableCell()
    {
        isTouchable = false;

        // 현재 색상을 가져온 후 알파 값을 0으로 설정
        Color color = opaqueImage.color;
        color.a = 0f;
        opaqueImage.color = color;
    }
}
