using UnityEngine;

namespace ML.PlaywallKids.DragonPark
{
    /// <summary>
    /// Sky path (for Hansen).
    /// </summary>
    public class DragonSkyPath : DragonPath
    {
        #region Public variables
        /// <summary>
        /// The path number. If it is zero, dragon will move path randomly.
        /// </summary>
        public int pathNumber = 0;
        #endregion

        #region Properties
        /// <summary>
        /// Is dragon idle state?
        /// </summary>
        /// <value><c>true</c> if is idle; otherwise, <c>false</c>.</value>
        public bool isIdle
        {
            get
            {
                // current, next animation infos
                AnimatorClipInfo[] currentInfos = dragonAnimation.animator.GetCurrentAnimatorClipInfo(0);
                AnimatorClipInfo[] nextInfos = dragonAnimation.animator.GetNextAnimatorClipInfo(0);

                // if animator is transitioning?
                if (nextInfos != null && nextInfos.Length > 0)
                {
                    _idleStateFrameCount = 0;

                    return false;
                }
                else if (currentInfos != null)
                {
                    // if current animation info is valid
                    if (currentInfos.Length > 0 && currentInfos[0].clip != null)
                    {
                        // "Fly_???" is idle state
                        if (currentInfos[0].clip.name.ToLower().Contains("fly"))
                        {
                            _idleStateFrameCount++;
                        }
                        // If not, it is not idle
                        else
                        {
                            _idleStateFrameCount = 0;
                        }

                        //Debug.Log ("current animation name : " + currentInfos[0].clip.name);

                        // If idle state continues in 3 frames and speed is zero, returns true!
                        return _idleStateFrameCount >= 3 && dragonAnimation.speed < 0.01f;
                    }
                }

                // default value
                return true;
            }
        }
        #endregion

        #region Private variables
        /// <summary>
        /// Current path name.
        /// </summary>
        private string _currentPathName = "";
        private bool _isFinishedMovingAlongPath = true;
        private int _idleStateFrameCount = 0;
        #endregion

        public void Start()
        {
            if (dragonAnimation.animatorHasFlyProperty)
                dragonAnimation.animator.SetBool("fly", true);
            dragonAnimation.usesNavMesh = false;
        }

        public override void Update()
        {
            if (!movesTarget && _touched && isIdle)
            {
                _touched = false;
                movesTarget = true;
            }

            base.Update();
        }

        public virtual void OnEnable()
        {
            if (dragonAnimation.animatorHasFlyProperty)
                dragonAnimation.animator.SetBool("fly", true);
        }

        public override void MovePath()
        {
            if (_isFinishedMovingAlongPath)
            {
                string pathName = GetNextSkyPathName();
                float speed = GetRandomSkySpeed(pathName);

                MovePath(pathName, speed, "pingpong");
            }
        }

        public override void OnMovePathFinish()
        {
            base.OnMovePathFinish();

            _isFinishedMovingAlongPath = true;
        }

        public string GetNextSkyPathName()
        {
            if (pathNumber == 0)
            {
                int newNumber = Random.Range(0, 5) + 1;
                return string.Format("skypath_{0}", newNumber);
            }
            else
            {
                if (_currentPathName.EndsWith("_") || pathNumber > 3)
                {
                    return string.Format("skypath_{0}", pathNumber);
                }
                else
                {
                    return string.Format("skypath_{0}_", pathNumber);
                }
            }
        }

        public float GetRandomSkySpeed(string pathName)
        {
            return Random.Range(3.0f, 4.5f);
        }

        public void OnDrawGizmos()
        {
            AnimatorClipInfo[] currentInfos = dragonAnimation.animator.GetCurrentAnimatorClipInfo(0);
            AnimatorClipInfo[] nextInfos = dragonAnimation.animator.GetNextAnimatorClipInfo(0);


            if (nextInfos != null && nextInfos.Length > 0)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(transform.position + Vector3.up, 0.5f);
            }
            else if (currentInfos != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(transform.position + Vector3.up, 0.5f);
            }
        }
    }
}