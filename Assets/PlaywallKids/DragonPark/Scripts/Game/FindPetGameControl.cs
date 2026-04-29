using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace ML.PlaywallKids.DragonPark
{
    public class FindPetGameControl : MonoBehaviour
    {
        public enum State
        {
            Title,
            Tutorial,
            Game,
            Result
        }

        #region Public variables
        public GameObject loading;

        public GameObject wait;
        public GameObject n3;
        public GameObject n2;
        public GameObject n1;
        public GameObject n0;

        public UIPanel gamePanel;
        public GrabCursorUI player1Cursor;
        public GrabCursorUI player2Cursor;

        public UILabel player1Label;
        public UILabel player2Label;

        public UIProgressBar timeSlider;
        public TweenRotation timeSpriteRotation;

        public UISprite player1Result;
        public UISprite player2Result;

        public GameObject player1Dragon;
        public GameObject player2Dragon;

        public AudioClip gotCha1Sound;
        public AudioClip gotCha2Sound;
        public AudioClip resultSound;

        public int level = 3;
        #endregion

        #region Properties
        private State _state = State.Game;
        public State state
        {
            get
            {
                return _state;
            }
            set
            {
                _HideState();
                _state = value;
                _ResetState();
            }
        }
        #endregion

        #region Private variables (Gameplay)
        private float _remainedTime = 0.0f;
        private int _p1Score = 0, _p2Score = 0;
        private bool _p1Grabable = true, _p2Grabable = true;
        private bool _playing = false;
        #endregion

        #region Constants
        public const float kGameTime = 60.0f;
        #endregion

        // Use this for initialization
        void Start()
        {
            loading.SetActive(false);

            player1Cursor.onGrab = () => { StartCoroutine(OnGrabP1()); };
            player2Cursor.onGrab = () => { StartCoroutine(OnGrabP2()); };

            if (player1Dragon == null)
            {
                Template3D template = null;

                if (!SettingsManager.singleMode)
                {
                    string identifier = ResourceManager.GetRecentTemplate3DIdentifier(MenuControl.leftUserSeq, MenuControl.leftUserId);
                    template = ResourceManager.LoadTemplate3D(identifier);
                }

                if (template == null)
                {
                    GameObject prefab = Dragon.LoadPrefab(Dragon.characterNames[Random.Range(0, Dragon.characterNames.Length)]);
                    GameObject go = (GameObject)Instantiate(prefab);
                    template = go.AddComponent<Template3D>();
                }
                else
                {
                    template.CleanMainTexture();
                }

                template.name = "Player 1";
                player1Dragon = template.gameObject;
                DragonFindPetPath path = player1Dragon.AddComponent<DragonFindPetPath>();
                path.player = 0;
                path.level = level;
                player1Dragon.transform.position = new Vector3(23.0f, 0.0f, -1.5f);
                player1Dragon.transform.LookAt(player1Dragon.transform.position + new Vector3(1.0f, 0.0f, 0.0f));

                player1Dragon.GetComponent<DragonAnimationControl>().movesAlongPath = false;
            }

            if (player2Dragon == null)
            {
                Template3D template = null;

                if (!SettingsManager.singleMode)
                {
                    string identifier = ResourceManager.GetRecentTemplate3DIdentifier(MenuControl.leftUserSeq, MenuControl.leftUserId);
                    template = ResourceManager.LoadTemplate3D(identifier);
                }

                if (template == null)
                {
                    GameObject prefab = Dragon.LoadPrefab(Dragon.characterNames[Random.Range(0, Dragon.characterNames.Length)]);
                    GameObject go = (GameObject)Instantiate(prefab);
                    template = go.AddComponent<Template3D>();
                }
                else
                {
                    template.CleanMainTexture();
                }

                template.name = "Player 2";
                player2Dragon = template.gameObject;
                DragonFindPetPath path = player2Dragon.AddComponent<DragonFindPetPath>();
                path.player = 1;
                path.level = level;
                player2Dragon.transform.position = new Vector3(23.0f, 0.0f, 1.5f);
                player2Dragon.transform.LookAt(player2Dragon.transform.position + new Vector3(1.0f, 0.0f, 0.0f));

                player2Dragon.GetComponent<DragonAnimationControl>().movesAlongPath = false;
            }

            StartGame();
        }

        // Update is called once per frame
        void Update()
        {
            switch (_state)
            {
                case State.Game:
                    UpdateGame();
                    break;
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                if (_playing)
                {
                    PauseGame();
                }
                else if (state == State.Game)
                {
                    ResumeGame();
                }
            }

            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.N))
            {
                EndGame();
            }
        }

        void OnDestroy()
        {
            player1Dragon = null;
            player2Dragon = null;
        }

        private void _HideState()
        {
            switch (_state)
            {
                case State.Title:
                    //titlePanel.cachedGameObject.SetActive(false);
                    break;
                case State.Tutorial:
                    //tutorialPanel.cachedGameObject.SetActive(false);
                    break;
                case State.Game:
                    gamePanel.cachedGameObject.SetActive(false);
                    break;
            }
        }

        private void _ResetState()
        {
            switch (_state)
            {
                case State.Title:
                    //titlePanel.cachedGameObject.SetActive(true);
                    break;
                case State.Tutorial:
                    //tutorialPanel.cachedGameObject.SetActive(true);
                    break;
                case State.Game:
                    gamePanel.cachedGameObject.SetActive(true);
                    ResetGame();
                    break;
            }
        }

        public void ResetGame()
        {
            StartCoroutine(_PerformResetGame());
        }

        private IEnumerator _PerformResetGame()
        {
            _playing = false;
            _p1Score = _p2Score = 0;

            float height = UIRoot.list[0].activeHeight;
            float width = height * ((float)Screen.width / (float)Screen.height);

            player1Cursor.gameObject.SetActive(true);
            player2Cursor.gameObject.SetActive(true);

            player1Cursor.transform.localPosition = new Vector3(-width * 0.25f, 0.0f);
            player2Cursor.transform.localPosition = new Vector3(width * 0.25f, 0.0f);

            player1Result.cachedGameObject.SetActive(false);
            player2Result.cachedGameObject.SetActive(false);

            wait.SetActive(true); n3.SetActive(true); n2.SetActive(false); n1.SetActive(false); n0.SetActive(false);

            yield return new WaitForSeconds(1.0f);

            n3.SetActive(false); n2.SetActive(true);

            yield return new WaitForSeconds(1.0f);

            n2.SetActive(false); n1.SetActive(true);

            yield return new WaitForSeconds(1.0f);

            n1.SetActive(false); n0.SetActive(true);

            player1Dragon.GetComponent<DragonAnimationControl>().movesAlongPath = true;
            player2Dragon.GetComponent<DragonAnimationControl>().movesAlongPath = true;

            _remainedTime = kGameTime;
            _playing = true;

            yield return new WaitForSeconds(1.0f);
            n0.SetActive(false); wait.SetActive(false);
        }

        public void StartGame()
        {
            state = State.Game;
        }

        public void PauseGame()
        {
            _playing = false;
            player1Dragon.GetComponent<DragonFindPetPath>().movesTarget = false;
            player2Dragon.GetComponent<DragonFindPetPath>().movesTarget = false;
        }

        public void ResumeGame()
        {
            _playing = true;
            player1Dragon.GetComponent<DragonFindPetPath>().movesTarget = true;
            player2Dragon.GetComponent<DragonFindPetPath>().movesTarget = true;
        }

        public void EndGame()
        {
            player1Cursor.gameObject.SetActive(false);
            player2Cursor.gameObject.SetActive(false);

            player1Cursor.grab = false;
            player2Cursor.grab = false;

            player1Result.cachedGameObject.SetActive(true);
            player2Result.cachedGameObject.SetActive(true);

            player1Result.cachedTransform.localScale = Vector3.zero;
            player2Result.cachedTransform.localScale = Vector3.zero;

            DragonAnimationControl dragonAnimation1 = player1Dragon.GetComponent<DragonAnimationControl>();
            DragonAnimationControl dragonAnimation2 = player2Dragon.GetComponent<DragonAnimationControl>();

            dragonAnimation1.movesAlongPath = false;
            dragonAnimation2.movesAlongPath = false;

            if (_p1Score < _p2Score)
            {
                player1Result.spriteName = "LOSE";
                player2Result.spriteName = "WIN";

                dragonAnimation1.Lose();
                dragonAnimation2.Win();
            }
            else if (_p1Score > _p2Score)
            {
                player1Result.spriteName = "WIN";
                player2Result.spriteName = "LOSE";

                dragonAnimation1.Win();
                dragonAnimation2.Lose();
            }
            else
            {
                player1Result.spriteName = "WIN";
                player2Result.spriteName = "WIN";

                dragonAnimation1.Win();
                dragonAnimation2.Win();
            }

            AudioSource.PlayClipAtPoint(resultSound, Camera.main.transform.position);

            TweenScale.Begin(player1Result.cachedGameObject, 0.25f, new Vector3(1.0f, 1.0f, 1.0f));
            TweenScale.Begin(player2Result.cachedGameObject, 0.25f, new Vector3(1.0f, 1.0f, 1.0f));

            StartCoroutine(BackToDragonPark());
        }

        public IEnumerator BackToDragonPark()
        {
            yield return new WaitForSeconds(5.0f);

            loading.SetActive(true);

            yield return new WaitForSeconds(1.0f);

            //Application.LoadLevelAsync("DragonPark");
            SceneManager.LoadSceneAsync("DragonPark");
        }

        public void UpdateGame()
        {
            if (_remainedTime > 0.0f)
            {
                if (_playing)
                {
                    player1Label.text = _p1Score.ToString();
                    player2Label.text = _p2Score.ToString();

                    timeSlider.value = _remainedTime / kGameTime;
                    timeSpriteRotation.enabled = true;
                    timeSpriteRotation.duration = 0.5f;
                    if (_remainedTime < kGameTime * 0.3333f)
                    {
                        timeSpriteRotation.duration -= (kGameTime * 0.3333f - _remainedTime) / kGameTime * 0.333333f * 0.45f;
                    }

                    UpdateCursorPositions();

                    Check();

                    _remainedTime -= Time.deltaTime;

                    if (_remainedTime <= 0.0f)
                    {
                        EndGame();
                    }
                }
            }
            else
            {
                timeSlider.value = 1.0f;
                timeSpriteRotation.enabled = false;
                timeSpriteRotation.cachedTransform.localRotation = Quaternion.identity;
            }
        }

        public void UpdateCursorPositions()
        {
            Vector3 cursor1Position = player1Cursor.transform.localPosition;
            Vector3 cursor2Position = player2Cursor.transform.localPosition;

            // for debug mode.
            if (Input.GetKey(KeyCode.LeftShift))
            {
                float height = UIRoot.list[0].activeHeight;
                float width = height * ((float)Screen.width / (float)Screen.height);

                float halfWidth = width * 0.5f;
                float halfHeight = height * 0.5f;

                if (Input.GetKey(KeyCode.A))
                {
                    Debug.Log("DD");
                    cursor1Position.x = Mathf.Max(-halfWidth, cursor1Position.x - height * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.D))
                {
                    cursor1Position.x = Mathf.Min(0.0f, cursor1Position.x + height * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.S))
                {
                    cursor1Position.y = Mathf.Max(-halfHeight, cursor1Position.y - height * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.W))
                {
                    cursor1Position.y = Mathf.Min(halfHeight, cursor1Position.y + height * Time.deltaTime);
                }


                if (Input.GetKey(KeyCode.J))
                {
                    cursor2Position.x = Mathf.Max(0.0f, cursor2Position.x - height * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.L))
                {
                    cursor2Position.x = Mathf.Min(halfWidth, cursor2Position.x + height * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.K))
                {
                    cursor2Position.y = Mathf.Max(-halfHeight, cursor2Position.y - height * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.I))
                {
                    cursor2Position.y = Mathf.Min(halfHeight, cursor2Position.y + height * Time.deltaTime);
                }
            }
            else
            {
                float aspectRatio = ((float)Screen.width / (float)Screen.height);
                float height = UIRoot.list[0].activeHeight;
                float width = height * aspectRatio;

                cursor1Position = PlayWallServer.sharedInstance.user1CursorPos;
                cursor2Position = PlayWallServer.sharedInstance.user2CursorPos;

                cursor1Position.x -= 1920;
                cursor1Position.y -= 720;
                cursor2Position.x -= 1920;
                cursor2Position.y -= 720;
            }

            player1Cursor.transform.localPosition = cursor1Position;
            player2Cursor.transform.localPosition = cursor2Position;
        }

        public void Check()
        {
            Vector3 cursor1Pos = UICamera.mainCamera.WorldToScreenPoint(player1Cursor.transform.position);
            Vector3 character1Pos = Camera.main.WorldToScreenPoint(player1Dragon.transform.position + new Vector3(0, 0.75f, 0.0f));

            if ((cursor1Pos - character1Pos).magnitude <= (float)Screen.height * 0.16f && _p1Grabable)
            {
                player1Cursor.grab = true;
            }
            else
            {
                player1Cursor.grab = false;
            }

            Vector3 cursor2Pos = UICamera.mainCamera.WorldToScreenPoint(player2Cursor.transform.position);
            Vector3 character2Pos = Camera.main.WorldToScreenPoint(player2Dragon.transform.position + new Vector3(0, 0.75f, 0.0f));

            if ((cursor2Pos - character2Pos).magnitude <= (float)Screen.height * 0.16f && _p2Grabable)
            {
                player2Cursor.grab = true;
            }
            else
            {
                player2Cursor.grab = false;
            }
        }

        public IEnumerator OnGrabP1()
        {
            player1Cursor.grab = false;
            _p1Score += 1;
            _p1Grabable = false;

            player1Dragon.GetComponent<DragonFindPetPath>().GotCha();
            AudioSource.PlayClipAtPoint(gotCha1Sound, Camera.main.transform.position);

            yield return new WaitForSeconds(1.5f);

            _p1Grabable = true;
        }

        public IEnumerator OnGrabP2()
        {
            player2Cursor.grab = false;
            _p2Score += 1;
            _p2Grabable = false;

            player2Dragon.GetComponent<DragonFindPetPath>().GotCha();
            AudioSource.PlayClipAtPoint(gotCha2Sound, Camera.main.transform.position);

            yield return new WaitForSeconds(1.5f);

            _p2Grabable = true;
        }
    }
}