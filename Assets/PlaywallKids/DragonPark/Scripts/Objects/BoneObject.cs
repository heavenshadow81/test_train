using UnityEngine;
using System;
using System.Collections.Generic;

namespace ML.PlaywallKids.DragonPark
{
    /// <summary>
    /// Base class of ALL skeleton-based objects (Dragon, Avatar, etc...)
    /// This class contains...
    /// 1. Setting bone scales
    /// 2. Attaching/Detaching accessory objects
    /// 3. Getting bone dummies
    /// Simple. :-)
    /// </summary>
    public class BoneObject : MonoBehaviour
    {
        #region Public variables
        public string characterName = "";
        #endregion

        #region Private variables
        // bones
        private Transform _rootBone = null;
        private Dictionary<string, Transform> _boneDict = new Dictionary<string, Transform>();
        [Serializable]
        protected class _BoneScaleDictionary : SerializableDictionary<string, float> { }
        [SerializeField]
        private _BoneScaleDictionary _boneScaleDict = new _BoneScaleDictionary();

        // accessories
        [Serializable]
        protected class _AccessoryDictionary : SerializableDictionary<string, GameObject> { }
        [SerializeField]
        private _AccessoryDictionary _accessoryDict = new _AccessoryDictionary();
        #endregion

        #region Constants
        // bone names
        public const string kHeadBone = "head";
        public const string kBodyBone = "bone";
        public const string kArmLBone = "arm_l";
        public const string kArmRBone = "arm_r";
        public const string kWingLBone = "wing_l";
        public const string kWingRBone = "wing_r";
        public const string kTailBone = "tail";
        public const string kManteauLBone = "manteau_l";
        public const string kManteauRBone = "manteau_r";
        public const string kFootstepBone = "footstep";
        public const string kHeadNubBone = "headnub";
        #endregion

        public virtual void Start()
        {
            _SetupBones();
            _SetupAccs();
        }

        public virtual void OnDestroy()
        {
            foreach (string key in _accessoryDict.Keys)
            {
                GameObject acc = _accessoryDict[key];
                if (acc != null)
                {
                    Destroy(acc);
                }
            }
        }

        protected virtual Transform _GetRootBone()
        {
            if (_rootBone == null)
            {
                _rootBone = transform.Find("Bip01");
                if (_rootBone == null)
                {
                    _rootBone = transform.Find("Bip02");
                }
                if (_rootBone == null)
                {
                    _rootBone = transform.Find("Bip001");
                }
                if (_rootBone == null)
                {
                    _rootBone = transform.Find("Arrow_Bip02");
                }
            }
            return _rootBone;
        }

        protected virtual Dictionary<string, Transform> _GetBoneDict()
        {
            if (_rootBone == null)
            {
                _rootBone = _GetRootBone();
            }

            if (_rootBone != null && _boneDict.Count == 0)
            {
                Transform[] hierarchy = _rootBone.GetComponentsInChildren<Transform>();

                foreach (Transform t in hierarchy)
                {
                    string boneName = "";

                    switch (t.name)
                    {
                        case "Bip01 Head":
                        case "Arrow_Bip01 Head":
                        case "Bip001 Head":
                            boneName = kHeadBone;
                            break;
                        case "Bip01 Spine":
                        case "Arrow_Bip01 Spine":
                        case "Bip001 Spine":
                            boneName = kBodyBone;
                            break;
                        case "Bip01 Tail":
                        case "Arrow_Bip01 Tail":
                        case "Bip001 Tail":
                            boneName = kTailBone;
                            break;
                        case "Bip01 L Clavicle":
                        case "Arrow_Bip01 L Clavicle":
                        case "Bip001 L Clavicle":
                            boneName = kArmLBone;
                            break;
                        case "Bip01 R Clavicle":
                        case "Arrow_Bip01 R Clavicle":
                        case "Bip001 R Clavicle":
                            boneName = kArmRBone;
                            break;
                        case "Bip001Wing_Bone01":
                        case "Bip001Wing_Bone001":
                        case "LeftWing_01":
                        case "Bone04":
                            boneName = kWingLBone;
                            break;
                        case "Bip001Wing_Bone04":
                        case "Bip001Wing_Bone004":
                        case "RightWing_01":
                        case "Bone01":
                            boneName = kWingRBone;
                            break;
                        case "Bip001Manteau_Bone01":
                        case "Bip001Manteau_Bone001":
                        case "LeftManteau_01":
                            boneName = kManteauLBone;
                            break;
                        case "Bip001Manteau_Bone04":
                        case "Bip001Manteau_Bone004":
                        case "LeftManteau_04":
                            boneName = kManteauRBone;
                            break;
                        case "Bip01 Footsteps":
                        case "Bip001 Footsteps":
                            boneName = kFootstepBone;
                            break;
                        case "Bip01 HeadNub":
                        case "Bip001 HeadNub":
                        case "Cougar_Bip01 HeadNub":
                            boneName = kHeadNubBone;
                            break;
                    }

                    if (!string.IsNullOrEmpty(boneName))
                    {
                        _boneDict[boneName] = t;
                    }
                }
            }
            return _boneDict;
        }

        private void _SetupBones()
        {
            _rootBone = _GetRootBone();
            _boneDict = _GetBoneDict();

            foreach (string boneName in _boneDict.Keys)
            {
                Transform t = _boneDict[boneName];

                if (_boneScaleDict.ContainsKey(boneName))
                {
                    float val = _boneScaleDict[boneName];
                    if (t != null)
                    {
                        t.localScale = new Vector3(val, val, val);
                    }
                }
            }
        }

        private void _SetupAccs()
        {
            List<string> keys = new List<string>(_accessoryDict.Keys);
            foreach (string key in keys)
            {
                GameObject go = _accessoryDict[key];
                SetAccessory(go, false);
            }
        }

        public float GetBoneScale(string boneName)
        {
            float ret = 0.0f;

            if (_boneScaleDict.ContainsKey(boneName))
            {
                ret = _boneScaleDict[boneName];
            }
            else
            {
                Transform t = null;
                if (_boneDict.ContainsKey(boneName) && _boneDict[boneName] != null)
                {
                    t = _boneDict[boneName];
                }

                if (t != null)
                {
                    Vector3 localScale = t.localScale;
                    ret = (localScale.x + localScale.y + localScale.z) / 3.0f;
                    _boneScaleDict[boneName] = ret;
                }
            }
            return ret;
        }

        public Dictionary<string, float> GetBoneScales()
        {
            Dictionary<string, float> dict = new Dictionary<string, float>();
            foreach (string key in _boneDict.Keys)
            {
                if (_boneDict[key] != null)
                {
                    dict[key] = GetBoneScale(key);
                }
            }
            return dict;
        }

        public void SetBoneScale(string boneName, float value)
        {
            _boneScaleDict[boneName] = value;

            Transform t = null;
            if (_boneDict.ContainsKey(boneName) && _boneDict[boneName] != null)
            {
                t = _boneDict[boneName];
            }

            if (t != null)
            {
                Vector3 localScale = new Vector3(value, value, value);
                t.localScale = localScale;
            }

            //TCCamera.sharedInstance.RequestRefreshTCRT();
        }

        public GameObject GetAccessory(string name)
        {
            GameObject go = null;

            _accessoryDict.TryGetValue(name, out go);

            return go;
        }

        public List<string> GetAccessoryNames()
        {
            List<string> accessoryNames = new List<string>();

            foreach (string key in _accessoryDict.Keys)
            {
                GameObject go = _accessoryDict[key];

                accessoryNames.Add(AccessoryManager.GetAccessoryName(go));
            }

            return accessoryNames;
        }

        public void SetAccessory(GameObject go)
        {
            SetAccessory(go, true);
        }

        public void SetAccessory(GameObject go, bool removePrev)
        {
            if (go == null) return;

            go.layer = gameObject.layer;
            foreach (Transform t in go.transform)
            {
                t.gameObject.layer = gameObject.layer;
            }

            Transform dummy = AccessoryManager.GetDummy(go, characterName);
            if (dummy != null)
            {
                string boneName = "";

                // Head
                if (dummy.name.Contains("Head") || dummy.name.Contains("Hair"))
                {
                    boneName = kHeadBone;
                }
                // Body
                else if (dummy.name.Contains("Body"))
                {
                    boneName = kBodyBone;
                }

                if (!string.IsNullOrEmpty(boneName))
                {
                    SetAccessory(boneName, go, removePrev);
                }
            }
        }

        public virtual void SetAccessory(string boneName, GameObject go, bool removePrev)
        {
            Transform bone = GetBone(boneName);
            Transform dummy = AccessoryManager.GetDummy(go, characterName);

            // remove prev
            GameObject prev = GetAccessory(boneName);
            if (prev != null)
            {
                if (removePrev)
                {
                    Destroy(prev);
                }
            }

            if (bone != null)
            {
                // set scale equally
                go.transform.parent = transform.parent;
                go.transform.localScale = transform.localScale;

                if (dummy != null)
                {
                    // attach dummy to bone
                    dummy.transform.parent = bone;

                    // attach object to dummy
                    go.transform.parent = dummy;

                    dummy.localPosition = Vector3.zero;
                    dummy.localRotation = Quaternion.Euler(-90, 90, 0); // 3Ds MAX -> Unity (different coordinate system :-/)

                    // for avatar
                    if (dummy.name.Contains("Man") || dummy.name.Contains("Girl"))
                    {
                        dummy.localRotation = Quaternion.Euler(0, -90, 180);
                    }

                    // sync local size
                    dummy.localScale = new Vector3(1.0f, 1.0f, 1.0f);

                    // attach accessory to bone and detach dummy
                    go.transform.parent = bone;
                    dummy.transform.parent = go.transform;
                }
                else
                {
                    go.transform.parent = bone;
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                }
            }

            _accessoryDict[boneName] = go;
        }

        public Transform GetBone(string boneName)
        {
            if (_boneDict.Count == 0)
            {
                _SetupBones();
            }

            Transform t = null;

            _boneDict.TryGetValue(boneName, out t);

            return t;
        }
    }
}