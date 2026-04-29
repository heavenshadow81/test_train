using UnityEngine;
using System.Collections;
using UserType;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public class Starfish : MonoBehaviour
    {
        public AnimationCurve appearAniCurve;
        public AnimationCurve affrightAniCurve;
        public AudioClip[] sndLaughArr;
        public AudioClip[] sndAppearArr;
        EState _state;
        public EState currentState
        {
            set
            {
                if (value != _state || value == EState.NONE)
                {
                    switch (value)
                    {
                        case EState.NONE:
                            scale = null;
                            cachedTransform.localScale = Vector3.zero;
                            currentState = EState.APPEAR;
                            break;
                        case EState.AFFRIGHT:
                            {
                                if (sndLaughArr != null)
                                {
                                    int index = Random.Range(0, sndLaughArr.Length);
                                    AudioSource.PlayClipAtPoint(sndLaughArr[index], Vector3.zero);
                                }
                                currentState = EState.DISAPPEAR;
                            }
                            break;
                        case EState.APPEAR:
                            {
                                if (sndAppearArr != null)
                                {
                                    int index = Random.Range(0, sndAppearArr.Length);
                                    AudioSource.PlayClipAtPoint(sndAppearArr[index], Vector3.zero);
                                }
                                scale = UITweener.Begin<TweenScale>(obj, 1.5f);
                                scale.animationCurve = appearAniCurve;
                                scale.from = Vector3.zero;
                                scale.to = Vector3.one;
                                scale.style = UITweener.Style.Once;
                                if (callBackFunc != null) scale.RemoveOnFinished(callBackFunc);

                                callBackFunc = new EventDelegate(() =>
                                {

                                    currentState = EState.IDLE;

                                });

                                scale.onFinished.Add(callBackFunc);
                            }
                            break;

                        case EState.DISAPPEAR:
                            {
                                scale = UITweener.Begin<TweenScale>(obj, 1f);
                                scale.from = cachedTransform.localScale;
                                scale.to = Vector3.zero;
                                scale.method = UITweener.Method.BounceOut;
                                scale.style = UITweener.Style.Once;
                                if (callBackFunc != null) scale.RemoveOnFinished(callBackFunc);
                                callBackFunc = new EventDelegate(() =>
                                {
                                    float x = Random.Range(UtilityScript.width * -0.35f, UtilityScript.width * 0.35f);
                                    float y = Random.Range(UtilityScript.height * -0.25f, UtilityScript.height * 0.1f);
                                    float z = cachedTransform.localPosition.z;
                                    cachedTransform.localPosition = new Vector3(x, y, z);
                                    currentState = EState.APPEAR;
                                });
                                scale.onFinished.Add(callBackFunc);
                            }
                            break;
                        case EState.IDLE:
                            {
                                float duration = 1.5f;
                                scale = UITweener.Begin<TweenScale>(obj, duration);
                                scale.to = new Vector3(0.8f, 0.8f, 0.8f);
                                scale.steeperCurves = true;
                                scale.animationCurve = null;
                                scale.style = UITweener.Style.PingPong;
                                scale.from = cachedTransform.localScale;
                                scale.method = UITweener.Method.Linear;
                                if (callBackFunc != null) scale.RemoveOnFinished(callBackFunc);

                                TweenRotation rotation = UITweener.Begin<TweenRotation>(obj, duration);
                                rotation.from = cachedTransform.localEulerAngles;
                                rotation.to = new Vector3(0, 0, cachedTransform.localEulerAngles.z + Random.Range(-20f, 20f));
                                rotation.style = UITweener.Style.PingPong;
                                rotation.method = UITweener.Method.Linear;
                            }
                            break;
                    }
                    _state = value;
                }
            }
        }

        Transform _cachedTransform;
        TweenScale scale;
        EventDelegate callBackFunc;
        public Transform cachedTransform
        {
            get
            {
                if (_cachedTransform == null)
                { _cachedTransform = this.transform; }
                return _cachedTransform;
            }
        }

        GameObject _obj;
        public GameObject obj
        {
            get
            {
                if (_obj == null)
                { _obj = this.gameObject; }
                return _obj;
            }
        }

        void OnEnable()
        {
            currentState = EState.NONE;


            /*
            for(int i = 0 ; i < aniCurve.keys.Length ; ++i)
            {
                Debug.Log(string.Format("index{0} : inTangent{1} , outTangent{2} ", i , aniCurve.keys[i].inTangent, aniCurve.keys[i].outTangent) );
            }*/
        }

        void DestroyUITweener()
        {
            UITweener tween = GetComponent<UITweener>();
            if (tween != null) Destroy(tween);
        }

        public void Touch()
        {
            currentState = EState.AFFRIGHT;
        }

    }
}