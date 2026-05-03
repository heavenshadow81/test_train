//#define USE_TAG
using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.Interaction
{
    using EContentState = InteractionContentsEnum.EState;
    //실제 체험 흐름 제어
    [RequireComponent(typeof(InteractionContents))]
    public class InteractionKickBallContentsController : MonoBehaviour
    {
        public GameObject wordsOfGoalinObjet;
        public ParticleSystem[] fireworks;
        public GameObject[] balls;
        public Transform[] startingPointsOfBall;
        public GameObject[] particlePrefabs;
        public Camera cam; // 경기장 용 카메라
        public Sprite[] guidanceSprites;

#if USE_TAG
        public bool IsTaged
        {
            get
            {
                if (contentsController == null) return true;
                return contentsController.isTaged;
            }
        }
#endif

        public bool IsPlaying
        {
            get
            {
                if (!contentsController) return false;
                return contentsController.IsPlaying;
            }
        }

        public int Score
        {
            get { return contentsController.uiController.score.Score; }
            set { contentsController.uiController.score.Score = value; }
        }

        InteractionContents contentsController;
        Coroutine coroutine;
        
        int countKickit;
        int countGoalIn;
        int indexReactivatingBall = 0;

        void Awake()
        {
            contentsController = GetComponent<InteractionContents>();
            if (!contentsController) contentsController = this.gameObject.AddComponent<InteractionContents>();
            contentsController.SetCallback(EContentState.PLAY_STATE0, CallbackGuidance);
            contentsController.SetCallback(EContentState.PLAYING, CallbackCheckActiveInHierarchy);
            
            contentsController.SetCallback(EContentState.PLAYING, CallbackInitialize);
            contentsController.SetCallback(EContentState.CLOSE_EVENT0, CallbackStopCheck);
            contentsController.SetCallback(EContentState.CLOSE_EVENT0, CallbackSetScore);
        }

        void OnEnable()
        {
            indexReactivatingBall = 0;
        }

        public bool EqualUser(string _userName)
        {
            return string.Compare(contentsController.UserName, _userName) == 0;
        }

        public void CountKickIt()
        {
            ++countKickit;
        }

        public void CollisionWithWall(Collider _other)
        {
            if (_other.tag.Contains("Player"))
            {
                //_other.gameObject.SetActive(false);
                //EmittParticle(particlePrefabs[0], _other.transform.position);
                StartCoroutine(AdjustScaleProcess(_other.transform));
            }
        }

        public void GoalIn(Collider _other)
        {
            int _points = 0;
            switch (_other.name.Split('_')[0])
            {
                case "BaseBall":
                    _points = 10;
                    Score += _points;
                    break;
                case "SoccerBall":
                    _points = 16;
                    Score += _points;
                    break;
                case "VolleyBall":
                    _points = 14;
                    Score += _points;
                    break;
                case "TennisBall":
                    _points = 12;
                    Score += _points;
                    break;
                case "BasketBall":
                    _points = 18;
                    Score += _points;
                    break;
                default: return;
            }
            ++countGoalIn;
            EmitParticle(particlePrefabs[1], _other.transform.position);
            contentsController.uiController.uiPointOfScoreManager.DisplayScore(_other.transform.position, _points);
            DoCelebration();
            _other.gameObject.SetActive(false);
        }

#if USE_TAG
        public void SetUser(string _jsonData)
        {
            contentsController.SetUser(_jsonData);
        }

        public void SetUser(string _userName, int _seqNo)
        {
            contentsController.SetUser(_userName, _seqNo);
        }
#endif

        public void DoCelebration()
        {
            StopCoroutine("CelebrationProcess");
            wordsOfGoalinObjet.gameObject.SetActive(false);
            StartCoroutine("CelebrationProcess");
        }

        void CallbackInitialize()
        {
            countGoalIn = 0;
            countKickit = 0;
        }

        void EmitParticle(GameObject _prefab, Vector3 _postion)
        {
            if (_prefab != null)
            {
                GameObject _clone = Instantiate(_prefab) as GameObject;
                ParticleSystem _particle = _clone.GetComponent<ParticleSystem>();
                if (_particle)
                {
                    _clone.gameObject.SetActive(true);
                    _clone.transform.position = _postion;
                    Destroy(_clone, 3.0f);
                }
                else
                {
                    Destroy(_clone, 0.01f);
                }
            }
        }

        public void SetStartingPoint(GameObject _go)
        {
            _go.transform.localPosition = startingPointsOfBall[indexReactivatingBall].localPosition;

            indexReactivatingBall = (indexReactivatingBall + 1) % startingPointsOfBall.Length;
            _go.SetActive(true);
            Rigidbody _r = _go.GetComponentInChildren<Rigidbody>();
            _r.velocity = Vector3.zero;
        }

        void CallbackGuidance()
        {
            contentsController.guidanceSprites = guidanceSprites;
            contentsController.words = new string[]{
            "본 체험은 시간내에\r\n공을 차서",@"공을 차서",
            "골대에 넣으면\r\n점수를 얻는 게임입니다.",@"골대에 넣어요",
            "공을 발로 차주세요",@"찰 준비!!!",
        };
        }

        void CallbackCheckActiveInHierarchy()
        {
            coroutine = StartCoroutine(CheckActiveInHierarchyProcess());
        }

        void CallbackTag()
        {
            contentsController.uiController.tagOfUser.text = "";
        }

        void CallbackStopCheck()
        {
            if (null != coroutine)
                StopCoroutine(coroutine);
        }

        void CallbackSetScore()
        {
            contentsController.uiController.resultObject.labels[0].text = string.Format("총 {0}번\n공을 찼습니다", countKickit);
            contentsController.uiController.resultObject.labels[1].text = countGoalIn.ToString();
            contentsController.uiController.resultObject.labels[2].text = contentsController.uiController.score.Score.ToString();
        }

        IEnumerator CelebrationProcess()
        {
            wordsOfGoalinObjet.SetActive(true);
            for (int i = 0; i < fireworks.Length; i++)
                fireworks[i].gameObject.SetActive(false);

            for (int i = 0; i < fireworks.Length; ++i)
            {
                yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
                fireworks[i].gameObject.SetActive(true);
                fireworks[i].Play();
            }

            wordsOfGoalinObjet.gameObject.SetActive(false);
            for (int i = 0; i < fireworks.Length; i++)
                fireworks[i].gameObject.SetActive(false);
        }
        IEnumerator CheckActiveInHierarchyProcess()
        {
            GameObject _go = this.gameObject;
            Collider[] _colliders = new Collider[balls.Length];
            Plane[] _planes = GeometryUtility.CalculateFrustumPlanes(cam);


            for (int i = 0, len = balls.Length; i < len; ++i)
            {
                _colliders[i] = balls[i].GetComponent<Collider>();
            }

            while (_go.activeInHierarchy)
            {
                for (int i = 0, len = balls.Length; i < len; ++i)
                {
                    if (!balls[i].activeInHierarchy)
                    {
                        SetStartingPoint(balls[i]);
                    }/*
                else
                {
                    if(_colliders[i]!=null )
                    {
                        if (! GeometryUtility.TestPlanesAABB(_planes, _colliders[i].bounds ) )
                            SetStartingPoint(balls[i]);
                    }
                }*/
                }
                yield return new WaitForFixedUpdate();
            }
        }

        IEnumerator AdjustScaleProcess(Transform _other)
        {
            Vector3 _oriScale = _other.localScale;
            Vector3 _particlePos = _other.position;
            float _scale = 0f;
            do
            {
                _scale = _other.localScale.x;
                _scale -= Time.deltaTime;
                /*
                if (0 < _scale)
                {
                    _other.localScale = new Vector3(_scale, _scale, _scale);
                }else
                {
                    _scale = 0f;
                    EmitParticle(particlePrefabs[0], _particlePos);
                    _other.gameObject.SetActive(false);
                    _other.localScale = _oriScale;
                }
                 */
                _other.localScale = new Vector3(_scale, _scale, _scale);

                yield return null;
            } while (_scale > 0f);

            if (_scale < 0f)
            {
                EmitParticle(particlePrefabs[0], _particlePos);
                _other.gameObject.SetActive(false);
                _other.localScale = _oriScale;
            }
        }
    }
}