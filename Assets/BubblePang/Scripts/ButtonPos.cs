using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
//버튼 위치 조절 : 플레이어들....
public class ButtonPos : MonoBehaviour
{
    //버튼위 위치들을 가지고 있을 ...
    [SerializeField]
    Transform[] pos;


    //버튼 위치...!
    public void SetButtonPos(PersonalUI p, IndexButton b)
    {
        print($"버튼 찾을 위치 {p.playerParameter.shuffleindex[p.playerParameter.index]}, 버튼 위치 {b.transform.GetSiblingIndex()}번째");
        b.transform.position = pos[ContentsController.Instance.contentsParameter.shufflepart[p.playerParameter.index]].GetChild(b.transform.GetSiblingIndex()).position;
    }
}
