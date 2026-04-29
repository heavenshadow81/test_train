using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;


//출처: https://greenteacat.tistory.com/35
//드래그 -드롭 기본 스크립트 참고

public class DraggerbleImage : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{

    //다시 돌아갈 위치
    public Vector2 defaultPosition;
    //몇번째 그림?
    public int index;
    //플레이어몇?
    

    UnityEngine.UI.Image thisimage;

    void OnEnable()
    {
        thisimage = GetComponent<UnityEngine.UI.Image>();
        thisimage.alphaHitTestMinimumThreshold = 0.5f;
        StartCoroutine(MemoryInitPos());
    }
    //드래그 시작하면..?
    public void OnBeginDrag(PointerEventData eventData)
    {
        DOTween.Complete(gameObject);
        
        thisimage.raycastTarget = false;
        thisimage.color = new Color(1, 1, 1, 0.7f);
    }
    //드래그 중이면
    public void OnDrag(PointerEventData eventData)
    {
        //현재 위치는 포인터 위치!
        Vector2 currentPos = eventData.position;
        //적용
        transform.position = currentPos;
    }
    //드래그가 끝나면
    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.hovered.Count > 0)
        {
            print($"겹친 오브젝트 갯수{eventData.hovered.Count}");
            for (int i = 0; i < eventData.hovered.Count; i++)
            {
                print(eventData.hovered[i]);
                if (eventData.hovered[i].GetComponent<UnityEngine.UI.Button>() && (eventData.hovered[i].transform.parent.parent.parent == transform.parent || eventData.hovered[i].transform.parent.parent == transform.parent))
                {
                    eventData.hovered[i].GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
                    break;
                }
            }
        }
        else
        {
            RaycastHit hit;
            Physics.SphereCast(transform.position, 3f, transform.forward, out hit, 0);
            if (hit.transform && hit.transform.GetComponent<UnityEngine.UI.Button>())
            {
                hit.transform.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
            }
        }
        //트윈 추가..?
        transform.DOMove(defaultPosition, 0.5f);
        thisimage.color = new Color(1, 1, 1, 1);
        //0.5초 뒤에 이동이 끝나므로
        //Invoke(nameof(ResetPosition), 0.5f);
        StartCoroutine(ResetPos());

    }
    //다시 터치할 수 있도록 조정
    void ResetPosition()
    {
        DOTween.Complete(gameObject);
        thisimage.raycastTarget = true;
    }
    IEnumerator ResetPos()
    {
        yield return new WaitForSeconds(0.5f);
        DOTween.Complete(gameObject);
        thisimage.raycastTarget = true;
        yield break;
    }
    IEnumerator MemoryInitPos()
    {
        yield return new WaitForSeconds(0.05f);
        defaultPosition = transform.position;
        yield break;
    }
}
