using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ML.MLBKids
{
    public class PageManager : MonoBehaviour
    {
        #region Properties
        public static PageManager instance { get; private set; }

        private Transform _pagesParent;
        public Transform pagesParent
        {
            get
            {
                if (_pagesParent == null)
                {
                    _pagesParent = transform.Find("Pages");
                }
                return _pagesParent;
            }
        }

        public bool isShowingPopup
        {
            get { return (_popup != null && _popup.gameObject.activeSelf) || _hiddenMenu.isShowing; }
        }
        #endregion

        #region Private variables
        private Popup _popup;
        private HiddenMenu _hiddenMenu;
        private Dictionary<System.Type, Page> _pages = new Dictionary<System.Type, Page>();
        #endregion

        #region Events
        public delegate void OnPageShow(Page page);
        public event OnPageShow onPageShow;
        #endregion

        #region Unity methods
        public void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void Start()
        {
            _hiddenMenu = GetComponentInChildren<HiddenMenu>(true);
        }

        public void OnDisable()
        {
            if (instance == this)
                instance = null;
        }
        #endregion

        #region Page Managements
        /// <summary>
        /// Shows a page.
        /// </summary>
        /// <typeparam name="T">The system type of a page.</typeparam>
        /// <returns>A new or current page. returns null if a page couldn't be found.</returns>
        public T ShowPage<T>() where T : Page
        {
            Page currentPage = GetPage<T>();
            bool needsInitialzie = currentPage == null;

            if (currentPage == null)
            {
                currentPage = gameObject.GetComponentInChildren<T>(true);
                if (currentPage != null)
                {
                    _pages.Add(typeof(T), currentPage);
                }
                else
                {
                    T pagePrefab = Resources.Load<T>(string.Format("UI/{0}", typeof(T).Name.Replace("Page", "")));
                    if (pagePrefab == null)
                    {
                        Debug.LogError(string.Format("PageManager.ShowPage() : Unable to find page at \"UI/{0}\"!", typeof(T).Name));
                    }
                    else
                    {
                        currentPage = Instantiate<T>(pagePrefab);
                        currentPage.name = pagePrefab.name;
						currentPage.transform.SetParent(pagesParent, false);
						_pages.Add(typeof(T), currentPage);
                    }
                }
            }

            if (currentPage != null)
            {
                currentPage.transform.SetSiblingIndex(pagesParent.childCount - 1);
                bool isAlreadyShown = currentPage.isShowing;
                currentPage.Show();
                if (needsInitialzie)
                    currentPage.Init();
                if (!isAlreadyShown)
                {
                    if (onPageShow != null)
                        onPageShow(currentPage);
                }
                return (T)currentPage;
            }
            
            return null;
        }

        /// <summary>
        /// Hides a page.
        /// </summary>
        /// <typeparam name="T">The system type of a page.</typeparam>
        public void HidePage<T>() where T : Page
        {
            T currentPage = GetPage<T>();
            if (currentPage != null)
            {
                currentPage.Hide();
            }
        }

        /// <summary>
        /// Gets the current page.
        /// </summary>
        /// <typeparam name="T">The system type of a page.</typeparam>
        /// <returns>The page object. null if it's not exist.</returns>
        public T GetPage<T>() where T : Page
        {
            if (_pages.ContainsKey(typeof(T)))
            {
                return (T)_pages[typeof(T)];
            }
            return null;
        }

        /// <summary>
        /// Hides all pages.
        /// </summary>
        public void HideAllPages()
        {
            var pages = GetComponentsInChildren<Page>();
            foreach (var page in pages)
            {
                if (_pages.ContainsKey(page.GetType()))
                {
                    page.Hide();
                }
                else
                {
                    page.gameObject.SetActive(false);
                }
            }
        }

        public bool IsShowing<T>() where T : Page
        {
			T page = GetPage<T> ();
			if (page != null)
				return page.isShowing;
            return false;
        }
        #endregion

        #region Popup
        public void ShowPopup(string message, System.Action handler = null)
        {
            if (_popup == null)
                _popup = GetComponentInChildren<Popup>(true);

            if (!_popup.gameObject.activeSelf)
                _popup.gameObject.SetActive(true);

            _popup.Set(message, handler);
        }
        #endregion
    }
}