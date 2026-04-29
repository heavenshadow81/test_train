using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    /// <summary>
    /// 콘텐츠 내 사용자 입력과 상호 작용 하는 UI객체들의 메모리풀 및 객체 관리
    /// </summary>
    public class UIManager : MonoBehaviour
    {

        public UIObject[] prefabs;

        #region COLLECTION_PROPERTIES
        /// <summary>
        /// 사용자 입력 ID 별 UIObject들 메모리 참조
        /// </summary>
        Dictionary<int, ListPool<UIObject>> MemoryPoolPerUIObject
        {
            get
            {
                if (_memoryPoolPerUIObject == null)
                    _memoryPoolPerUIObject = new Dictionary<int, ListPool<UIObject>>();
                return _memoryPoolPerUIObject;
            }
        }

        /// <summary>
        /// ?
        /// </summary>
        Dictionary<int, Dictionary<int, UIObject>> EnabledObjectPerID
        {
            get
            {
                if (_enabledObjectPerID == null)
                    _enabledObjectPerID = new Dictionary<int, Dictionary<int, UIObject>>();
                return _enabledObjectPerID;
            }
        }
        #endregion COLLECTION_PROPERTIES

        #region  COLLECTIONS
        Dictionary<int, Dictionary<int, UIObject>> _enabledObjectPerID;
        Dictionary<int, ListPool<UIObject>> _memoryPoolPerUIObject;
        #endregion COLLECTIONS

        Transform cachedTransform;

        void Awake()
        {
            cachedTransform = this.transform;
        }

        void OnDisable()
        {
            if (_memoryPoolPerUIObject != null)
                _memoryPoolPerUIObject.Clear();
            _memoryPoolPerUIObject = null;

            if (_enabledObjectPerID != null)
                _enabledObjectPerID.Clear();
            _enabledObjectPerID = null;
        }

        /// <summary>
        /// 프리팹 메모리 풀 생성
        /// </summary>
        /// <param name="_key"></param>
        /// <param name="_prefab"></param>
        /// <returns></returns>
        public bool GenerateMemoryPool(int _key, UIObject _prefab)
        {
            if (MemoryPoolPerUIObject.ContainsKey(_key))
                return false; //이미 해당하는 key로 메모리 풀 생성

            MemoryPoolPerUIObject.Add(_key, new ListPool<UIObject>(_prefab));
            return true;
        }

        /// <summary>
        /// UIObject.Value 반환
        /// </summary>
        /// <param name="_touchID">사용자 터치 ID</param>
        /// <param name="_indexUI">UIObject.Value</param>
        /// <returns></returns>
        public float GetValue(int _touchID, int _indexUI)
        {
            if (!EnabledObjectPerID.ContainsKey(_touchID)) //사용자 터치가 미등록 된경우
                return 0f;

            Dictionary<int, UIObject> _uiDictionary = EnabledObjectPerID[_touchID];

            if (!_uiDictionary.ContainsKey(_indexUI)) // 해당하는 UI index가 없을 경우
                return 0f;

            return _uiDictionary[_indexUI].Value;
        }

        /// <summary>
        /// 사용자 터치에 해당 하는 UI 객체 출력
        /// </summary>
        /// <param name="_touchID">사용자 터치 ID</param>
        /// <param name="_indexUI">UI 객체 index</param>
        /// <param name="_position">축력 될 좌표</param>
        /// <param name="_value">해당 UI 객체의 값</param>
        /// <param name="_parent">출력 될 부모 객체</param>
        /// <returns></returns>
        public UIObject DisplayUIObject(int _touchID, int _indexUI, Vector3 _position, float _value, Transform _parent = null)
        {
            UIObject ui = null;

            if (EnabledObjectPerID.ContainsKey(_touchID))
            {
                Dictionary<int, UIObject> _uiDic = EnabledObjectPerID[_touchID];
                if (!_uiDic.ContainsKey(_indexUI))
                {
                    ui = GetObject(_indexUI, _parent);
                    if (!ui)
                        return null;
                    _uiDic.Add(_indexUI, ui);
                }
                else
                {
                    ui = _uiDic[_indexUI];
                    if (!ui)
                        return null;
                }
            }
            else
            {
                Dictionary<int, UIObject> _dic = new Dictionary<int, UIObject>();
                ui = GetObject(_indexUI, _parent);
                _dic.Add(_indexUI, ui);
                EnabledObjectPerID.Add(_touchID, _dic);
            }

            if (ui == null)
                return null;
            ui.gameObject.SetActive(true);
            ui.Value = _value;
            ui.CachedTransform.localPosition = _position;

            return ui;
        }

        /// <summary>
        /// 터치 ID 에 해당 하는 UI 객체 반환
        /// </summary>
        /// <param name="_touchID"></param>
        /// <param name="_indexUI"></param>
        /// <returns></returns>
        public UIObject GetObject(int _touchID, int _indexUI)
        {
            if (EnabledObjectPerID.ContainsKey(_touchID))
            {
                Dictionary<int, UIObject> _dic = EnabledObjectPerID[_touchID];
                if (_dic.ContainsKey(_indexUI))
                { return _dic[_indexUI]; }
                _dic = null;
            }
            return null;
        }

        /// <summary>
        /// UI index에 해당 하는 UI객체 생성
        /// </summary>
        /// <param name="_uiIndex"></param>
        /// <param name="_parent"></param>
        /// <returns></returns>
        public UIObject GetObject(int _uiIndex, Transform _parent)
        {
            if (_uiIndex >= prefabs.Length)
                return null;

            UIObject ui = MemoryPoolPerUIObject[_uiIndex].GetComponent;
            ui.CachedTransform.parent = _parent == null ? this.cachedTransform : _parent;
            ui.CachedTransform.localScale = Vector3.one;
            ui.CachedTransform.localRotation = Quaternion.identity;
            ui.gameObject.SetActive(true);
            return ui;
        }

        /// <summary>
        /// UI 객체 상태 비활성화
        /// </summary>
        /// <param name="_touchID"></param>
        public void DisableUIObject(int _touchID)
        {
            if (!EnabledObjectPerID.ContainsKey(_touchID))
                return;

            Dictionary<int, UIObject> _uiDic = EnabledObjectPerID[_touchID];

            foreach (KeyValuePair<int, UIObject> keys in _uiDic)
            {
                keys.Value.gameObject.SetActive(false);
            }

            EnabledObjectPerID.Remove(_touchID);
            _uiDic.Clear();
        }
    }
}