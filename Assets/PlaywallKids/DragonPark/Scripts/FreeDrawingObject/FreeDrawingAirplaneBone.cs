using System.Collections.Generic;
using UnityEngine;

namespace ML.PlaywallKids.DragonPark
{
    public class FreeDrawingAirplaneBone : FreeDrawingObjectBone
    {
        #region Properties
        public override FreeDrawingObjectType objectType
        {
            get
            {
                return FreeDrawingObjectType.Airplane;
            }
        }
        #endregion

        #region Constants
        public const string kWingBone = "wing";
        public const string kPropellerBone = "propeller";
        #endregion

        #region Bone
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
                    case "Dummy_Airplane_Body":
                        boneName = kBodyBone;
                        break;
                    case "Dummy_Airplane_Eyes_L":
                        boneName = kEyeLBone;
                        break;
                    case "Dummy_Airplane_Eyes_R":
                        boneName = kEyeRBone;
                        break;
                    case "Dummy_Airplane_Propeller":
                        boneName = kPropellerBone;
                        break;
                    case "Dummy_Airplane_Tail":
                        boneName = kTailBone;
                        break;
                    case "Dummy_Airplane_Wing":
                        boneName = kWingBone;
                        break;
                }

                if (!string.IsNullOrEmpty(boneName))
                {
                    boneDict[boneName] = t;
                }
            }

            return boneDict;
        }
        #endregion
    }
}