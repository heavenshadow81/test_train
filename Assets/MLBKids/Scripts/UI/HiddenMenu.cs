using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ML.MLBKids
{
    public class HiddenMenu : MonoBehaviour
    {
        public GameObject hidden1;  // admin menu
        public GameObject hidden2;  // game menu

        public bool isShowing { get { return hidden1.activeSelf || hidden2.activeSelf; } }

        public void Awake()
        {
            Hide();
        }

        public void Start()
        {
            PageManager pageManager = PageManager.instance;
            pageManager.onPageShow += _OnPageShow;
        }
        private void OnEnable()
        {
            StartCoroutine(StartHiddenMenu());
        }
        public void Show()
        {
            PageManager pageManager = PageManager.instance;
            if (pageManager.IsShowing<LoadingPage>())
            {
                Debug.Log("Couldn't show hidden menu while loading!");
                return;
            }
            else if (pageManager.IsShowing<AdsPage>())
            {
                Show_Hidden1();
            }
            else
            {
                Show_Hidden2();
            }
        }

        public void Show_Hidden1()
        {
            hidden1.SetActive(true);
        }

        public void Show_Hidden2()
        {
            hidden2.SetActive(true);
        }

        public void Hide_Hidden1()
        {
            hidden1.SetActive(false);
        }

        public void Hide_Hidden2()
        {
            hidden2.SetActive(false);
        }

        public void Hide()
        {
            Hide_Hidden1();
            Hide_Hidden2();
        }
        //시작 메뉴..?
        public void Hidden1_Start()
        {
            PageManager.instance.GoFromAdsToMenu();
            hidden1.SetActive(false);
        }
        #region 추가 함수
        IEnumerator StartHiddenMenu()
        {
            yield return new WaitForSeconds(0.1f);
            Hidden1_Start();
            yield break;
        }
        #endregion

        public void Hidden1_Quit()
        {
            PageManager pageManager = PageManager.instance;

            Stadium.instance?.Cleanup();
            pageManager.ShowPopup(Localization.Get("quit_msg"), () =>
            {
                Application.Quit();
            });
            hidden1.SetActive(false);
        }

        public void Hidden1_Admin()
        {
            //hidden1.SetActive(false);
        }

        public void Hidden2_Quit()
        {
            PageManager pageManager = PageManager.instance;
            if (pageManager.IsShowing<MenuPage>())
            {
                Stadium.instance?.Cleanup();
                pageManager.GoFromMenuToAds();
            }
            else if (Stadium.instance != null)
            {
                Stadium.instance.Cleanup();
                pageManager.ShowPage<MenuPage>();
            }
            hidden2.SetActive(false);
        }

        private void _OnPageShow(Page page)
        {
            System.Type pageType = page.GetType();
            if (pageType.Equals(typeof(AdsPage)))
            {
                Hide_Hidden2();
            }
            else if (pageType.Equals(typeof(MenuPage)))
            {
                Hide_Hidden1();
            }
        }
    }
}