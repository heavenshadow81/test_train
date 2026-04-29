using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Windows.Kinect;

namespace ML.MLBKids
{
    using GameMode = Stadium.GameMode;
    
    public enum KinectCondition
    {
        None,
        StandBack,
        ComeUpClose
    }

    public class TutorialPage : Page
    {
        public enum State
        {
            Welcome,
            Tutorial,
            Ready
        }

        public Text messageText;
        public Text timerText;
        public Image image;
        public Button startButton;

        public Sprite welcomeSprite;
        public string standText;
        public string comeUpCloseText;
        public string standBackText;
        public string rightPositionText;
        public string standNearScreenText;
        public string gameWillBeStartedText;

        public Sprite[] pitchGameSprites;
        public string[] pitchGameTexts;
        public Sprite[] hitGameSprites;
        public string[] hitGameTexts;

        public GameMode gameMode { get; private set; }
        public KinectCondition condition { get; private set; }
        public bool showsTutorial { get; private set; }
        public State state { get; private set; }

        public System.Action<bool> onTutorialComplete { get; private set; }

        private bool _conditionSatisfied = false;
        private float _timer;
        private Coroutine _tutorialCoroutine;
        private Tween _imageTween;

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
            Cleanup();
            Stadium.instance.SetBlur(false, 0.5f);
        }

        public void Set(GameMode newMode, KinectCondition newCondition, bool newShowsTutorial, System.Action<bool> tutorialCompleteHandler)
        {
            gameMode = newMode;
            condition = newCondition;
            showsTutorial = newShowsTutorial;
            onTutorialComplete = tutorialCompleteHandler;

            state = State.Welcome;
            image.sprite = welcomeSprite;
            messageText.text = Localization.Get(standText);

            image.gameObject.SetActive(true);
            messageText.gameObject.SetActive(true);
            timerText.gameObject.SetActive(true);
            startButton.gameObject.SetActive(false);

            image.rectTransform.localScale = Vector3.one;
            _timer = float.MaxValue;
            _conditionSatisfied = false;

            Stadium.instance.SetBlur(true, showsTutorial ? 0 : 0.5f);
        }

        public void LateUpdate()
        {
            switch (state)
            {
                case State.Welcome:
                    if (condition == KinectCondition.StandBack)
                    {
                        //Body trackedBody = KinectHelper.instance.trackedBody;
                        //if (trackedBody != null)
                        //{
                        //    var spine = trackedBody.Joints[JointType.SpineBase];
                        //    var depthZ = spine.Position.Z;
                        //    if (depthZ < 2.0f)
                        //    {
                        //        messageText.text = Localization.Get(standBackText);
                        //        if (_timer >= float.MaxValue || _conditionSatisfied)
                        //            _timer = GlobalConstants.instance.tutorialStandWaitTime;
                        //        _conditionSatisfied = false;
                        //    }
                        //    else if (depthZ > 3.0f)
                        //    {
                        //        messageText.text = Localization.Get(comeUpCloseText);
                        //        if (_timer >= float.MaxValue || _conditionSatisfied)
                        //            _timer = GlobalConstants.instance.tutorialStandWaitTime;
                        //        _conditionSatisfied = false;
                        //    }
                        //    else
                        //    {
                        //        messageText.text = Localization.Get(rightPositionText);
                        //        if (_timer >= GlobalConstants.instance.tutorialReadyTime)
                        //        {
                        //            _timer = GlobalConstants.instance.tutorialReadyTime;
                        //            if (!showsTutorial)
                        //                _timer = 0;
                        //        }
                        //        _conditionSatisfied = true;
                        //    }
                        //}
                        //else
                        //{
                        //    messageText.text = Localization.Get(standText);
                        //    if (_timer >= float.MaxValue || _conditionSatisfied)
                        //        _timer = GlobalConstants.instance.tutorialStandWaitTime;
                        //    _conditionSatisfied = false;
                        //}

                        if (_timer > 0.0f && _timer < float.MaxValue)
                            _timer -= Time.deltaTime;
                    }
                    else if (condition == KinectCondition.ComeUpClose)
                    {
                        if (_timer >= float.MaxValue)
                            _timer = GlobalConstants.instance.tutorialReadyTime;
                        messageText.text = Localization.Get(standNearScreenText);
                        timerText.text = Mathf.CeilToInt(_timer).ToString();
                        _conditionSatisfied = true;
                        _timer -= Time.deltaTime;
                    }

                    if (_timer <= 0.0f)
                    {
                        if (_conditionSatisfied)
                        {
                            if (showsTutorial)
                                ShowTutorial();
                            else
                                End(true);
                        }
                        else
                            End(false);
                    }
                    break;
                case State.Ready:
                    {
                        if (_timer > 0.0f)
                        {
                            _timer -= Time.deltaTime;
                            if (_timer <= 0.0f)
                            {
                                End(true);
                            }
                        }
                    }
                    break;
                case State.Tutorial:
                    break;
            }

            if (timerText.gameObject.activeSelf)
            {
                if (_timer >= float.MaxValue)
                    timerText.text = "";
                else
                    timerText.text = Mathf.CeilToInt(_timer).ToString();
            }

#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                switch(state)
                {
                    case State.Welcome:
                        {
                            if (showsTutorial)
                                ShowTutorial();
                            else
                                End(true);
                        }
                        break;
                    case State.Tutorial:
                        {
                            Ready();
                        }
                        break;
                    case State.Ready:
                        {
                            End(true);
                        }
                        break;
                }
            }
#endif
        }

        private IEnumerator _ShowTutorial(Sprite[] sprites, string[] texts)
        {
            image.gameObject.SetActive(true);
            messageText.gameObject.SetActive(true);
            timerText.gameObject.SetActive(false);
            startButton.gameObject.SetActive(false);

            if (sprites != null && texts != null)
            {
                for (int i = 0; i < Mathf.Max(sprites.Length, texts.Length); i++)
                {
                    Sprite spr = i < sprites.Length ? sprites[i] : welcomeSprite;
                    string txt = i < texts.Length ? texts[i] : "";

                    if (i > 0)
                    {
                        if (_imageTween != null) { _imageTween.Kill(); _imageTween = null; }
                        _imageTween = image.rectTransform.DOScaleX(0.0f, 0.25f).SetEase(Ease.Linear);
                        yield return new WaitForSeconds(0.25f);
                    }

                    Vector3 scale = image.rectTransform.localScale;
                    scale.x = 0;
                    image.rectTransform.localScale = scale;
                    image.sprite = spr;
                    messageText.text = Localization.Get(txt);

                    if (_imageTween != null) { _imageTween.Kill(); _imageTween = null; }
                    _imageTween = image.rectTransform.DOScaleX(1.0f, 0.25f).SetEase(Ease.Linear);

                    yield return new WaitForSeconds(1.75f);
                }
            }

            _imageTween = image.rectTransform.DOScaleX(0.0f, 0.25f).SetEase(Ease.Linear);
            yield return new WaitForSeconds(0.25f);
            messageText.text = "";
            yield return new WaitForSeconds(0.25f);

            _imageTween = null;
            _tutorialCoroutine = null;

            Ready();
        }

        public void ShowTutorial()
        {
            if (_tutorialCoroutine == null)
            {
                if (gameMode == GameMode.Hit)
                    _tutorialCoroutine = StartCoroutine(_ShowTutorial(hitGameSprites, hitGameTexts));
                else if (gameMode == GameMode.Pitch)
                    _tutorialCoroutine = StartCoroutine(_ShowTutorial(pitchGameSprites, pitchGameTexts));
            }
            if (_tutorialCoroutine == null)
                state = State.Ready;
            else
                state = State.Tutorial;
        }

        public void Ready()
        {
            Cleanup();
            state = State.Ready;

            image.gameObject.SetActive(false);
            messageText.gameObject.SetActive(true);
            timerText.gameObject.SetActive(true);
            startButton.gameObject.SetActive(true);

            messageText.text = Localization.Get(gameWillBeStartedText);

            _timer = GlobalConstants.instance.tutorialStartButtonWaitTime;
            timerText.text = Mathf.FloorToInt(_timer).ToString();

            RectTransform sbrtf = startButton.GetComponent<RectTransform>();
            sbrtf.localScale = Vector3.zero;
            startButton.GetComponent<RectTransform>().DOScale(1.0f, 0.5f).SetEase(Ease.OutBack);
        }

        public void End(bool success)
        {
            Hide();

            if (onTutorialComplete != null)
                onTutorialComplete(success);
        }

        public void Cleanup()
        {
            if (_imageTween != null)
            {
                _imageTween.Kill();
                _imageTween = null;
            }
            if (_tutorialCoroutine != null)
            {
                StopCoroutine(_tutorialCoroutine);
                _tutorialCoroutine = null;
            }
        }
    }
}