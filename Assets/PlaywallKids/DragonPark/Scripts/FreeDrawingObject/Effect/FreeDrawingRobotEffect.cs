using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.DragonPark
{
    public class FreeDrawingRobotEffect : MonoBehaviour
    {
        #region Public variables
        public GameObject electricRingPrefab;

        public GameObject electricWavePrefab;

        public GameObject laserPrefab;

        public Texture2D[] laserColorTextures;

        public GameObject laserGunPrefab;

        public Texture2D[] laserGunColorTextures;

        public GameObject laserSparkPrefab;

        public Texture2D[] laserSparkColorTextures;

        public GameObject rocketPrefab;

        public GameObject flashPrefab;

        public GameObject rainbowLightPrefab;

        public AudioClip laserGunSound;

        public float laserSpeed = 3.0f;
        public float laserShowTime = 5.0f;
        public float laserUpdateTime = 10.0f;
        public float laserGunSoundVolume = 1.0f;
        public float antennaEffectTime = 5.0f;
        #endregion

        #region Properties
        private System.WeakReference _robot;
        public FreeDrawingRobotBone robot
        {
            get
            {
                if (_robot == null)
                {
                    FreeDrawingRobotBone robot = GetComponent<FreeDrawingRobotBone>();
                    if (robot != null)
                    {
                        _robot = new System.WeakReference(robot);
                    }
                }

                return (_robot != null ? _robot.Target as FreeDrawingRobotBone : null);
            }
        }

        private System.WeakReference _freeDrawingAnimation;
        public FreeDrawingAnimationControl freeDrawingAnimation
        {
            get
            {
                if (_freeDrawingAnimation == null)
                {
                    FreeDrawingAnimationControl freeDrawingAnimation = GetComponent<FreeDrawingAnimationControl>();
                    if (freeDrawingAnimation != null)
                    {
                        _freeDrawingAnimation = new System.WeakReference(freeDrawingAnimation);
                    }
                }

                return (_freeDrawingAnimation != null ? _freeDrawingAnimation.Target as FreeDrawingAnimationControl : null);
            }
        }
        #endregion

        #region Private variables
        private bool _initialized = false;

        private LineRenderer _leftEyeLaser, _rightEyeLaser;
        private ParticleSystem _leftEyeLaserGun, _rightEyeLaserGun;
        private GameObject _leftEyeLaserSpark, _rightEyeLaserSpark;
        private ParticleSystem[] _leftEyeLaserSparkParticles, _rightEyeLaserSparkParticles;
        private ParticleSystem _electricRing, _electricWave;
        private ParticleSystem _leftFootRocket, _rightFootRocket;
        private AudioSource _laserGunAudioSource;

        private float _laserTime = 0.0f;
        private float _antennaEffectTime = 0.0f;
        private bool _showsLaserGun = false;
        #endregion

        #region Initialization
        public void Awake()
        {
            Init();
        }

        public void Init()
        {
            if (_initialized == false && robot != null)
            {
                _initialized = true;
                _InitLasers();
                _InitLaserSmokes();
                _InitAntennaEffects();
                _InitRocketEffects();
            }
        }

        private void _InitLasers()
        {
            LineRenderer[] lasers = new LineRenderer[2];
            ParticleSystem[] laserGuns = new ParticleSystem[2];
            string[] eyeBones = FreeDrawingRobotBone.eyeBoneNames;
            for (int i = 0; i < 2; i++)
            {
                if (laserPrefab != null)
                {
                    GameObject go = (GameObject)Instantiate(laserPrefab);
                    lasers[i] = go.GetComponent<LineRenderer>();
                    lasers[i].transform.parent = transform;
                    lasers[i].enabled = false;

                    Material mat = lasers[i].material;
                    lasers[i].material = mat;
                }

                if (laserGunPrefab != null)
                {
                    GameObject go = (GameObject)Instantiate(laserGunPrefab);
                    laserGuns[i] = go.GetComponent<ParticleSystem>();
                    laserGuns[i].transform.parent = transform;
                    var emission = laserGuns[i].emission;
                    emission.enabled = false;
                    laserGuns[i].Play();

                    Material mat = laserGuns[i].GetComponent<Renderer>().material;
                    laserGuns[i].GetComponent<Renderer>().material = mat;
                }
            }

            _leftEyeLaser = lasers[0];
            _rightEyeLaser = lasers[1];
            _leftEyeLaserGun = laserGuns[0];
            _rightEyeLaserGun = laserGuns[1];

            _laserGunAudioSource = GetComponent<AudioSource>();
            if (_laserGunAudioSource == null)
            {
                _laserGunAudioSource = gameObject.AddComponent<AudioSource>();
            }
            _laserGunAudioSource.playOnAwake = false;
            _laserGunAudioSource.loop = true;
            _laserGunAudioSource.Stop();

            Attach(FreeDrawingRobotBone.kEyeLBone);
            Attach(FreeDrawingRobotBone.kEyeRBone);
        }

        private void _InitLaserSmokes()
        {
            if (laserSparkPrefab == null) return;

            GameObject[] laserSmokeGOs = new GameObject[2];
            string[] eyeBones = FreeDrawingRobotBone.eyeBoneNames;
            for (int i = 0; i < 2; i++)
            {
                GameObject go = (GameObject)Instantiate(laserSparkPrefab);
                laserSmokeGOs[i] = go;
                laserSmokeGOs[i].transform.parent = transform;
            }

            _leftEyeLaserSpark = laserSmokeGOs[0];
            _rightEyeLaserSpark = laserSmokeGOs[1];

            _leftEyeLaserSparkParticles = _leftEyeLaserSpark.GetComponentsInChildren<ParticleSystem>();
            _rightEyeLaserSparkParticles = _rightEyeLaserSpark.GetComponentsInChildren<ParticleSystem>();

            if (_leftEyeLaserSparkParticles != null)
            {
                foreach (ParticleSystem ps in _leftEyeLaserSparkParticles)
                {
                    var emission = ps.emission;
                    emission.enabled = false;
                    ps.Play();

                    Material mat = ps.GetComponent<Renderer>().material;
                    ps.GetComponent<Renderer>().material = mat;
                }
            }
            if (_rightEyeLaserSparkParticles != null)
            {
                foreach (ParticleSystem ps in _rightEyeLaserSparkParticles)
                {
                    var emission = ps.emission;
                    emission.enabled = false;
                    ps.Play();

                    Material mat = ps.GetComponent<Renderer>().material;
                    ps.GetComponent<Renderer>().material = mat;
                }
            }

            Attach(FreeDrawingRobotBone.kEyeLBone);
            Attach(FreeDrawingRobotBone.kEyeRBone);
        }

        private void _InitAntennaEffects()
        {
            GameObject go;

            go = (GameObject)Instantiate(electricRingPrefab);
            _electricRing = go.GetComponent<ParticleSystem>();
            go = (GameObject)Instantiate(electricWavePrefab);
            _electricWave = go.GetComponent<ParticleSystem>();

            Attach(FreeDrawingRobotBone.kAntennaBone);
        }

        private void _InitRocketEffects()
        {
            ParticleSystem[] rockets = new ParticleSystem[2];
            string[] footBones = { FreeDrawingRobotBone.kFootLBone, FreeDrawingRobotBone.kFootRBone };

            if (rocketPrefab != null)
            {
                for (int i = 0; i < 2; i++)
                {
                    GameObject go = (GameObject)Instantiate(rocketPrefab);
                    rockets[i] = go.GetComponent<ParticleSystem>();
                    rockets[i].transform.parent = transform;
                    rockets[i].Stop(true);
                }
            }

            _leftFootRocket = rockets[0];
            _rightFootRocket = rockets[1];

            Attach(FreeDrawingRobotBone.kFootLBone);
            Attach(FreeDrawingRobotBone.kFootRBone);
        }
        #endregion

        #region Attach/Detach
        public void Attach()
        {
            Attach(FreeDrawingRobotBone.kEyeLBone);
            Attach(FreeDrawingRobotBone.kEyeRBone);
            Attach(FreeDrawingRobotBone.kFootLBone);
            Attach(FreeDrawingRobotBone.kFootRBone);
            Attach(FreeDrawingRobotBone.kAntennaBone);
        }

        public void Detach()
        {
            Detach(FreeDrawingRobotBone.kEyeLBone);
            Detach(FreeDrawingRobotBone.kEyeRBone);
            Detach(FreeDrawingRobotBone.kFootLBone);
            Detach(FreeDrawingRobotBone.kFootRBone);
            Detach(FreeDrawingRobotBone.kAntennaBone);
        }

        public void Attach(string boneName)
        {
            if (string.IsNullOrEmpty(boneName)) return;

            List<Component> targetEffects = _GetTargetEffects(boneName);

            Transform bone = robot.GetBone(boneName);

            GameObject acc = robot.GetAccessory(boneName);

            if (acc != null)
            {
                _AttachEffects(boneName, acc, targetEffects);
            }
            else if (bone != null)
            {
                _AttachEffects(boneName, bone.gameObject, targetEffects);
            }
            else
            {
                _DisableEffects(targetEffects);
            }
        }

        public void Detach(string boneName)
        {
            if (string.IsNullOrEmpty(boneName)) return;

            List<Component> targetEffects = _GetTargetEffects(boneName);

            _DisableEffects(targetEffects);
        }

        private void _AttachEffects(string boneName, GameObject acc, List<Component> effects)
        {
            if (string.IsNullOrEmpty(boneName) || acc == null || effects == null || effects.Count == 0) return;

            Transform parent = acc.transform;

            if (boneName.Equals(FreeDrawingRobotBone.kAntennaBone))
            {
                FreeDrawingRobotAntenna antenna = acc.GetComponent<FreeDrawingRobotAntenna>();
                if (antenna != null)
                {
                    Transform tailBone = antenna.GetBone(FreeDrawingRobotAntenna.kTailBone);
                    parent = tailBone;
                }
                else
                {
                    Transform[] tfs = acc.GetComponentsInChildren<Transform>();
                    foreach (Transform tf in tfs)
                    {
                        if (tf.name.Equals("Dummy_Antenna_Tail"))
                        {
                            parent = tf;
                            break;
                        }
                    }
                }
            }

            _EnableEffects(parent, effects);
        }

        private void _EnableEffects(Transform parent, List<Component> effects)
        {
            foreach (Component c in effects)
            {
                if (c != null)
                {
                    c.transform.parent = parent;
                    c.transform.localPosition = Vector3.zero;
                    c.transform.localRotation = Quaternion.identity;
                    c.transform.localScale = Vector3.one;
                    c.gameObject.SetActive(true);
                }
            }
        }

        private void _DisableEffects(List<Component> effects)
        {
            foreach (Component c in effects)
            {
                if (c != null)
                {
                    c.transform.parent = transform;
                    c.gameObject.SetActive(false);
                }
            }
        }

        private List<Component> _GetTargetEffects(string boneName)
        {
            List<Component> targetEffects = new List<Component>();

            if (!string.IsNullOrEmpty(boneName))
            {
                if (boneName.Equals(FreeDrawingRobotBone.kAntennaBone))
                {
                    if (_electricRing != null) targetEffects.Add(_electricRing);
                    if (_electricWave != null) targetEffects.Add(_electricWave);
                }
                else if (boneName.Equals(FreeDrawingRobotBone.kEyeLBone))
                {
                    if (_leftEyeLaser != null) targetEffects.Add(_leftEyeLaser);
                    if (_leftEyeLaserSparkParticles != null) targetEffects.AddRange(_leftEyeLaserSparkParticles);
                    if (_leftEyeLaserGun != null) targetEffects.Add(_leftEyeLaserGun);
                }
                else if (boneName.Equals(FreeDrawingRobotBone.kEyeRBone))
                {
                    if (_rightEyeLaser != null) targetEffects.Add(_rightEyeLaser);
                    if (_rightEyeLaserSparkParticles != null) targetEffects.AddRange(_rightEyeLaserSparkParticles);
                    if (_rightEyeLaserGun != null) targetEffects.Add(_rightEyeLaserGun);
                }
                else if (boneName.Equals(FreeDrawingRobotBone.kFootLBone))
                {
                    if (_leftFootRocket != null) targetEffects.Add(_leftFootRocket);
                }
                else if (boneName.Equals(FreeDrawingRobotBone.kFootRBone))
                {
                    if (_rightFootRocket != null) targetEffects.Add(_rightFootRocket);
                }
            }

            return targetEffects;
        }
        #endregion

        #region Update
        // Update is called once per frame
        void Update()
        {
            if (robot != null)
            {
                _UpdateLasers();
                _UpdateAntennaEffects();
                _UpdateRockets();
            }
        }

        private void _UpdateLasers()
        {
            // hide laser
            if (_laserTime < laserUpdateTime - laserShowTime)
            {
                if (_leftEyeLaser != null)
                {
                    _leftEyeLaser.enabled = false;

                    if (laserColorTextures != null && laserColorTextures.Length > 0)
                    {
                        if (laserColorTextures.Length > robot.color)
                        {
                            _leftEyeLaser.material.mainTexture = laserColorTextures[robot.color];
                        }
                        else
                        {
                            _leftEyeLaser.material.mainTexture = laserColorTextures[0];
                        }
                    }
                }
                if (_rightEyeLaser != null)
                {
                    _rightEyeLaser.enabled = false;

                    if (laserColorTextures != null && laserColorTextures.Length > 0)
                    {
                        if (laserColorTextures.Length > robot.color)
                        {
                            _rightEyeLaser.material.mainTexture = laserColorTextures[robot.color];
                        }
                        else
                        {
                            _rightEyeLaser.material.mainTexture = laserColorTextures[0];
                        }
                    }
                }

                if (_leftEyeLaserGun != null)
                {
                    var emission = _leftEyeLaserGun.emission;
                    emission.enabled = false;

                    if (laserGunColorTextures != null && laserGunColorTextures.Length > 0)
                    {
                        if (laserGunColorTextures.Length > robot.color)
                        {
                            _leftEyeLaserGun.GetComponent<Renderer>().material.mainTexture = laserGunColorTextures[robot.color];
                        }
                        else
                        {
                            _leftEyeLaserGun.GetComponent<Renderer>().material.mainTexture = laserGunColorTextures[0];
                        }
                    }
                }
                if (_rightEyeLaserGun != null)
                {
                    var emission = _rightEyeLaserGun.emission;
                    emission.enabled = false;

                    if (laserGunColorTextures != null && laserGunColorTextures.Length > 0)
                    {
                        if (laserGunColorTextures.Length > robot.color)
                        {
                            _rightEyeLaserGun.GetComponent<Renderer>().material.mainTexture = laserGunColorTextures[robot.color];
                        }
                        else
                        {
                            _rightEyeLaserGun.GetComponent<Renderer>().material.mainTexture = laserGunColorTextures[0];
                        }
                    }
                }
                if (_laserGunAudioSource != null)
                {
                    _laserGunAudioSource.Stop();
                }

                if (_leftEyeLaserSpark != null)
                {
                    foreach (ParticleSystem ps in _leftEyeLaserSparkParticles)
                    {
                        var emission = ps.emission;
                        emission.enabled = false;

                        if (laserSparkColorTextures != null && laserSparkColorTextures.Length > 0)
                        {
                            if (laserSparkColorTextures.Length > robot.color)
                            {
                                ps.GetComponent<Renderer>().material.mainTexture = laserSparkColorTextures[robot.color];
                            }
                            else
                            {
                                ps.GetComponent<Renderer>().material.mainTexture = laserSparkColorTextures[0];
                            }
                        }
                    }
                }
                if (_rightEyeLaserSpark != null)
                {
                    foreach (ParticleSystem ps in _rightEyeLaserSparkParticles)
                    {
                        var emission = ps.emission;
                        emission.enabled = false;

                        if (laserSparkColorTextures != null && laserSparkColorTextures.Length > 0)
                        {
                            if (laserSparkColorTextures.Length > robot.color)
                            {
                                ps.GetComponent<Renderer>().material.mainTexture = laserSparkColorTextures[robot.color];
                            }
                            else
                            {
                                ps.GetComponent<Renderer>().material.mainTexture = laserSparkColorTextures[0];
                            }
                        }
                    }
                }
            }
            // show laser
            else
            {
                float currentShowTime = _laserTime - (laserUpdateTime - laserShowTime);

                foreach (string eyeBoneName in FreeDrawingRobotBone.eyeBoneNames)
                {
                    if (_showsLaserGun)
                    {
                        _ShowLaserBeam(eyeBoneName, currentShowTime);
                    }
                    else
                    {
                        _ShowLaserGun(eyeBoneName, currentShowTime);
                    }
                }
            }

            _laserTime = _laserTime + Time.deltaTime;
            if (_laserTime >= laserUpdateTime)
            {
                _laserTime -= laserUpdateTime;

                _showsLaserGun = (_leftEyeLaserGun != null ? !_showsLaserGun : false);
            }
        }

        private void _ShowLaserBeam(string eyeBoneName, float currentShowTime)
        {
            GameObject acc = robot.GetAccessory(eyeBoneName);
            if (acc != null)
            {
                Transform accTf = acc.transform;
                Vector3 forward = accTf.forward;

                LineRenderer laser = (eyeBoneName.Equals(FreeDrawingRobotBone.kEyeLBone) ? _leftEyeLaser : _rightEyeLaser);
                if (laser != null)
                {
                    Vector3 from = accTf.position;
                    Vector3 to = from;

                    // hit test
                    RaycastHit hitInfo;
                    if (Physics.Raycast(accTf.position, forward, out hitInfo, currentShowTime * laserSpeed))
                    {
                        // hit point
                        to = hitInfo.point;

                        // play laser spark effect
                        if (laser == _leftEyeLaser && _leftEyeLaserSpark != null)
                        {
                            foreach (ParticleSystem ps in _leftEyeLaserSparkParticles)
                            {
                                var emission = ps.emission;
                                emission.enabled = true;
                            }
                            _leftEyeLaserSpark.transform.position = to;
                            _leftEyeLaserSpark.transform.rotation = Quaternion.Euler(-90, 0, 0);
                        }
                        else if (_rightEyeLaserSpark != null)
                        {
                            foreach (ParticleSystem ps in _rightEyeLaserSparkParticles)
                            {
                                var emission = ps.emission;
                                emission.enabled = true;
                            }
                            _rightEyeLaserSpark.transform.position = to;
                            _rightEyeLaserSpark.transform.rotation = Quaternion.Euler(-90, 0, 0);
                        }
                    }
                    else
                    {
                        to = from + forward * currentShowTime * laserSpeed;

                        // hide laser spark effect
                        if (laser == _leftEyeLaser && _leftEyeLaserSpark != null)
                        {
                            foreach (ParticleSystem ps in _leftEyeLaserSparkParticles)
                            {
                                var emission = ps.emission;
                                emission.enabled = true;
                            }
                        }
                        else if (_rightEyeLaserSpark != null)
                        {
                            foreach (ParticleSystem ps in _rightEyeLaserSparkParticles)
                            {
                                var emission = ps.emission;
                                emission.enabled = true;
                            }
                        }
                    }

                    laser.enabled = true;
                    laser.positionCount = 2;
                    laser.SetPosition(0, from);
                    laser.SetPosition(1, to);
                }
            }
        }

        private void _ShowLaserGun(string eyeBoneName, float currentShowTime)
        {
            GameObject acc = robot.GetAccessory(eyeBoneName);
            if (acc != null)
            {
                Transform accTf = acc.transform;
                Vector3 forward = accTf.forward;

                ParticleSystem laserGun = (eyeBoneName.Equals(FreeDrawingRobotBone.kEyeLBone) ? _leftEyeLaserGun : _rightEyeLaserGun);
                if (laserGun != null)
                {
                    var emission = laserGun.emission;
                    emission.enabled = true;
                    if (_laserGunAudioSource != null)
                    {
                        _laserGunAudioSource.clip = laserGunSound;
                        _laserGunAudioSource.volume = laserGunSoundVolume;
                        if (_laserGunAudioSource.isPlaying == false)
                        {
                            _laserGunAudioSource.Play();
                        }
                    }
                }
            }
        }

        private void _UpdateAntennaEffects()
        {
            if (_electricRing == null || _electricWave == null) return;

            var emissionRing = _electricRing.emission;
            var emissionWave = _electricWave.emission;

            if (!emissionRing.enabled && !emissionWave.enabled)
            {
                emissionRing.enabled = true;
                _electricRing.Play();
            }
            else if (emissionRing.enabled && emissionWave.enabled)
            {
                emissionWave.enabled = false;
            }

            _antennaEffectTime += Time.deltaTime;

            if (_antennaEffectTime >= antennaEffectTime)
            {
                if (emissionRing.enabled)
                {
                    emissionRing.enabled = false;
                    emissionWave.enabled = true;
                    _electricWave.Play();
                }
                else
                {
                    emissionWave.enabled = false;
                    emissionRing.enabled = true;
                    _electricRing.Play();
                }

                _antennaEffectTime -= antennaEffectTime;
            }
        }

        private void _UpdateRockets()
        {
            if (_leftFootRocket != null)
            {
                if (freeDrawingAnimation != null && (freeDrawingAnimation.landing || freeDrawingAnimation.rising))
                {
                    if (_leftFootRocket.isPlaying == false)
                    {
                        _leftFootRocket.transform.localRotation = Quaternion.Euler(90, 0, 0);
                        _leftFootRocket.Play(true);
                    }
                }
                else
                {
                    if (_leftFootRocket.isPlaying)
                    {
                        _leftFootRocket.Stop(true);
                    }
                }
            }

            if (_rightFootRocket != null)
            {
                if (freeDrawingAnimation != null && (freeDrawingAnimation.landing || freeDrawingAnimation.rising))
                {
                    if (_rightFootRocket.isPlaying == false)
                    {
                        _rightFootRocket.transform.localRotation = Quaternion.Euler(90, 0, 0);
                        _rightFootRocket.Play(true);
                    }
                }
                else
                {
                    if (_rightFootRocket.isPlaying)
                    {
                        _rightFootRocket.Stop(true);
                    }
                }
            }
        }
        #endregion
    }
}