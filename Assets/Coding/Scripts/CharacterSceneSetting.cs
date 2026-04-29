using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//캐릭터 선택창
public class CharacterSceneSetting : MonoBehaviour
{

    private void OnEnable()
    {
        //TotalParameter.Instance.persons = ContentsOptions.GetPlayerNumber();
        print(TotalParameter.Instance.persons);
        for(int i = 0; i< Coding.UIController.Instance.players.Length; i++)
        {
            Coding.UIController.Instance.players[i].SetActive(i < TotalParameter.Instance.persons);
        }
    }
    private void OnDisable()
    {
        
    }
}
