using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ML.PlaywallKids.DragonPark
{
    public class AvatarGameControl : MonoBehaviour
    {
        #region Public variables
        public GameObject wait;
        public GameObject n3;
        public GameObject n2;
        public GameObject n1;
        public GameObject n0;

        public UILabel player1Label;
        public UILabel player2Label;

        public UIProgressBar timeSlider;
        public TweenRotation timeSpriteRotation;

        public UISprite player1Region;
        public UISprite player2Region;

        public UISprite player1Result;
        public UISprite player2Result;

        public Avatar player1Avatar, player2Avatar;

        public GameObject[] monsterPrefabs;

        public GameObject smokePrefab;

        public AudioClip readyGoSound;
        public AudioClip gotChaSound;
        public AudioClip resultSound;
        #endregion

        #region Private variables (Gameplay)
        private float _remainedTime = 0.0f;
        private int _p1Score = 0, _p2Score = 0;
        private bool _p1Clap, _p2Clap;
        private float _p1MonsterGenTime, _p2MonsterGenTime;
        private Vector3 _p1RegionPos, _p2RegionPos;

        private List<GameObject> _player1Monsters = new List<GameObject>();
        private List<GameObject> _player2Monsters = new List<GameObject>();
        private List<int> _player1MonsterScores = new List<int>();
        private List<int> _player2MonsterScores = new List<int>();

        private bool _playing = false;
        #endregion

        #region Constants
        public const float kGameTime = 60.0f;
        public const float kMonsterGenTime = 5.0f;
        #endregion

        // Update is called once per frame
        void Update()
        {
            UpdateGame();

            if (Input.GetKeyDown(KeyCode.P))
            {
                if (_playing)
                {
                    PauseGame();
                }
                else
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
            player1Avatar = player2Avatar = null;
        }

        public void ResetGame()
        {
            StartCoroutine(_PerformResetGame());
        }

        private IEnumerator _PerformResetGame()
        {
            _playing = false;
            _p1Score = _p2Score = 0;

            foreach (GameObject go in _player1Monsters)
            {
                Destroy(go);
            }
            _player1Monsters.Clear();
            _player1MonsterScores.Clear();

            foreach (GameObject go in _player2Monsters)
            {
                Destroy(go);
            }
            _player2Monsters.Clear();
            _player2MonsterScores.Clear();

            _p1MonsterGenTime = float.MaxValue;
            _p2MonsterGenTime = float.MaxValue;
            _p1Clap = false;
            _p2Clap = false;
            _p1RegionPos = player1Avatar.transform.position + new Vector3(1.0f, 1.0f, 0.0f);
            _p2RegionPos = player2Avatar.transform.position + new Vector3(1.0f, 1.0f, 0.0f);

            player1Result.cachedGameObject.SetActive(false);
            player2Result.cachedGameObject.SetActive(false);

            player1Region.color = player2Region.color = Color.white;

            //player1Region.transform.localPosition = UICamera.currentCamera.WorldToScreenPoint(_p1RegionPos);
            //player2Region.transform.localPosition = UICamera.currentCamera.WorldToScreenPoint(_p2RegionPos);

            wait.SetActive(true); n3.SetActive(true); n2.SetActive(false); n1.SetActive(false); n0.SetActive(false);

            AudioSource.PlayClipAtPoint(readyGoSound, Vector3.zero);

            yield return new WaitForSeconds(1.0f);

            n3.SetActive(false); n2.SetActive(true);

            yield return new WaitForSeconds(1.0f);

            n2.SetActive(false); n1.SetActive(true);

            yield return new WaitForSeconds(1.0f);

            n1.SetActive(false); n0.SetActive(true);

            _remainedTime = kGameTime;
            _playing = true;

            yield return new WaitForSeconds(1.0f);
            n0.SetActive(false); wait.SetActive(false);
        }

        public void StartGame()
        {
            ResetGame();
        }

        public void PauseGame()
        {
            _playing = false;
        }

        public void ResumeGame()
        {
            _playing = true;
        }

        public void EndGame()
        {
            player1Result.cachedGameObject.SetActive(true);
            player2Result.cachedGameObject.SetActive(true);

            player1Result.cachedTransform.localScale = Vector3.zero;
            player2Result.cachedTransform.localScale = Vector3.zero;

            while (_player1Monsters.Count > 0)
            {
                DestroyMonster(1, 0, false);
            }
            while (_player2Monsters.Count > 0)
            {
                DestroyMonster(2, 0, false);
            }

            if (_p1Score < _p2Score)
            {
                player1Result.spriteName = "LOSE";
                player2Result.spriteName = "WIN";
            }
            else if (_p1Score > _p2Score)
            {
                player1Result.spriteName = "WIN";
                player2Result.spriteName = "LOSE";
            }
            else
            {
                player1Result.spriteName = "WIN";
                player2Result.spriteName = "WIN";
            }

            AudioSource.PlayClipAtPoint(resultSound, Vector3.zero);

            TweenScale.Begin(player1Result.cachedGameObject, 0.25f, new Vector3(1.0f, 1.0f, 1.0f));
            TweenScale.Begin(player2Result.cachedGameObject, 0.25f, new Vector3(1.0f, 1.0f, 1.0f));
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

                    UpdateMonsters();

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
                timeSlider.value = 0.0f;
                timeSpriteRotation.enabled = false;
                timeSpriteRotation.cachedTransform.localRotation = Quaternion.identity;
            }
        }

        public void UpdateMonsters()
        {
            if (_p1MonsterGenTime >= kMonsterGenTime)
            {
                if (_player1Monsters.Count < 3)
                {
                    MakeMonster(1, _player1Monsters, _player1MonsterScores);

                    _p1MonsterGenTime = 0.0f;
                }
            }
            else
            {
                _p1MonsterGenTime += Time.deltaTime;
            }

            if (_p2MonsterGenTime >= kMonsterGenTime)
            {
                if (_player2Monsters.Count < 3)
                {
                    MakeMonster(2, _player2Monsters, _player2MonsterScores);

                    _p2MonsterGenTime = 0.0f;
                }
            }
            else
            {
                _p2MonsterGenTime += Time.deltaTime;
            }
        }

        public void MakeMonster(int player, List<GameObject> monsters, List<int> scores)
        {
            int idx = Random.Range(0, monsterPrefabs.Length);

            GameObject prefab = monsterPrefabs[idx];

            GameObject monster = (GameObject)Instantiate(prefab);
            monster.transform.localScale = new Vector3(.33f, .33f, .33f);

            monsters.Add(monster);
            scores.Add(10 * (idx + 1));
            MovePath(monster, player, 1.0f + idx * 1.0f);
        }

        public void MovePath(GameObject go, int pos, float speed = 1.0f)
        {
            if (go == null) return;

            Vector3[] path = iTweenPath.GetPath(string.Format("MonsterPath{0}", pos));
            for (int i = 0; i < path.Length; i++)
            {
                if (pos == 1)
                {
                    path[i] -= new Vector3(22.0f, -0.8f, -3.0f);
                    path[i].x *= 2.0f; path[i].z *= 2.0f;
                    path[i] += player1Avatar.transform.position;
                }
                else if (pos == 2)
                {
                    path[i] -= new Vector3(22.0f, -0.8f, 2.3f);
                    path[i].x *= 2.0f; path[i].z *= 2.0f;
                    path[i] += player2Avatar.transform.position;
                }
            }

            go.transform.position = path[0];

            iTween.MoveTo(go, iTween.Hash("path", path,
                                                "speed", speed,
                                                "easetype", iTween.EaseType.linear,
                                                "oncomplete", "MovePath",
                                                "looptype", "loop",
                                                "orienttopath", true));
        }

        public void Check()
        {
            Transform player1LeftHandJoint = player1Avatar.skeleton.joints[(int)AvatarSkeleton.Joint.HandLeft];
            Transform player1RightHandJoint = player1Avatar.skeleton.joints[(int)AvatarSkeleton.Joint.HandRight];
            Transform player2LeftHandJoint = player2Avatar.skeleton.joints[(int)AvatarSkeleton.Joint.HandLeft];
            Transform player2RightHandJoint = player2Avatar.skeleton.joints[(int)AvatarSkeleton.Joint.HandRight];

            float dist1 = (player1LeftHandJoint.position - player1RightHandJoint.position).magnitude;
            float dist2 = (player2LeftHandJoint.position - player2RightHandJoint.position).magnitude;

            bool clap1 = dist1 < (_p1Clap ? 0.25f : 0.125f);
            bool clap2 = dist2 < (_p2Clap ? 0.25f : 0.125f);

            if (!_p1Clap && clap1)
            {
                OnClapP1();
            }
            if (!_p2Clap && clap2)
            {
                OnClapP2();
            }

            // region color
            float p1RegionDist = float.MaxValue, p2RegionDist = float.MaxValue;
            foreach (GameObject go in _player1Monsters)
            {
                p1RegionDist = Mathf.Min(p1RegionDist, (go.transform.position - _p1RegionPos).magnitude);
            }
            foreach (GameObject go in _player2Monsters)
            {
                p2RegionDist = Mathf.Min(p2RegionDist, (go.transform.position - _p2RegionPos).magnitude);
            }
            //player1Region.color = Color.Lerp(Color.white, Color.black, p1RegionDist * .666f);
            //player2Region.color = Color.Lerp(Color.white, Color.black, p2RegionDist * .666f);

            // for debug
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Q))
            {
                OnClapP1();
            }
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.W))
            {
                OnClapP2();
            }

            if (_p1Clap != clap1)
            {
                Debug.Log("AvatarGameControl.Check() : Clap P1");
            }
            if (_p2Clap != clap2)
            {
                Debug.Log("AvatarGameControl.Check() : Clap P2");
            }

            _p1Clap = clap1;
            _p2Clap = clap2;
        }

        public void OnClapP1()
        {
            if (_player1Monsters.Count > 0)
            {
                bool sound = false;

                for (int i = 0; i < _player1Monsters.Count; i++)
                {
                    GameObject go = _player1Monsters[i];
                    if ((go.transform.position - _p1RegionPos).magnitude <= 1.0f)
                    {
                        // Destroy and score
                        DestroyMonster(1, i, true);

                        // index
                        i--;

                        // sound
                        if (!sound)
                        {
                            sound = true;
                            AudioSource.PlayClipAtPoint(gotChaSound, Vector3.zero);
                        }
                    }
                }

                if (_player1Monsters.Count == 0)
                {
                    _p1MonsterGenTime = float.MaxValue;
                }
            }
        }

        public void OnClapP2()
        {
            if (_player2Monsters.Count > 0)
            {
                bool sound = false;

                for (int i = 0; i < _player2Monsters.Count; i++)
                {
                    GameObject go = _player2Monsters[i];
                    if ((go.transform.position - _p2RegionPos).magnitude <= 1.0f)
                    {
                        // Destroy and score
                        DestroyMonster(2, i, true);

                        // index
                        i--;

                        // sound
                        if (!sound)
                        {
                            sound = true;
                            AudioSource.PlayClipAtPoint(gotChaSound, Vector3.zero);
                        }
                    }
                }

                if (_player2Monsters.Count == 0)
                {
                    _p2MonsterGenTime = float.MaxValue;
                }
            }
        }

        public void DestroyMonster(int player, int index, bool appendScore = true)
        {
            List<GameObject> monsters = null;
            List<int> scores = null;

            if (player == 1)
            {
                monsters = _player1Monsters;
                scores = _player1MonsterScores;
            }
            else
            {
                monsters = _player2Monsters;
                scores = _player2MonsterScores;
            }

            // object
            GameObject go = monsters[index];

            // score
            int score = scores[index];
            scores.RemoveAt(index);

            if (appendScore)
            {
                if (player == 1)
                {
                    _p1Score += score;
                }
                else
                {
                    _p2Score += score;
                }
            }

            // smoke
            MakeSmoke(go.transform.position, score);

            //destroy
            monsters.RemoveAt(index);
            Destroy(go);
        }

        public void MakeSmoke(Vector3 position, int score)
        {
            GameObject smoke = (GameObject)Instantiate(smokePrefab);
            smoke.transform.position = position;
            ParticleSystem particle = smoke.GetComponent<ParticleSystem>();
            var mainModule = particle.main;

            switch (score)
            {
                case 20:
                    mainModule.startColor = new Color(0.25f, 0.25f, 1.0f, 0.5f);
                    mainModule.startSizeMultiplier -= .2f;
                    break;
                case 30:
                    mainModule.startColor = new Color(1.0f, 0.2f, 0.2f, 0.88f);
                    mainModule.startSizeMultiplier -= .4f;
                    break;
                default:
                    break;
            }
        }
    }
}