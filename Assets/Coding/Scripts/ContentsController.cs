using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//콘텐츠 컨트롤러

namespace Coding
{
    public class ContentsController : MonoBehaviour
    {
        #region 변수
        static ContentsController _instance;
        //읽기 전용 싱글턴
        public static ContentsController Instance
        {
            get => _instance;
        }
        public bool player1;
        #endregion

        #region 이벤트
        //콘텐츠 시작시 호출할 이벤트
        public event System.Action<int> Initialize, SoundPlay;
        //콘텐츠 설정 변경시 호출할 이벤트
        public event System.Action<int, int> SetParameter;
        public event System.Action Final;
        #endregion

        #region 함수
        //시작시 처음 설정...!
        public void Init()
        {
            //맵 크기 조절>가로, 세로 셀 갯수
            int row = ContentsOptions.GetDifficult() switch
            {
                Difficult.Easy => 3,
                Difficult.Normal => 3,
                _ => 3,
            };
            int column = ContentsOptions.GetDifficult() switch
            {
                Difficult.Easy => 5,
                Difficult.Normal => 7,
                _ => 4
            };

            //콘텐츠 설정 값 변경
            SetParameter?.Invoke(row, column);
            //콘텐츠 초기화
            Initialize?.Invoke((int)ContentsOptions.GetDifficult());
        }
        //난이도 설정
        public void SetDifficult()
        {
            int row = ContentsOptions.GetDifficult() switch
            {
                Difficult.Easy => 4,
                Difficult.Normal => 5,
                _ => 3,
            };
            int column = ContentsOptions.GetDifficult() switch
            {
                Difficult.Easy => 5,
                Difficult.Normal => 6,
                _ => 4
            };
            SetParameter?.Invoke(row, column);
        }
        //재시작> 지금은 씬 재로딩으로...>재로딩 후 다시 스타트 누르기...
        public void Restart()
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }

        //콘텐츠 종료!!
        public void Finalization()
        {
            print(Final.Method);
            Final?.Invoke();
        }
        //소리 재생!!
        public void SoundEffect(int index)
        {
            SoundPlay?.Invoke(index);
        }

        #endregion
        #region 유니티 함수
        private void Awake()
        {
            if (!_instance)
            {
                _instance = this;
            }
        }

        #endregion
    }
}
