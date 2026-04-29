using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//맵 활성화되면 캐릭터 시작 위치로 강제 이동
public class CharacterInit : MonoBehaviour
{
    [SerializeField]
    Transform[] startPos, monsterPos;
    [SerializeField]
    Transform startPosOne;
    private void OnEnable()
    {
        if (TotalParameter.Instance.persons > 1 && startPos.Length > 0)
        {
            for (int i = 0; i < Coding.UIController.Instance.characters.Length; i++)
            {
                Coding.UIController.Instance.characters[i].transform.position = startPos[i].position;
            }
        }
        else
        {
            Coding.UIController.Instance.characters[0].transform.position = startPosOne.position;
        }
        if (monsterPos.Length > 0)
        {
            foreach(var pos in monsterPos)
            {
                Instantiate(MazeTiles.Instance.Monsters[Random.Range(0, MazeTiles.Instance.Monsters.Length)], pos.position, Quaternion.Euler(Vector3.zero));
            }
        }
    }

}
