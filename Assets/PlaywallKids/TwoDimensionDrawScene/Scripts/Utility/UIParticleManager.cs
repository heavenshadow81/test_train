using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UserType;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    /// <summary>
    /// 파티클 재생 및 파티클 참조 관리 클래스
    /// </summary>
    public class UIParticleManager : MonoBehaviour
    {

        struct loopInfo
        {
            /// <summary>
            /// ParticleSystem[] 배열 index
            /// </summary>
            public int arrayIndex;
            /// <summary>
            /// 생성 시 자동 재생 
            /// </summary>
            public bool bActive;

            public loopInfo(int _id = -1, bool _active = false)
            {
                arrayIndex = _id;
                bActive = _active;
            }
        }

        public Transform CachedTransform { get; private set; }

        /// <summary>
        /// 파티클 프리팹
        /// </summary>
        public ParticleSystem[] particles;
        /// <summary>
        /// true : disable, false = GameObject.Destroy()
        /// </summary>
        public bool dontDestroy;
        public bool switchOn;

        /// <summary>
        /// 메모리 풀 클래스
        /// </summary>
        CObjectListToDic<int, ParticleSystem> ParticleDic
        {
            get
            {
                if (_particleMemoryPool == null)
                {
                    _particleMemoryPool = new CObjectListToDic<int, ParticleSystem>(
                (int _index) =>
                {
                    if (particles.Length == 0) return null;
                    if (particles.Length <= _index) _index = 0;
                    if (particles[_index] == null) return null;
                    ParticleSystem _ps = Instantiate(particles[_index]) as ParticleSystem;

                    _ps.gameObject.SetActive(false);
                    return _ps;
                },
                (ParticleSystem ps) =>
                { return ps != null && !ps.gameObject.activeInHierarchy; }
                );
                }
                return _particleMemoryPool;
            }
        }
        /// <summary>
        /// ParticleDic property 용 멤버 변수
        /// </summary>
        CObjectListToDic<int, ParticleSystem> _particleMemoryPool;
        /// <summary>
        /// 파티클 ID별 무한 재생 되는 파티클 콜렉션
        /// </summary>
        Dictionary<int, GameObject> _loopParticle;
        /// <summary>
        /// 사용자 입력 별 무한 재생 되는 파티클  콜렉션
        /// </summary>
        Dictionary<int, loopInfo> _loopParticlePerTouchID;

        #region
        /*
        void Awake()
        {
            for(int i = 0 ; i< particles.Length ; ++i)
            {
                Renderer[] renders = particles[i].gameObject.GetComponentsInChildren<Renderer>();
                foreach (var value in renders)
                {
                    if (value != null)
                    {
                        Material[] m = value.materials;
                        for (int j = 0; j < m.Length; ++j)
                        {
                            m[j].renderQueue = 5000;
                        }
                    }
                }
            }

        }*/
        #endregion

        public static UIParticleManager instance;

        void Awake()
        {
            CachedTransform = this.transform;
            instance = this;
        }

        /// <summary>
        /// 각죵 콜렉션 및 프리팹 설정 초기화
        /// </summary>
        void OnEnable()
        {
            _loopParticle = new Dictionary<int, GameObject>();
            _loopParticlePerTouchID = new Dictionary<int, loopInfo>();
            for (int i = 0, len = particles.Length; i < len; ++i)
            {
                if (particles[i] == null) continue;

                CFX_AutoDestructShuriken cfx = particles[i].gameObject.GetComponent<CFX_AutoDestructShuriken>();
                if (cfx != null) cfx.OnlyDeactivate = dontDestroy;
            }
        }

        void OnDisable()
        {
            if (_particleMemoryPool != null)
            {
                Dictionary<int, List<ParticleSystem>> dic = _particleMemoryPool.dictionary;
                foreach (KeyValuePair<int, List<ParticleSystem>> kv in dic)
                {
                    List<ParticleSystem> list = kv.Value;
                    for (int i = 0, len = list.Count; i < len; ++i)
                    {
                        if (list[i] == null) continue;
                        GameObject.Destroy(list[i].gameObject);
                    }
                    list.Clear();
                    list = null;
                }

                _particleMemoryPool.Destroy();
            }

            if (_loopParticle != null)
            {
                foreach (KeyValuePair<int, GameObject> kv in _loopParticle)
                {
                    GameObject.Destroy(kv.Value);
                }
            }

            _loopParticle = null;
            _particleMemoryPool = null;
        }

        /// <summary>
        /// 사용자 ID 별 파티클 재생 정지
        /// 콜렉션들에서 제외 된 객체의 참조는 _particleMemoryPool 에서 유지 하고 있음
        /// </summary>
        /// <param name="_touchID"></param>
        /// <returns></returns>
        public bool Disable(int _touchID)
        {
            if (!_loopParticle.ContainsKey(_touchID)) return false;

            if (_loopParticlePerTouchID.ContainsKey(_touchID))
            {
                _loopParticlePerTouchID.Remove(_touchID);
            }

            _loopParticle[_touchID].transform.parent = null;
            _loopParticle[_touchID].transform.localPosition = Vector3.zero;
            _loopParticle[_touchID].SetActive(false);
            _loopParticle.Remove(_touchID);
            return true;
        }

        /// <summary>
        /// 파티클 단발성 재생
        /// </summary>
        /// <param name="_parent">부모 객체</param>
        /// <param name="_pos">재생 될 좌표</param>
        /// <param name="_index">ParticleSystem[] 배열 index</param>
        /// <returns></returns>
        public ParticleSystem OneShotEmitt(Transform _parent, Vector3 _pos, int _index)
        {
            ParticleSystem ps = ParticleDic.getObject(_index);

            if (_parent != null)
            {
                ps.transform.parent = _parent;
                ps.gameObject.layer = _parent.gameObject.layer;
            }
            else
            {
                ps.gameObject.layer = Constante.DEFAULT;
            }
            ps.gameObject.SetActive(true);
            ps.transform.localPosition = _pos;
            ps.Play();

            return ps;
        }

        /// <summary>
        /// 파티클 재생
        /// </summary>
        /// <param name="_parent">부모 객체</param>
        /// <param name="pos">재생 좌표</param>
        /// <param name="_bDisable">재생이 종료 시 동적 삭제 또는 객체 상태 비활성 옵션</param>
        /// <param name="_loop">무한 재생 옵션</param>
        /// <param name="_touchID">사용자 터치 ID</param>
        /// <param name="_index">ParticleSystem[] 배열 index</param>
        public void Emitt(Transform _parent, Vector3 pos, bool _bDisable = false, bool _loop = false, int _touchID = 0, int _index = -1)
        {
            if (particles == null || particles.Length == 0 || !switchOn) return;

            //index가 배열 범위에 벗어 날 경우 임의의 파티클 재생
            if (_index < 0 || _index >= particles.Length) _index = Random.Range(0, particles.Length);

            if (_loop)
            {
                if (!_loopParticlePerTouchID.ContainsKey(_touchID))  //새로운 터치정보
                {
                    //_loopParticlePerTouchID 콜렉션에 추가
                    _loopParticlePerTouchID.Add(_touchID, new loopInfo(_index, true));
                }
                else // 기존에 입력 된 터치 정보 
                {
                    loopInfo info = _loopParticlePerTouchID[_touchID];
                    if (info.arrayIndex == _index && info.bActive) // 재생 할 파티클의 종류 확인
                    {
                        // 현재 재생 할 파티클 과  이미 재생 중인 파티클이 동일한 경우  새로운 좌표로 위치 이동
                        _loopParticle[_touchID].transform.localPosition = pos;
                        return;
                    }

                    // 재생 중인 파티클과 현재 재생 할 파티클이 다른 경우
                    info.arrayIndex = _index;
                    info.bActive = true;
                    _loopParticlePerTouchID[_touchID] = info;
                }
            }

            // 해당 파티클  객체 획득
            ParticleSystem ps = ParticleDic.getObject(_index);
            if (ps == null) return;

            if (_parent != null)
            {
                ps.transform.parent = _parent;
                ps.transform.localPosition = pos;
                ps.gameObject.layer = _parent.gameObject.layer;
            }
            else
            {
                ps.transform.position = pos;
                ps.gameObject.layer = Constante.DEFAULT;
            }

            ps.loop = _loop;

            if (_loop)
            {
                //무한 재생 될 파티클 등록
                if (_loopParticle.ContainsKey(_touchID))
                {
                    // 기존의 파티클은 정지
                    _loopParticle[_touchID].SetActive(false);
                    // 현재 파티클로 새로 등록
                    _loopParticle[_touchID] = ps.gameObject;
                }
                else
                {
                    _loopParticle.Add(_touchID, ps.gameObject);
                }
            }

            ps.gameObject.SetActive(true);
            ps.Play(true);
        }
    }
}