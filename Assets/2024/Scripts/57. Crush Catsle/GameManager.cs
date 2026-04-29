using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace CrushCatsle
{
    public class GameManager : TouchManager_3DTouch
    {
        public static GameManager Instance;

        [Header("컨테이너")]
        [SerializeField] Cannon cannon;
        [SerializeField] GameObject candySpawner;
        [SerializeField] GameObject crossHair;
        [SerializeField] FindShapePuzzle.CameraMove cam;
        [SerializeField] MagicTimer timer;
        [SerializeField] Image fadeImage;

        [Header("발사 쿨다운")]
        float fireCooldown = 0.4f; 
        private float lastFireTime = -1f; 

        private Vector3 targetPosition;
        private bool canFire = true;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        protected override void Start()
        {
            base.Start();

            isTouchable = false;
            timer.OnTimerEnd += DisableTouch;
        }

        private void OnDestroy()
        {
            if (Instance != null)
            {
                Instance = null;
            }

            timer.OnTimerEnd -= DisableTouch;
        }

        public void GameStart()
        {
            fadeImage.DOFade(0f, 1f).OnComplete(() => isTouchable = true);
        }

        public override void HandleInput(Vector2 pos)
        {
            isTouchable = true;

            if (!canFire) return;

            Ray ray = Camera.main.ScreenPointToRay(pos);
            RaycastHit hit;

            int targetLayerMask = LayerMask.GetMask("Target");

            if (Physics.Raycast(ray, out hit, 100f, targetLayerMask))
            {
                targetPosition = hit.point;

                if (Time.time >= lastFireTime + fireCooldown)
                {                   
                    cannon.TurnCannon(targetPosition);
                    lastFireTime = Time.time;

                    MoveCrossHair(targetPosition + new Vector3(0, 0, -0.5f));
                }
            }
        }

        private void MoveCrossHair(Vector3 targetPos)
        {
            crossHair.transform.DOMove(targetPos, 0.5f).SetEase(Ease.Linear);
        }

        public void GameClear()
        {
            victoryUI.SetActive(true);
            SoundMGR.Instance.bgmSource.Stop();
            SoundMGR.Instance.SoundPlay("win");
        }

        public void SpawnCandy()
        {
            timer.PauseTimer();
            canFire = false;

            cam.MoveCam(1f, () =>
            {
                SoundMGR.Instance.SoundPlay("Fanfare");
                candySpawner.SetActive(true);
                cannon.gameObject.SetActive(false);
                DOVirtual.DelayedCall(4f, GameClear);
            });          
        }

        private void DisableTouch()
        {
            canFire = false;
        }
    }
}
