using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.EAS
{
    public class EASPetMotionAnimationControl : MonoBehaviour
    {
        #region Public variables
        public List<string> animList = new List<string>();

        public int playCount = 1;

        public System.Action onFinished;
        #endregion

        #region Properties
        private Animator _animator = null;
        public Animator animator
        {
            get
            {
                if (_animator == null)
                {
                    _animator = GetComponent<Animator>();
                    if (_animator == null)
                    {
                        _animator = gameObject.AddComponent<Animator>();
                    }
                }
                return _animator;
            }
        }
        #endregion

        #region Private variables
        private List<string> _playingAnims = new List<string>();

        private List<string> _playedAnims = new List<string>();

        private bool _playing = false;

        private float _time = 0.0f;
        #endregion

        public void Start()
        {
            // force to enable the animator on start.
            animator.enabled = true;
        }

        public void Update()
        {
            if (_playing)
            {
                AnimatorTransitionInfo tInfo = animator.GetAnimatorTransitionInfo(0);
                AnimatorStateInfo currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
                AnimatorStateInfo nextStateInfo = animator.GetNextAnimatorStateInfo(0);
                AnimatorClipInfo[] nextInfo = animator.GetNextAnimatorClipInfo(0);

                if (tInfo.nameHash == 0 && nextStateInfo.nameHash == 0 &&
                    currentStateInfo.IsName("Default") &&
                    nextInfo.Length == 0 &&
                    _time > 2.0f)
                {
                    if (_playingAnims.Count > 0)
                    {
                        string anim = _playingAnims[0];
                        _playingAnims.RemoveAt(0);

                        animator.SetTrigger(anim);

                        DebugUtil.LogFormat("EASPetMotionAnimationControl - Playing aniamation ({0})... ({1}/{2})",
                                             anim, animList.Count * playCount - _playingAnims.Count,
                                             animList.Count * playCount);
                    }
                    else
                    {
                        DebugUtil.Log("EASPetMotionAnimationControl - Finished animation.");

                        if (onFinished != null)
                            onFinished();

                        _playing = false;
                    }
                }

                _time += Time.deltaTime;
            }
        }


        public void Play()
        {
            _playingAnims.Clear();

            _playing = true;

            for (int i = 0; i < playCount; i++)
            {
                _playingAnims.AddRange(animList);
            }

            for (int i = 0; i < animList.Count; ++i)
            {
                string anim = animList[i];
                if (!_playedAnims.Contains(anim))
                { _playedAnims.Add(anim); }
            }
            /*
                foreach (string anim in animList)
                {
                    if (!_playedAnims.Contains(anim))
                        _playedAnims.Add(anim);
                }
            */
            _time = 0.0f;

            if (_playingAnims.Count > 0)
            {
                animator.speed = 1.0f;

                string anim = _playingAnims[0];

                AnimatorStateInfo currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
                AnimatorTransitionInfo tInfo = animator.GetAnimatorTransitionInfo(0);
                if (currentStateInfo.IsName(_playingAnims[0]))
                {
                    // force to play the animation.
                    animator.Play(anim, 0, 0.0f);
                }
                else if (tInfo.nameHash != 0)
                {
                    // reset all triggers
                    DebugUtil.Log("EASPetMotionAnimationControl - Reset Trigger");

                    for (int i = 0; i < _playedAnims.Count; ++i)
                    { animator.ResetTrigger(_playedAnims[i]); }
                    /*
                        foreach (string animName in _playedAnims)
                            animator.ResetTrigger(animName);
                     */
                    animator.ResetTrigger("stop");

                    // play the animation
                    animator.SetTrigger(anim);
                }
                else
                {
                    // set trigger the specified animation.
                    animator.SetTrigger(anim);
                }

                _playingAnims.RemoveAt(0);

                DebugUtil.LogFormat("EASPetMotionAnimationControl - Playing aniamation ({0})... ({1}/{2})",
                                     anim, 1,
                                     animList.Count * playCount);
            }
        }

        public void Pause()
        {
            animator.speed = 0.0f;

            DebugUtil.Log("EASPetMotionAnimationControl - Pause");
        }

        public void Resume()
        {
            animator.speed = 1.0f;

            DebugUtil.Log("EASPetMotionAnimationControl - Resume");
        }

        public void Stop()
        {
            if (_playing)
            {
                _playingAnims.Clear();

                animator.speed = 1.0f;

                animator.SetTrigger("stop");

                _playing = false;

                DebugUtil.Log("EASPetMotionAnimationControl - Stop");

                if (onFinished != null)
                    onFinished();
            }
        }
    }
}