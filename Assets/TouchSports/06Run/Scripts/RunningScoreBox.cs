using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace ML.T_Sports.Running
{
    public class RunningScoreBox : MonoBehaviour
    {
        public Text[] Score;

        public Text[] GetScoreText()
        {
            return Score;
        }
        public void ResetScore()
        {
            for (int i = 0; i < Score.Length; i++)
                Score[i].text = "00:00:00";
        }
    }
}

