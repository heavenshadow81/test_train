using UnityEngine;
using UnityEngine.UI;

namespace ML.T_Sports.Common
{
    /// <summary>
    /// 키오스크 원격 컨트롤 화면.
    /// <see cref="ContentsManagerBase"/> 싱글톤 오브젝트를 참조하며 콘텐츠 상태를 제어한다.
    /// </summary>
    public class RemoteControlPanel : MonoBehaviour, IContentsManagerListener
    {
        public RemoteControlPanelPropertyRow[] propertyRows;
        public GameObject readyButton, playButton;

        public void Start()
        {
            if (Display.displays.Length > 0)
                Display.displays[0].Activate();
            if (Display.displays.Length > 1)
                Display.displays[1].Activate();

            if (ContentsManagerBase.Current != null)
                ContentsManagerBase.Current.AddListener(this);

            Refresh();
        }

        public void Refresh()
        {
            if (ContentsManagerBase.Current != null)
            {
                readyButton.SetActive(!ContentsManagerBase.Current.IsReady);
                playButton.SetActive(ContentsManagerBase.Current.IsReady);
            }

            for (int i = 0; i < propertyRows.Length; i++)
            {
                propertyRows[i].Refresh();
            }
        }

        public void Home()
        {
            Debug.Log("Back to main menu...");

            if (ContentsManagerBase.Current != null)
                ContentsManagerBase.Current.RemoveListener(this);

            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("00TouchSports_Main");
        }

        #region Play/Stop
        public void Ready()
        {
            if (ContentsManagerBase.Current != null)
            {
                Debug.Log("RemoteControlPanel.Ready()");
                if (!ContentsManagerBase.Current.IsReady)
                    ContentsManagerBase.Current.Ready();
            }
        }

        public void Play()
        {
            if (ContentsManagerBase.Current != null)
            {
                Debug.Log("RemoteControlPanel.Play()");

                if (ContentsManagerBase.Current.IsPaused)
                    ContentsManagerBase.Current.Pause();
                else
                    ContentsManagerBase.Current.Play();
            }
        }

        public void Pause()
        {
            if (ContentsManagerBase.Current != null && ContentsManagerBase.Current.IsPlaying && !ContentsManagerBase.Current.IsPaused)
            {
                Debug.Log("RemoteControlPanel.Pause()");

                ContentsManagerBase.Current.Pause();
            }

            Refresh();
        }

        public void Stop()
        {
            if (ContentsManagerBase.Current != null)
            {
                Debug.Log("RemoteControlPanel.Stop()");

                if (ContentsManagerBase.Current.IsPaused)
                    ContentsManagerBase.Current.Pause();
                ContentsManagerBase.Current.Stop();
            }
        }
        #endregion

        #region Reset
        public void ResetPropertiesToDefaults()
        {
            if (ContentsManagerBase.Current != null && !ContentsManagerBase.Current.IsPlaying)
            {
                ContentsManagerBase.Current.ResetPropertiesToDefaults();
                Refresh();
            }
        }

        public void ResetSharedPropertiesToDefaults()
        {
            if (ContentsManagerBase.Current != null)
            {
                ContentsManagerBase.Current.ResetSharedPropertiesToDefaults();
                Refresh();
            }
        }
        #endregion

        #region Listener
        public void OnPlay()
        {
            Refresh();
        }

        public void OnStop()
        {
            Refresh();
        }

        public void OnReady()
        {
            Refresh();
        }
        #endregion
    }
}