using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

namespace BubblePang
{
    //UI 플레이어 캔버스 부분 저장, 소환 등등..
    public class UIController : MonoBehaviour, IUIController
    {
        #region 변수
        //싱글턴 만들기...
        static UIController _instance;
        public static UIController Instance
        {
            get => _instance;
        }

        //시작 버튼
        [SerializeField]
        GameObject startButton;

        // 시작과 함께 사라지는 오브젝트들
        [SerializeField]
        GameObject backDinoPrefabs;

        //변수: 플레이어 각각 가지고 있을 버튼 몇 번째인지?와 몇 개까지 나타나게 할지+ 테마, 
        //현재 플레이어 UI들...> 모두 비활성화 및 활성화
        List<PersonalUI> personalUIs = new List<PersonalUI>();

        // 버튼 배경 이미지
        [SerializeField]
        GameObject[] backImage;

        #endregion

        #region 인터페이스
        //추가
        public void Add(IPersonal personal)
        {
            if (!personalUIs.Contains((PersonalUI)personal))
            {
                personalUIs.Add((PersonalUI)personal);
                personal.Disactive();
            }

        }
        //삭제
        public void Remove(IPersonal personal)
        {
            if (personalUIs.Contains((PersonalUI)personal))
            {
                personalUIs.Remove((PersonalUI)personal);
            }
        }
        #endregion

        #region
        //싱글턴 객체 지정
        private void Awake()
        {
            if (!_instance)
            {
                _instance = this;
            }
        }
        //
        void Start()
        {
            DeactiveUI += SetDeactive;
            InitializeContent += SetActive;
        }
        #endregion
        #region 이벤트
        //UI비활성화
        public Action DeactiveUI;
        //초기화
        public event Action InitializeContent;
        #endregion
        #region 함수

        //비활성화!
        void SetDeactive()
        {
            foreach (var ui in personalUIs)
            {
                ui.Disactive();
            }

            // 버튼 배경 이미지 비활성화
            for (int i = 0; i < ContentsController.Instance.contentsParameter.person + 1; i++)
            {
                backImage[i].gameObject.SetActive(false);
            }

        }
        //활성화
        void SetActive()
        {
            for (int i = 0; i < ContentsController.Instance.contentsParameter.person + 1; i++)
            {
                personalUIs[i].gameObject.SetActive(true);
                backImage[i].gameObject.SetActive(true);
            }
        }

        //랜덤으로 섞기
        public T[] ShuffleArray<T>(T[] array, int seed)
        {
            System.Random ran = new System.Random(seed);

            for (int i = 0; i < array.Length - 1; i++)
            {
                int randomIndex = ran.Next(i, array.Length);
                T tempItem = array[randomIndex];
                array[randomIndex] = array[i];
                array[i] = tempItem;
            }
            return array;
        }

        //UI활성화
        public void Init()
        {
            InitializeContent?.Invoke();
            backDinoPrefabsOut();
        }

        #endregion
        // Restart 버튼 클릭 event
        public void RestartClick()
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }

        // 배경 공룡들 타겟찾아가게 하기
        void backDinoPrefabsOut()
        {
            for (int i = 0; i < backDinoPrefabs.transform.childCount; i++)
            {
                backDinoPrefabs.transform.GetChild(i).GetComponent<DinoNavMesh>().MoveToRandomDestination();
            }

            StartCoroutine(backDinoRemove());
        }

        // 타겟 찾아간 배경 공룡들 안보이게 하기
        IEnumerator backDinoRemove()
        {
            yield return new WaitForSeconds(10.0f);

            backDinoPrefabs.SetActive(false);
        }

    }
}
