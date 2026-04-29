using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//타일 경계(넘어갈 수 없는 부분)
public class TileEdge : MonoBehaviour
{
    //부딪힐 때 갈 위치
    [SerializeField]
    Transform Pos;
    //부딪히면...? 

    private void OnTriggerEnter(Collider other)
    {
        //if(other.GetComponent<플레이어>())ㅖ
        //{
        //상황에 따라 충돌 이벤트 처리/ 충돌이 아니면... 그냥 통과 처리
        other.gameObject.transform.position = Pos.position;
        //}
    }

}
