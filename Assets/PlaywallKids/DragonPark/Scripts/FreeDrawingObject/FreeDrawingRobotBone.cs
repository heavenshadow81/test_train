using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.DragonPark
{
    public class FreeDrawingRobotBone : FreeDrawingObjectBone
    {
        #region Public variables
        public GameObject[] antennaPrefabs;
        public GameObject[] mouthPrefabs;
        #endregion

        #region Properties
        public override FreeDrawingObjectType objectType
        {
            get
            {
                return FreeDrawingObjectType.Robot;
            }
        }

        private FreeDrawingRobotEffect _robotEffect = null;
        public FreeDrawingRobotEffect robotEffect
        {
            get
            {
                if (_robotEffect == null)
                {
                    _robotEffect = GetComponent<FreeDrawingRobotEffect>();
                }
                return _robotEffect;
            }
        }

        private static string[] _eyeBoneNames;
        public static string[] eyeBoneNames
        {
            get
            {
                if (_eyeBoneNames == null)
                {
                    _eyeBoneNames = new string[] { kEyeLBone, kEyeRBone };
                }
                return _eyeBoneNames;
            }
        }
        #endregion

        #region Constants
        public const string kLegLBone = "leg_l";
        public const string kLegRBone = "leg_r";
        public const string kFootLBone = "foot_l";
        public const string kFootRBone = "foot_r";
        public const string kAntennaBone = "antenna";
        public const string kMouthBone = "mouth";
        #endregion

        public override void PrepareDefaultAccessories()
        {
            _PrepareDefaultEyeAccessories();

            // rotation correction for eyes
            if (_leftEye != null)
            {
                _leftEye.transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
            if (_rightEye != null)
            {
                _rightEye.transform.localRotation = Quaternion.Euler(0, 180, 0);
            }

            // antenna
            if (antennaPrefabs != null && antennaPrefabs.Length > 0)
            {
                GameObject prefab = antennaPrefabs[Random.Range(0, antennaPrefabs.Length)];

                GameObject antenna = (GameObject)Instantiate(prefab) as GameObject;
                SetAccessory(FreeDrawingRobotBone.kAntennaBone, antenna, true);
                AutoRotate ar = antenna.AddComponent<AutoRotate>();
                ar.isLocal = true;
            }

            // mouth
            if (mouthPrefabs != null && mouthPrefabs.Length > 0)
            {
                GameObject prefab = mouthPrefabs[Random.Range(0, mouthPrefabs.Length)];

                GameObject mouth = (GameObject)Instantiate(prefab) as GameObject;
                SetAccessory(FreeDrawingRobotBone.kMouthBone, mouth, true);
                mouth.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }

        protected override Transform _GetRootBone()
        {
            Transform rootBone = transform;
            return rootBone;
        }

        protected override Dictionary<string, Transform> _GetBoneDict()
        {
            Dictionary<string, Transform> boneDict = new Dictionary<string, Transform>();

            Transform[] hierarchy = _GetRootBone().GetComponentsInChildren<Transform>();

            foreach (Transform t in hierarchy)
            {
                string boneName = "";

                switch (t.name)
                {
                    case "Dummy_Robot_Head":
                        boneName = kHeadBone;
                        break;
                    case "Dummy_Robot_Body":
                        boneName = kBodyBone;
                        break;
                    case "Dummy_Robot_Arms_L":
                        boneName = kArmLBone;
                        break;
                    case "Dummy_Robot_Arms_R":
                        boneName = kArmRBone;
                        break;
                    case "Dummy_Robot_Leg_L":
                        boneName = kLegLBone;
                        break;
                    case "Dummy_Robot_Leg_R":
                        boneName = kLegRBone;
                        break;
                    case "Dummy_Robot_Foot_L":
                        boneName = kFootLBone;
                        break;
                    case "Dummy_Robot_Foot_R":
                        boneName = kFootRBone;
                        break;
                    case "Dummy_Robot_Eyes_L":
                        boneName = kEyeLBone;
                        break;
                    case "Dummy_Robot_Eyes_R":
                        boneName = kEyeRBone;
                        break;
                    case "Dummy_Robot_Antenna":
                        boneName = kAntennaBone;
                        break;
                    case "Dummy_Robot_Mouth":
                        boneName = kMouthBone;
                        break;
                }

                if (!string.IsNullOrEmpty(boneName))
                {
                    boneDict[boneName] = t;
                }
            }

            return boneDict;
        }

        public override void SetAccessory(string boneName, GameObject go, bool removePrev)
        {
            // before calling base methods, detach the effect first.
            if (robotEffect != null)
            {
                robotEffect.Detach(boneName);
            }

            // base call.
            base.SetAccessory(boneName, go, removePrev);

            // attach the effect
            if (robotEffect != null)
            {
                robotEffect.Attach(boneName);
            }

            // rotation correction for arms
            if (boneName.Equals(kArmLBone))
            {
                go.transform.localRotation = Quaternion.Euler(0, -90, 0);
            }
            else if (boneName.Equals(kArmRBone))
            {
                go.transform.localRotation = Quaternion.Euler(0, 90, 0);
            }
        }
    }
}