using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FC_ChessDelete : MonoBehaviour
{
    // 애니메이션 이벤트에 연결할 함수
    public void DestroyObject()
    {
        Destroy(gameObject);  // 오브젝트 삭제
    }
}
