using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.InputSystem.EnhancedTouch;
using LGM.AnimalMatch;
using UnityEngine.UI;
using ML.SportsMiniGame.KinectSkating;
using static InvBaseItem;

public class FCManger : PlayManager_PlayGround
{
    [SerializeField] LetterSlot_FC[] stones; // 스톤들
    
    public static int colorScore; //찾은 색상의 수

    protected override void Init()
    {
        base.Init();

        SettingSlot();
    }
    void SettingSlot()
    {
        FC_ColorManager.instance.frame.transform.DOScale(0, 0.2f).OnComplete(() =>
        {
            FC_ColorManager.instance.frame.transform.DOScale(1f, 0.3f);
        });
        //위치값 리스트 생성
        List <Vector3> positions = new List<Vector3>();

        foreach (var slot in stones)
        {
            //각 스톤의 위치값 리스트에 저장
            positions.Add(slot.transform.position);

            //스톤 작아졌다가 커짐
            slot.transform.DOScale(0, 0.2f).OnComplete(() =>
            {
                slot.transform.DOScale(1.5f, 0.3f);
            });
        }
        
        //리스트의 순서를 랜덤하게 바꿈
        ShuffleArray(positions);

        for(int i = 0; i < positions.Count; i++)
        {
            //스톤 오브젝트의 위치값을 리스트 순서대로 변경
            stones[i].transform.position = positions[i];
        }

        isPerfect = true;
    }
    void ShuffleArray(List<Vector3> array)
    {
        for (int i = array.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Vector3 temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }

    public override void HandleInput(Vector2 inputPosition)
    {
        // 터치한 오브젝트가 적합하지 않으면 즉시 다시 터치 가능
        isTouchable = true;

        // 터치/마우스 위치를 월드 좌표로 변환
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(inputPosition);

        // 터치/마우스 위치에서 카드 찾기
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

        if (hit.collider != null)
        {
            // 터치한 카드가 어떤 태그를 가지고 있는지 확인
            if (hit.collider.CompareTag(TeamNameString))
            {
                SoundMGR.Instance.SoundPlay("PlayGround_Moving");
                LetterSlot_FC selectedSlot = hit.collider.GetComponent<LetterSlot_FC>();
                if (selectedSlot != null)
                {
                    CheckAnswer(selectedSlot);
                }
            }
        }
    }
    void CheckAnswer(LetterSlot_FC selectedSlot)
    {
        if (selectedSlot.name.Contains(FC_ColorManager.instance.frame.sprite.name))
        {
            // 정답 처리 메서드 호출
            CorrectAnswer(selectedSlot.gameObject);
        }
        else
        {
            // 오답 처리 메서드 호출
            WrongAnswer(selectedSlot.gameObject);
        }
    }

    public override void CorrectAnswer(GameObject touched)
    {
        Vector3 originalPosition = touched.transform.position;

        //이펙트 생성
        GameObject particle = Instantiate(effect, originalPosition, Quaternion.identity);
        Destroy(particle, 1f); // 1초 후 이펙트 제거

        //콜라이더 비활성화 후 스케일이 0이되면 활성화
        touched.GetComponent<BoxCollider2D>().enabled = false;
        touched.transform.DOScale(0, 0.3f).OnComplete(() =>
        {
            touched.GetComponent<BoxCollider2D>().enabled = true;
        });

        //컬러 스코어가 4보다 작을 때
        if(colorScore<3)
        {
            //컬러 스코어 +1
            colorScore++;
            //UI 찾은 만큼 활성화
            FC_ColorManager.instance.colors[colorScore - 1].SetActive(true);            
        }
        else
        {
            colorScore++;
            FC_ColorManager.instance.colors[colorScore - 1].SetActive(true);
            Invoke("ColorChange", 0.5f);
        }
    }

    void ColorChange()
    {
        for (int i = 0; i < FC_ColorManager.instance.colors.Length; i++)
        {
            FC_ColorManager.instance.colors[i].SetActive(false);
        }

        //찾은 색상 수 초기화
        colorScore = 0;
        //다음 
        FC_ColorManager.instance.stage++;
        FC_ColorManager.instance.frame.transform.DOScale(0, 0.2f).OnComplete(() =>
        {
            FC_ColorManager.instance.frame.sprite = FC_ColorManager.instance.frameColor[FC_ColorManager.instance.stage];
            FC_ColorManager.instance.frame.transform.DOScale(1f, 0.3f);
        });

        if(FC_ColorManager.instance.stage==7)
        {
            ChangeGauge();
        }
    }

    public override void WrongAnswer(GameObject touched)
    {
        //print($"false: {touched}");
        touched.transform.DOScale(1, 0.2f).OnComplete(() =>
        {
            touched.transform.DOScale(1.5f, 0.2f);
        });
    }
}
