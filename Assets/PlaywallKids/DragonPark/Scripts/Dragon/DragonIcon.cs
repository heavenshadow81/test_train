using UnityEngine;

namespace ML.PlaywallKids.DragonPark
{
    public class DragonIcon : MonoBehaviour
    {
        private const string LAYER_NAME = "OtherLayer";

        private new Camera camera;
        private Transform target;

        private GameObject dragon;

        private RenderTexture _texture = null;
        public RenderTexture texture
        {
            get
            {
                if (_texture == null)
                {
                    MakeRenderTexture();
                }
                return _texture;
            }
        }

        public void SetDragon(GameObject character, int depth)
        {
            Init();
            transform.position = Vector3.up * depth * Screen.height;

            dragon = (GameObject)Instantiate(character);

            DragonAnimationControl control = dragon.GetComponent<DragonAnimationControl>();
            if (control != null)
                control.UseTemplete3D();

            Animation characterAnimation = dragon.GetComponent<Animation>();
            if (characterAnimation == null)
            {
                characterAnimation = dragon.GetComponentInChildren<Animation>();
            }

            if (characterAnimation != null)
            {
                characterAnimation.enabled = false;
            }

            Animator characterAnimator = dragon.GetComponent<Animator>();
            if (characterAnimator == null)
            {
                characterAnimator = dragon.GetComponentInChildren<Animator>();
            }

            if (characterAnimator != null)
            {
                characterAnimator.enabled = false;
            }

            DragonAnimationControl dragonAnimation = dragon.GetComponent<DragonAnimationControl>();
            if (dragonAnimation != null)
            {
                dragonAnimation.movesAlongPath = false;
            }

            SetInitTransform(dragon, target);
            ChangeLayersRecursively(dragon.transform, LAYER_NAME);
        }

        private void Init()
        {
            // MakeCamera
            camera = NGUITools.AddChild<Camera>(gameObject);

            SetInitTransform(camera.gameObject, transform);

            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.orthographic = true;
            camera.cullingMask = (1 << LayerMask.NameToLayer(LAYER_NAME));
            camera.orthographicSize = 0.5f;
            camera.nearClipPlane = -10f;
            camera.farClipPlane = 10f;

            // Make RenderTexture
            MakeRenderTexture();


            // Make Target
            target = new GameObject("Target").transform;

            SetInitTransform(target.gameObject, transform);

            target.localRotation = Quaternion.Euler(0f, 180f, 0f);
            target.localPosition = Vector3.down * 0.9f;

            ChangeLayersRecursively(transform, LAYER_NAME);
        }

        public Texture2D texture2D;

        private void MakeRenderTexture()
        {
            _texture = new RenderTexture(128, 128, 10);
            if (camera != null)
            {
                camera.targetTexture = _texture;
                camera.Render();

                texture2D = new Texture2D(128, 128, TextureFormat.RGB24, false);
            }
            else
                Debug.LogWarning("Camera is Null");
        }

        private void SetInitTransform(GameObject go, Transform parent)
        {
            go.transform.parent = parent;

            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.transform.localRotation = Quaternion.identity;
        }

        private void ChangeLayersRecursively(Transform trans, string name)
        {
            trans.gameObject.layer = LayerMask.NameToLayer(name);
            foreach (Transform child in trans)
            {
                ChangeLayersRecursively(child, name);
            }
        }
    }
}