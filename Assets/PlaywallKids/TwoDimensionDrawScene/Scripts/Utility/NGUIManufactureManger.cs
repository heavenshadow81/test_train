using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    using Common;

    /// <summary>
    /// 파일에서 좌표와 회전 값을 읽을 후 프리팹 동적 생성 및 설정
    /// </summary>
    public class NGUIManufactureManger : MonoBehaviour
    {
        const int INDEX = 0;
        const int POSITION = 3;
        const int ROTATION = 6;

        public GameObject[] prefabs;
        public GameObject parent;
        public string path;
        public string fileName;

        List<GameObject> list;

        void OnDisable()
        {
            if (list == null || list.Count == 0) return;

            for (int i = 0, len = list.Count; i < len; ++i)
            {
                Destroy(list[i]);
            }
            list.Clear();
            list = null;
        }

        /// <summary>
        /// 객체 생성
        /// </summary>
        /// <returns></returns>
        public List<GameObject> GenerateObjects()
        {
            list = new List<GameObject>();
            if (string.IsNullOrEmpty(fileName)) return null;
            string resPath = System.IO.Path.Combine(path, fileName).Replace("\\", "/");
            TextAsset textAsset = (TextAsset)Resources.Load(resPath, typeof(TextAsset));
            string[] stringLines = textAsset.text.Split("\n"[0]); ;

            for (int i = 0, len = stringLines.Length; i < len; ++i)
            {
                string[] item = CSVUtil.SplitCsvLine(stringLines[i]);
                int _index = System.Convert.ToInt32(item[INDEX]);
                float[] _float3 = new float[3];
                int cnt = 0;
                for (int j = 1; j <= POSITION; ++j, ++cnt)
                { _float3[cnt] = (float)System.Convert.ToDouble(item[j]); }

                Vector3 _scale = prefabs[_index].transform.localScale;
                GameObject obj = (parent ? NGUITools.AddChild(parent, prefabs[_index]) : Instantiate(prefabs[_index])) as GameObject;
                obj.transform.localPosition = new Vector3(_float3[0] * ScreenUtil.NGUIWidth - ScreenUtil.NGUIWidth * 0.5f,
                                                           _float3[1] * ScreenUtil.NGUIHeight - ScreenUtil.NGUIHeight * 0.5f,
                                                           _float3[2]);
                obj.transform.localScale = _scale;
                _float3[0] = _float3[1] = _float3[2] = 0f;

                if (item.Length > POSITION)
                {
                    cnt = 0;
                    for (int j = POSITION + 1; j < item.Length && j <= ROTATION; ++j, ++cnt)
                    {
                        if (string.IsNullOrEmpty(item[j])) continue;
                        _float3[cnt] = (float)System.Convert.ToDouble(item[j]);
                    }

                    obj.transform.eulerAngles = new Vector3(_float3[0], _float3[1], _float3[2]);
                }
                list.Add(obj);
            }

            return list;
        }

        public List<GameObject> GetList()
        {
            return list;
        }
    }
}