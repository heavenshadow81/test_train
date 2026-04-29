using UnityEngine;

namespace ML.PlaywallKids.DragonPark
{
    public class AToolStartPanelBalloon : MonoBehaviour
    {
        #region Public variables
        public UITexture imageTexture;
        public UILabel nameLabel;
        public Renderer balloon;
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

        #region Private variables
        private float _time = 0.0f;
        private float _currentTime = 0.0f;
        private Material _material;
        #endregion

        public void Start()
        {
            _material = balloon.material;
            balloon.material = _material;
            _material.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
        }

        public void OnDestroy()
        {
            if (_material.mainTexture != null)
            {
                Destroy(_material.mainTexture);
                _material.mainTexture = null;
            }
        }

        public void Pop(float time)
        {
            imageTexture.enabled = false;
            nameLabel.enabled = false;
            _time = time;
        }

        public void Update()
        {
            if (_time > 0.0f)
            {
                if (_currentTime >= _time)
                {
                    Destroy(gameObject);
                }
                else
                {
                    Color color = _material.color;
                    color.a -= Time.deltaTime / _time;
                    _material.color = color;
                    _currentTime += Time.deltaTime;
                }
            }
        }
    }
}