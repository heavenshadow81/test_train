//#define USE_TAG

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UserType
{
    public class Constante
    {
        public const int DEFAULT            = 0;
        public const int TEMPLATE3D         = 9;
        public const int NGUI               = 11;
        public const int TWODIMENSION_PANEL = 12;
        public const int INTERACTION_OBJECT = 14;
        public const int KICKBALL_COLLIDER = 16;
        public const int OTHERLAYER         = 19;
        public const int COLLIDER           = 20;
        public const int DONT_COLLIDE_WITH_SAMETHING = 31;

#if USE_TAG
        public const string RETURN_CODE_ZERO           = "RC0000";

        public const string LOOPBACK_IP = "127.0.0.1";
        public const string NUMBER_OF_PORT = "4444";

        public const string PARAM_FILE            = "file";
        public const string PARAM_CONTENT_TYPE    = "content_type";
        public const string PARAM_SHARE_OPTION    = "share_yn";
        public const string PARAM_NFC_ID          = "nfc_id";

        public const string KEY_CONTENT_TYPE      = "contenttype";
        public const string KEY_RESULT_SCORE      = "score";
        public const string KEY_CONTENTS_SEQ      = "contentseq";
        public const string KEY_USER_NAME         = "name";
        public const string KEY_USERLIST          = "userList";

        public const string SUBKEY_USER_NAME      = "userName";
        public const string SUBKEY_NFCID          = "nfcId";

        public const string VALUE_2D_DRAWING      = "c_0004";
        public const string VALUE_3D_DRAWING      = "c_0005";
        public const string VALUE_KICKBALL        = "c_0006";
        public const string VALUE_THROW_PAINTBALL = "c_0007";
        public const string VALUE_THE_FRISBEE     = "c_0011";   
        public const string VALUE_TOUCH_SLIME     = "c_0008";
        public const string VALUE_MOTION_JUMP     = "c_0009";

        public const string DO_TAGGING            = "매직밴드를 찍어주세요";
#endif
    }

    public class Tween
    {
        public static TweenScale SetTweenScale(AnimationCurve _aniCurve, GameObject _obj, Vector3 _to, float _duration)
        {
            TweenScale scale = TweenScale.Begin(_obj, _duration, _to);
            scale.animationCurve = _aniCurve;
            return scale;
        }

        public static void PlayTween<T>(ref T _scale, AnimationCurve _aniCurve, UITweener.Style _style, float _duration) where T : UITweener
        {
            _scale.enabled = true;
            _scale.tweenFactor = 0f;
            _scale.duration = _duration;
            _scale.animationCurve = _aniCurve;
            _scale.style = _style;
            _scale.Play(true);
        }
    }

    struct UserInputInfo
    {
        public ETarget targetType;
        public Vector2 oriPos;
        public Vector2 endPos;
        public Transform target; // adjust position, localScale, localRotation
        public Component component;
        public float value;

        public UserInputInfo(ETarget _t = ETarget.NONE)
        {
            value = 0;
            target = null;
            component = null;
            targetType = _t;
            oriPos = endPos = Vector2.zero;
        }

        public UserInputInfo(ETarget _t, Vector2 _coordi, Transform _target, Component _componet)
        {
            value = 0;
            targetType = _t;
            target = _target;
            component = _componet;
            oriPos = endPos = _coordi;
        }
    }

    public enum ETarget { NONE = 0, BALL = 1, OBSTRUCTION = 2 }
}

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    /// <summary>
    /// 모래 파티클 관리 및 제어 클래스
    /// </summary>
    public class SandParticleManager
    {
        /// <summary>
        /// 사용자 인덱스 bit flag
        /// </summary>
        long lActiveIndex;

        public SandParticleManager()
        {
            InitSandPool();
            lActiveIndex = 0;
        }

        ~SandParticleManager()
        {

        }

        public void Destroy()
        {
            if (_particleDic != null)
            { _particleDic.Destroy(); }

            _particleDic = null;
            _particlePrefab = null;
        }

        private CObjectDictionary<int, SandParticle> _particleDic;
        public CObjectDictionary<int, SandParticle> ParticleDic
        {
            get
            {
                if (_particleDic == null)
                {
                    InitSandPool();
                }
                return _particleDic;
            }
        }

        public GameObject _particlePrefab;
        public GameObject ParticlePrefab
        {
            get
            {
                if (_particlePrefab == null)
                    _particlePrefab = Resources.Load(TwoDimensionInteractionSandDrawPanel.szParticlePath + "Sand_Particle") as GameObject;

                if (_particlePrefab.GetComponent<SandParticle>() == null)
                    _particlePrefab.AddComponent<SandParticle>();
                return _particlePrefab;
            }
        }

        /// <summary>
        /// 메모리 풀 초기화
        /// </summary>
        void InitSandPool()
        {
            _particleDic = new CObjectDictionary<int, SandParticle>(
                (int _iKey) =>
                {
                    GameObject _prefab = GameObject.Instantiate(ParticlePrefab) as GameObject;
                    SandParticle _sand = _prefab.GetComponent<SandParticle>();
                    if (_sand == null)
                    {
                        _prefab.AddComponent<SandParticle>();
                        _sand = _prefab.GetComponent<SandParticle>();


                        _prefab.SetActive(false);
                    }

                    return _sand;

                }
                );
        }

        public void StopEmit(int iKey)
        {
            if (!CheckActivate(iKey)) return;

            if (CheckActivate(iKey))
                ParticleDic.getObject(iKey).StopEmit();

            ReleaseActivate(iKey);
        }

        public void PlayEmit(int iKey, Transform _parent, Vector3 _pos)
        {
            if (CheckActivate(iKey)) return;

            SetActivate(iKey);
            Debug.Log(ParticleDic.getObject(iKey));
            SandParticle _sand = ParticleDic.getObject(iKey) as SandParticle;

            if (_sand.cachedTransform.parent == null)
            { _sand.cachedTransform.parent = _parent; }

            _sand.PlayEmit();
            _sand.cachedTransform.localRotation = Quaternion.Euler(new Vector3(-26f, -166f, -1f));
            _sand.cachedTransform.localPosition = _pos;
        }

        /// <summary>
        /// 비트 연산으로 현재 모래 파티클 재생 여부 확인
        /// </summary>
        /// <param name="_iKey"></param>
        /// <returns> 해당 bit가 1이면 재생 중 0 이면 정지 중 </returns>
        bool CheckActivate(int _iKey)
        {
            return (lActiveIndex & (long)0x01 << _iKey) != 0;
        }

        void SetActivate(int _iKey)
        {
            lActiveIndex |= ((long)0x01 << _iKey);
        }

        void ReleaseActivate(int _iKey)
        {
            lActiveIndex &= ~((long)0x01 << _iKey);
        }
    }

    public enum EEventType
    {
        NONE, ALARM, ACTIVATE, INTERACTION
    }

    public interface IEvent
    {
        /// <summary>
        /// 콘텐츠 체험 중 인터렉션에 대한 재정의 함수
        /// </summary>
        /// <returns>return true : 인터렉션 종료, return false : 인터렉션 중</returns>
        bool StateInPlay();
        /// <summary>
        /// 콘텐츠 내 이벤트 발생시 시작 함수
        /// </summary>
        /// <returns>return true : 이벤트 시작 준비 완료 종료, return false : 이벤트 시작 하기 위한 준비 하는 중</returns>
        bool StateEventReady();
        /// <summary>
        /// 콘텐츠 내 이벤트 중 일때 계속 호출되는 함수
        /// </summary>
        /// <returns>return true : 이벤트 종료 , return false : 이벤트 진행 중</returns>
        bool StateEventActivates();
    }
}

namespace com.Loxwell.File
{

    public class FileName
    {
        public const string HandStamp = "HandImage";
        public const string AppleStamp = "imgAppleStamp";
        public const string MelonStamp = "imgMelonStamp";
        public const string OrangeStamp = "imgOrangeStamp";
        public const string StrawberryStamp = "imgStrawberryStamp";
        public const string TomatoStamp = "imgTomatoStamp";
        public const string WaterMelonStamp = "imgWatermelonStamp";
        public const string FootStamp = "FootImage";
        public const string RightFootStamp = "RightFootImage";
        public const string SandBrush = "sand64";
    }
}