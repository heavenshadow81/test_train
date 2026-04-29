using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.DragonPark
{
    public class FreeDrawingRobotAntenna : BoneObject
    {
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
                    case "Dummy_Antenna_Tail":
                        boneName = kTailBone;
                        break;
                }

                if (!string.IsNullOrEmpty(boneName))
                {
                    boneDict[boneName] = t;
                }
            }

            return boneDict;
        }
    }
}