//#define USE_TAG

using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.MotionJump
{
    /// <summary>
    /// 사용자 인식 확인 및 튜토리얼 제어 클래스
    /// </summary>
    public class UIConnectToUserController : MonoBehaviour
    {
#if USE_TAG
    public UIAtlas[] atlases;
    public UILabel[] words;
#endif

        public UI2DSprite imgGuideInfo;
        public UISprite imgNumeric;
        public Transform originPos;
        public Transform centerPos;
        public Transform targetPos;
        public string numericFileName;
        public Sprite[] guidanceSprites;
        public Sprite tagSprite, readySprite, startSprite;

        [HideInInspector]
        public int counter;

        /// <summary>
        /// Texture 이름
        /// </summary>
        string[] infoFileImgName = new string[] { "imgGuidePleaseStepOn", "imgGuideShakeArms", "imgGuideLeanUpper", "imgGuideDropCaution" };

#if USE_TAG
        public int indexOfTagging
        {
            set
            {
                if (value == 0)
                    imgGuideInfo.sprite2D = tagSprite;
                else
                    imgGuideInfo.sprite2D = readySprite;
            }
        }
#endif

        /// <summary>
        /// 키넥트 사용자 인식 , JumpGameControllers.SendCommand() 에서 할당함
        /// </summary>
        public bool findUser
        {
            get;
            set;
        }
        /// <summary>
        /// 튜토리얼 시작 전 사용자 추적 유지 식별 (true : 일정시간 추적 완료) : (false : 추적 실패)
        /// </summary>
        public bool findCheckComplete
        {
            get;
            private set;
        }

        /// <summary>
        /// 튜토리얼 완료 될때 까지 사용자 추적이 완료
        /// </summary>
        public bool maintainConnect
        {
            get;
            private set;
        }

        public void Initialize()
        {
            imgNumeric.cachedGameObject.SetActive(false);
            findUser = false;
            maintainConnect = false;
            findCheckComplete = false;

            if (!imgGuideInfo.cachedGameObject.activeInHierarchy)
                imgGuideInfo.cachedGameObject.SetActive(true);
            if (imgNumeric.cachedGameObject.activeInHierarchy)
                imgNumeric.cachedGameObject.SetActive(false);

#if USE_TAG
            indexOfTagging = 0;
            for (int i = 0; i < words.Length; ++i)
            {
                words[i].alpha = 1f;
            }
#else
            imgGuideInfo.sprite2D = guidanceSprites[0];
#endif
            imgGuideInfo.width = 700;
            imgGuideInfo.height = 700;
            findCheckComplete = false;
            imgGuideInfo.cachedTransform.localPosition = centerPos.localPosition;
        }

#if UNITY_EDITOR
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (findUser && !maintainConnect)
                {
                    if (counter > 0)
                        counter--;
                }
                else
                { counter = 5; }
                findUser = true;
            }
        }
#endif

        /// <summary>
        /// 키넥트에서 사용자 추적 성공시 안내 이미지 시계방향으로 비활성화 단계
        /// </summary>
        /// <returns></returns>
        IEnumerator CheckUserProcess()
        {
            float _findTime = 0;
            float _waitTime = 3f;

#if USE_TAG
        for (int i = 0; i < words.Length; ++i)
            words[i].alpha = 1f;
#endif
            do
            {
                if (findUser)
                { _findTime += Time.deltaTime; }
                else
                {
#if USE_TAG
                for (int i = 0; i < words.Length; ++i)
                    words[i].alpha = 1f;
#endif
                    _findTime = 0;
                }
                float _volume = (_waitTime - _findTime) / _waitTime;
                if (_volume < 0f) _volume = 0f;
                if (_volume != imgGuideInfo.fillAmount)
                    imgGuideInfo.fillAmount = _volume;

#if USE_TAG
            for (int i = 0; i < words.Length; ++i)
                words[i].alpha = _volume;
#endif
                yield return new WaitForEndOfFrame();
            } while (!(_findTime > _waitTime));
            imgGuideInfo.cachedTransform.localPosition = targetPos.localPosition;
            yield return new WaitForSeconds(0.15f);
            imgGuideInfo.fillAmount = 1f;
            findCheckComplete = true;
            #region
            /*
            //TweenTransform tween = TweenTransform.Begin(imgGuideInfo.cachedGameObject, 0.5f, targetPos);
            bool bNext = false;
            TweenTransform tween = DoTweenTransform(imgGuideInfo, null, targetPos, 0.1f, infoFileImgName[0]);
            tween.onFinished.Add( new EventDelegate( ()=>
                {
                    bNext = true;
                }
                ) );
            do
            {

              yield return new WaitForEndOfFrame();
            } while (!bNext);
             */
            #endregion
        }

        /// <summary>
        /// 사용자 인식 트리거 함수
        /// </summary>
        public void FindUser()
        {
            findCheckComplete = false;
            StartCoroutine(CheckUserProcess());
        }

        /// <summary>
        /// 사용자 튜토리얼 트리거 함수
        /// </summary>
        public void Notice()
        { StartCoroutine(NoticeProcess()); }

#if USE_TAG
    public void SetStringWords(int _index, string _words)
    {
        words[_index].text = _words;
    }
#endif
        /// <summary>
        /// 사용자 튜토리얼 안내
        /// </summary>
        /// <returns></returns>
        IEnumerator NoticeProcess()
        {
            UI2DSprite tempImg = null;
            int bitCheck = 0;
            int imgCnt = 1;
            float cntDown = 0;
            bool bCheck = false;
            counter = 6;

            imgNumeric.cachedGameObject.SetActive(false);
            do
            {
                cntDown += Time.deltaTime;
                counter -= (int)cntDown;
                if ((int)cntDown > 0) cntDown = 0;
                //if user stands ahead of kinect for 5seconds

                if (counter < 6 && counter > 0)
                {
                    //정수 단위 체크
                    if ((bitCheck & 0x01 << counter) == 0)
                    {
                        if (counter % 2 == 1)
                        {
                            //다음 이미지 재생
                            tempImg = NGUITools.AddChild<UI2DSprite>(gameObject, imgGuideInfo.cachedGameObject);
                            TweenTransform tempTween = DoTweenTransform(tempImg, null, targetPos, 0.1f, null);
                            tempTween.onFinished.Add(new EventDelegate(() => { Destroy(tempImg.gameObject); Destroy(tempTween.gameObject); }));

                            DoTweenTransform(imgGuideInfo, originPos, centerPos, 0.1f, guidanceSprites[imgCnt++]);
                        }

                        if (!imgNumeric.cachedGameObject.activeInHierarchy)
                        { imgNumeric.cachedGameObject.SetActive(true); }
                        imgNumeric.spriteName = numericFileName + counter.ToString();
                        bitCheck |= 0x01 << counter;
                    }
                    bCheck = (counter == 1);
                }

                if (bCheck) yield return new WaitForSeconds(1f);
                yield return new WaitForEndOfFrame();
            } while (!bCheck);

            DoTweenTransform(imgGuideInfo, originPos, centerPos, 0.1f, startSprite);

            imgNumeric.cachedGameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            TweenTransform tween = DoTweenTransform(imgGuideInfo, null, targetPos, 0.1f, null);
            //값 초기화
            findCheckComplete = false;
            //초기화
            findUser = false;
            //튜토리얼 종료 성공
            maintainConnect = true;
        }

        TweenTransform DoTweenTransform(UI2DSprite _img, Transform _origin, Transform _target, float _duration, Sprite _sprite)
        {
            TweenTransform tween = TweenTransform.Begin(_img.cachedGameObject, _duration, _origin, _target);

            if (!_img.cachedGameObject.activeInHierarchy)
                _img.cachedGameObject.SetActive(true);
            _img.sprite2D = _sprite;
            _img.MakePixelPerfect();
            return tween;
        }
    }
}