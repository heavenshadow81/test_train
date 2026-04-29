using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public class TwoDimensionGoalKeeperManager : BilateralSymmetryPosition
    {
        GoalKeeper[] goalKeepers;

        void Awake()
        {
            float height = UtilityScript.height * 0.4f;
            int num = objects.Length;
            goalKeepers = new GoalKeeper[num];
            for (int i = 0; i < num; ++i)
            {
                goalKeepers[i] = objects[i].gameObject.GetComponent<GoalKeeper>();
            }
        }
    }
}