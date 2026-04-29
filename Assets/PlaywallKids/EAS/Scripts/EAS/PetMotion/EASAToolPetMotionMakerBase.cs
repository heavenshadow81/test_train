using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.EAS
{
    using DragonPark;
    public class EASAToolPetMotionMakerBase : EASAnimatablePanel
    {
        #region Public variables
        public Transform characterPos;

        public RuntimeAnimatorController animController;

        public AnimationClip[] anims;
        #endregion

        #region Properties
        private Dragon _dragon = null;
        public Dragon dragon
        {
            get
            {
                return _dragon;
            }
            set
            {
                if (_dragon != null)
                {
                    Destroy(_dragon.gameObject);
                    _dragon = null;
                }

                _dragon = value;
                _dragon.transform.parent = characterPos;
                animationControl.animator.runtimeAnimatorController = animController;
            }
        }

        public EASPetMotionAnimationControl animationControl
        {
            get
            {
                EASPetMotionAnimationControl animationControl = _dragon.GetComponent<EASPetMotionAnimationControl>();
                if (animationControl == null)
                    animationControl = _dragon.gameObject.AddComponent<EASPetMotionAnimationControl>();
                return animationControl;
            }
        }
        #endregion

        public virtual void Update()
        {
        }

        public AnimationClip GetClip(string name)
        {
            foreach (AnimationClip c in anims)
            {
                if (c != null && c.name.Equals(name))
                    return c;
            }
            return null;
        }

        public void Play(List<string> anims)
        {
            if (anims != null)
            {
                animationControl.animList.Clear();
                animationControl.animList.AddRange(anims);

                animationControl.playCount = 1;

                animationControl.Play();
            }
        }

        public void Play(string[] anims)
        {
            if (anims != null)
            {
                Play(new List<string>(anims));
            }
        }

        public void Play(AnimationClip[] anims)
        {
            List<string> animNames = new List<string>();
            foreach (var anim in anims)
            {
                if (anim != null) animNames.Add(anim.name);
            }
            Play(animNames);
        }

        public void Pause()
        {
            animationControl.Pause();
        }

        public void Resume()
        {
            animationControl.Resume();
        }

        public void Stop()
        {
            animationControl.Stop();
        }
    }
}