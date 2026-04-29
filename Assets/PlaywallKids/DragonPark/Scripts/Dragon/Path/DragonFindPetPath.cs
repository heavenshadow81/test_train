using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ML.PlaywallKids.DragonPark
{
    /// <summary>
    /// The path controller used in "FindPet" game.
    /// The subclass of DragonPath.
    /// </summary>
    public class DragonFindPetPath : DragonPath
    {
        #region Public variables
        /// <summary>
        /// The player ID. (0, 1)
        /// </summary>
        public int player = 0;

        /// <summary>
        /// Game level of player. (0 ~ 2)
        /// </summary>
        public int level = 0;

        /// <summary>
        /// The moved count.
        /// </summary>
        public int movedCount = 0;
        #endregion

        #region Properties
        private Transform[] _hideTransfoms;
        public Transform[] hideTransforms
        {
            get
            {
                if (_hideTransfoms == null)
                {
                    GameObject dummyParent = GameObject.Find("_FindPet_Dummy_HidePositions");
                    if (dummyParent)
                    {
                        Transform dummyParentTf = dummyParent.transform;
                        List<Transform> dummies = new List<Transform>(dummyParentTf.GetComponentsInChildren<Transform>());

                        if (dummies.Contains(dummyParentTf))
                            dummies.Remove(dummyParentTf);

                        dummies.Sort((a, b) => { return a.name.CompareTo(b.name); });

                        _hideTransfoms = dummies.ToArray();
                    }
                }
                return _hideTransfoms;
            }
        }

        private Transform[] _sleepTransforms;
        public Transform sleepTransform
        {
            get
            {
                if (_sleepTransforms == null)
                {
                    GameObject dummyParent = GameObject.Find("_FindPet_Dummy_SleepPositions");
                    if (dummyParent)
                    {
                        Transform dummyParentTf = dummyParent.transform;
                        List<Transform> dummies = new List<Transform>(dummyParentTf.GetComponentsInChildren<Transform>());

                        if (dummies.Contains(dummyParentTf))
                            dummies.Remove(dummyParentTf);

                        dummies.Sort((a, b) => { return a.name.CompareTo(b.name); });

                        _sleepTransforms = dummies.ToArray();
                    }
                }

                return _sleepTransforms[Mathf.Min(player, 1)];
            }
        }
        #endregion

        #region Private variables
        private Vector3[] _currentPath = null;
        private bool _gotCha = false;
        #endregion

        #region Constants
        /// <summary>
        /// Path count of player.
        /// </summary>
        public const int kPathCountOfPlayer = 4;

        /// <summary>
        /// When dragon needs to be sleeping?
        /// </summary>
        public const int kSleepCycle = 4;

        /// <summary>
        /// Wait time!
        /// </summary>
        public const float kWaitTimeFrom = 5.0f, kWaitTimeTo = 10.0f;

        public const float kDefaultMaxSpeed = 3.0f;

        /// <summary>
        /// Run fast!
        /// </summary>
        public const float kRunMaxSpeed = 6.5f;
        #endregion

        public override void MovePath()
        {
            movedCount++;

            if (movedCount % kSleepCycle == 0)
            {
                float speed = GetSpeed();
                Vector3 pos = sleepTransform.position;

                iTween.MoveTo(target, iTween.Hash("x", pos.x, "y", pos.y, "z", pos.z, "speed", speed, "easetype", iTween.EaseType.linear,
                                                  "oncomplete", "OnMovePathFinish",
                                                  "oncompletetarget", gameObject,
                                                  "orienttopath", true));

                Debug.Log(string.Format("DragonFindPetPath.MovePath() : Moving {0} to sleep position!", name));
            }
            else
            {
                float speed = GetSpeed();

                Transform by, to;
                while (true)
                {
                    GetRandomHideTransform(out by, out to);
                    if (_currentPath == null ||
                       _currentPath.Length < 1 ||
                    (Mathf.Abs((by.position - _currentPath[_currentPath.Length - 1]).magnitude) > 2.0f &&
                     Mathf.Abs((to.position - _currentPath[_currentPath.Length - 1]).magnitude) > 2.0f))
                    {
                        break;
                    }
                }

                List<Vector3> path = new List<Vector3>(3);
                path.Add(transform.position);
                path.Add(by.position);
                path.Add(to.position);

                _currentPath = path.ToArray();

                iTween.MoveTo(target, iTween.Hash("path", _currentPath, "speed", speed, "easetype", iTween.EaseType.linear,
                              "oncomplete", "OnMovePathFinish",
                              "oncompletetarget", gameObject,
                              "orienttopath", true));

                Debug.Log(string.Format("DragonFindPetPath.MovePath() : Moving {0} to {1}->{2}", name, by, to));
            }
        }

        public override void OnMovePathFinish()
        {
            StartCoroutine(MoveNewPath());
        }

        public IEnumerator MoveNewPath()
        {
            if (movedCount > 1)
            {
                dragonAnimation.maxSpeed = kDefaultMaxSpeed;
            }

            if (movedCount % kSleepCycle == 0)
            {
                dragonAnimation.Sleep();
            }

            // Wait for seconds!
            yield return new WaitForSeconds(Random.Range(kWaitTimeFrom, kWaitTimeTo));

            if (movedCount % kSleepCycle == 0)
            {
                dragonAnimation.Wake(true);

                while (true)
                {
                    AnimatorClipInfo[] infos = dragonAnimation.animator.GetCurrentAnimatorClipInfo(0);
                    if (infos.Length == 0)
                    {
                        break;
                    }
                    else
                    {
                        if (infos[0].clip.name.Contains("Sleep") == false)
                            break;
                    }
                    yield return null;
                }
            }

            // Move!
            MovePath();
        }

        public new void Sleep()
        {
            dragonAnimation.Sleep();
        }

        public void GotCha()
        {
            _gotCha = true;

            movedCount = 0;

            dragonAnimation.maxSpeed = kRunMaxSpeed;

            StopPath();

            dragonAnimation.Wake(true);

            MovePath();
        }

        public override void Update()
        {
        }

        public void GetRandomHideTransform(out Transform by, out Transform to)
        {
            // all dummy count of player
            int dummyCountOfPlayer = hideTransforms.Length / 2;

            // each hide position has 2 dummies.
            int posCountOfPlayer = dummyCountOfPlayer / 2;

            // random
            int random = dummyCountOfPlayer * player + Random.Range(0, posCountOfPlayer) * 2;

            Transform pos1 = hideTransforms[random];
            Transform pos2 = hideTransforms[random + 1];

            // Swap!
            if (Random.Range(0.0f, 1.0f) >= .5f)
            {
                Transform temp = pos1;
                pos1 = pos2;
                pos2 = temp;
            }

            by = pos1;
            to = pos2;
        }

        public float GetSpeed()
        {
            if (dragonAnimation.maxSpeed < kRunMaxSpeed)
            {
                return 1.0f + level * 1.0f;
            }
            else
            {
                return kRunMaxSpeed;
            }
        }

        public void OnDrawGizmos()
        {
            if (_currentPath != null)
            {
                iTween.DrawPath(_currentPath);
            }
        }
    }
}