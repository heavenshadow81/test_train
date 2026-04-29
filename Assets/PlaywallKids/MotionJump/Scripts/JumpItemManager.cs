using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.MotionJump
{
    /// <summary>
    /// 코인, 아이템, 장애물 등 생성 하는 클래스
    /// </summary>
    public class JumpItemManager : MonoBehaviour
    {
        public GameObject[] items;
        public GameObject itemPrefab;
        [Range(1, 10)]
        public float widthSpacing;
        [Range(1, 10)]
        public float heightSpacing;

        private Dictionary<int, List<GameObject>> listDictionary = new Dictionary<int, List<GameObject>>();

        CObjectListToDic<int, GameObject> mItemDic;
        /// <summary>
        /// 메모리 풀
        /// </summary>
        CObjectListToDic<int, GameObject> itemDictionary
        {
            get
            {
                if (mItemDic == null)
                {
                    mItemDic = new CObjectListToDic<int, GameObject>(
                    (int _itemType) =>
                    {
                        GameObject go = Instantiate(GetItem(_itemType)) as GameObject;
                        go.transform.parent = this.transform;
                        return go;
                    },
                    (GameObject go) =>
                    { return !go.gameObject.activeInHierarchy; }
                    );
                }
                return mItemDic;
            }
        }

        void Awake()
        {
            if (listDictionary == null)
                listDictionary = new Dictionary<int, List<GameObject>>();
        }

        void OnDestroy()
        {
            listDictionary.Clear();
            listDictionary = null;

            foreach (var element in mItemDic.dictionary)
            { element.Value.Clear(); }
            mItemDic = null;
        }

        /// <summary>
        /// 그리드 형태의 패턴으로 아이템 생성
        /// </summary>
        /// <param name="_patterns"></param>
        /// <param name="_worldSpaceHeight">월드 높이</param>
        /// <param name="_width">가로로 생성 될 아이템 개수</param>
        /// <param name="_origin_x">생성 될 시작 x 좌표</param>
        /// <returns></returns>
        public List<GameObject> CreateItem(int[][] _patterns, int _worldSpaceHeight, int _width, float _origin_x)
        {
            List<GameObject> temp = new List<GameObject>();

            // 생성 될 배경 높이
            float height = _worldSpaceHeight;

            // index 2 높이 만큼 생성
            for (int cnt = 0; cnt < 2; ++cnt)
            {
                // 아이템 패턴 한종류
                int[] pattern = _patterns[Random.Range(0, _patterns.Length - 1)];

                // 높이
                int _height = (int)(pattern.Length / _width);

                int rest_num = pattern.Length;
                for (int h = 0; h < _height; ++h)
                {
                    // 배열 끝부터 검색
                    rest_num -= (int)_width;
                    for (int w = 0; w < _width; ++w)
                    {
                        int idx = pattern[w + rest_num];
                        if (idx == 0) continue;

                        GameObject go = itemDictionary.getObject(idx);
                        float x = _origin_x + ((w % _width) * widthSpacing);
                        //아래부터 생성
                        float y = height + h * heightSpacing;
                        go.gameObject.SetActive(true);
                        go.transform.localPosition = new Vector3(x, y, 0);
                        temp.Add(go);
                    }
                }
                //그 다음 높이
                height += _height * heightSpacing;
            }
            return temp;
        }

        public void SetItemList(int _idx, List<GameObject> _list)
        {
            if (!listDictionary.ContainsKey(_idx))
            { listDictionary.Add(_idx, _list); }
            else
            { listDictionary[_idx] = _list; }
        }

        /// <summary>
        /// 배경 index에 해당하는 모든 아이템 비활성화
        /// </summary>
        /// <param name="_idx"></param>
        public void DisableItems(int _idx)
        {
            if (listDictionary.ContainsKey(_idx))
            {
                List<GameObject> currentList = listDictionary[_idx];
                for (int i = 0; i < currentList.Count; ++i)
                { currentList[i].SetActive(false); }
                currentList = null;
            }
        }

        public GameObject GetItem(int _idx)
        {
            if (_idx == (int)EItemType.COIN) return items[0];
            else if (_idx == (int)EItemType.SPEED)
                return items[1];

            return itemPrefab;
        }

        public void Init()
        {
            Dictionary<int, List<GameObject>> dic = itemDictionary.dictionary;

            foreach (var value in dic)
            {
                List<GameObject> list = value.Value;
                for (int i = 0; i < list.Count; ++i)
                { list[i].SetActive(false); }
                list = null;
            }
        }
    }
}