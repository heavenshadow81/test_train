using UnityEngine;

namespace ML.PlaywallKids.MotionJump
{
    using Common;

    /// <summary>
    /// 게임오버 팝업창
    /// </summary>
    public class UIGameOverController : MonoBehaviour
    {
        /// <summary>
        /// 게임오버 배경창
        /// </summary>
        public UISprite imgGameoverBalloon;
        /// <summary>
        /// 게임오버 애니메이션 객체
        /// </summary>
        public GameObject gameOverObj;
        public float duration;

        [HideInInspector]
        public EventDelegate events; // initialze at extra class

        [SerializeField]
        public AnimationCurve aniCurve;

        void OnEnable()
        {
            if (duration == 0) duration = 0.7f;
            gameOverObj.transform.localScale = Vector3.zero;
        }

        /// <summary>
        /// 게임 오버 된 사용자 위치에서 말풍선이 나오게 설계함
        /// 현재는 정중앙에 출력함
        /// </summary>
        /// <param name="_pos"></param>
        /// <param name="bFlip"></param>
        public void GameOverDisplay(Vector3 _pos, bool bFlip)
        {

            if (imgGameoverBalloon == null) return;

            if (!this.gameObject.activeInHierarchy)
                this.gameObject.SetActive(true);

            if (bFlip) imgGameoverBalloon.flip = (_pos.x <= 0.5) ? UIBasicSprite.Flip.Nothing : UIBasicSprite.Flip.Horizontally;
            TweenScale tweenScale = TweenScale.Begin(gameOverObj, duration, Vector3.one);
            tweenScale.animationCurve = aniCurve;

            if (events != null && !tweenScale.onFinished.Contains(events))
            { tweenScale.onFinished.Add(events); }

            // _pos.x = Mathf.Lerp(-1 * border, border, (_pos.x -0.5f ) * (UtilityScript.fWidth * 0.5f));
            _pos.x = (_pos.x - 0.5f) * (ScreenUtil.NGUIWidth * 0.5f);
            _pos.y = (_pos.y - 0.2f) * ScreenUtil.NGUIHeight;
            _pos.z = 0;
            /*
    #if UNITY_EDITOR
            Debug.Log(_pos);
    #endif'*/
            gameOverObj.transform.localPosition = _pos;
        }
    }
}