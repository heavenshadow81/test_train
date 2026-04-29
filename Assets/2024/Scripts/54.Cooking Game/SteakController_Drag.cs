using UnityEngine;
using DG.Tweening;

namespace CookingGame
{
    public class SteakController_Drag : MonoBehaviour
    {
        [Header("참조")]
        [SerializeField] private FryingPan fryingPan;
        private Rigidbody steakRigidbody;

        [Header("던지기 설정")]
        private float throwThreshold = 130f;
        private float liftHeight = 0.02f;
        private float throwDuration = 0.5f;

        [Header("굽기 설정")]
        [SerializeField] private Material cookedMaterial;
        private float cookTime = 7.0f;
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

        private Vector2 dragStart;
        private Vector2 dragEnd;
        private bool isDragging = false;
        private bool isThrown = false;

        private void Awake()
        {
            steakRigidbody = GetComponent<Rigidbody>();
        }

        void Update()
        {
            // 터치 입력 처리
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        dragStart = touch.position;
                        isDragging = true;
                        break;

                    case TouchPhase.Moved:
                        dragEnd = touch.position;
                        break;

                    case TouchPhase.Ended:
                        isDragging = false;
                        OnEndDrag();
                        break;
                }
            }
            // 마우스 입력 처리 (터치 입력이 없을 때)
            else if (Input.GetMouseButtonDown(0))
            {
                dragStart = Input.mousePosition;
                isDragging = true;
            }
            else if (isDragging && Input.GetMouseButton(0))
            {
                dragEnd = Input.mousePosition;
            }
            else if (isDragging && Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                OnEndDrag();
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

        public void OnEndDrag()
        {
            if (!isThrown)
            {
                Vector2 dragDirection = (dragEnd - dragStart).normalized;
                float dragDistance = (dragEnd - dragStart).magnitude;

                if (fryingPan.IsMoving && dragDistance >= throwThreshold)
                {
                    ThrowSteak(dragDirection);
                }
                else
                {
                    LiftSteak();
                }
            }
        }

        private void ThrowSteak(Vector2 direction)
        {
            isCooked = false;
            isThrown = true;
            contactStartTime = 0;

            Vector3 targetPosition = fryingPan.transform.position + new Vector3(0, liftHeight, 0);
            Vector3 rotationAxis = new Vector3(-direction.y, 0, direction.x).normalized;
            Quaternion targetRotation = Quaternion.AngleAxis(90, rotationAxis) * transform.rotation;

            steakRigidbody.DOMove(targetPosition, throwDuration).OnComplete(() =>
            {
                isThrown = false;
            });

            transform.DORotateQuaternion(targetRotation, throwDuration);
            int randomIndex = Random.Range(1, 2);
            SoundMGR.Instance.SoundPlay($"Flip{randomIndex}");
        }

        private void LiftSteak()
        {
            isCooked = false;
            isThrown = true;
            contactStartTime = 0;

            steakRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            steakRigidbody.DOMove(transform.position + Vector3.up * (liftHeight / 3), throwDuration / 2)
                .SetLoops(2, LoopType.Yoyo)
                .OnComplete(() =>
                {
                    steakRigidbody.constraints = RigidbodyConstraints.None;
                    isThrown = false;
                });
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Point"))
            {                
                CheckIfAlreadyCooked();

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
                    SoundMGR.Instance.SoundPlayIfNotPlaying("Swizzle");

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
                        DestroySmoke();
                        SoundMGR.Instance.SoundStop("Swizzle");
                        fryingPan.StopMovement();
                        GameManager.Instance.GameClear();
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Point") && isThrown)
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
                if (!meshRenderer.material.name.Contains("Cooked"))
                {
                    return false;
                }
            }
            return true;
        }

        private void DestroySmoke()
        {
            SoundMGR.Instance.SoundStop("Swizzle");
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
