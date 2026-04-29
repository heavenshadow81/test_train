using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.MotionJump
{
    public class UITimeDisplayer : MonoBehaviour
    {
        public UISprite[] numArr;
        public string fileName;

        public void ChangeTime(float _seconds)
        {
            int min = (int)(_seconds / 60);
            if (min >= 60) min %= 60;
            int sec = (int)(_seconds % 60);
            ChangeTime(min, sec);
        }

        public void ChangeTime(int _min, int _seconds)
        {
            List<int> numList = null;
            int cnt = 0;
            for (int i = 0, length = 2; i < length; ++i)
            {
                int time = i == 0 ? _seconds : _min;
                numList = NumericSplit.Split(time);
                for (int j = 0; j < length; ++j)
                {
                    if (!numArr[cnt].cachedGameObject.activeInHierarchy) numArr[cnt].cachedGameObject.SetActive(true);
                    if (numList.Count > j)
                    { numArr[cnt].spriteName = fileName + numList[j].ToString(); }
                    else
                    { numArr[cnt].spriteName = fileName + "0"; }
                    cnt++;
                }
            }

            numList.Clear();
            numList = null;
        }
    }
}