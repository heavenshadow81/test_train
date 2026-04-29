using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ML.MLBKids
{
    public class Loader : MonoBehaviour
    {
        public PageManager pageManager;
        private LoadingPage _loadingPage;

        private void Awake()
        {
            try
            {
                string text = File.ReadAllText("lang.txt");
                Localization.defaultLanguage = text;
            }
            catch { }
            Localization.Load();

            pageManager.HideAllPages();
            _loadingPage = pageManager.ShowPage<LoadingPage>();
        }

        IEnumerator Start()
        {
            // Checking License
#if !UNITY_EDITOR
            //if (!MACCertified.IsPlayable())
            //{
            //    pageManager.ShowPopup(Localization.Get("unregistered_device"), () =>
            //    {
            //        Application.Quit();
            //    });
            //    DelayedCall.Stop("popup_auto_hide");
            //    yield break;
            //}
#endif

            // Multi-touch
            if (!Input.touchSupported)
            {
                Debug.Log("This platform doesn't support unity touch.");
            }
            else
            {
                Debug.Log("This platform supports unity touch.");
            }

            // Kinect
            //KinectHelper kinectHelper = KinectHelper.instance;
            //if (!kinectHelper.Init())
            {
#if !UNITY_EDITOR
                pageManager.ShowPopup("PC에서 Kinect 장치를 확인할 수 없습니다. 연결 상태를 확인해주세요.", () =>
                {
                    Application.Quit();
                });
                DelayedCall.Stop("popup_auto_hide");
                yield break;
#endif
            }

            var async = SceneManager.LoadSceneAsync("Stadium", LoadSceneMode.Additive);
            while (!async.isDone)
            {
                _loadingPage.progress = async.progress;
                yield return null;
            }
            _loadingPage.progress = 0.9f;
            
            Scene mainScene = SceneManager.GetActiveScene();
            Scene stadiumScene = SceneManager.GetSceneByName("Stadium");
            SceneManager.SetActiveScene(stadiumScene);

            GameObject stadium = GameObject.Find("Stadium");
            //SceneManager.SetActiveScene(mainScene);

            yield return null;
            _loadingPage.progress = 1.0f;
            yield return new WaitForSeconds(0.25f);
            
            pageManager.ShowPage<AdsPage>();
            pageManager.HidePage<LoadingPage>();
        }
    }
}