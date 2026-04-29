using UnityEngine;
using UnityEngine.UI;

public class ButtunInteractable : MonoBehaviour
{
    public Button leftB;    // 왼쪽버튼
    public Button rightB;   // 오른쪽 버튼
    GameManager gmr;        // 게임매니저
   
    private void Awake()
    {
        gmr = FindObjectOfType<GameManager>();
    }
    void Update()
    {
        if (gmr.stateClass.state != GameState.GamePlay)
        {
            rightB.interactable = false;
            leftB.interactable = false;
        }
        else
        {
            rightB.interactable = true;
            leftB.interactable = true;
        }
    }

    public void GoRight()
    {
        rightB.interactable = true;
        leftB.interactable = false;
    }
    public void GoLeft()
    {
        rightB.interactable = false;
        leftB.interactable = true;
    }
}
