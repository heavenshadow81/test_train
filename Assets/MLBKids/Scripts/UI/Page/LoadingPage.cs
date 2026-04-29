using UnityEngine;
using UnityEngine.UI;

namespace ML.MLBKids
{
    public class LoadingPage : Page
    {
        public Text loadingText;

        private float _progress = 0.0f;
        public float progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                loadingText.text = string.Format("{0:0}%", _progress * 100.0f);
            }
        }

        public override void Show()
        {
            base.Show();
            progress = _progress;
        }
    }
}