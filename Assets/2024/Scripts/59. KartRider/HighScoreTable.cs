using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KartRider
{
    public class HighScoreTable : MonoBehaviour
    {
        [SerializeField] Transform entryContainer; //랭킹 템플릿이 생성될 위치
        [SerializeField] Transform entryTemplate; //랭킹 템플릿 (순위, 기록, 이름)텍스트
        List<Transform> bestrecordTransformList;

        void RankingReset()
        {
            entryTemplate.gameObject.SetActive(false); //템플릿 비활성화
            string jsonString = PlayerPrefs.GetString("bestrecordTable");
            Bestrecords bestrecords = JsonUtility.FromJson<Bestrecords>(jsonString);

            //스코어 정렬
            for (int i = 0; i < bestrecords.bestrecordList.Count; i++)
            {
                for (int j = i + 1; j < bestrecords.bestrecordList.Count; j++)
                {
                    if (bestrecords.bestrecordList[j].record < bestrecords.bestrecordList[i].record)
                    {
                        //더 빨리 도착한 순서로 정렬
                        BestrecordEntry tmp = bestrecords.bestrecordList[i];
                        bestrecords.bestrecordList[i] = bestrecords.bestrecordList[j];
                        bestrecords.bestrecordList[j] = tmp;
                    }
                }
            }

            bestrecordTransformList = new List<Transform>();

            for (int i = 0; i < bestrecords.bestrecordList.Count && i < 10; i++)
            {
                BestrecordEntry bestrecordEntry = bestrecords.bestrecordList[i];
                CreateBestRecordEntryTransform(bestrecordEntry, entryContainer, bestrecordTransformList);
            }
        }

        void CreateBestRecordEntryTransform(BestrecordEntry bestrecordEntry, Transform container, List<Transform> transformList)
        {
            float templateHeight = 100f; //템플릿 간의 간격

            int PosX = 0;
            int index = transformList.Count;

            if (transformList.Count >= 5) //6번째부터 오른쪽에 배치
            {
                PosX = 640;
                index = transformList.Count - 5;
            }

            Transform entryTransform = Instantiate(entryTemplate, container); //템플릿 컨테이너 안에 생성
            RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>(); //템플릿의 좌표 컴포넌트
            entryRectTransform.anchoredPosition = new Vector2(PosX, -templateHeight * index); //템플릿의 위치 지정
            entryTransform.gameObject.SetActive(true); //템플릿 활성화

            int rank = transformList.Count + 1; //랭크 변수
            string rankString; //랭크별 텍스트

            switch (rank) //1,2,3위를 제외하고 나머지에 TH붙임
            {
                default:
                    rankString = rank + "th"; break;

                case 1: rankString = "1st"; break;
                case 2: rankString = "2nd"; break;
                case 3: rankString = "3rd"; break;
            }

            entryTransform.Find("rank").GetComponent<TextMeshProUGUI>().text = rankString; //랭크 텍스트에 각 랭킹 들어감

            //레코드 텍스트 분,초,밀리초로 텍스트 표기
            float record = bestrecordEntry.record;

            int minutes = Mathf.FloorToInt(record / 60f);
            int seconds = Mathf.FloorToInt(record % 60f);
            int milliseconds = Mathf.FloorToInt((record * 100) % 100);

            var recordText = entryTransform.Find("record").GetComponent<TextMeshProUGUI>();
            recordText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);

            //이름 표기
            string name = bestrecordEntry.name;
            entryTransform.Find("name").GetComponent<TextMeshProUGUI>().text = name;

            transformList.Add(entryTransform);
        }

        public void AddBestrecordEntry(float record, string name)
        {
            BestrecordEntry bestrecordEntry = new BestrecordEntry { record = record, name = name };

            string jsonString = PlayerPrefs.GetString("bestrecordTable", ""); // 기본값을 빈 문자열로 설정
            Bestrecords bestrecords = JsonUtility.FromJson<Bestrecords>(jsonString);

            // bestrecords가 null일 경우 초기화
            if (bestrecords == null)
            {
                bestrecords = new Bestrecords();
                bestrecords.bestrecordList = new List<BestrecordEntry>();
            }

            bestrecords.bestrecordList.Add(bestrecordEntry);

            string json = JsonUtility.ToJson(bestrecords);
            PlayerPrefs.SetString("bestrecordTable", json);
            PlayerPrefs.Save();

            RankingReset();
        }

        class Bestrecords
        {
            public List<BestrecordEntry> bestrecordList;
        }

        /*
         * Represents a single Best Record entry
         **/

        [System.Serializable]
        class BestrecordEntry
        {
            public float record;
            public string name;
        }
    }
}
