using UnityEngine;
using UnityEngine.EventSystems;
public class Instruction : MonoBehaviour, IPointerDownHandler
{
    public GameObject ui;
    private void Awake()
    {
        //Time.timeScale = 0; // 게임이 실행되지 않도록 timescale을 0으로 변경
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Time.timeScale = 1; // 게임이 정상 작동하도록 timescale을 1로 변경
        ui.SetActive(false);// 오브젝트 비활성화
    }
}
