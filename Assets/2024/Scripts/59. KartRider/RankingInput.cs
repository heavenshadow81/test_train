using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KartRider
{
    public class RankingInput : MonoBehaviour
    {
        [SerializeField] GameObject rank; //랭킹보드
        [SerializeField] HighScoreTable rankTable; //랭킹스크립트
        [SerializeField] TextMeshProUGUI[] names; //닉네임 텍스트배열
        [SerializeField] CameraController cam; //카메라 컨트롤러 스크립트
        [SerializeField] GameObject timerBox; //타이머박스 오브젝트

        public void Close()
        {
            gameObject.SetActive(false);
            rankTable.AddBestrecordEntry(cam.timer, names[0].text+ names[1].text + names[2].text);
            timerBox.SetActive(false);
            rank.SetActive(true);
        }
    }
}
