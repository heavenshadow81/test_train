using System.Collections;
using UnityEngine;

namespace RowBoat
{
    public class Shark : MonoBehaviour
    {
        private float moveDistance = 30f; // 좌우로 이동할 최대 거리
        [SerializeField] private float moveSpeed = 2f;     // 이동 속도
        private float bounceForce = 500f; // 충돌 시 플레이어가 튕겨나가는 힘

        private Vector3 startPosition; // 시작 위치 저장

        void Start()
        {
            // 상어의 시작 위치를 저장
            startPosition = transform.position;

            // 1~3초 사이의 랜덤한 시간 동안 지연 후 이동 시작
            float randomDelay = Random.Range(1f, 3f);
            StartCoroutine(StartMovementAfterDelay(randomDelay));
        }

        private IEnumerator StartMovementAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            StartCoroutine(MoveShark());
        }

        private IEnumerator MoveShark()
        {
            while (true)
            {
                // Mathf.PingPong을 사용하여 좌우로 이동
                float offset = Mathf.PingPong(Time.time * moveSpeed, moveDistance * 2) - moveDistance;
                transform.position = startPosition + new Vector3(0, 0, offset);
                yield return null;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Player"))
            {
                Rigidbody playerRb = collision.collider.GetComponent<Rigidbody>();
                if (playerRb != null)
                {
                    // 충돌 방향 계산
                    Vector3 collisionDirection = collision.contacts[0].point - transform.position;
                    collisionDirection = collisionDirection.normalized;

                    // 충돌 방향으로 힘을 가해 플레이어를 튕겨나가게 함
                    playerRb.AddForce(collisionDirection * bounceForce, ForceMode.Impulse);
                    SoundMGR.Instance.SoundPlay("Shark");
                }
            }
        }
    }
}
