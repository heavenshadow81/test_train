using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;

public class CharacterMoving : MonoBehaviour
{
    int way;
    float Timer;
    float RotateSpeed = 0f;
    bool roration = false;
    public Animator[] anim; 
    public TextMeshProUGUI Text;
    int buttonB = 0;

    public Button buttonR;
    public Button buttonL;

    public Sprite ROn;
    public Sprite LOn;

    Sprite ROff;
    Sprite LOff;

    GameManager gmr;
    private void OnEnable()
    {
        buttonB = 0;
        ROff = buttonR.image.sprite;
        LOff = buttonL.image.sprite;
        gmr = FindObjectOfType<GameManager>();
    }
    public void Logic()
    {
        Text.text = string.Format($"{RotateSpeed}");
        Timer = Timer + Time.deltaTime;

        if (roration) 
        {  
            transform.Rotate(0, 0, Time.deltaTime * RotateSpeed * way, Space.Self); 
        }
        if (RotateSpeed > 100f)
        {   
            for (int i = 0; i < anim.Length; i++)
            { anim[i].SetTrigger("SpeedMax"); }
        }
        if (Timer >= 3 && Mathf.Abs(transform.rotation.eulerAngles.z - 180) < 1)
        {   
            for (int i = 0; i < anim.Length; i++)
            { anim[i].SetTrigger("SpeedZero"); }
            ReTurn(); 
        }
    }
    public void RightBtn()
    {
        if(buttonB == 0 || buttonB == 1)
        {
            StartCoroutine(RBtn());

            buttonB = 2;
            way = 1;
            PushBtn();
        }
    }
    public void LeftBtn()
    {
        if(buttonB == 0 || buttonB == 2)
        {
            StartCoroutine (LBtn());

            way = -1;
            PushBtn();
            buttonB = 1;
        }
    }
    void PushBtn()
    {
        Timer = 0;
        RotateSpeed += 10;
        roration = true;
    }
    void ReTurn()
    {
        roration = false;
        transform.DORotate(new Vector3(0, 0, 0), 1).SetEase(Ease.Linear).OnComplete(() => 
        {
            gmr.stateClass.resultState = GameResult.Success;
            gmr.zozo.Change(GameState.GameResult);
        });
        //RotateSpeed = 0;
        Timer = 0;
    }

    IEnumerator RBtn()
    {
        buttonR.image.sprite = ROn;
        yield return new WaitForSeconds(0.3f);
        buttonR.image.sprite = ROff;
    }
    IEnumerator LBtn()
    {
        buttonL.image.sprite = LOn;
        yield return new WaitForSeconds(0.3f);
        buttonL.image.sprite = LOff;
    }
}
