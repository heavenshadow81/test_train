using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;  // DOTween 네임스페이스 추가

namespace CookiesParty
{
    public class CannonBarrel : MonoBehaviour
    {
        private float rotateAngle = 25f;  // 회전할 각도
        private float minRotateDuration = 1f;  // 회전에 걸리는 최소 시간
        private float maxRotateDuration = 3f;  // 회전에 걸리는 최대 시간
        private float minStartDelay = 0.5f;  // 최소 지연 시간
        private float maxStartDelay = 2f;    // 최대 지연 시간
        private float spawnInterval = 1.5f; // 쿠키 생성 간격
        [SerializeField] private Transform cookieSpawner;
        [SerializeField] private GameObject fireEffect;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private MagicTimer timer;

        private Coroutine spawnCoroutine; // 쿠키 스폰 코루틴을 관리하기 위한 변수

        private void OnEnable()
        {
            gameManager.OnGameStart += StartGame;
            timer.OnTimerEnd += EndGame;
        }

        private void OnDisable()
        {
            gameManager.OnGameStart -= StartGame;
            timer.OnTimerEnd -= EndGame;
        }

        private void StartGame()
        {
            // 랜덤한 시간 뒤에 회전 시작
            StartCoroutine(StartRotationAfterDelay());
        }

        private void EndGame()
        {
            StopAllCoroutines();
            DOTween.KillAll();
        }

        private IEnumerator StartRotationAfterDelay()
        {
            // 랜덤 지연 시간 설정
            float randomDelay = Random.Range(minStartDelay, maxStartDelay);

            // 해당 시간만큼 대기
            yield return new WaitForSeconds(randomDelay);

            // 캐논 회전 시작
            RotateCannon();

            // 쿠키 스폰 시작 (코루틴 사용)
            spawnCoroutine = StartCoroutine(SpawnCookieRoutine());
        }

        private void RotateCannon()
        {
            // 랜덤 회전 지속 시간 설정
            float randomRotateDuration = Random.Range(minRotateDuration, maxRotateDuration);

            // 현재 각도에서 Z축으로 rotateAngle만큼 회전 후 다시 반대로 회전하는 애니메이션
            transform.DORotate(new Vector3(0, 0, rotateAngle), randomRotateDuration, RotateMode.LocalAxisAdd)
                .SetEase(Ease.InOutSine)  // 부드러운 회전을 위해 Ease 설정
                .SetLoops(-1, LoopType.Yoyo);  // 무한 반복하며 왔다갔다 회전
        }

        // 쿠키를 0.5초마다 생성하는 코루틴
        private IEnumerator SpawnCookieRoutine()
        {
            while (true)
            {
                SpawnCookie();  // 쿠키 스폰
                yield return new WaitForSeconds(spawnInterval);  // 지정된 간격만큼 대기
                fireEffect.SetActive(false);
            }
        }

        private void SpawnCookie()
        {
            ObjectPooler.SpawnFromPool<Cookie>("Cookie", cookieSpawner.position, cookieSpawner.rotation);
            fireEffect.SetActive(true);
            SoundMGR.Instance.SoundPlay("Cannon");
        }

        private void OnDestroy()
        {
            // 오브젝트가 파괴될 때 쿠키 스폰 코루틴 중지
            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
            }
        }
    }
}
