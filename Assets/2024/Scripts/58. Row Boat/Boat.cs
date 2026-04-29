using System.Collections;
using UnityEngine;

namespace RowBoat
{
    public class Boat : MonoBehaviour
    {
        [SerializeField] private Transform leftOars;
        [SerializeField] private Transform rightOars;
        [SerializeField] private float turnSpeed = 4f; // ȸ�� �ӵ� ����
        [SerializeField] private float maxMoveSpeed = 35f;
        [SerializeField] private float initialAcceleration = 10f;
        [SerializeField] private float accelerationIncrement = 7f;
        [SerializeField] private float launchSpeed = 10f;
        [SerializeField] private GameObject leftEffect;
        [SerializeField] private GameObject rightEffect;

        private Rigidbody rb;
        private bool isLeftOarActive = false;
        private bool isRightOarActive = false;
        private bool isStop = true;
        private float currentSpeed;
        private float targetYRotation;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            currentSpeed = initialAcceleration;
            targetYRotation = transform.eulerAngles.y;
        }

        void FixedUpdate()
        {
            if (!isStop)
            {
                LimitSpeed();
                MoveForward();
                SmoothRotation();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            StartCoroutine(ResetPlayerSpeed());
        }

        private IEnumerator ResetPlayerSpeed()
        {
            yield return new WaitForSeconds(0.5f);
            rb.linearVelocity = Vector3.zero;
            ResetBoatSpeed();
        }

        public void OnClick_LeftButton()
        {
            if (isRightOarActive) return;

            if (isStop)
            {
                rb.linearVelocity = (-transform.right + transform.forward) * launchSpeed;
                isStop = false;
            }
            else
            {
                isLeftOarActive = true;
                targetYRotation += turnSpeed; // ȸ���� ����
                targetYRotation = Mathf.Clamp(targetYRotation, -90f, 90f); // -90���� 90�� ���̷� ����
                StartCoroutine(RowLeftOar());
            }
        }

        public void OnClick_RightButton()
        {
            if (isLeftOarActive) return;

            if (isStop)
            {
                rb.linearVelocity = (-transform.right - transform.forward) * launchSpeed;
                isStop = false;
            }
            else
            {
                isRightOarActive = true;
                targetYRotation -= turnSpeed; // ȸ���� ����
                targetYRotation = Mathf.Clamp(targetYRotation, -90f, 90f); // -90���� 90�� ���̷� ����
                StartCoroutine(RowRightOar());
            }
        }

        private IEnumerator RowLeftOar()
        {
            if (!isLeftOarActive) yield break;

            SoundMGR.Instance.SoundPlay("Oar");
            leftEffect.SetActive(true);

            float timer = 0f;
            while (timer < 0.5f)
            {
                leftOars.localRotation = Quaternion.Euler(Mathf.Sin(timer * Mathf.PI * 4) * 20, 0, 0);
                timer += Time.deltaTime;
                yield return null;
            }

            leftEffect.SetActive(false);
            isLeftOarActive = false;
        }

        private IEnumerator RowRightOar()
        {
            if (!isRightOarActive) yield break;

            SoundMGR.Instance.SoundPlay("Oar");
            rightEffect.SetActive(true);

            float timer = 0f;
            while (timer < 0.5f)
            {
                rightOars.localRotation = Quaternion.Euler(Mathf.Sin(timer * Mathf.PI * 4) * 20, 0, 0);
                timer += Time.deltaTime;
                yield return null;
            }

            rightEffect.SetActive(false);
            isRightOarActive = false;
        }

        private void MoveForward()
        {
            // ���� �ӵ��� �����ϴ� �Լ�
            Vector3 forwardForce = -transform.right * currentSpeed;
            rb.AddForce(forwardForce, ForceMode.Force);

            // ���ӵ� ����
            if (currentSpeed < maxMoveSpeed)
            {
                currentSpeed += accelerationIncrement * Time.fixedDeltaTime;
            }
        }

        private void SmoothRotation()
        {
            Quaternion targetRotation = Quaternion.Euler(transform.eulerAngles.x, targetYRotation, transform.eulerAngles.z);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 2f); // ȸ�� �ε巴�� ����
        }

        private void LimitSpeed()
        {
            if (rb.linearVelocity.magnitude > maxMoveSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxMoveSpeed;
            }
        }

        public void ResetBoatSpeed()
        {
            isStop = true;
            currentSpeed = initialAcceleration; // �ʱ� ���ӵ��� �缳��
        }
    }
}
