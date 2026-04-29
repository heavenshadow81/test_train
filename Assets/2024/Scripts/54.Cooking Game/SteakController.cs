using UnityEngine;
using DG.Tweening;

namespace CookingGame
{
    public class SteakController : MonoBehaviour
    {
        [Header("참조")]
        [SerializeField] private FryingPan fryingPan;
        private Rigidbody steakRigidbody;

        [Header("던지기 설정")]
        [SerializeField] private float throwThreshold = 1f;
        [SerializeField] private float liftHeight = 0.02f;
        [SerializeField] private float throwDuration = 0.5f;

        [Header("굽기 설정")]
        [SerializeField] private Material cookedMaterial;
        [SerializeField] private float cookTime = 5.0f;
        private float contactStartTime = 0f;
        private bool isCooked = false;
        private MeshRenderer steakRenderer;

        [Header("연기")]
        [SerializeField] private GameObject whiteSmoke;
        [SerializeField] private GameObject darkSmoke;
        private GameObject darkSmokeInstance;
        private GameObject whiteSmokeInstance;
        private Vector3 darkOffSet = new Vector3(-0.00299f, 0.0001f, 0.00432f);
        private Vector3 whiteOffSet = new Vector3(0.0002f, -0.00306f, 0.0041f);

        private bool isThrown = false;

        private void Awake()
        {
            steakRigidbody = GetComponent<Rigidbody>();
        }

        void Update()
        {
            if (!isThrown && Input.GetMouseButtonDown(0))
            {
                if (fryingPan.AngularVelocity.magnitude >= throwThreshold)
                {
                    ThrowSteak();
                }
                else
                {
                    LiftSteak();
                }
            }
        }

        void FixedUpdate()
        {
            if (!isThrown)
            {
                Vector3 gravityDirection = -fryingPan.transform.up;
                steakRigidbody.AddForce(gravityDirection * 9.81f, ForceMode.Acceleration);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Point"))
            {
                CheckIfAlreadyCooked();

                // 이미 darkSmokeInstance가 있으면 새로 생성하지 않음
                if (steakRenderer.material.name.Contains("Cooked"))
                {
                    if (darkSmokeInstance == null)
                    {
                        darkSmokeInstance = Instantiate(darkSmoke, transform.position + darkOffSet, darkSmoke.transform.rotation);
                        darkSmokeInstance.transform.SetParent(transform);
                    }
                }
                else
                {
                    if (whiteSmokeInstance == null)
                    {
                        whiteSmokeInstance = Instantiate(whiteSmoke, transform.position + whiteOffSet, whiteSmoke.transform.rotation);
                        whiteSmokeInstance.transform.SetParent(transform);
                    }
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.CompareTag("Point") && !isCooked)
            {
                if (CheckIfAlreadyCooked()) return;

                if (contactStartTime == 0f)
                {
                    contactStartTime = Time.realtimeSinceStartup;
                }

                float elapsedTime = Time.realtimeSinceStartup - contactStartTime;

                if (elapsedTime >= cookTime)
                {
                    DestroySmoke();
                    steakRenderer.material = cookedMaterial;
                    isCooked = true;

                    if (darkSmokeInstance == null)
                    {
                        darkSmokeInstance = Instantiate(darkSmoke, transform.position + darkOffSet, darkSmoke.transform.rotation);
                        darkSmokeInstance.transform.SetParent(transform);
                    }

                    if (CheckCookOver())
                    {
                        Debug.Log("굽기완료");
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if(other.gameObject.CompareTag("Point") && isThrown)
            {
                DestroySmoke();
            }
        }

        private bool CheckIfAlreadyCooked()
        {
            Ray ray = new Ray(transform.position, -fryingPan.transform.up);

            int layerMask = LayerMask.GetMask("SteakSurface");

            if (Physics.Raycast(ray, out RaycastHit hit, 0.1f, layerMask))
            {
                steakRenderer = hit.collider.GetComponent<MeshRenderer>();

                if (steakRenderer != null && steakRenderer.material == cookedMaterial)
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckCookOver()
        {
            MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();

            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                // 하나라도 cookedMaterial이 아닌 경우 false 반환
                if (!meshRenderer.material.name.Contains("Cooked"))
                {
                    return false;
                }
            }
            // 모든 MeshRenderer가 cookedMaterial일 경우 true 반환
            return true;
        }

        private void ThrowSteak()
        {
            isCooked = false;
            isThrown = true;
            contactStartTime = 0;

            Vector3 targetPosition = fryingPan.transform.position + Vector3.up * liftHeight;
            Vector3 arrivePosition = fryingPan.transform.position + new Vector3(0, 0.01f, 0);

            Vector3 rotationAxis = fryingPan.AngularVelocity.normalized;
            Quaternion targetRotation = Quaternion.AngleAxis(90, rotationAxis) * transform.rotation;

            steakRigidbody.DOMove(targetPosition, throwDuration).OnComplete(() =>
            {
                isThrown = false;
            });

            transform.DORotateQuaternion(targetRotation, throwDuration);
        }

        private void LiftSteak()
        {
            isCooked = false;
            isThrown = true;
            contactStartTime = 0;
            //DestroySmoke();

            steakRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            steakRigidbody.DOMove(transform.position + Vector3.up * (liftHeight / 3), throwDuration / 2)
                .SetLoops(2, LoopType.Yoyo)
                .OnComplete(() =>
                {
                    steakRigidbody.constraints = RigidbodyConstraints.None;
                    isThrown = false;
                });
        }


        private void DestroySmoke()
        {
            if (darkSmokeInstance != null)
            {
                Destroy(darkSmokeInstance);
                darkSmokeInstance = null;
            }

            if (whiteSmokeInstance != null)
            {
                Destroy(whiteSmokeInstance);
                whiteSmokeInstance = null;
            }
        }
    }
}
