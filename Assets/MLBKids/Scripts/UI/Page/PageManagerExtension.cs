using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ML.MLBKids
{
    public static class PageManagerExtension
    {
        public static void GoFromAdsToMenu(this PageManager pageManager)
        {
            pageManager.HidePage<AdsPage>();
            pageManager.ShowPage<MenuPage>();
        }

        public static void GoFromMenuToAds(this PageManager pageManager)
        {
            pageManager.HidePage<MenuPage>();
            pageManager.ShowPage<AdsPage>();
        }
    }
}