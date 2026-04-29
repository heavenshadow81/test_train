using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

namespace ML.PlaywallKids.Aquarium
{
    public class AquariumUserTouchArea : MonoBehaviour
    {
        public AquariumMenuControl.Menu currentMenu;
        public UISprite loadBarBack, loadBarFront;
        public GameObject goMenu;
        public UILabel lbText;
        public UIButton btnExit;

        public GameObject menuItemPrefab;

        public bool isPressed { get; private set; }
        public bool isOpen { get; private set; }
        public new Camera camera { get; private set; }
        public new Collider collider { get; private set; }

        private int userId = -1;
        private int touchId = -1;

        public GameObject _current;

        private float _pressTime = 0.0f;

        private List<ContentsStoreItemInfo> _playList = null;
        public List<ContentsStoreItemInfo> playList
        {
            get
            {
                if (_playList == null)
                {
#if UNITY_EDITOR && BIGBOARD_STANDALONE
                    if (BigboardServer.bGetContentList == false)
                        BigboardServerDataManager.TestLogin();
#endif
                    _playList = BigboardServerDataManager.GetListUseContentsStoreItemInfo(ContentsStoreItemType.Mode.Aquarium);
                }
                return _playList;
            }
        }

        void Start()
        {
            collider = GetComponent<Collider>();
            camera = UICamera.list[0].cachedCamera;

            loadBarBack.cachedGameObject.SetActive(false);
            loadBarBack.alpha = 0;
            menuItemPrefab.SetActive(false);
            _FadeMessage(true);

            InitUser();
        }

        public void OnPress(bool pressed)
        {
            if (pressed)
            {
                if (touchId == -1)
                    touchId = UICamera.currentTouchID;

                if (isOpen)
                    CloseMenu();
                else
                {
                    _FadeLoadBar(true);
                    _FadeMessage(false);
                    loadBarFront.fillAmount = 0;
                    _pressTime = 0;
                    isPressed = true;
                }
            }
            else if (UICamera.currentTouchID == touchId)
            {
                if (!isOpen)
                {
                    _FadeLoadBar(false);
                    _FadeMessage(true);
                }
                touchId = -1;
                isPressed = false;
            }
        }

        public void OnDragOut(GameObject go)
        {
            if (UICamera.currentTouchID == touchId)
            {
                if (!isOpen)
                {
                    _FadeLoadBar(false);
                    _FadeMessage(true);
                }
                touchId = -1;
                isPressed = false;
            }
        }

        void Update()
        {
            if (isPressed && !isOpen)
            {
                float time = 1f;
                loadBarBack.transform.position = camera.ScreenToWorldPoint(UICamera.GetTouch(touchId, true).pos);
                loadBarFront.fillAmount += Time.deltaTime / time;
                if (loadBarFront.fillAmount >= 1f)
                {
                    _FadeLoadBar(false);
                    OpenMenu();
                }
            }
        }

        public void InitUser()
        {
            currentMenu = AquariumMenuControl.Menu.None;
            goMenu.SetActive(false);
            loadBarFront.fillAmount = 0f;

            isPressed = false;
            NGUITools.SetActive(btnExit.gameObject, false);

            if (goMenu.transform.childCount <= 0)
            {
                for (int i = 0; i < playList.Count; i++)
                {
                    BigboardContentMode contentMode = (BigboardContentMode)playList[i].seq;

                    GameObject menuItem = NGUITools.AddChild(goMenu, menuItemPrefab);

                    // sprite
                    UISprite sprite = menuItem.GetComponentInChildren<UISprite>();
                    sprite.spriteName = contentMode.ToString();
                    sprite.depth += i * 2;

                    // text
                    UILabel label = menuItem.GetComponentInChildren<UILabel>();
                    label.text = playList[i].name.Replace(" ", "\n");
                    label.depth += i * 2;

                    // event
                    UIButton btn = menuItem.GetComponent<UIButton>();
                    EventDelegate.Set(btn.onClick, () => ButtonClick(contentMode));
                }
            }

            if (playList.Count <= 0)
                Deactivate();
        }

        public void SetUser(int id)
        {
            userId = id;
        }

        private void OpenMenu()
        {
            isOpen = true;
            _FadeMessage(false);
            goMenu.SetActive(true);
            goMenu.transform.position = loadBarBack.transform.position;
            MenuAnimation(true);
        }

        private void CloseMenu()
        {
            isOpen = false;
            _FadeMessage(true);
            MenuAnimation(false);
        }

        private void MenuAnimation(bool open)
        {
            float distance = 160.0f;
            float circumference = Mathf.PI * distance;
            float value = 1;
            float startRoation = Mathf.PI * 1.5f;
            if (goMenu.transform.childCount > 1)
            {
                value = (1f / (goMenu.transform.childCount - 1) * Mathf.PI);
                startRoation = Mathf.PI;
            }
            for (int i = 0; i < goMenu.transform.childCount; i++)
            {
                float theta = value * i;
                theta = (theta - Mathf.PI * 0.5f) / (Mathf.PI * 0.5f);
                theta = Mathf.Pow(Mathf.Abs(theta), 1.2f) * (theta > 0 ? 1.0f : -1.0f);
                theta = (theta * Mathf.PI * 0.5f) + Mathf.PI * 0.5f;
                Transform child = goMenu.transform.GetChild(i);
                child.transform.localScale = Vector3.one * (open ? 0.5f : 1.0f);
                float x = distance * Mathf.Cos(theta + startRoation);
                float y = distance * -Mathf.Sin(theta + startRoation);

                DOTween.Kill(child);
                Sequence seq = DOTween.Sequence();
                seq.Insert(0, child.DOLocalMove(open ? new Vector2(x, y) : Vector2.zero, open ? 0.2f : 0.125f).SetEase(Ease.Linear));
                seq.Insert(0, child.DOScale(open ? 1.0f : 0.5f, 0.125f));
                seq.SetTarget(child);
                seq.OnComplete(() => { if (!open) goMenu.SetActive(false); });
                seq.Play();
            }
        }

        public void Activate()
        {
            collider.enabled = true;
            _FadeMessage(true);
            InitUser();
        }

        public void Deactivate()
        {
            isOpen = false;
            collider.enabled = false;
            _FadeMessage(false);
            MenuAnimation(false);
        }
        public void Hide()
        {
            Activate();
        }

        #region Menu
        public void SetBtnExitParent(GameObject parent)
        {
            if (parent != null)
            {
                // 버튼 위치를 anchor로 잡아주기
                const int x = 100, y = 100;
                const int startX = -10, startY = 10 - y;

                btnExit.gameObject.SetActive(true);
                UIWidget widget = btnExit.GetComponent<UIWidget>();
                widget.SetAnchor(parent);

                widget.leftAnchor.absolute = startX;
                widget.rightAnchor.absolute = startX + x;

                widget.bottomAnchor.absolute = startY;
                widget.topAnchor.absolute = startY + y;

                // 패널이 닫힐 경우 버튼을 가리도록 이벤트 추가
                var panel = parent.GetComponent<AnimatablePanel>();
                if (panel != null)
                    panel.onBeginHide += _BtnExitHandler;
            }
        }

        private void _BtnExitHandler(AnimatablePanel panel)
        {
            btnExit.gameObject.SetActive(false);
            panel.onBeginHide -= _BtnExitHandler;
        }

        public void ButtonClick(BigboardContentMode mode)
        {
            if (_current == null)
            {
                switch (mode)
                {
                    case BigboardContentMode.Aquarium_Fish: _current = AToolFish(); break;
                    case BigboardContentMode.Aquarium_Interaction: _current = Interaction(); break;
                }
            }
        }

        public GameObject AToolFish()
        {
            Deactivate();
            return AquariumMenuControl.sharedInstance.ShowAToolFish(userId);
        }

        public GameObject Interaction()
        {
            Hide();
            AquariumMenuControl.sharedInstance.Interaction(userId);
            return null;
        }
        #endregion

        #region Tweens
        private void _FadeLoadBar(bool show)
        {
            DOTween.Kill(loadBarBack);
            if (show)
            {
                loadBarBack.gameObject.SetActive(true);
                DOTween.To((x) => loadBarBack.alpha = x, loadBarBack.alpha, 1.0f, 0.25f * (1.0f - loadBarBack.alpha)).SetTarget(loadBarBack);
            }
            else
            {
                DOTween.To((x) => loadBarBack.alpha = x, loadBarBack.alpha, 0.0f, 0.25f * loadBarBack.alpha)
                    .SetTarget(loadBarBack)
                    .OnComplete(() => loadBarBack.cachedGameObject.SetActive(false));
            }
        }

        private void _FadeMessage(bool show)
        {
            DOTween.Kill(lbText);
            if (show)
            {
                lbText.gameObject.SetActive(true);
                DOTween.To((x) => lbText.alpha = x, lbText.alpha, 1.0f, 0.25f * (1.0f - lbText.alpha))
                    .SetTarget(lbText)
                    .OnComplete(() =>
                    {
                        DOTween.To((x) => lbText.alpha = x, 1.0f, 0.25f, 1.0f).SetEase(Ease.InSine).SetLoops(-1, LoopType.Yoyo).SetTarget(lbText);
                    });
            }
            else
            {
                DOTween.To((x) => lbText.alpha = x, lbText.alpha, 0.0f, 0.25f * lbText.alpha)
                    .SetTarget(lbText)
                    .OnComplete(() => lbText.cachedGameObject.SetActive(false));
            }
        }
    }
    #endregion
}