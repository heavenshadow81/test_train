//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//namespace ML.T_Sports.Jump
//{
//    public class JumpScoreBox : MonoBehaviour
//    {

//        public Text[] Score;
//        public Image ScoreTitle;
//        public Sprite[] Titles;
//        public Image ScoreBottom;
//        public Sprite[] Bottoms;

//        public Text[] GetScoreText()
//        {
//            return Score;
//        }
//        public void ResetScore()
//        {
//            for (int i = 0; i < Score.Length; i++)
//                Score[i].text = "";
//        }
//        private void Awake()
//        {
//            //SetActivate(false);
//        }
//        public void SetActivate(bool active)
//        {
//            if (active)
//            {
//                ScoreTitle.sprite = Titles[0];
//                ScoreBottom.sprite = Bottoms[0];
//            }
//            else
//            {
//                Debug.Log("SetActivate " + active);
//                ScoreTitle.sprite = Titles[1];
//                ScoreBottom.sprite = Bottoms[1];
//            }
//        }
//    }
//}


