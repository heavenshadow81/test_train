using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//UI들 모두 들고 있을 싱글턴...

namespace Coding
{
    public class UIController : MonoBehaviour
    {
        static UIController _instance;
        public static UIController Instance { get => _instance; }

        public Canvas gamesceneUI;

        //캐릭터 승리!! 캐릭터 보기, 이동 캐릭터 카메라

        public UnityEngine.UI.RawImage[] characterVictroy, characterView, movingCharacterView;

        public Camera[] movingCharacterview1, movingCharacterview2;

        //버튼 뒷 배경!!!

        public UnityEngine.UI.Image[] uibackground;

        //플레이어 선택창, 플레이 플레이어UI, 실제 플레이 하는 캐릭터!
        public GameObject[] players, players2, characters;

        // 사용 방법 버튼
        public GameObject[] gameTipImg;

        // 이동 횟수 이미지
        public GameObject[] moveImg;

        // 이동 횟수 텍스트
        public Text[] moveCountText;

        public Image fail;

        public bool IsPrivateCameraConnected { get; private set; }
        public event EventHandler PrivateCameraStateChangedEvent;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }

            IsPrivateCameraConnected = true;
            PrivateCameraStateChangedEvent += (sender, args) => { };
        }

        internal void SetActivePrivateCamera(bool value)
        {
            IsPrivateCameraConnected = value;
            PrivateCameraStateChangedEvent(this, EventArgs.Empty);
        }
    }
}
