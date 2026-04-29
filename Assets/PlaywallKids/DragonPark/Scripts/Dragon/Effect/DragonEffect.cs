using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.DragonPark
{
    /// <summary>
    /// The effect manager of the dragon object.
    /// </summary>
    [RequireComponent(typeof(DragonAnimationControl))]
    public class DragonEffect : MonoBehaviour
    {
        #region Public variables
        /// <summary>
        /// Smoke effect. Used in ground.
        /// </summary>
        public GameObject smokePrefab;

        public GameObject zzzPrefab;

        public GameObject kissPrefab;

        public GameObject speedPrefab;

        public GameObject heartPrefab;

        public GameObject sighPrefab;

        public GameObject exclamationPrefab;

        public GameObject popPrefab;
        #endregion

        #region Properties
        public DragonAnimationControl dragonAnimation { get; private set; }
        #endregion

        #region Private variables
        private ParticleSystem _smoke;
        private ParticleSystem _zzz;

        private Dictionary<string, List<ParticleSystem>> _instantParticleListDict = new Dictionary<string, List<ParticleSystem>>();
        private Dictionary<string, GameObject> _instantEffectGODict = new Dictionary<string, GameObject>();
        #endregion

        #region Constants
        public const string kParticleNameZzz = "zzz";
        public const string kParticleNameKiss = "kiss";
        public const string kParticleNameHeart = "heart";
        public const string kParticleNameSigh = "sigh";
        public const string kParticleNameExclamation = "exclamation";
        public const string kParticleNamePop = "pop";
        #endregion

        public virtual void Start()
        {
            dragonAnimation = GetComponent<DragonAnimationControl>();

            if (smokePrefab != null)
            {
                DragonPath path = dragonAnimation.path;

                if ((path != null && !path.GetType().Equals(typeof(DragonSkyPath))) || path == null)
                {
                    GameObject smokeGO = Instantiate(smokePrefab);
                    smokeGO.SetActive(true);

                    smokeGO.transform.parent = transform;
                    smokeGO.transform.localPosition = new Vector3(0, 0.05f, 0);

                    _smoke = smokeGO.GetComponent<ParticleSystem>();
                }
            }

            if (zzzPrefab != null)
            {
                GameObject go = Instantiate(zzzPrefab);
                go.SetActive(true);

                go.transform.parent = transform;
                go.transform.position = _GetPosition(new Vector3(-0.35f, 0.1f, 0.0f), BoneObject.kHeadBone);

                _zzz = go.GetComponent<ParticleSystem>();
            }
        }

        public virtual void Update()
        {
            UpdateSmoke();
            UpdateZzz();
            UpdateInstantParticles();

#if UNITY_EDITOR
            // Debug
            if (Input.GetKeyDown(KeyCode.K))
            {
                Kiss();
            }

            // Debug
            if (Input.GetKeyDown(KeyCode.P))
            {
                Pop();
            }
#endif
        }

        public virtual void UpdateSmoke()
        {
            if (_smoke != null)
            {
                float speed = dragonAnimation.speed;

                if (speed <= 1.2f)
                {
                    var emission = _smoke.emission;
                    emission.enabled = false;
                }
                else
                {
                    var emission = _smoke.emission;
                    emission.enabled = true;
                    emission.rateOverTime = (speed - 1.2f) * 4.0f + 1.0f;
                }
            }
        }

        public virtual void UpdateZzz()
        {
            if (_zzz != null)
            {
                if (dragonAnimation.isSleeping && !name.Contains("Cougar"))
                {
                    var emission = _zzz.emission;
                    emission.enabled = true;
                }
                else
                {
                    var emission = _zzz.emission;
                    emission.enabled = false;
                }
            }
        }

        public virtual void UpdateInstantParticles()
        {
            foreach (string key in _instantParticleListDict.Keys)
            {
                List<ParticleSystem> particles = _instantParticleListDict[key];
                for (int i = 0; i < particles.Count; i++)
                {
                    ParticleSystem particle = particles[i];
                    if (particle.emission.enabled)
                    {
                        if (!particle.isPlaying)
                        {
                            var emission = particle.emission;
                            emission.enabled = false;
                        }
                    }
                }
            }
        }

        public void Kiss()
        {
            _Effect(kParticleNameKiss, kissPrefab, new Vector3(0.0f, 0.8f, 1.0f));
        }

        public void Heart()
        {
            _Effect(kParticleNameHeart, heartPrefab, new Vector3(0.0f, 1.2f, 0.0f));
        }

        public void Sigh()
        {
            _Effect(kParticleNameSigh, sighPrefab, new Vector3(0.0f, 0.0f, 0.5f));
        }

        public void Exclamation()
        {
            _Effect(kParticleNameExclamation, exclamationPrefab, new Vector3(0, 1.2f, 0));
        }

        public void Pop()
        {
            GameObject go = Instantiate(popPrefab, transform.position + Vector3.up, Quaternion.identity);
            go.SetActive(true);

            ParticleSystem particle = go.GetComponent<ParticleSystem>();
            particle.Play();

            Destroy(go, particle.main.duration + 0.5f);
        }

        private void _Effect(string key, GameObject prefab, Vector3 localPosition, string boneName = null)
        {
            if (string.IsNullOrEmpty(key) || prefab == null) return;

            // should we create new particle instance?
            bool flag = true;

            // position (world)
            Vector3 position = _GetPosition(localPosition, boneName);

            // enumerate existing instantiated particles
            // if any particle is not playing, just reuse!
            List<ParticleSystem> particles = null;
            if (_instantParticleListDict.ContainsKey(key))
            {
                particles = _instantParticleListDict[key];
            }
            else
            {
                particles = new List<ParticleSystem>();
                _instantParticleListDict[key] = particles;
            }

            for (int i = 0; i < particles.Count; i++)
            {
                ParticleSystem particle = particles[i];
                if (!particle.emission.enabled)
                {
                    var emission = particle.emission;
                    emission.enabled = true;
                    particle.Play();
                    particle.transform.position = position;

                    flag = false;
                    break;
                }
            }

            // Create new particle
            if (flag)
            {
                GameObject go = Instantiate(prefab, position, Quaternion.identity);
                go.SetActive(true);

                ParticleSystem particle = go.GetComponent<ParticleSystem>();
                if (particle != null)
                {
                    particles.Add(particle);
                }
                else
                {
                    Destroy(go);
                }
            }
        }

        private Vector3 _GetPosition(Vector3 localPosition, string boneName = null)
        {
            Vector3 position = transform.TransformPoint(localPosition);

            // If bone name is specified, find the bone and check the position
            if (!string.IsNullOrEmpty(boneName))
            {
                BoneObject boneObject = GetComponentInChildren<BoneObject>();
                if (boneObject != null)
                {
                    Transform bone = boneObject.GetBone(boneName);
                    if (bone != null)
                    {
                        position = bone.TransformPoint(localPosition);
                    }
                }
            }

            return position;
        }
    }
}