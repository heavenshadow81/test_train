using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FindPotion
{
    public class GameManager : TouchManager_3DTouch
    {
        public static GameManager Instance { get; private set; }  // 싱글톤 인스턴스

        public bool isCheckAnswer = false;
        [SerializeField] private Potion[] potions = null; // 미리 등록한 이동할 오브젝트 배열
        [SerializeField] private Transform target;
        [SerializeField] private GameObject button;
        public int successStack = 0;

        [SerializeField] private Image fade;
        [SerializeField] private GameObject wall;
        [SerializeField] private GameObject introCam;
        [SerializeField] private GameObject gameCam;

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
        }

        private void OnEnable()
        {
            StartCoroutine(StartRoutine());
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (Instance != null)
            {
                Instance = null;
            }
        }

        IEnumerator StartRoutine()
        {
            SoundMGR.Instance.SoundPlay("Bubble");
            fade.DOFade(0, 1f);

            yield return new WaitForSeconds(3f);

            fade.DOFade(1, 0.5f).OnComplete(() =>
            {
                introCam.SetActive(false);
                SoundMGR.Instance.SoundStop("Bubble");
                wall.SetActive(true);
                gameCam.SetActive(true);
                fade.DOFade(0, 0.5f).OnComplete(() =>
                {                   
                    PotionManager.Instance.Init();
                });              
            });       
        }

        public override void HandleInput(Vector2 pos)
        {
            // 스크린 좌표로부터 레이 생성
            Ray ray = Camera.main.ScreenPointToRay(pos);
            RaycastHit hit;

            // 레이캐스트를 쏴서 충돌 여부 확인
            if (Physics.Raycast(ray, out hit))
            {
                if(hit.collider.CompareTag("Green"))
                {
                    // 레이가 충돌한 오브젝트 이름으로 Potion 객체를 가져옴
                    Potion targetPotion = GetPotionFromName(hit.collider.gameObject.name);

                    if (targetPotion != null)
                    {
                        ToggleButton(); // 버튼 안보이게 처리
                        targetPotion.MovePotion(target, () => targetPotion.ActivateWaterDrop());
                    }
                    else
                    {
                        Debug.LogWarning($"해당 이름에 대응하는 Potion이 없습니다: {hit.collider.gameObject.name}");
                    }
                }
                else
                {
                    // 터치한 오브젝트가 적합하지 않으면 즉시 다시 터치 가능
                    isTouchable = true;
                }
            }
            else
            {
                // 터치한 오브젝트가 적합하지 않으면 즉시 다시 터치 가능
                isTouchable = true;
            }
        }

        private Potion GetPotionFromName(string objectName)
        {
            // 이름에 따른 Potion 객체를 반환
            switch (objectName)
            {
                case "1":
                    return potions[0];
                case "2":
                    return potions[1];
                case "3":
                    return potions[2];
                case "4":
                    return potions[3];
                case "5":
                    return potions[4];
                default:
                    Debug.LogWarning($"해당 이름({objectName})의 포션을 찾을 수 없습니다.");
                    return null; // 이름이 매칭되지 않는 경우 null 반환
            }
        }

        public void ToggleButton()
        {
            button.gameObject.SetActive(!button.gameObject.activeSelf);
        }

        public void GameClear()
        {
            victoryUI.SetActive(true);
            SoundMGR.Instance.bgmSource.Stop();
            SoundMGR.Instance.SoundPlay("win");
        }

        public void EnableTouch()
        {
            isTouchable = true;
        }

    }
}


