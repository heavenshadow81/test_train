using UnityEngine;

namespace ML.PlaywallKids.DragonPark
{
    public class FreeDrawingObjectPath : DragonPath
    {
        #region Properties
        public bool isRunning
        {
            get
            {
                AnimatorClipInfo[] infos = dragonAnimation.animator.GetCurrentAnimatorClipInfo(0);
                if (infos != null)
                {
                    if (infos.Length > 0 && infos[0].clip != null)
                    {
                        return infos[0].clip.name.ToLower().Contains("run");
                    }
                }
                return true;
            }
        }
        #endregion

        #region Private variables
        private float _touchElapsedTime = 0.0f;
        #endregion

        public override void Update()
        {
            base.Update();

            if (_touched)
            {
                if (isRunning && _touchElapsedTime >= 1.0f)
                {
                    _touched = false;
                    movesTarget = true;
                }
                _touchElapsedTime += Time.deltaTime;
            }
            else
            {
                _touchElapsedTime = 0.0f;
            }
        }

        public override void MovePath()
        {
            string pathName = GetRandomPathName();
            float speed = 2.0f;

            MovePath(pathName, speed, "pingpong");
        }
    }
}