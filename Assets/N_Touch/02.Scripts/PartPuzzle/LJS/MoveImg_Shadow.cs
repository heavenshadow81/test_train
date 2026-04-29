using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 그림자퍼즐
/// </summary>
public class MoveImg_Shadow : MonoBehaviour {    

    public void Move(int randomNum)
    {
        string temp = "Pos" + randomNum;
        transform.DOMove(GameObject.Find(temp).transform.position, 2).SetEase(Ease.OutBack).SetId("Move");
    }
    //DOTween kill 필요



    public void MovePos(string name)
    {
        transform.SetParent(GameObject.Find(GameObject.Find(name + "Pos").transform.parent.name + "Img").transform);

        // transform.DOMove(GameObject.Find(name + "Pos").transform.position, 2).SetEase(Ease.OutBack).SetId("Move");
        // transform.DOScale(new Vector2(0, 0), 2).SetEase(Ease.OutBack).SetId("Scale");
        // GetComponent<Image>().DOFade(0, 2).SetEase(Ease.InOutCubic).SetId("Fade");
        // GameObject.Find(transform.name + "Pos").GetComponent<ImgFadeInOut>().ImageFadeOut();

        transform.DOMove(GameObject.Find(name + "Pos").transform.position, 3).SetEase(Ease.OutBack).SetId("Move");
        transform.DOScale(new Vector2(0, 1), 3).SetEase(Ease.InBounce).SetId("Scale");
        GetComponent<Image>().DOFade(0, 1).SetEase(Ease.InOutCubic).SetId("Fade");
        GameObject.Find(transform.name + "Pos").GetComponent<ImgFadeInOut>().ImageFadeOut();



        Invoke("KillDotween", 3);
    }

    void KillDotween()
    {
        //DOTween.Kill("Move");
        //DOTween.Kill("Scale");
        //DOTween.Kill("Fade");
        DOTween.Kill(this);
    }

}
