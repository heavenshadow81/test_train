using System.Collections;
using System.Collections.Generic;

using UnityEngine;
//카메라 설정 바꾸기..
public class ChangeCamera : MonoBehaviour
{
    [SerializeField]
    Camera cam;
    //카메라 위치
    [SerializeField]
    Transform[] camPos;
    private void OnEnable()
    {
        //난이도에 따라 카메라 위치 조정(맵크기가 달라서 맵을 가득 채운 상태로 보이기 위함)
        int camPo = ContentsOptions.GetDifficult() switch
        {
            Difficult.Easy => 0,
            Difficult.Normal => 1,
            _ => 0
        };
        cam.transform.position = camPos[camPo].position;
    }
}
