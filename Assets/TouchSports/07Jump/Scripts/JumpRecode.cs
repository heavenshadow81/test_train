//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//using UnityEngine.UI;
//namespace ML.T_Sports.Jump
//{
//    public class JumpRecode : MonoBehaviour
//    {
//        public RectTransform MyRecodingPosition;
//        public Animation RecodingAnimation;
//        public Image RecodeHand;
//        public Color BestColor;
//        public Text MyRecode;
//        public float RecodeValue;

//        public Transform Mytr;
//        public JumpRecode anotherRecode;
//        public bool colorchanging;
//        bool firstTouch;
//        bool Reset;
             
//        private void Awake()
//        {
//            Reset = false;
//            firstTouch = true;
//            Mytr = this.transform;
//            RecodeValue = -10000f;
//            MyRecode = Mytr.GetChild(0).GetComponent<Text>();
//            RecodingAnimation = Mytr.GetComponent<Animation>();
//            MyRecodingPosition = Mytr.GetComponent<RectTransform>();
//            RecodeHand = Mytr.GetComponent<Image>();
//        }
//        public void SetReady()
//        {
//            firstTouch = true;
//            Reset = false;
//        }
//        /// <summary>
//        /// 신기록을 새우면 true를 반환함.
//        /// </summary>
//        public bool SetRecoding(Vector2 recodsPos, float values)
//        {
//            MyRecodingPosition.anchoredPosition = recodsPos;
//            RecodeValue = values;
//            RecodingAnimation.Play("TouchRecode");

//            //Transform tmp = Mytr.parent;
//            //Mytr.parent = null;
//            //Mytr.parent = tmp;

//            if (anotherRecode.RecodeValue > RecodeValue)
//            {
//                RecodeHand.color = Color.gray;
//                return false;
//            }
//            else
//            {
//                RecodeHand.color = BestColor;
//                if (firstTouch)
//                {
//                    firstTouch = false;
//                    anotherRecode.firstTouch = false;
//                }
//                else
//                    anotherRecode.RecodeHand.color = Color.gray;

//                return true;
//            }
//        }
//        /*
//        IEnumerator ColorChange(Color Target)
//        {
//            yield return new WaitForSeconds(0.1f);
//            Color tmp = RecodeHand.color;
//            float _time = 0;
//            colorchanging = true;
//            while (colorchanging)
//            {
//                _time += Time.deltaTime;
//                tmp = Color.Lerp(tmp, Target, Mathf.PingPong(Time.deltaTime,1));                
//                RecodeHand.color = tmp;
//                yield return new WaitForSeconds(0.1f);
//            }
//        }*/
//        public void CountingStart()
//        {
//            StartCoroutine(Counting());
//        }
//        IEnumerator Counting()
//        {
//            float tmp = 0;
//            string plus = "";
//            float per = RecodeValue / 100;
//            if (RecodeValue > 0)
//            {
//                plus = "+";
//                while (tmp < RecodeValue)
//                {
//                    if (Reset)
//                        break;
//                    tmp += per * 3;
//                    MyRecode.text = plus + tmp.ToString("N1");
//                    yield return new WaitForSeconds(0.01f);
//                }
//                MyRecode.text = plus + RecodeValue.ToString("N1");
//            }
//            else if (RecodeValue < 0)
//            {
//                while (tmp > RecodeValue)
//                {
//                    if (Reset)
//                        break;
//                    tmp += per * 3;
//                    MyRecode.text = tmp.ToString("N1");
//                    yield return new WaitForSeconds(0.01f);
//                }
//                MyRecode.text = RecodeValue.ToString("N1");
//            }

//            if (Reset)
//            {
//                MyRecode.text = "";
//                RecodeValue = -1;
//            }
                
//        }
//        public void ResetHand()
//        {
//            Reset = true;
//            RecodeValue = -1;
//            RecodingAnimation.Play("ResetRecode");
//            MyRecode.text = "";
//        }
//    }
//}