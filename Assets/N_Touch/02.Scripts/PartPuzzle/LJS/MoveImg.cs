using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;

public class MoveImg : MonoBehaviour
{
    // 추가
    Image settingBtn;
    //

    float aroundDis = 10f;
    bool activebool = true;
    Transform tr;

    public void Move()
    {
        //BGAnimalManager bam= GameObject.Find("BGAnimalManager").GetComponent<BGAnimalManager>();
        //tr = bam.CheckMoveAnimalPos();
        tr = GameObject.Find(transform.name + "Pos").transform;
        
        //20221118 추가
        settingBtn = GameObject.Find("LevelSettingBtn").GetComponent<Image>();
        settingBtn.raycastTarget = false;
        //

        ImgSetParent();

        //이동하여 배치
        //transform.DOMove(tr.position, 3).SetEase(Ease.OutBack).SetId("Move");
        //transform.DOScale(new Vector2(5, 5), 3).SetEase(Ease.InBounce).SetId("Scale");

        //이동후 나타나기
        transform.DOMove(tr.position, 3).SetEase(Ease.OutBack).SetId("Move")
        //20221118 추가
            .OnUpdate(() => { settingBtn.raycastTarget = false;})
            .OnComplete(()=>settingBtn.raycastTarget = true)
            .OnKill(() => settingBtn.raycastTarget = true);        
        transform.DOScale(new Vector2(0, 1), 3).SetEase(Ease.InBounce).SetId("Scale");
        GetComponent<Image>().DOFade(0, 2).SetEase(Ease.InOutCubic).SetId("Fade");

    }

    void Update()
    {
        if (activebool)
        {
            float dis = (tr.position - transform.position).sqrMagnitude;

            if (dis < aroundDis)
            {
                //transform.SetParent(GameObject.Find("BGAnimalPos").transform);
                GameObject.Find(transform.name + "Pos").GetComponent<ImgFadeInOut>().ImageFadeOut();
                Invoke("KillDotween", 1);
                activebool = false;
            }
        }
    }

    void KillDotween()
    {
        DOTween.Kill("Move");
        DOTween.Kill("Scale");
        DOTween.Kill("Fade");
        Destroy(gameObject);
    }

    void ImgSetParent()
    {
        transform.SetParent(GameObject.Find(GameObject.Find(tr.name).transform.parent.name + "Img").transform);
    }
}