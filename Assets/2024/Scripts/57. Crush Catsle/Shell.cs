using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace CrushCatsle
{
    public class Shell : MonoBehaviour
    {
        [SerializeField] GameObject hitEffect;
        float travelDuration = 1f; // ���ư��� �ð� ����
        float arcHeight = 2f; // �������� ���� ����
        bool hasCollided = false;
        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        public void FireTowards(Vector3 targetPosition)
        {
            rb.isKinematic = true;

            // �߰� ������ ���� �����Ͽ� ������ ��θ� ����
            Vector3 midPoint = (transform.position + targetPosition) / 2;
            midPoint.y += arcHeight; // ������ ���� ����

            // ��� �� �迭 ����
            Vector3[] path = new Vector3[] { transform.position, midPoint, targetPosition };

            // DOTween���� ������ ��� ���� �̵�
            transform.DOPath(path, travelDuration, PathType.CatmullRom)
                .SetEase(Ease.Linear) // ������ �ӵ��� �̵�
                .OnComplete(() =>
                {
                    // �ִϸ��̼��� ���� �� ������ �ӵ��� �����ϱ� ���� kinematic ����
                    rb.isKinematic = false;

                    // ��ǥ ���������� �ӵ��� �����Ͽ� �� �������� ��� ���ư��� ��
                    Vector3 finalDirection = (targetPosition - midPoint).normalized;
                    rb.velocity = finalDirection * 10f; // ���ϴ� �ӵ��� ���� (10f�� �ӵ��� ũ��, ���� ����)
                });
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (hasCollided) return;

            hasCollided = true;
            StartCoroutine(OnHitRoutine());

            if(collision.gameObject.CompareTag("target"))
            {
                SoundMGR.Instance.SoundPlay("Pop");
            }
        }

        private void OnHitTarget()
        {
            StartCoroutine(OnHitRoutine());
        }

        IEnumerator OnHitRoutine()
        {        
            GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);

            yield return new WaitForSeconds(travelDuration);

            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            hasCollided = false;
            ObjectPooler.ReturnToPool(gameObject);
            CancelInvoke();
        }
    }
}
