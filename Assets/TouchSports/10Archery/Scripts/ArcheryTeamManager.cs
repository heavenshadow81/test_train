using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ML.T_Sports.Common;
using DG.Tweening;

namespace ML.T_Sports.Archery
{
    public enum ArcheryTeamState
    {
        Play,
        Pause,
        Ready,
        Finish
    }
    public class ArcheryTeamManager : ContentsManagerBase
    {
        public static ArcheryTeamManager instance;
        public ArcheryTeamState state;
        public int ChanceValue;
        public GameObject Player_A;
        public Camera Camera_A, UI_A;

        public GameObject Player_B;
        public Camera Camera_B, UI_B;
        public GameObject VerticalLine;
        public GameObject ReadyGUI;

        public ArcheryScoreManager[] scores;
        public ArcheryTeamTouchPoint touch;
        public EFMPlayer StartButtonSound;
        public EFMPlayer PropertyButtonSound;

        public GameObject target;

        int playerCount;
        public override void Init()
        {
            InitProperty(ContentsPropertyType.Chance, 3, 10, 10);
            InitProperty(ContentsPropertyType.Player, 1, 2, 1);
            SetPropertyValue(ContentsPropertyType.Player, 1);
            InitProperty(ContentsPropertyType.GameMode, 0, 1, 0);
            SetPropertyValue(ContentsPropertyType.GameMode, 0);
            SetPropertyValue(ContentsPropertyType.Player, 1);



            playerCount = GetPropertyValueInt(ContentsPropertyType.Player);
            SetPlayer(playerCount);
            ChanceValue = GetPropertyValueInt(ContentsPropertyType.Chance);
            state = ArcheryTeamState.Ready;
            instance = this;
        }
        private void Start()
        {
            float sfx = GetSharedPropertyValueFloat(ContentsPropertyType.SFX);
            Common.SoundManager.instance.SetEFMVolume(sfx);
            float bgm = GetSharedPropertyValueFloat(ContentsPropertyType.BGM);
            Common.SoundManager.instance.SetBGMVolume(bgm);
        }
        public override void Play()
        {
            if (!IsPlaying)
            {
                target.GetComponent<DOTweenAnimation>().DOPlay();
                ReadyGUI.SetActive(false);
                IsPlaying = true;
                touch.ArrowDestroy();
                SetPlayer(GetPropertyValueInt(ContentsPropertyType.Player));
                StartButtonSound.EFMRandomPlay();
                touch.ChangeCheck = false;
                ChanceValue = GetPropertyValueInt(ContentsPropertyType.Chance);
                for(int i = 0; i < scores.Length;i++)
                    scores[i].Init(ChanceValue);
                state = ArcheryTeamState.Play;
                base.Play();
            }
        }

        public GameObject PauseCanvas;
        public override void Pause()
        {
            base.Pause();
            PauseCanvas.SetActive(IsPaused);
        }
        public override void Stop()
        {
            if (IsPlaying)
            {
                IsPlaying = false;
                state = ArcheryTeamState.Finish;
                base.Stop();
            }
        }
        public override void OnChangePropertyValue(ContentsPropertyType type, float prevValue, float newValue)
        {
            switch (type)
            {
                case ContentsPropertyType.Chance:
                    ChanceValue = (int)newValue;
                    for (int i = 0; i < scores.Length; i++)
                    {
                        scores[i].SetChance(ChanceValue);
                    }
                    PropertyButtonSound.EFMRandomPlay();
                    break;
                case ContentsPropertyType.GameMode:
                    if (newValue == 0)
                        PropertyButtonSound.EFMRandomPlay();
                    //UnityEngine.SceneManagement.SceneManager.LoadScene("10ArcherySingleMod");
                    break;
                case ContentsPropertyType.Player:
                    SetPlayer((int)newValue);
                    playerCount = (int)newValue;
                    break;
            }
        }

        public override void OnChangeSharedPropertyValue(ContentsPropertyType type, float prevValue, float newValue)
        {
            switch (type)
            {
                case ContentsPropertyType.SFX:
                    Common.SoundManager.instance.SetEFMVolume(newValue);
                    break;
                case ContentsPropertyType.BGM:
                    Common.SoundManager.instance.SetBGMVolume(newValue);
                    break;
            }
        }

        public void SetPlayer(int playerValue)
        {
            switch (playerValue)
            {
                case 1:
                    VerticalLine.SetActive(false);
                    Player_B.SetActive(false);
                    Camera_A.rect = new Rect(0, 0, 1, 1);
                    UI_A.rect = Camera_A.rect;
                    touch.Player = 1;
                    break;
                case 2:
                    VerticalLine.SetActive(true);
                    Player_B.SetActive(true);
                    Camera_A.rect = new Rect(0, 0, 0.5f, 1);
                    Camera_B.rect = new Rect(0.5f, 0, 1, 1);
                    UI_A.rect = Camera_A.rect;
                    UI_B.rect = Camera_B.rect;
                    touch.Player = 2;
                    break;                
            }
        }
        public bool CheckScoreState(int idx)
        {

            return true;
        }
        public void AddScore(int idx, int addScore)
        {
            scores[idx].AddScore(addScore);
        }

        public void CheckPlayerState()
        {
            int tmp = 0;
            for (int i = 0; i < playerCount; i++)
            {
                if (scores[i].MyChance > 0)
                {
                    tmp = 1;
                }
            }
            if (tmp == 0)
            {
                SetReseut();
            }
        }
        public void SetReseut()
        {            
            if (IsPlaying)
            {
                Debug.Log("끝");
                IsPlaying = false;
                state = ArcheryTeamState.Finish;
                base.Stop();
                target.GetComponent<DOTweenAnimation>().DOPause();

                if (target != null)
                {
                    // target의 자식 중 이름이 target01인 오브젝트를 찾습니다.
                    Transform target01 = target.transform.Find("target01");

                    if (target01 != null)
                    {
                        // target01의 자식 오브젝트들을 파괴합니다.
                        foreach (Transform child in target01)
                        {
                            if (child.name == "Arrow2(Clone)")
                            GameObject.Destroy(child.gameObject);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("target01을 찾을 수 없습니다.");
                    }
                }
                else
                {
                    Debug.LogWarning("target을 찾을 수 없습니다.");
                }
                
            }
        }
    }
}

