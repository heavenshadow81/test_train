using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSelect : MonoBehaviour
{
    public GameObject btnGroup;
    Button[] selectGame = new Button[8];
    public int gameNumber;

    bool gameCheck;

    // Start is called before the first frame update
    void Start()
    {
        gameCheck = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameCheck)
        {
            if (btnGroup.transform.childCount > 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    selectGame[i] = btnGroup.transform.GetChild(i).gameObject.GetComponent<Button>();
                    //print($"자식 {i}번 이름: {selectGame[i].name}");
                }
                gameCheck = true;
            }
        }

        if(gameNumber == 1)
        {
            //selectGame[1].onClick
        }
    }
}
