using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // DOTween ���ӽ����̽� �߰�

namespace CookiesParty
{
    public class Cookie : MonoBehaviour
    {
        [SerializeField] private Material[] cookieMaterials = null;
        [SerializeField] private Material[] teamMaterials = null;
        [SerializeField] private Material[] materials = null;
        [SerializeField] private TeamName[] teamNames = null;
        private float forwardForce = 100f;
        private float sideForce = 10f;

        private MeshRenderer meshRender;
        private Rigidbody rb;

        [SerializeField] private MagicTimer timer;

        private void Awake()
        {
            meshRender = GetComponent<MeshRenderer>();
            rb = GetComponent<Rigidbody>(); // Rigidbody ������Ʈ�� �����ɴϴ�.

            materials = meshRender.materials;
            timer = FindObjectOfType<MagicTimer>();
        }

        private void OnEnable()
        {
            timer.OnTimerEnd += DeactiveDelay;

            // ������ ���� ����
            SetupCookie();

            float xForce = Random.Range(-sideForce, sideForce);
            float yForce = Random.Range(forwardForce * 0.5f, forwardForce);
            float zForce = Random.Range(-sideForce, sideForce);
            Vector3 force = new Vector3(xForce, yForce, zForce);
            Vector3 localForce = transform.TransformDirection(force);  // ���� ��ǥ�� ���� ��ǥ�� ��ȯ
            rb.velocity = localForce;

            // Y�� ȸ�� �ִϸ��̼� ���� (���� ����)
            transform.DORotate(new Vector3(0f, 360f, 0f), 1f, RotateMode.LocalAxisAdd)
                     .SetEase(Ease.Linear)  // ������ �ӵ��� ȸ��
                     .SetLoops(-1, LoopType.Incremental);  // ���� �ݺ�

            Invoke(nameof(DeactiveDelay), 10);
        }

        private void OnDisable()
        {
            timer.OnTimerEnd -= DeactiveDelay;

            ObjectPooler.ReturnToPool(gameObject);    // �� ��ü�� �ѹ���
            CancelInvoke();    // Monobehaviour�� Invoke�� �ִٸ�

            // DOTween���� ȸ�� �ִϸ��̼��� ����
            transform.DOKill();  // �� ������Ʈ�� ���õ� ��� DOTween �ִϸ��̼��� ����
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Respawn"))
            {
                gameObject.SetActive(false);
            }
        }

        void DeactiveDelay()
        {
            gameObject.SetActive(false);
        }

        private void SetupCookie()
        {
            // ������ ���� ����
            int randIdx = Random.Range(0, cookieMaterials.Length);
            materials[0] = cookieMaterials[randIdx];

            // ������ ���� ����
            int randIdx2 = Random.Range(0, teamMaterials.Length);
            materials[1] = teamMaterials[randIdx2];

            // ����� materials �迭�� �ٽ� Renderer�� ����
            meshRender.materials = materials;

            // teamMaterials �迭���� ���õ� �ε����� �´� �� �̸����� �±� ����
            TeamName selectedTeam = teamNames[randIdx2];
            gameObject.tag = selectedTeam.ToString();  // �±׸� �� �̸��� ���� ����
        }

        public Material GetCookieMat()
        {
            return materials[0];
        }
    }
}
