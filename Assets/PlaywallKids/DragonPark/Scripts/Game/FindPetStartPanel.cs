using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Video;

namespace ML.PlaywallKids.DragonPark
{
    public class FindPetStartPanel : MonoBehaviour
    {
        #region Public variables
        public GameObject page0;
        public GameObject page1;
        public GameObject page2;
        public GameObject page3;
        public GameObject page4;
        public GameObject page5;
        public GameObject page6;
        public GameObject page7;

        public Animator selectGameAnimator;

        public VideoPlayer tutorialMovieTexture;
        #endregion

        #region Properties
        #endregion

        #region Private variables
        private List<GameObject> _pages = new List<GameObject>();
        private int _currentStep = 0;

        DragonAnimationControl _dragonAnimation1;
        DragonAnimationControl _dragonAnimation2;

        DragonComeToFront _dragonComeToFront1;
        DragonComeToFront _dragonComeToFront2;

        private bool _flag = false;
        private bool _receivedRightHandMotion = false;
        #endregion

        public void Start()
        {
            _pages.Add(page0);
            _pages.Add(page1);
            _pages.Add(page2);
            _pages.Add(page3);
            _pages.Add(page4);
            _pages.Add(page5);
            _pages.Add(page6);
            _pages.Add(page7);

            foreach (GameObject page in _pages) page.SetActive(false);

            if (!SettingsManager.singleMode)
            {
                var template1 = SimpleInstantiatedTemplateControl.GetCurrentTemplate(MenuControl.leftUserId);
                var template2 = SimpleInstantiatedTemplateControl.GetCurrentTemplate(MenuControl.rightUserId);

                if (template1 != null && template2 != null && (template1 != template2))
                {
                    _dragonAnimation1 = template1.GetComponent<DragonAnimationControl>();
                    _dragonAnimation2 = template2.GetComponent<DragonAnimationControl>();

                    _dragonComeToFront1 = _dragonAnimation1.GetComponent<DragonComeToFront>();
                    if (_dragonComeToFront1 == null)
                    {
                        _dragonComeToFront1 = _dragonAnimation1.gameObject.AddComponent<DragonComeToFront>();
                        _dragonComeToFront1.receivesTouch = false;
                    }
                    _dragonComeToFront1.userId = 0;
                    _dragonComeToFront2 = _dragonAnimation2.GetComponent<DragonComeToFront>();
                    if (_dragonComeToFront2 == null)
                    {
                        _dragonComeToFront2 = _dragonAnimation2.gameObject.AddComponent<DragonComeToFront>();
                        _dragonComeToFront2.receivesTouch = false;
                    }
                    _dragonComeToFront2.userId = 1;

                    DragonComeToFront.userCount = 2;

                    NextStep();
                }
                else
                {
                    page0.SetActive(true);
                }
            }
            else
            {
                _currentStep = 6;
                PlayWallServer.sharedInstance.currentMotion = 0x46;
                NextStep();
            }
        }

        void OnDestroy()
        {
            DragonComeToFront.userCount = MenuControl.userCount;
            if (_dragonComeToFront1 != null)
            {
                _dragonComeToFront1.userId = MenuControl.leftUserId;
                _dragonComeToFront1.Back();
            }
            if (_dragonComeToFront2 != null)
            {
                _dragonComeToFront2.userId = MenuControl.rightUserId;
                _dragonComeToFront2.Back();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (_currentStep == 2)
            {
                if (PlayWallServer.sharedInstance.user1MotionFlag && PlayWallServer.sharedInstance.user2MotionFlag)
                {
                    NextStep();
                }
            }
            else if (_currentStep == 3)
            {
                AnimatorTransitionInfo transitionInfo = selectGameAnimator.GetAnimatorTransitionInfo(0);
                if (transitionInfo.nameHash == 0)
                {
                    if (PlayWallServer.sharedInstance.currentMotion == 0x42 && !_receivedRightHandMotion)
                    {
                        selectGameAnimator.SetTrigger("right");

                        _receivedRightHandMotion = true;
                        StartCoroutine(RightHandMotionDone());
                    }
                    else if (PlayWallServer.sharedInstance.currentMotion == 0x43)
                    {
                        NextStep();
                    }
                }
            }
            else if (_currentStep == 4)
            {
                if (PlayWallServer.sharedInstance.user1MotionFlag && !(_dragonComeToFront1.isComing || _dragonComeToFront1.came))
                {
                    _dragonComeToFront1.Come();
                }
                if (PlayWallServer.sharedInstance.user2MotionFlag && !(_dragonComeToFront2.isComing || _dragonComeToFront2.came))
                {
                    _dragonComeToFront2.Come();
                }

                if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Q))
                    _dragonComeToFront1.Come();
                if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.P))
                    _dragonComeToFront2.Come();
                if (_dragonComeToFront1.came && _dragonComeToFront2.came)
                {
                    NextStep();
                }
            }
            else if (_currentStep == 5)
            {
                if (PlayWallServer.sharedInstance.user1MotionFlag ||
                   (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Q)))
                {
                    if (_dragonAnimation1.isIdle)
                    {
                        _dragonAnimation1.Touch_Test();
                    }
                }
                if (PlayWallServer.sharedInstance.user2MotionFlag ||
                   (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.P)))
                {
                    if (_dragonAnimation2.isIdle)
                    {
                        _dragonAnimation2.Touch_Test();
                    }
                }
                if (PlayWallServer.sharedInstance.user1MotionFlag && PlayWallServer.sharedInstance.user2MotionFlag)
                {
                    NextStep();
                }
            }
            else if (_currentStep == 6)
            {
                if (PlayWallServer.sharedInstance.user1MotionFlag && PlayWallServer.sharedInstance.user2MotionFlag)
                {
                    NextStep();
                }
            }

            // Debug Mode
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.N))
            {
                NextStep();
            }
        }

        public IEnumerator RightHandMotionDone()
        {
            float time = 0.0f;
            while (time < 1.0f)
            {
                if (PlayWallServer.sharedInstance.currentMotion == 0x43)
                {
                    break;
                }
                time += Time.deltaTime;
                if (time >= 1.0f && PlayWallServer.sharedInstance.currentMotion == 0x42)
                {
                    PlayWallServer.sharedInstance.SendMotionDone();
                    _receivedRightHandMotion = false;
                }
                yield return null;
            }
        }

        public void NextStep()
        {
            if (_currentStep + 1 >= _pages.Count) return;
            GameObject prev = _pages[_currentStep];
            prev.SetActive(false);
            GameObject next = null;
            if (_pages.Count > _currentStep + 1)
            {
                next = _pages[_currentStep + 1];
                next.SetActive(true);
            }

            if (_currentStep == 1)
            {
                PlayWallServer.sharedInstance.currentMotion = 0x40;
                PlayWallServer.sharedInstance.SendMotionDone();
            }
            else if (_currentStep > 1)
            {
                PlayWallServer.sharedInstance.SendMotionDone();
            }

            _currentStep++;

            // post-process
            if (_currentStep == 1)
            {
                // Next step after 3 seconds.
                Invoke("NextStep", 3.0f);
            }
            else if (_currentStep == 7)
            {
                StartCoroutine(Go());
            }
        }

        public IEnumerator Go()
        {
            tutorialMovieTexture.Stop();
            tutorialMovieTexture.Play();

            float duration = (float)tutorialMovieTexture.time;
            while (duration > 0)
            {
                yield return null;

                duration -= Time.deltaTime;

                if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.N))
                {
                    duration = 0;
                }
            }

            NextStep();
            MenuControl.sharedInstance.loading.SetActive(true);
            yield return new WaitForSeconds(1.0f);
            UnityEngine.SceneManagement.SceneManager.LoadScene("FindPet");
        }
    }
}
