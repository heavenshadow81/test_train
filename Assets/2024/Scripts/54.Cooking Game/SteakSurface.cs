using UnityEngine;

namespace CookingGame
{
    public class SteakSurface : MonoBehaviour
    {
        [Header("설정")]
        [SerializeField] private Material cookedMaterial; // 익은 상태의 머테리얼
        [SerializeField] private float cookTime = 3.0f; // 익히는 데 필요한 시간 (초)

        private float contactTime = 0f; // 프라이팬과의 접촉 시간
        private bool isCooked = false; // 현재 면이 익었는지 여부

        public void HandleContact(float deltaTime)
        {
            if (isCooked) return;

            contactTime += deltaTime; // 프라이팬과의 접촉 시간 누적

            if (contactTime >= cookTime)
            {
                CookSteak();
            }
        }

        private void CookSteak()
        {
            GetComponent<Renderer>().material = cookedMaterial;
            isCooked = true;
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag("FryingPan"))
            {
                contactTime = 0f;
            }
        }
    }
}
