
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class TouchCustomShape : MonoBehaviour//, IDropHandler
{
    //버튼 그림
    Image thisImage;
    //플레이어 매니저
    PlayerManager pm;
    
    //재생
    
    
    // Use this for initialization
    void Start()
    {
        //특정 모양일 때 투명한 곳은 터치되지 않게 하기 위한 처리

        thisImage = GetComponent<Image>();
        pm = transform.parent.parent.GetComponentInParent<PlayerManager>();
        thisImage.alphaHitTestMinimumThreshold = 0.5f;
        
    }
    
    
    //버튼을 눌렀을 때 이미지 색깔 바꾸기
    public void SetColor(int color)
    {
        if (!thisImage)
        {
            thisImage = GetComponent<Image>();
        }

        if(color == 0)
        {
            thisImage.DOKill();
            thisImage.color = new Color(color, color, color);
        }
        else
        {
            //thisImage.color = new Color(color, color, color);
            thisImage.DOColor(new Color(color, color, color), 500f);//임재성님 코드
        }
    }
    //버튼 눌렀을 때 이미지 바로 바꾸기
    public void SetColorInstance(int color)
    {
        thisImage.DOKill();
        thisImage.color = new Color(color, color, color);
    }
    //위에 포인터가 오면...(IDropHandler)인터페이스
    //public void OnDrop(PointerEventData eventData)
    //{
    //    DraggerbleImage dimage;
    //    //까망인 상태
    //    dimage = eventData.pointerDrag.gameObject.GetComponent<DraggerbleImage>();
        
    //    //얘 위치를 찍는지 확인
    //    print(transform.GetSiblingIndex());
    //    if (eventData.pointerDrag.gameObject.GetComponent<Image>() == pm.GetPartImage())
    //    {
    //        dimage.index = transform.GetSiblingIndex();
    //        switch (TouchContentsManger.Instance.contentsType)
    //        {
    //            case TouchType.Part:
    //                pm.SelectImage(dimage.index);
    //                break;
    //            case TouchType.Shadow:
    //                pm.SelectShadowImage(dimage.index);
    //                break;
    //        }
    //    }
    //}

}
