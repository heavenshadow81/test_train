using UnityEngine;

namespace ML.PlaywallKids.Interaction
{
    public class UIInteractionController : MonoBehaviour
    {
        public Camera uiCam;
        public TouchMotionScoreController score;
        public GameObject guidanceObject;
        public UI2DSprite guidanceImage;
        public UILabel wordsOfGuidance;
        public UILabel tagOfUser;
        public NumericsDisplayer numericOfCountdown;
        public ResultObject resultObject;
        public TouchMotionPointManager uiPointOfScoreManager;
        public InteractionTimeBar barTypeTimer;
        public InteractionTimeBar circleTypeTimer;

        [HideInInspector]
        public InteractionTimeBar currentTimer;

        void Awake()
        {
            currentTimer = barTypeTimer;
        }

        void OnDisable()
        {
            if (currentTimer)
                currentTimer.Active = false;
            if (score)
                score.Active = false;
            if (numericOfCountdown)
                numericOfCountdown.Active = false;
            if (guidanceObject)
                guidanceObject.SetActive(false);
        }

        public void ShowGuidanceObject(bool active)
        {
            if (numericOfCountdown)
                numericOfCountdown.Active = active;

            if (guidanceObject)
                guidanceObject.SetActive(active);
        }
    }
}