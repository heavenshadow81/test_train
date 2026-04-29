using UnityEngine;

namespace ML.MLBKids
{
    using GameMode = Stadium.GameMode;

    public class ScoreManager : MonoBehaviour
    {
        #region Enums
        public enum ScoreType
        {
            Normal = 0,
            Nice,
            HomeRun,
            Bad,
            Ball,
            Strike
        }
        #endregion

        #region Public variables
        public int score = 0;
        public int balls = 0;
        public int homerun = 0;
        public int strike = 0;
        public int ball = 0;

        #region Constants
        public int ballHitScore = 10;
        public int ballHitNiceScore = 20;
        public int ballHomeRunScore = 100;
        public int ballStrikeNiceScore = 50;
        public int ballStrikeNormalScore = 20;
        public int ballBallScore = 0;
        #endregion
        #endregion

        #region Properties
        public static ScoreManager instance
        {
            get; private set;
        }

        public static int currentScore
        {
            get { return instance != null ? instance.score : 0; }
        }

        public static GameMode gameMode { get; set; }
        #endregion

        #region Events
        // delegates
        public delegate void OnAddScore(int newScore, int add, ScoreType scoreType);

        // events
        public static event OnAddScore onAddScore;
        #endregion

        #region Unity Methods
        public void Awake()
        {
            instance = this;
        }

        public void OnDestroy()
        {
            instance = null;
        }
        #endregion

        #region Score managements
        public static void Clear()
        {
            if (instance != null)
            {
                instance.score = 0;
                instance.balls = 0;
                instance.homerun = 0;
                instance.strike = 0;
                instance.ball = 0;
            }
        }

        public static void AddScore(int addScore, ScoreType scoreType)
        {
            if (instance != null)
            {
                instance.score += addScore;
                instance.balls += 1;
                if (scoreType == ScoreType.HomeRun)
                    instance.homerun += 1;
                if (scoreType == ScoreType.Strike)
                    instance.strike += 1;
                if (gameMode == GameMode.Pitch)
                {
                    if (scoreType == ScoreType.Nice)
                        instance.strike += 1;
                }
                if (scoreType == ScoreType.Ball)
                    instance.ball += 1;

                if (onAddScore != null)
                    onAddScore(instance.score, addScore, scoreType);
            }
        }
        #endregion
    }
}