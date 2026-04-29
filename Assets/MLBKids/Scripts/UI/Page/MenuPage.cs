using UnityEngine;
using DG.Tweening;

namespace ML.MLBKids
{
    public class MenuPage : Page
    {
        public GameObject back;

        #region Overrides
        public override void Show()
        {
            base.Show();
            back.SetActive(true);
            Stadium.instance.SetBlur(true, 0);
            //KinectHelper.instance.onBodyTracked += _OnBodyTracked;
            //_OnBodyTracked(KinectHelper.instance.trackingId);
            DelayedCall.Stop("hide_menu_page_delayed");
            if (GetComponent<Animator>() != null)
                GetComponent<Animator>().Play("Menu");
        }

        public override void Hide()
        {
           // KinectHelper.instance.onBodyTracked -= _OnBodyTracked;
            DelayedCall.Stop("back_to_ads");
            GetComponent<Animator>().Play("Hide");
            back.SetActive(false);
            //DelayedCall.Begin("hide_menu_page_delayed", 0.25f, (flag) =>
            //{
            //    if (flag) base.Hide();
            //});
        }
        #endregion

        #region Actions
        public void OnSelectGame1()
        {
            Stadium stadium = Stadium.instance;
            stadium.Init(Stadium.GameMode.Pitch);
            //stadium.Tutorial();
            stadium.Play();
            stadium.SetBlur(false, 0.0f);
            Hide();
        }

        public void OnSelectGame2()
        {
            Stadium stadium = Stadium.instance;
            stadium.Init(Stadium.GameMode.Hit);
            //stadium.Tutorial();
            stadium.Play();
            stadium.SetBlur(false, 0.0f);
            Hide();
        }
        #endregion

        #region Kinect
        private void _OnBodyTracked(ulong trackingId)
        {
            if (trackingId == 0)
            {
                //bool bodyTooClose = KinectHelper.instance.isLastBodySeemsComeClose;
                //DelayedCall.Begin("back_to_ads",
                //    bodyTooClose ?
                //    GlobalConstants.instance.closeToScreenAnyActionWaitTime :
                //    GlobalConstants.instance.anyActionWaitTime,
                //    (flag) =>
                //    {
                //        if (flag)
                //        {
                //            PageManager.instance.GoFromMenuToAds();
                //        }
                //    });
            }
            else
            {
                DelayedCall.Stop("back_to_ads");
            }
        }
        #endregion
    }
}