using UnityEngine;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    /// <summary>
    /// 사용자 입력과 상호작용을 제어하는 클래스
    /// </summary>
    public class TwoDimensionInteractionPanel : MonoBehaviour, IEvent
    {

        protected UIPanel _cPanel;
        public UIPanel cPanel
        {
            get
            {
                if (_cPanel == null)
                {
                    _cPanel = this.gameObject.GetComponent<UIPanel>();
                    if (_cPanel == null)
                    {
                        this.gameObject.AddComponent<UIPanel>();
                        _cPanel = this.gameObject.GetComponent<UIPanel>();
                    }
                }
                return _cPanel;
            }
        }


        public void Destroy()
        {
            if (cPanel.cachedGameObject.activeInHierarchy) cPanel.cachedGameObject.SetActive(false);

            _cPanel = null;
        }

        public virtual bool StateInPlay() { return true; }
        public virtual bool StateEventReady() { return true; }
        public virtual bool StateEventActivates() { return true; }
    }
}