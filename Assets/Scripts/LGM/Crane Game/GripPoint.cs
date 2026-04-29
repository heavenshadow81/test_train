using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGM
{
    namespace CraneGame
    {
        public class GripPoint : MonoBehaviour
        {
            [Range(0, 100)]
            public int probability; // 잡힐 확률

            // 0~100의 확률로 인형 반환
            public bool PossibilityGripDoll()
            {
                int random = Random.Range(0, 101);  // 0 ~ 100까지의 값
                if (probability <= random)
                {
                    return true;
                }
                return false;
            }
        }
    }
}
