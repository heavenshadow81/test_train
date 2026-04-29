using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BumperCar
{
    public class MinimapMove : MonoBehaviour
    {
        public GameObject[] positions;

        Vector3 right = new Vector3(0, 0, 0); //자동차 오른쪽 방향 회전값 저장
        Vector3 left = new Vector3(0, 0, 180); //자동차 왼쪽 방향 회전값 저장
        Vector3 down = new Vector3(0, 0, 270); //자동차 아래 방향 회전값 저장
        Vector3 up = new Vector3(0, 0, 90); //자동차 위 방향 회전값 저장

        bool rightOn; //오른쪽 방향 값
        bool downOn; //아래 방향 값
        bool leftOn; //왼쪽 방향 값
        bool upOn; //위 방향 값

        private void OnEnable()
        {
            transform.localPosition = positions[0].transform.localPosition; //시작할 때 미니맵 자동차의 위치는 0번
            downOn = true; //아래 방향값을 true로
        }

        public void UpdateLogic()
        {

            if (downOn) //아래 방향값이 true면
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, positions[1].transform.localPosition, 2.6f*Time.deltaTime); //미니맵 위치 1번으로 이동

                if (transform.localPosition == positions[1].transform.localPosition) //미니맵 위치 1번에 도착했다면
                {
                    downOn = false; //아래 방향값을 false로
                    rightOn = true; //오른쪽 방향값을 true로
                    gameObject.transform.eulerAngles = right; //자동차 회전값을 오른쪽으로
                }
            }
            else if (rightOn) //오른쪽 방향값이 true면
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, positions[2].transform.localPosition, 2.6f * Time.deltaTime); //미니맵 위치 2번으로 이동

                if (transform.localPosition == positions[2].transform.localPosition) //미니맵 위치 2번에 도착했다면
                {
                    rightOn = false; //오른쪽 방향값을 false로
                    upOn = true; //위 방향값을 true로
                    gameObject.transform.eulerAngles = up; //자동차 회전값을 위로
                }
            }
            else if (upOn) //위 방향값이 true면
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, positions[3].transform.localPosition, 2.6f * Time.deltaTime); //미니맵 위치 3번으로 이동

                if (transform.localPosition == positions[3].transform.localPosition) //미니맵 위치 3번에 도착했다면
                {
                    upOn = false; //위 방향값을 false로
                    leftOn = true; //왼쪽 방향값을 true로
                    gameObject.transform.eulerAngles = left; //자동차 회전값을 왼쪽으로
                }
            }
            else if (leftOn) //왼쪽 방향값이 true면
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, positions[0].transform.localPosition, 2.6f * Time.deltaTime); //미니맵 위치 0번으로 이동

                if (transform.localPosition == positions[0].transform.localPosition) //미니맵 위치 0번에 도착했다면
                {
                    leftOn = false; //왼쪽 방향값을 false로
                    downOn = true; //아래 방향값을 true로
                    gameObject.transform.eulerAngles = down; //자동차 회전값을 아래로
                }
            }
        }
    }
}
