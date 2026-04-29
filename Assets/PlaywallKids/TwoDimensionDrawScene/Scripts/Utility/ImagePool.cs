using UnityEngine;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public class ImagePool
    {
        readonly string path;
        readonly string fileName;
        CObjectList<UITexture> mImage;
        Texture2D _handImg = null;
        public ImagePool(string _path, string _name)
        {
            if (string.IsNullOrEmpty(_path))
            {
                _path = "";
            }
            else
            {
                path = _path;
            }

            if (string.IsNullOrEmpty(_name))
            {
                Debug.LogError("file not found");
                return;
            }

            fileName = _name;

            mImage = new CObjectList<UITexture>(
                10,
                () =>
                {
                    GameObject _hand = new GameObject();
                    if (_handImg == null) { _handImg = (Texture2D)Resources.Load(path + fileName, typeof(Texture2D)); }
                    _hand.AddComponent<UITexture>();
                    UITexture _texture = _hand.GetComponent<UITexture>();


                    _texture.mainTexture = GameObject.Instantiate(_handImg) as Texture;
                    _hand.SetActive(false);

                    return _texture;
                },

            (UITexture _img) =>
            {
                return !_img.cachedGameObject.activeInHierarchy;
            }
            );
        }

        public UITexture GetImage()
        {

            return mImage.GetObject(); ;
        }

        public void Destroy()
        {
            mImage.Destroy();
        }
    }
}