using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//public enum FadeOut
//{
//    none, fade, Out
//}
namespace Bax.P0.Client.UnityWorld.Intro
{
    /// <summary>
    /// 컨텐츠 게임 입장버튼
    /// </summary>
    public class GameContentsChange : MonoBehaviour
    {
        private IntroProcess intro;

        public Button btn;

        //private int childIdx;
        public int parentIdx;
        public SceneChangeState state;

        CanvasGroup fade;


        private void Awake()
        {
            //컴포넌트 저장
            //intro = FindObjectOfType<IntroProcess>();
            btn = GetComponent<Button>();

            //0번은 Intro 씬이기때문에 + 1
            //childIdx = transform.GetSiblingIndex() + 2;

            //버튼 클릭시 실행될 리스너에 함수 저장
            btn.onClick.AddListener(SceneChange);      
        }

        private void Start()
        {
            //페이드인아웃 용도 CanvasGroup 저장
            fade = Settings.instance.fadePanel;
        }

        bool isdown = false;

        public void SceneChange()
        {
            if (isdown) return;
            isdown = true;
            //인트로씬 2번카메라 비활성화
            AdminSetting.SubCamActiveF?.Invoke();
            //버튼 사운드
            SoundMGR.Instance.SoundPlay("0.컨텐츠입장");
            //BGM 정지
            SoundMGR.Instance.bgmSource.Stop();
            //컨텐츠 입장 버튼 부모의 배열번호
            //int parentIdx = transform.parent.GetSiblingIndex();
            //화면잠금
            //fade.blocksRaycasts = true;

            //페이드 아웃
            Settings.instance.TweeningFadeInOut(() =>
            {
                Settings.instance.IntroState = state;
                Settings.instance.introChange = IntroChange.Game;
                //Settings.instance.mouseToggle.isOn = true;

              fadeInChanger(SceneChangeState.MagicLand, parentIdx);
                
                fadeInChanger(SceneChangeState.Zootopia, parentIdx);
                
                fadeInChanger(SceneChangeState.Attraction, parentIdx);
              
            }, 1f, 1,true);
        }

        /// <summary>
        /// 페이드인 씬 체인지 
        /// </summary>
        /// <param name="IntroState"></param>
        /// <param name="SceneBuildIdx"></param>
        /// <param name="BlockRayCast"></param>
        private async void fadeInChanger(SceneChangeState IntroState ,int SceneBuildIdx, bool BlockRayCast = false)
        {
            
            isdown = false;
            if (Settings.instance.IntroState == IntroState)
            {

                await Settings.instance.SceneChangeAsync(SceneBuildIdx, () =>
                {
                    print(SceneBuildIdx);
                    //페이드인 후 화면잠금풀림
                    Settings.instance.TweeningFadeInOut(() => { fade.blocksRaycasts = BlockRayCast; }, 1f, 0, true);
                });
            }
        }
    }
}