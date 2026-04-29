using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.MotionJump
{
    /// <summary>
    /// 콘텐츠 배경의 파티클 생성 및 참조 관리
    /// </summary>
    public class JumpEnvironmentController : MonoBehaviour
    {
        public GameObject[] particles;
        Transform cachedTransform;
        JumpEnvironmentTable _fileTable;
        JumpEnvironmentTable FileTable
        {
            get
            {
                if (_fileTable == null)
                {
                    _fileTable = new JumpEnvironmentTable();
                }
                return _fileTable;
            }
        }

        Dictionary<int, List<GameObject>> _beCreatedObjDictionary;
        Dictionary<int, List<GameObject>> BeCreatedObjDictionary
        {
            get
            {
                if (_beCreatedObjDictionary == null)
                {
                    _beCreatedObjDictionary = new Dictionary<int, List<GameObject>>();
                }
                return _beCreatedObjDictionary;
            }
        }

        void Awake()
        {
            cachedTransform = this.transform;
        }

        void OnDisable()
        {
            BeCreatedObjDictionary.Clear();
            _beCreatedObjDictionary = null;
        }

        /// <summary>
        /// 파티클 생성 함수
        /// </summary>
        /// <param name="_cam">카메라 뷰포트 이내 공간에 생성</param>
        /// <param name="_index">현재 배경 index</param>
        public void CreateParticleObjects(Camera _cam, int _index)
        {
            if (_index < 0 || FileTable.values.Length <= _index) return;
            if (BeCreatedObjDictionary.ContainsKey(_index))
            { ObjectsSetActive(_index, true); return; }

            List<GameObject> temp = new List<GameObject>();
            for (int i = 0; i < FileTable.values.Length; ++i)
            {
                int fileIndex = FileTable.values[i].index;
                //현재 배경 _index 와 파일 데이터 index 비교
                if (fileIndex == _index)
                {

                    int cnt = FileTable.values[i].cnt;
                    float temp_h = 0; ;
                    while (cnt > 0)
                    {// 배경용 파티클 생성
                        --cnt;
                        float height = Random.Range(temp_h + 1, temp_h + 4);
                        if (height < temp_h) { height += 1.45f; }
                        temp_h = height;
                        GameObject obj = CreateObject(FileTable.values[i].szFileName);
                        if (obj != null)
                        {
                            Vector3 viewPosition = new Vector3(Random.Range(0.2f, 0.8f), Random.Range(0.5f, 0.8f) * height, 38);
                            obj.transform.position = _cam.ViewportToWorldPoint(viewPosition);
                            obj.SetActive(true);
                            obj.GetComponent<ParticleSystem>().Play();
                            temp.Add(obj);
                            obj = null;
                        }
                    }
                }
                else
                { if (fileIndex + 1 > _index) break; }
            }

            BeCreatedObjDictionary.Add(_index, temp);
            temp = null;
        }

        public void ObjectsSetActive(int _index, bool _enable)
        {
            if (BeCreatedObjDictionary.ContainsKey(_index))
            {
                List<GameObject> list = BeCreatedObjDictionary[_index];
                for (int i = 0; i < list.Count; ++i)
                { list[i].SetActive(_enable); }
                list = null;
            }
        }

        T CreateObject<T>(string _itemName) where T : Component
        {
            return CreateObject(_itemName).GetComponent<T>();
        }

        GameObject CreateObject(string _itemName)
        {
            GameObject go = null;
            for (int i = 0; i < particles.Length; ++i)
            {
                if (string.Compare(_itemName, particles[i].name) == 0)
                {
                    go = Instantiate(particles[i]) as GameObject;
                    break;
                }
            }
            if (go == null) return null;
            ParticleSystem ps = go.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                var mainModule = ps.main;
                mainModule.loop = true;
            }
            go.transform.parent = transform;
            go.SetActive(false);
            return go;
        }
    }

    /// <summary>
    /// 배경 파티클 정보 관리
    /// </summary>
    public class JumpEnvironmentTable
    {
        const string filePath = "Interaction/Jump/";
        const string fileName = "JumpEnvironmentFile";

        /// <summary>
        /// 해당 배경에 파티클 종류 및 개수 저장
        /// </summary>
        public struct EnvironmentInfo
        {
            /// <summary>
            /// 배경 인덱스(높이)
            /// </summary>
            public int index;
            /// <summary>
            /// 파티클 이름
            /// </summary>
            public string szFileName;
            /// <summary>
            /// 생성 할 파티클 수
            /// </summary>
            public int cnt;
        }

        public EnvironmentInfo[] values
        { get; private set; }

        public JumpEnvironmentTable()
        {
            TextAsset text = Resources.Load(filePath + fileName) as TextAsset;
            if (text == null) return;

            string[] szLines = text.text.Split("\n"[0]);
            values = new EnvironmentInfo[szLines.Length];

            for (int i = 0; i < szLines.Length; ++i)
            {
                string[] szRow = Common.CSVUtil.SplitCsvLine(szLines[i]);
                if (szRow.Length == 0)
                    continue;

                values[i].index = int.Parse(szRow[0]);
                values[i].szFileName = szRow[1];
                values[i].cnt = int.Parse(szRow[2]);

            }
        }

        ~JumpEnvironmentTable()
        { values = null; }
    }
}