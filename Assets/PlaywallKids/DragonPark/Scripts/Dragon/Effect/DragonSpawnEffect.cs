using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace ML.PlaywallKids.DragonPark
{
    [RequireComponent(typeof(DragonAnimationControl))]
    public class DragonSpawnEffect : MonoBehaviour
    {
        public GameObject shadowPrefab;
        public GameObject particlePrefab;

        public AudioClip effectSound;
        public AudioClip spawnSound;

        private DragonAnimationControl _animationControl;
        private bool _movesAlongPath = false;
        private bool _usesNavMesh = false;

        public event Action onComplete;

        public void Awake()
        {
            enabled = false;
        }

        public void Play()
        {
            enabled = true;

            _animationControl = GetComponent<DragonAnimationControl>();
            _movesAlongPath = _animationControl.movesAlongPath;
            _usesNavMesh = _animationControl.usesNavMesh;
            _animationControl.movesAlongPath = false;
            _animationControl.usesNavMesh = false;

            StartCoroutine(_Process());
        }

        private IEnumerator _Process()
        {
            GameObject shadow = Instantiate(shadowPrefab);
            GameObject particle = Instantiate(particlePrefab);
            Collider c = GetComponent<Collider>();
            if (c != null) c.enabled = false;

            shadow.name = "_shadow";
            shadow.hideFlags = HideFlags.HideInHierarchy;
            particle.name = "_particle";
            particle.hideFlags = HideFlags.HideInHierarchy;
            shadow.gameObject.SetActive(true);
            particle.gameObject.SetActive(false);

            // 오브젝트 위치를 밑으로 이동
            Vector3 originalPos = transform.position;
            Vector3 groundPos = originalPos;
            groundPos.y = 0;
            shadow.transform.position = particle.transform.position = groundPos + Vector3.up * 0.01f;
            transform.position = groundPos + Vector3.down * 6.0f;

            // 효과음
            if (effectSound != null)
                AudioSource.PlayClipAtPoint(effectSound, Camera.main.transform.position);

            // 그림자, 파티클 표시
            shadow.transform.localScale = Vector3.one * 0.001f;
            shadow.transform.DOScale(1.5f, 0.25f);
            yield return new WaitForSeconds(0.25f);
            particle.gameObject.SetActive(true);
            particle.transform.localScale = Vector3.one * 1.5f;
            yield return new WaitForSeconds(1.0f);

            // 오브젝트 소환
            float appearTime = 1.5f;
            transform.DORotate(Vector3.up * (360.0f * 4.0f), appearTime, RotateMode.WorldAxisAdd);
            transform.DOMoveY(groundPos.y + 4.0f, appearTime * 0.66f).SetEase(Ease.OutSine).OnComplete(() =>
            {
                transform.DOMoveY(originalPos.y, appearTime * 0.34f).SetEase(Ease.InSine);
            });
            yield return new WaitForSeconds(appearTime * 0.166f);
            if (spawnSound != null)
                AudioSource.PlayClipAtPoint(spawnSound, Camera.main.transform.position);
            yield return new WaitForSeconds(appearTime * 0.333f);
            shadow.transform.DOScale(0.0f, 0.25f);
            yield return new WaitForSeconds(appearTime * 0.5f + 0.5f);
            Destroy(shadow);
            Destroy(particle);

            // 애니메이션 상태 원래대로 돌려놓기
            _animationControl.movesAlongPath = _movesAlongPath;
            _animationControl.usesNavMesh = _usesNavMesh;
            if (c != null) c.enabled = true;

            if (onComplete != null)
            {
                onComplete();
                onComplete = null;
            }

            enabled = false;
        }
    }
}