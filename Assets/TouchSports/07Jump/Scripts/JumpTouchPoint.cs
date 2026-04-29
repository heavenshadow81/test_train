//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using ML.T_Sports.Common;
//namespace ML.T_Sports.Jump
//{
//    public class JumpTouchPoint : MonoBehaviour
//    {
//        public Image HightA;
//        public RectTransform HightBarA;
//        public JumpRecode[] RecodeA;
//        public int RecodeIdxA;
//        public float coolA;

//        public Image HightB;
//        public RectTransform HightBarB;
//        public JumpRecode[] RecodeB;
//        public int RecodeIdxB;
//        public float coolB;

//        public Image HightC;
//        public RectTransform HightBarC;
//        public JumpRecode[] RecodeC;
//        public int RecodeIdxC;
//        public float coolC;

//        public Image HightD;
//        public RectTransform HightBarD;
//        public JumpRecode[] RecodeD;
//        public int RecodeIdxD;
//        public float coolD;

//        public EFMPlayer Hit;

//        public int PlayerValues;

//        private void Awake()
//        {
//            coolA = 0;
//            coolB = 0;
//            coolC = 0;
//            coolD = 0;
//        }
//        private void Start()
//        {
//            ScreenHight_cm = JumpSingleManager.instance.GetPropertyValueFloat(ContentsPropertyType.Height);
//        }
//        public float ScreenHight_cm;

//        void Update()
//        {
//            if (JumpSingleManager.instance.state == JumpState.Play || JumpSingleManager.instance.state == JumpState.HightSetting)
//            {

//                int TouchCount = TouchModule.TouchModuleInput.touchCount;

//                if (TouchCount != 0)
//                {
//                    Touch[] touchs = TouchModule.TouchModuleInput.touches;
//                    for (int i = 0; i < touchs.Length; i++)
//                    {
//                        Vector3 pos = touchs[i].position;
//                        float Hight = pos.y;
//                        float per_Hight = Hight / 1080;
//                        float BarHight = 38 + (per_Hight * 980);
//                        float recode;
//                        print($"{pos.x}, {pos.y}");
//                        if (pos.x < 472)
//                        {
//                            //픽셀 차이 = recode
//                            recode = pos.y - HightBarA.anchoredPosition.y;
//                            float cm = ScreenHight_cm / 1080 * recode;
//                            if (JumpSingleManager.instance.state == JumpState.HightSetting)
//                                SetPlayerHight(HightA, HightBarA, per_Hight, BarHight, 0);
//                            else if (JumpSingleManager.instance.state == JumpState.Play && coolA == 0)
//                            {
//                                if (cm < 0 || JumpSingleManager.instance.scores[0].state != PlayerState.Play)
//                                    return;
//                                JumpSingleManager.instance.DeActivateExplanation(0);
//                                if (RecodeA[RecodeIdxA].SetRecoding(pos, cm))
//                                {
//                                    Hit.EFMRandomPlay();
//                                    RecodeIdxA += 1;
//                                    if (RecodeIdxA >= RecodeA.Length)
//                                        RecodeIdxA = 0;
//                                }
//                                JumpSingleManager.instance.SetRecodeScores(0, cm);
//                                coolA = 1f;
//                            }
//                        }
//                        else if (pos.x >= 472 && pos.x < 946)
//                        {
//                            recode = pos.y - HightBarB.anchoredPosition.y;
//                            float cm = ScreenHight_cm / 1080 * recode;
//                            if (JumpSingleManager.instance.state == JumpState.HightSetting)
//                                SetPlayerHight(HightB, HightBarB, per_Hight, BarHight, 1);
//                            else if (JumpSingleManager.instance.state == JumpState.Play && coolB == 0)
//                            {
//                                if (cm < 0 || JumpSingleManager.instance.scores[1].state != PlayerState.Play)
//                                    return;
//                                JumpSingleManager.instance.DeActivateExplanation(1);
//                                if (RecodeB[RecodeIdxB].SetRecoding(pos, cm))
//                                {
//                                    Hit.EFMRandomPlay();
//                                    RecodeIdxB += 1;
//                                    if (RecodeIdxB >= RecodeB.Length)
//                                        RecodeIdxB = 0;
//                                }
//                                JumpSingleManager.instance.SetRecodeScores(1, cm);
//                                coolB = 1f;
//                            }
//                        }
//                        else if (pos.x >= 946 && pos.x < 1434)
//                        {
//                            recode = pos.y - HightBarC.anchoredPosition.y;
//                            float cm = ScreenHight_cm / 1080 * recode;
//                            if (JumpSingleManager.instance.state == JumpState.HightSetting)
//                                SetPlayerHight(HightC, HightBarC, per_Hight, BarHight, 2);
//                            else if (JumpSingleManager.instance.state == JumpState.Play && coolC == 0)
//                            {
//                                if (cm < 0 || JumpSingleManager.instance.scores[2].state != PlayerState.Play)
//                                    return;
//                                JumpSingleManager.instance.DeActivateExplanation(2);
//                                if (RecodeC[RecodeIdxC].SetRecoding(pos, cm))
//                                {
//                                    Hit.EFMRandomPlay();
//                                    RecodeIdxC += 1;
//                                    if (RecodeIdxC >= RecodeC.Length)
//                                        RecodeIdxC = 0;
//                                }
//                                JumpSingleManager.instance.SetRecodeScores(2, cm);
//                                coolC = 1f;
//                            }
//                        }
//                        else if (pos.x >= 1434)
//                        {
//                            recode = pos.y - HightBarD.anchoredPosition.y;
//                            float cm = ScreenHight_cm / 1080 * recode;
//                            if (JumpSingleManager.instance.state == JumpState.HightSetting)
//                                SetPlayerHight(HightD, HightBarD, per_Hight, BarHight, 3);
//                            else if (JumpSingleManager.instance.state == JumpState.Play && coolD == 0)
//                            {
//                                if (cm < 0 || JumpSingleManager.instance.scores[3].state != PlayerState.Play)
//                                    return;
//                                JumpSingleManager.instance.DeActivateExplanation(3);
//                                if (RecodeD[RecodeIdxD].SetRecoding(pos, cm))
//                                {
//                                    Hit.EFMRandomPlay();
//                                    RecodeIdxD += 1;
//                                    if (RecodeIdxD >= RecodeD.Length)
//                                        RecodeIdxD = 0;
//                                }
//                                JumpSingleManager.instance.SetRecodeScores(3, cm);
//                                coolD = 1f;
//                            }
//                        }
//                    }
//                }


//                if (coolA > 0)
//                    coolA -= Time.deltaTime;
//                else
//                    coolA = 0;

//                if (coolB > 0)
//                    coolB -= Time.deltaTime;
//                else
//                    coolB = 0;

//                if (coolC > 0)
//                    coolC -= Time.deltaTime;
//                else
//                    coolC = 0;

//                if (coolD > 0)
//                    coolD -= Time.deltaTime;
//                else
//                    coolD = 0;
//            }
//        }

//        public void ResetHands()
//        {
//            for (int i = 0; i < RecodeA.Length; i++)
//            {
//                RecodeA[i].ResetHand();
//                RecodeB[i].ResetHand();
//                RecodeC[i].ResetHand();
//                RecodeD[i].ResetHand();
//            }
//        }
//        public void HandReady()
//        {
//            for (int i = 0; i < RecodeA.Length; i++)
//            {
//                RecodeA[i].SetReady();
//                RecodeB[i].SetReady();
//                RecodeC[i].SetReady();
//                RecodeD[i].SetReady();
//            }
//        }
//        void SetPlayerHight(Image HightFill, RectTransform HightBar, float perhight, float barhight, int idx)
//        {
//            if (PlayerValues <= idx)
//            {
//                return;
//            }
//            JumpSingleManager.instance.SetReadyOut(idx);
//            HightFill.fillAmount = perhight;
//            HightBar.anchoredPosition = new Vector2(0, barhight);
//        }
//        public void ResetHightBar()
//        {
//            HightBarA.anchoredPosition = new Vector2(0, 1018);
//            HightBarB.anchoredPosition = new Vector2(0, 1018);
//            HightBarC.anchoredPosition = new Vector2(0, 1018);
//            HightBarD.anchoredPosition = new Vector2(0, 1018);
//            HightA.fillAmount = 1f;
//            HightB.fillAmount = 1f;
//            HightC.fillAmount = 1f;
//            HightD.fillAmount = 1f;
//        }

//    }
//}

