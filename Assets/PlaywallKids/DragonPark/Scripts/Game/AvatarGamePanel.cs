using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Video;

namespace ML.PlaywallKids.DragonPark
{
    public class AvatarGamePanel : MonoBehaviour
    {
        #region Public variables
        // Message
        public UILabel messageLabel;

        // Balloon
        public GameObject balloonPrefab;

        // Panels
        public GameObject mainPanel;
        public GameObject gamePanel;

        // Prefabs
        public AToolAvatarMainPanel avatarMainPanelPrefab;

        // Game
        public GameObject monsterPathLeft, monsterPathRight;

        // Avatar
        public Avatar leftAvatar, rightAvatar;

        // Movie
        public GameObject tutorial;
        public UILabel tutorialLabel;
        public VideoPlayer tutorialMovieTexture;
        #endregion

        #region Private variables
        private bool _startingGame = false;

        private List<Template3D> _selectedTemplates = new List<Template3D>();
        private List<AToolAvatarMainPanel> _panels = new List<AToolAvatarMainPanel>();
        private Dictionary<Template3D, AvatarGamePanelCheckDragonUI> _balloonDict = new Dictionary<Template3D, AvatarGamePanelCheckDragonUI>();
        #endregion

        void Start()
        {
            balloonPrefab.SetActive(false);

            mainPanel.SetActive(true);
            gamePanel.SetActive(false);

            tutorial.SetActive(false);

            monsterPathLeft = (GameObject)Instantiate(monsterPathLeft);
            monsterPathRight = (GameObject)Instantiate(monsterPathRight);
            monsterPathLeft.transform.parent = monsterPathRight.transform.parent = transform;

            //PlayWallServer.sharedInstance.user1Skeleton = PlayWallServer.sharedInstance.user2Skeleton = null;

            if (SettingsManager.singleMode)
            {
                MakeAToolPanel();
                MakeAToolPanel();
            }
        }

        void OnDestroy()
        {
            MenuControl.leftUserSeq = MenuControl.rightUserSeq = 0;

            // clean avatars
            if (leftAvatar != null)
            {
                if (_selectedTemplates.Count > 0)
                {
                    MenuControl.leftUserSeq = _selectedTemplates[0].userSeq;
                    MenuControl.leftUserId = _selectedTemplates[0].userId;
                }
                Destroy(leftAvatar.gameObject);
            }
            if (rightAvatar != null)
            {
                if (_selectedTemplates.Count > 1)
                {
                    MenuControl.rightUserSeq = _selectedTemplates[1].userSeq;
                    MenuControl.rightUserId = _selectedTemplates[1].userId;
                }
                Destroy(rightAvatar.gameObject);
            }

            foreach (Template3D template in _balloonDict.Keys)
            {
                var balloon = _balloonDict[template];
                Destroy(balloon.gameObject);
            }

            _balloonDict.Clear();
            _selectedTemplates.Clear();
        }

        public IEnumerator PlayGame()
        {
            yield return null;

            if (tutorial != null)
            {
                tutorial.SetActive(true);
            }

            if (tutorialLabel != null)
            {
                int duration = 5;
                while (duration >= 0)
                {
                    tutorialLabel.text = string.Format("{0}초 뒤 게임이 시작됩니다!", duration);

                    if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.N))
                    {
                        duration = -1;
                    }

                    yield return new WaitForSeconds(1.0f);
                    duration -= 1;
                }
            }
            else if (tutorialMovieTexture != null)
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

                if (tutorial != null)
                {
                    tutorial.SetActive(false);
                }
            }

            StartGame();
        }

        public void StartGame()
        {
            _startingGame = false;

            if (gamePanel.activeSelf == false)
            {
                mainPanel.SetActive(false);
                gamePanel.SetActive(true);

                AvatarGameControl gameControl = gamePanel.GetComponent<AvatarGameControl>();
                gameControl.player1Avatar = leftAvatar;
                gameControl.player2Avatar = rightAvatar;
                gameControl.ResetGame();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (_balloonDict.Count + _panels.Count < 2)
            {
                int touchCount = CustomInput.touchCount;
                for (int i = 0; i < touchCount; i++)
                {
                    TouchInfo touch = CustomInput.GetTouch(i);
                    if (touch.phase != TouchInfo.Phase.Begin) continue;

                    Vector3 position = touch.position;
                    position.z = 0;

                    Ray ray = Camera.main.ScreenPointToRay(position);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        Template3D template = hit.collider.GetComponent<Template3D>();
                        MakeBalloon(template, position);
                    }
                }
            }
            else
            {
                if (_panels.Count > 0 && _panels[0].gameObject.activeSelf == false && leftAvatar == null)
                {
                    MakeAvatar(_panels[0].avatar, 0);
                }
                if (_panels.Count > 1 && _panels[1].gameObject.activeSelf == false && rightAvatar == null)
                {
                    MakeAvatar(_panels[1].avatar, 1);
                }
                if (leftAvatar != null)
                {
                    PlayWallServer.sharedInstance.user1Skeleton = leftAvatar.skeleton;
                }
                if (rightAvatar != null)
                {
                    PlayWallServer.sharedInstance.user2Skeleton = rightAvatar.skeleton;
                }

                if (leftAvatar != null && rightAvatar != null)
                {
                    if (!SettingsManager.singleMode)
                    {
                        if (PlayWallServer.sharedInstance.receivingSkeletionJoints && !gamePanel.activeSelf)
                        {
                            bool playGame = PlayWallServer.sharedInstance.readyToPlayAvatarGame;
                            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.N))
                            {
                                playGame = true;
                            }

                            if (playGame && !_startingGame)
                            {
                                _startingGame = true;
                                StartCoroutine(PlayGame());
                            }
                            else
                            {
                                messageLabel.text = "게임을 시작하려면 제자리에서 점프를 해주세요.";
                            }
                        }
                    }
                    else
                    {
                        if (PlayWallServer.sharedInstance.receivingSkeletionJoints)
                        {
                            messageLabel.text = "플레이월 속에 당신의 아바타가 탄생하고 있습니다.";
                        }
                        else
                        {
                            messageLabel.text = "발판 위에 서 주세요.";
                        }
                    }
                }
            }
        }

        public void MakeBalloon(Template3D template, Vector3 position)
        {
            if (template != null && !_selectedTemplates.Contains(template) && !_balloonDict.ContainsKey(template))
            {
                DragonAnimationControl dragonAnimation = template.GetComponent<DragonAnimationControl>();
                if (dragonAnimation != null)
                {
                    dragonAnimation.movesAlongPath = false;
                }
                Vector3 balloonPosition = UICamera.mainCamera.ScreenToWorldPoint(position);
                AvatarGamePanelCheckDragonUI newBalloon = NGUITools.AddChild(gameObject, balloonPrefab).GetComponent<AvatarGamePanelCheckDragonUI>();
                newBalloon.template = template;
                newBalloon.userSeq = template.userSeq;
                newBalloon.gameObject.SetActive(true);
                newBalloon.transform.position = balloonPosition;
                newBalloon.transform.localPosition += new Vector3(0, 150.0f, 0.0f);
                _balloonDict[template] = newBalloon;

                UIPanel panel = newBalloon.GetComponent<UIPanel>();
                if (panel != null)
                {
                    panel.depth = panel.depth + _panels.Count;
                }
            }
        }

        public void MakeAvatar(Avatar original, int position)
        {
            Avatar avatar = (Avatar)Instantiate(original);
            Transform[] tfs = avatar.GetComponentsInChildren<Transform>();
            foreach (Transform t in tfs)
            {
                t.gameObject.layer = LayerMask.NameToLayer("Default");
            }
            avatar.transform.parent = null;
            avatar.transform.position = new Vector3(21.0f, 0.0f, (position == 0 ? -2.0f : 3.5f));
            avatar.transform.localRotation = Quaternion.Euler(0, 90, 0);
            avatar.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

            if (position == 0)
            {
                leftAvatar = avatar;
            }
            else
            {
                rightAvatar = avatar;
            }
        }

        public void MakeAToolPanel(int userSeq = 0)
        {
            AToolAvatarMainPanel newPanel = NGUITools.AddChild(gameObject, avatarMainPanelPrefab.gameObject).GetComponent<AToolAvatarMainPanel>();
            newPanel.transform.localPosition = new Vector3(-400.0f + 800.0f * _panels.Count, -UIRoot.list[0].activeHeight * 0.1f, 0.0f);

            if (userSeq == 0)
            {
                newPanel.gender = (Random.Range(0.0f, 1.0f) >= 0.5f ? "m" : "f");
            }
            else
            {
                PlayWallWebServer.GetAvatarPicture(userSeq, (texture) => { newPanel.headImage = texture; });
            }

            _panels.Add(newPanel);

            if (_panels.Count > 1)
            {
                messageLabel.text = "플레이월 속에 당신의 아바타가 탄생하고 있습니다.";
            }
        }

        public void Select(Template3D template, bool yes)
        {
            if (template != null && !_selectedTemplates.Contains(template))
            {
                if (_balloonDict.ContainsKey(template))
                {
                    _balloonDict.Remove(template);
                }

                if (yes)
                {
                    _selectedTemplates.Add(template);

                    if (_panels.Count < _selectedTemplates.Count)
                    {
                        MakeAToolPanel(template.userSeq);
                    }
                }

                DragonAnimationControl dragonAnimation = template.GetComponent<DragonAnimationControl>();
                if (dragonAnimation != null)
                {
                    dragonAnimation.movesAlongPath = true;
                }
            }
        }
    }
}