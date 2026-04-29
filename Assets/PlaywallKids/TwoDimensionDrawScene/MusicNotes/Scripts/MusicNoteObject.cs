using UnityEngine;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public class MusicNoteObject : MonoBehaviour
    {
        public GameObject[] notes;

        public GameObject[] effects;

        public float explodeTime = 0.35f;

        #region Private variables
        // Music Note (inner object)
        private GameObject _musicNote;

        // Animation Time
        private float _animationTime = 0.0f;
        private float _time = 0.0f;

        // This variable defines the current object's animation type. 0 means none.
        private int _animationType = 0;

        // Animation Types
        private const int ANIMATION_SHOW = 1;
        private const int ANIMATION_EXPLODE = 2;
        #endregion

        public void Start()
        {
            if (notes == null || notes.Length == 0)
            {
                Debug.LogError("MusicNoteObject.Start() : The variable \"notes\" are not set.");
                return;
            }
            else
            {
                for (int i = 0; i < notes.Length; i++)
                {
                    if (notes[i] == null)
                    {
                        Debug.LogError(string.Format("MusicNoteObject.Start() : The element {0} of \"notes\" is null.", i));
                        return;
                    }
                }
            }

            _musicNote = (GameObject)Instantiate(notes[Random.Range(0, notes.Length)]);
            _musicNote.SetActive(true);

            _musicNote.transform.parent = transform;
            _musicNote.transform.localPosition = Vector3.zero;
            _musicNote.transform.localRotation = Quaternion.Euler(0, 180, 0);
            _musicNote.transform.localScale = Vector3.one * 0.1f;

            foreach (Transform noteT in _musicNote.transform)
            {
                noteT.gameObject.layer = gameObject.layer;
            }

            _StartAnimationTimer(ANIMATION_SHOW, 1.0f);

            // Rigidbody
            Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
            Rigidbody2D rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
            if (rigidbody == null)
            {
                if (rigidbody2D == null)
                {
                    rigidbody2D = gameObject.AddComponent<Rigidbody2D>();
                }

                rigidbody2D.mass = 1.0f;
                rigidbody2D.gravityScale = 0.0f;
                rigidbody2D.linearDamping = 0.125f;
                rigidbody2D.angularDamping = 2.0f;
                rigidbody2D.interpolation = RigidbodyInterpolation2D.Interpolate;
                rigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            }
            else
            {
                rigidbody.mass = 1.0f;
                rigidbody.useGravity = false;
                rigidbody.linearDamping = 0.125f;
                rigidbody.angularDamping = 2.0f;
                rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
                rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            }

            // Collider
            if (rigidbody != null)
            {
                if (_musicNote.GetComponent<Collider>() == null)
                {
                    var collider = gameObject.AddComponent<SphereCollider>();
                    collider.radius = 15.0f;
                }
            }
            else
            {
                if (_musicNote.GetComponent<Collider2D>() == null)
                {
                    var collider = gameObject.AddComponent<CircleCollider2D>();
                    collider.radius = 15.0f;
                }
            }

            // Sets the music note's color randomly
            InitMaterial();
        }

        public void Update()
        {
            if (_animationType != 0)
            {
                float t = _animationTime / _time;

                if (_animationType == ANIMATION_SHOW)
                {
                    _musicNote.transform.localScale = Vector3.one * (t * 0.9f + 0.1f);
                }
                else if (_animationType == ANIMATION_EXPLODE)
                {
                    _musicNote.transform.localScale = Vector3.one + Vector3.one * 4.0f * t;

                    // Change the color
                    Renderer renderer = _musicNote.GetComponent<Renderer>();
                    if (renderer == null)
                        renderer = _musicNote.GetComponentInChildren<Renderer>();

                    if (renderer != null)
                    {
                        Material material = renderer.material;
                        if (material != null)
                        {
                            if (material.HasProperty("_Color"))
                            {
                                Color color = material.GetColor("_Color");
                                color.a = 1.0f - t;
                                material.SetColor("_Color", color);
                            }
                            renderer.material = material;
                        }
                    }
                }

                _animationTime += Time.deltaTime;

                if (_animationTime >= _time)
                {
                    _animationType = 0;
                }
            }
        }

        public void InitMaterial()
        {
            Renderer renderer = _musicNote.GetComponent<Renderer>();
            if (renderer == null)
                renderer = _musicNote.GetComponentInChildren<Renderer>();

            if (renderer != null)
            {
                Material material = renderer.material;
                if (material != null)
                {
                    if (material.HasProperty("_Color"))
                    {
                        material.SetColor("_Color", new Color(Random.Range(0.4f, 1.0f), Random.Range(0.4f, 1.0f), Random.Range(0.4f, 1.0f)));
                    }
                    renderer.material = material;
                }
            }
        }

        public void Explode()
        {
            if (_animationType == ANIMATION_EXPLODE)
                return;

            GameObject go = gameObject;

            // Turn off the collider and rigidbody
            var colliders2D = go.GetComponents<Collider2D>();
            for (int i = 0, cnt = colliders2D.Length; i < cnt; i++) colliders2D[i].enabled = false;
            colliders2D = go.GetComponentsInChildren<Collider2D>();
            for (int i = 0, cnt = colliders2D.Length; i < cnt; i++) colliders2D[i].enabled = false;
            var colliders = go.GetComponents<Collider>();
            for (int i = 0, cnt = colliders.Length; i < cnt; i++) colliders[i].enabled = false;
            colliders = go.GetComponentsInChildren<Collider>();
            for (int i = 0, cnt = colliders.Length; i < cnt; i++) colliders[i].enabled = false;

            Rigidbody2D rigidbody2D = go.GetComponent<Rigidbody2D>();
            if (rigidbody2D != null) Destroy(rigidbody2D);
            Rigidbody rigidbody = go.GetComponent<Rigidbody>();
            if (rigidbody != null) Destroy(rigidbody);

            // Shows effect
            if (effects != null && effects.Length > 0)
            {
                GameObject prefab = effects[Random.Range(0, effects.Length)];
                if (prefab != null)
                {
                    GameObject effectGo = (GameObject)Instantiate(prefab);
                    effectGo.transform.position = go.transform.position;
                    Destroy(effectGo, 5.0f);
                }
            }

            // Play animation
            _StartAnimationTimer(ANIMATION_EXPLODE, explodeTime);

            // Destroy the game object
            Destroy(go, explodeTime + 0.01f);
        }

        private void _StartAnimationTimer(int animationType, float time)
        {
            _animationType = animationType;
            _time = time;
            _animationTime = 0.0f;
        }
    }
}