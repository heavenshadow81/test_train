//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using ML.T_Sports.Common;
//namespace ML.T_Sports.Jump
//{
//    public enum JumpState
//    {
//        HightSetting,
//        Play,
//        Pause,
//        Finish
//    }
//    public class JumpSingleManager : ContentsManagerBase
//    {
//        public static JumpSingleManager instance;
//        public JumpState state;
//        public Image[] Explanations;
//        public Sprite HightSetting, PlayExplanation;
//        public Animation[] ExplanAnimation;
//        public JumpScoreManager[] scores;
//        public bool Explanation;
//        private int Players;
//        private int Chance;
//        public EFMPlayer Cheers;
//        public GameObject[] ReadyObj;
//        public bool[] HightReady;
//        void OnEnable()
//        {
//            Init();
//        }
//        public override void Init()
//        {
//            InitProperty(ContentsPropertyType.Chance, 2, 4, 3);
//            InitProperty(ContentsPropertyType.Player, 1, 4, 4);
//            InitProperty(ContentsPropertyType.Height, 100, 300, 200);
//            //InitProperty(ContentsPropertyType.GameMode, 0, 1, 0);

//            float sfx = GetSharedPropertyValueFloat(ContentsPropertyType.SFX);
//            SoundManager.instance.SetEFMVolume(sfx);
//            float bgm = GetSharedPropertyValueFloat(ContentsPropertyType.BGM);
//            SoundManager.instance.SetBGMVolume(bgm);


//            Explanation = true;
//            instance = this;
//            print(instance);
//            NeedsReady = true;
//            Players = GetPropertyValueInt(ContentsPropertyType.Player);
//            state = JumpState.HightSetting;
//            HightReady = new bool[4];
//            SetInit();
//        }

//        int testidx = 3;
//        public void SetReadyOut(int idx)
//        {
//            ReadyObj[idx].SetActive(false);
//            HightReady[idx] = true;
//        }
//        public void SetInit()
//        {
//            Explanation = true;
//            for (int i = 0; i < ExplanAnimation.Length; i++)
//                ExplanAnimation[i].Play("SetExplanation");
//            for (int i = 1; i < scores.Length; i++)
//            {
//                HightReady[i] = false;
//                ReadyObj[i].SetActive(false);
//                Debug.Log("Init");
//            }
//            Players = GetPropertyValueInt(ContentsPropertyType.Player);
//            for (int i = 0; i < Players; i++)
//            {
//                if (!HightReady[i])
//                    ReadyObj[i].SetActive(true);
//            }
//        }
//        public void SetHightSetting()
//        {
//            state = JumpState.HightSetting;

//        }
//        public GameObject PauseCanvas;
//        public override void Pause()
//        {
//            base.Pause();
//            PauseCanvas.SetActive(IsPaused);
//        }
//        public override void Ready()
//        {
//            base.Ready();
//            SetHightSetting();
//            for (int i = 0; i < HightReady.Length; i++)
//                HightReady[i] = false;
//            for (int i = 0; i < Players; i++)
//            {
//                ReadyObj[i].SetActive(true);
//            }
//        }
//        public override void OnChangeSharedPropertyValue(ContentsPropertyType type, float prevValue, float newValue)
//        {
//            switch (type)
//            {
//                case ContentsPropertyType.SFX:
//                    SoundManager.instance.SetEFMVolume(newValue);
//                    break;
//                case ContentsPropertyType.BGM:
//                    SoundManager.instance.SetBGMVolume(newValue);
//                    break;
//            }
//        }

//        public bool SetReadyCheck()
//        {
//            for (int i = 0; i < Players; i++)
//            {
//                if (ReadyObj[i].activeSelf)
//                    return false;
//            }
//            return true;
//        }
//        public override void Play()
//        {
//            if (!IsPlaying)
//            {
//                Chance = GetPropertyValueInt(ContentsPropertyType.Chance);
//                Players = GetPropertyValueInt(ContentsPropertyType.Player);
//                if (!SetReadyCheck())
//                {
//                    Debug.Log("준비가 덜됨");
//                    return;
//                }
//                state = JumpState.Play;

//                base.Play();
//                Debug.Log(Players + "명의 플레이어 준비");

//                for (int i = 0; i < Explanations.Length; i++)
//                {
//                    Explanations[i].sprite = PlayExplanation;
//                }
//            }
//        }
//        public override void Stop()
//        {
//            base.Stop();
//            ResultCheck();

//            //준비 버튼에 들어가야함.
//            //SetHightSetting();
//        }

//        public void DeActivateExplanation(int idx)
//        {
//            if (!Explanation)
//                return;
//            Explanation = false;
//            ExplanAnimation[idx].Play("ExplanationOut");
//        }
//        public void ResultCheck()
//        {
//            if (IsPlaying)
//                IsPlaying = false;
//            int tmp = 0;
//            for (int i = 0; i < scores.Length; i++)
//            {
//                if (scores[i].state == PlayerState.Play)
//                {
//                    tmp++;
//                }
//            }

//            if (tmp == 0)
//            {
//                state = JumpState.Pause;
//            }
//        }
//    }
//}

