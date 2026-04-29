using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//참고 https://welcomeheesuk.tistory.com/49
//터치 영역 지정 가능하게...
public class TouchPuzzleEffect : MonoBehaviour, IPointerDownHandler
{
	[SerializeField]
	ParticleSystem effect;
    

    public void OnPointerDown(PointerEventData eventData)
    {
        effect.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, Camera.main.nearClipPlane));
        effect.Play();
    }
    
}
