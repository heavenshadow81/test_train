using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace LGM
{
    namespace CraneGame
    {
        public class ScoreManager : Singleton<ScoreManager>
        {
            public int score = 0;   // 현재 점수
            public int oneScore = 100;  // 정답 당 점수
            public TextMeshProUGUI text;    // 점수 텍스트
            
            private void Update()
            {
                text.text = "점수: " + score + "점";   // 점수 텍스트로 출력
            }
        }
    }
}

