using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Scale_SQ : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites = null;
    [SerializeField] private Image[] objectImages = null;
    [SerializeField] RectTransform barTransform;


    private int[] weights = new int[2]; // 각 이미지의 무게를 저장할 배열
    private string answerString; // 더 무거운 쪽의 정답 문자열

    public void SetScale()
    {
        SetObjects();
        AdjustBarRotation();
    }

    public void SetObjects()
    {
        // 15개의 이미지 중에서 중복되지 않게 2개의 이미지를 무작위로 선택
        List<int> chosenIndices = new List<int>();

        while (chosenIndices.Count < 2)
        {
            int randomIndex = Random.Range(0, sprites.Length);

            if (!chosenIndices.Contains(randomIndex))
            {
                chosenIndices.Add(randomIndex);
            }
        }

        // 선택된 인덱스를 기반으로 objectImages 배열에 이미지 할당 및 무게 설정
        for (int i = 0; i < chosenIndices.Count; i++)
        {
            objectImages[i].sprite = sprites[chosenIndices[i]];
            weights[i] = Random.Range(1, 10); // 1~10 사이의 랜덤 무게를 할당
        }
    }

    public void AdjustBarRotation()
    {
        float targetRotationZ = 0f;

        // 무게를 비교하여 더 무거운 쪽을 판단하고 barTransform을 회전
        if (weights[0] > weights[1])
        {
            targetRotationZ = 15f; // 왼쪽으로 회전
            answerString = ">";
        }
        else if (weights[0] < weights[1])
        {
            targetRotationZ = -15f; // 오른쪽으로 회전
            answerString = "<";
        }
        else
        {
            targetRotationZ = 0f; // 무게가 같음
            answerString = "=";
        }

        // 회전을 DOTween으로 애니메이션
        ShakeBar(targetRotationZ);
    }

    public void ShakeBar(float targetRotationZ = 0)
    {
        // 현재 barTransform의 z 회전값을 가져옴
        float currentZRotation = barTransform.eulerAngles.z;

        // z 회전값이 180도 이상이면 음수로 변환 (예: 350도 -> -10도)
        if (currentZRotation > 180f)
        {
            currentZRotation -= 360f;
        }

        // 만약 targetRotationZ가 0이면, 미세한 회전을 추가하여 OutBounce를 유도
        if (targetRotationZ == 0)
        {
            Sequence shakeSequence = DOTween.Sequence();

            // 약간의 미세한 회전 후 다시 0으로 복귀
            shakeSequence.Append(barTransform.DORotate(new Vector3(0, 0, 1f), 0.2f))
                         .Append(barTransform.DORotate(Vector3.zero, 0.8f))
                         .SetEase(Ease.OutBounce);
        }
        else
        {
            Sequence shakeSequence = DOTween.Sequence();

            // 약간의 미세한 회전 후 목표 위치로 회전
            shakeSequence.Append(barTransform.DORotate(new Vector3(0, 0, targetRotationZ + 2f), 0.2f))
                         .Append(barTransform.DORotate(new Vector3(0, 0, targetRotationZ), 1f)
                         .SetEase(Ease.OutBounce)); // 부드럽게 회전하고 끝에서 살짝 튕김
        }
    }

    // 바의 현재 z축 회전값을 반환하는 메서드
    public float GetBarZRotation()
    {
        float zRotation = barTransform.eulerAngles.z;

        // z 회전값이 180도 이상이면 음수로 변환 (예: 350도 -> -10도)
        if (zRotation > 180f)
        {
            zRotation -= 360f;
        }

        return zRotation;
    }

    public string GetAnswerString()
    {
        return answerString;
    }

    public Image[] GetImages()
    {
        return objectImages;
    }
}
