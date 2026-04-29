using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Railroad
{
    public class ButtonManager : MonoBehaviour
    {
        [SerializeField] RailManager railManager;

        public void OnClick_LeftButton()
        {
            railManager.SpawnLeftRail();
        }

        public void OnClick_RightButton()
        {
            railManager.SpawnRightRail();
        }

        public void OnClick_StraightButton()
        {
            railManager.SpawnStraightRail();
        }
    }
}


