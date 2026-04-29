using UnityEngine;

namespace ML.PlaywallKids.DragonPark
{
    public class AvatarGamePanelCheckDragonUI : MonoBehaviour
    {
        #region Public variables
        public AvatarGamePanel parent;
        public Template3D template;
        public UITexture imageTexture;
        public UILabel nameLabel;
        public TweenScale tweenScale;
        #endregion

        #region Properties
        private int _userSeq = 0;
        public int userSeq
        {
            get
            {
                return _userSeq;
            }
            set
            {
                _userSeq = value;

                nameLabel.text = "";
                if (_userSeq > 0)
                {
                    PlayWallWebServer.GetPicture(_userSeq, (texture) =>
                    {
                        imageTexture.mainTexture = texture;
                    });
                    PlayWallWebServer.GetName(_userSeq, (name) =>
                    {
                        nameLabel.text = name;
                    });
                }
            }
        }
        #endregion

        public void Yes()
        {
            Flag(true);
        }

        public void No()
        {
            Flag(false);
        }

        public void Flag(bool yes)
        {
            tweenScale.PlayReverse();
            AutoDestroy ad = gameObject.AddComponent<AutoDestroy>();
            ad.time = 0.5f;

            parent.Select(template, yes);
        }
    }
}