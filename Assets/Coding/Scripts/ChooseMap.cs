using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//맵 생성(선택)
public class ChooseMap : MonoBehaviour
{
    [SerializeField]
    bool one;
    private void OnEnable()
    {
        //현재 맵!!
        print(ContentsOptions.GetDifficult());
        var currentmaps = ContentsOptions.GetDifficult() switch
        {
            Difficult.Easy => MazeTiles.Instance.EasyMap,
            Difficult.Normal => MazeTiles.Instance.NormalMap,
            _ => MazeTiles.Instance.EasyMap,
        };
        if (!one)
        {
            currentmaps = MazeTiles.Instance.NormalMap;
        }
        
        //시작 위치!!
        var startPos = ContentsOptions.GetDifficult() switch
        {
            Difficult.Easy => MazeTiles.Instance.EasyStart,
            Difficult.Normal => MazeTiles.Instance.NormalStart,
            _ => MazeTiles.Instance.EasyStart,
        };
        //배경 맵 선택
        int a = Random.Range(0, currentmaps.Length);
        
        for (int i = 0; i < currentmaps.Length; i++)
        {
            currentmaps[i].SetActive(i == a);
        }

        // 
        CharacterSourceContainer.Instance.Tiles();

        //플레이어 활성
        for (int i = 0; i < Coding.UIController.Instance.players2.Length; i++)
        {
            Coding.UIController.Instance.players2[i].SetActive(i < TotalParameter.Instance.persons);
        }
    }
    
}
