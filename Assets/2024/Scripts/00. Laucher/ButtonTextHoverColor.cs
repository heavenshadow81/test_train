using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonTextHoverColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TMP_Text buttonLabel;    
    [SerializeField] private Color hoverColor;

    private void Awake()
    {
        buttonLabel = GetComponent<TMP_Text>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonLabel.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonLabel.color = Color.white;
    }
}
