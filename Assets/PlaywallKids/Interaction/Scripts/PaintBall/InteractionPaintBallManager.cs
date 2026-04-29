//#define USE_TAG

using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

namespace ML.PlaywallKids.Interaction
{
    using Common;

#if USE_TAG
    public class InteractionPaintBallManager : InteractionBaseClass
#else
public class InteractionPaintBallManager : MonoBehaviour
#endif
    {
        private const int BALL_COUNT = 30;
        private const int EFFECT_COUNT = 256;
        private const int EFFECT_ALLOWANCE = 30;

        public Camera targetCamera;

        public GameObject goBall;
        public GameObject goPaintWater;
        public GameObject goPaintEffect;

        public GameObject goBG_2x3;
        public GameObject goBG_2x6;

        public AudioSource sndCollision;

        public Sprite[] guidanceSprites;

        private List<InteractionPaintBall> listBall;
        private List<ParticleSystem> listPaintWater;
        private List<InteractionPaintEffect> listPaintEffect;

        private InteractionContents contentsController;
        private InteractionPaintBallWall[] arrayWall;

        private List<InteractionPaintEffect> listDeletePaintEffect;

        private Vector3 vecLeft, vecRight;
        private int cntThrow;
        private int cntGoal;

        int _num;
#if USE_TAG
        public override int numberOfPlayers
#else
    public int numberOfPlayers
#endif
        {
            get
            {

                return _num;
            }
            set
            {
                if (!contentsController) contentsController = GetComponent<InteractionContents>();
                if (value <= 0)
                    _num = 1;
                else
                    _num = value;

                contentsController.numberOfPlayers = _num;
                contentsController.indexOfNumber = 0;
            }
        }

        public bool canPlay
        {
            get
            {
                return contentsController.IsPlaying;
            }
        }

        void Awake()
        {
            if (contentsController == null) contentsController = this.GetComponent<InteractionContents>();
            contentsController.SetCallback(InteractionContentsEnum.EState.NONE, Initialize);
            contentsController.SetCallback(InteractionContentsEnum.EState.CLOSE_EVENT0, SetScore);
            contentsController.SetCallback(InteractionContentsEnum.EState.PLAY_STATE0, CallbackGuidance);
        }

        void OnDisable()
        {
            contentsController.uiController.score.Active = false;
        }

        // Use this for initialization
        void Start()
        {

            bool is2x6 = ScreenUtil.screenType == ScreenType.Bigboard2x6;
            goBG_2x3.SetActive(!is2x6);
            goBG_2x6.SetActive(is2x6);

            arrayWall = (is2x6 ? goBG_2x6 : goBG_2x3).transform.GetComponentsInChildren<InteractionPaintBallWall>();
            for (int i = 0; i < arrayWall.Length; i++)
                switch (arrayWall[i].type)
                {
                    case InteractionPaintBallWall.WallType.FRONT:
                        vecLeft += new Vector3(0, arrayWall[i].transform.position.y, arrayWall[i].transform.position.z);
                        vecRight += new Vector3(0, arrayWall[i].transform.position.y, arrayWall[i].transform.position.z);
                        break;

                    case InteractionPaintBallWall.WallType.LEFT:
                        vecLeft += new Vector3(arrayWall[i].transform.position.x, 0, 0);
                        break;

                    case InteractionPaintBallWall.WallType.RIGHT:
                        vecRight += new Vector3(arrayWall[i].transform.position.x, 0, 0);
                        break;
                }

            listBall = new List<InteractionPaintBall> { goBall.GetComponent<InteractionPaintBall>() };
            goBall.GetComponent<InteractionPaintBall>().UnUsed();
            for (int i = 1; i < BALL_COUNT; i++)
            {
                InteractionPaintBall ball = _InstantiateAndAddtiveObject<InteractionPaintBall>(listBall, goBall);
                ball.UnUsed();
            }

            listPaintWater = new List<ParticleSystem> { goPaintWater.GetComponent<ParticleSystem>() };
            for (int i = 1; i < BALL_COUNT; i++)
                _InstantiateAndAddtiveObject<ParticleSystem>(listPaintWater, goPaintWater);

            listPaintEffect = new List<InteractionPaintEffect> { goPaintEffect.GetComponent<InteractionPaintEffect>() };
            for (int i = 1; i < EFFECT_COUNT; i++)
            {
                InteractionPaintEffect effect = _InstantiateAndAddtiveObject<InteractionPaintEffect>(listPaintEffect, goPaintEffect);
                effect.UnUsed();
            }

            listDeletePaintEffect = new List<InteractionPaintEffect>();
            for (int i = 0; i < EFFECT_ALLOWANCE; i++)
            {
                listDeletePaintEffect.Add(listPaintEffect[i]);
                listPaintEffect[i].UnUsed();
                listPaintEffect[i].SetUsed(true);
            }

        }



        private void Initialize()
        {
            cntGoal = cntThrow = 0;

            for (int i = 0, len = listPaintEffect.Count; i < len; i++)
            {
                listPaintEffect[i].UnUsed();
            }
        }

        private void CallbackGuidance()
        {
            contentsController.guidanceSprites = guidanceSprites;
            contentsController.words = new string[]{
            "본 체험은\r\n시간내에 물감을", // @"물감을 던져요",
            "던져서 점수를\r\n얻는 게임입니다.",//@"과녁을 향해",
            "화면상의 벽에\r\n물감을 던져보세요."//@"벽을 향해 던져요!",
        };
        }

        private void SetScore()
        {
            int _score = contentsController.uiController.score.Score;

            contentsController.uiController.resultObject.texts = new string[]{
            string.Format("총 {0}번\n던졌습니다", cntThrow),
            cntGoal.ToString(),
            _score.ToString()
        };
#if USE_TAG
            NFCUserInfo.SendData(_score, contentsController.sequneceNumber);
#endif
        }

#if USE_TAG
        public override void SetStringValue(string _jsonStr)
        {
            contentsController.SetUser(_jsonStr);
        }
#endif

        public void Throw(Camera _cam, int i, float x, float y, Vector3 navigate, float power)
        {
            Vector3 position = _cam.ViewportToWorldPoint(new Vector3(x, y, 0) + _cam.transform.forward);
            _GetUnusedPaintBall().Shoot(ParticleActive, position, new Vector3(x, y), vecLeft, vecRight, navigate, power);
        }

        public void Throw(Camera _cam, int i, float x, float y)
        {
            ++cntThrow;
            Vector3 position = _cam.ViewportToWorldPoint(new Vector3(x, y, 0) + _cam.transform.forward);
            _GetUnusedPaintBall().Shoot(ParticleActive, position, new Vector3(x, y), vecLeft, vecRight);
        }

        public void ColliderWithWalls(Collider _other, int _point)
        {
            cntGoal++;
            Vector2 _viewPort = targetCamera.WorldToViewportPoint(_other.transform.position);
            Vector2 _position = ScreenUtil.ViewportToNGUIScreen(_viewPort);

            contentsController.uiController.uiPointOfScoreManager.DisplayScore(_position, _point);
            contentsController.uiController.score.Score += _point;
        }

        // Update is called once per frame
        //	void Update () {
        //	}

        int effectLayerCounter = 0;
        void ParticleActive(InteractionPaintBall paintBall)
        {
            ParticleSystem particle = _GetUnusedPaintWater();
            particle.transform.position = paintBall.transform.position;
            var particleMain = particle.main;
            particleMain.startColor = paintBall.GetColor();
            particle.GetComponent<Renderer>().sortingOrder = effectLayerCounter + 2;
            particle.Play();

            if (sndCollision != null)
                sndCollision.PlayOneShot(sndCollision.clip);
            //StartCoroutine(ParticlePlay(particle));

            InteractionPaintEffect effect = _GetUnusedPaintEffect();
            effect.Setting(paintBall, arrayWall, effectLayerCounter++);
        }

        IEnumerator ParticlePlay(ParticleSystem play)
        {
            yield return new WaitForFixedUpdate();
            play.Play();
        }

        InteractionPaintBall _GetUnusedPaintBall()
        {
            for (int i = 0; i < listBall.Count; i++)
                if (listBall[i].IsUseable())
                    return listBall[i];

            //  Debug.Log("Empty PaintBall");
            return _InstantiateAndAddtiveObject<InteractionPaintBall>(listBall, goBall);
        }

        ParticleSystem _GetUnusedPaintWater()
        {
            for (int i = 0; i < listPaintWater.Count; i++)
                if (listPaintWater[i].IsAlive() == false)
                    return listPaintWater[i];

            //   Debug.Log("Empty PaintWater");
            return _InstantiateAndAddtiveObject<ParticleSystem>(listPaintWater, goPaintWater);
        }

        InteractionPaintEffect _GetUnusedPaintEffect()
        {
            InteractionPaintEffect effect = null;
            for (int i = 0; i < listPaintEffect.Count; i++)
                if (listPaintEffect[i].IsUseable())
                {
                    effect = listPaintEffect[i];
                    break;
                }

            if (effect == null)
            {
                if (listDeletePaintEffect.Count > EFFECT_ALLOWANCE)
                {
                    effect = listDeletePaintEffect[0];
                    listDeletePaintEffect[EFFECT_ALLOWANCE].FadeOut();

                    listDeletePaintEffect.RemoveAt(0);
                }
                else
                {
                    //       Debug.Log("Empty PaintEffect => Something Wrong");
                    effect = _InstantiateAndAddtiveObject<InteractionPaintEffect>(listPaintEffect, goPaintEffect);
                }
            }

            listDeletePaintEffect.Add(effect);
            return effect;
        }

        T _InstantiateAndAddtiveObject<T>(List<T> list, GameObject go) where T : Component
        {
            GameObject obj = Instantiate(go) as GameObject;
            obj.transform.parent = go.transform.parent;

            T result = (T)obj.GetComponent(typeof(T));
            if (list != null)
                list.Add(result);

            return result;
        }
    }
}