using UnityEngine;
using UnityEngine.UI;

namespace ML.MLBKids
{
    /// <summary>
    /// Base class of all UI pages.
    /// </summary>
    public class Page : MonoBehaviour
    {
        public virtual bool isShowing { get; private set; }

        public virtual void Show()
        {
            if (!isShowing)
            {
                isShowing = true;
                gameObject.SetActive(true);
            }
        }

        public virtual void Hide()
        {
            if (isShowing)
            {
                isShowing = false;
                gameObject.SetActive(false);
            }
        }

        public virtual void Init() { }
    }
}