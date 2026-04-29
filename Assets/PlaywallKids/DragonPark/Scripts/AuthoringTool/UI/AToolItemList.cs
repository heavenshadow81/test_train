using UnityEngine;

namespace ML.PlaywallKids.DragonPark
{
    public class AToolItemList : MonoBehaviour
    {
        #region Public variables
        // buttons
        public UIButton prevButton, nextButton;

        // positions
        public UIButton[] itemButtons;

        // curve
        public AnimationCurve popCurve;
        #endregion

        #region Properties
        private GameObject[] _items = new GameObject[0];
        public GameObject[] items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;

                if (_items == null)
                {
                    _items = new GameObject[0];
                }

                _Init();
            }
        }

        private System.Action<int> _onClick = null;
        public System.Action<int> onClick
        {
            get
            {
                return _onClick;
            }
            set
            {
                _onClick = value;

                for (int i = 0, cnt = itemButtons.Length; i < cnt; i++)
                {
                    int index = i;
                    UIButton btn = itemButtons[index];
                    btn.onClick.Clear();
                    btn.onClick.Add(new EventDelegate(() =>
                    {
                        if (onClick != null)
                        {
                            onClick(_currentPageIndex_Internal + index);
                        }
                    }));
                }
            }
        }

        private int _currentPageIndex = 0;
        public int currentPageIndex
        {
            get
            {
                return _currentPageIndex;
            }
        }

        public bool rotateItem
        {
            set
            {
                for (int i = 0, cnt = itemButtons.Length; i < cnt; i++)
                {
                    UIButton btn = itemButtons[i];

                    Transform dummy = _GetDummy(btn);

                    AutoRotate ar = dummy.GetComponent<AutoRotate>();
                    if (ar == null)
                    {
                        ar = dummy.gameObject.AddComponent<AutoRotate>();
                        ar.axis = Vector3.up;
                        ar.anglePerSecond = 90.0f;
                    }

                    ar.enabled = value;
                }
            }
        }
        #endregion

        #region Private Properties

        private int _currentPageIndex_Internal
        {
            get
            {
                return currentPageIndex;
            }
            set
            {
                _currentPageIndex = Mathf.Min(Mathf.Max(0, items.Length - 1), Mathf.Max(0, value));
                Refresh();
            }
        }
        #endregion

        void Start()
        {
            _Init();
        }

        private void _Init()
        {
            _currentPageIndex_Internal = 0;
        }

        public void Refresh()
        {
            bool enablesPrevButton = true;
            bool enablesNextButton = true;

            if (_currentPageIndex_Internal == 0)
            {
                enablesPrevButton = false;
            }
            if (System.Math.Ceiling((double)items.Length / itemButtons.Length) == _currentPageIndex_Internal + 1)
            {
                enablesNextButton = false;
            }

            for (int i = 0, cnt = itemButtons.Length; i < cnt; i++)
            {
                UIButton btn = itemButtons[i];

                // change color randomly
                btn.GetComponent<UISprite>().color = btn.defaultColor = new Color(Random.Range(0.25f, 1.0f), Random.Range(.25f, 1.0f), Random.Range(.25f, 1.0f), 1.0f);

                // get dummy transform
                Transform dummy = _GetDummy(btn);

                // removes previous dummy icons
                if (dummy.childCount > 0)
                {
                    foreach (Transform t in dummy)
                    {
                        if (t != dummy)
                        {
                            Destroy(t.gameObject);
                        }
                    }
                }

                if (_currentPageIndex_Internal + i < items.Length)
                {
                    btn.isEnabled = true;

                    GameObject prefab = items[_currentPageIndex_Internal + i];
                    GameObject icon = null;
                    if (prefab != null)
                    {
                        if (prefab.GetComponent<UIWidget>() != null)
                        {
                            icon = NGUITools.AddChild(dummy.gameObject, prefab);
                        }
                        else
                        {
                            icon = (GameObject)Instantiate(prefab);
                        }
                    }

                    if (icon != null)
                    {
                        // active
                        icon.SetActive(true);

                        // layer
                        icon.layer = LayerMask.NameToLayer("NGUI");

                        // transform
                        icon.transform.parent = dummy;
                        icon.transform.localPosition = Vector3.zero;
                        if (icon.name.Contains("Ribbon") || icon.name.Contains("Necktie"))
                        {
                            icon.transform.localRotation = Quaternion.identity;
                        }
                        else if (icon.GetComponent<UIWidget>() != null)
                        {
                            icon.transform.localRotation = Quaternion.identity;
                        }
                        else
                        {
                            icon.transform.localRotation = Quaternion.Euler(-90, 90, 0);
                        }
                        icon.transform.localScale = Vector3.zero;

                        // simple tweens
                        AttachTweens(icon, i * 0.25f, .5f);
                    }
                }
                else
                {
                    enablesNextButton = false;
                    btn.isEnabled = false;
                }
            }

            prevButton.isEnabled = enablesPrevButton;
            nextButton.isEnabled = enablesNextButton;
        }

        public void AttachTweens(GameObject go, float delay, float duration)
        {
            TweenScale ts = go.AddComponent<TweenScale>();
            ts.from = Vector3.zero;
            ts.to = new Vector3(1.0f, 1.0f, 1.0f);
            ts.duration = duration;
            ts.delay = delay;
            ts.animationCurve = popCurve;
            ts.PlayForward();
        }

        public void Prev()
        {
            _currentPageIndex_Internal -= itemButtons.Length;
        }

        public void Next()
        {
            _currentPageIndex_Internal += itemButtons.Length;
        }

        private Transform _GetDummy(UIButton btn)
        {
            Transform t = btn.transform;
            for (int i = 0, cnt = t.childCount; i < cnt; i++)
            {
                Transform child = t.GetChild(i);
                if (child.name.Contains("Dummy"))
                {
                    return child;
                }
            }
            return null;
        }
    }
}