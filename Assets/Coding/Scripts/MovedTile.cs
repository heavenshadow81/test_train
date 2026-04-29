using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//캐릭터 지나간 타일 표시
public class MovedTile : MonoBehaviour
{
    //렌더러
    [SerializeField]
    MeshRenderer ren;
    //캐릭터 지나가면
    private void OnTriggerEnter(Collider other)
    {
        ren.material = MazeTiles.Instance.Mat;
    }
}
