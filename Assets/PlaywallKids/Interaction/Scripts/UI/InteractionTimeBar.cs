using UnityEngine;

namespace ML.PlaywallKids.Interaction
{
    /// <summary>
    /// 인터렉션 시간 Bar (Replacement of TouchMotionTimeBarDisplay)
    /// </summary>
    public class InteractionTimeBar : MonoBehaviour
    {
        public UISlider timeBar;

        public bool Active
        {
            get { return gameObject.activeSelf; }
            set { gameObject.SetActive(value); }
        }

        Color color;
        bool bOnOff;
        float fTime;
        float iPlayTime;

        void Awake()
        {
            if (!timeBar) timeBar = GetComponentInChildren<UISlider>();
            color = timeBar.foregroundWidget.color;
        }

        void OnDisable()
        {
            timeBar.foregroundWidget.color = color;
        }

        public void InitTime(int _iTime)
        {
            iPlayTime = _iTime;
        }

        public void ChangeTime(int _iTime)
        {
            if (iPlayTime == 0) iPlayTime = 1;
            float percentage = _iTime / iPlayTime;

            timeBar.value = percentage;
            if (percentage < 0.25f)
            {
                fTime++;
                if (fTime % 3 == 0)
                {
                    fTime = 0;
                    timeBar.foregroundWidget.color = bOnOff ? Color.red : color;
                    bOnOff = !bOnOff;
                }
            }
        }
    }
}