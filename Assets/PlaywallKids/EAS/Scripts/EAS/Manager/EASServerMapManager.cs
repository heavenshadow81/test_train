using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.EAS
{
    public class EASServerMapManager : MonoBehaviour
    {
        public Animator animator;
        public UISprite tween;
        public GameObject dragonPark;
        public GameObject kavatar;

        public GameObject currentMap;

        private static EASServerMapManager _sharedInstance = null;
        public static EASServerMapManager sharedInstance
        {
            get
            {
                if (_sharedInstance == null)
                {
                    _sharedInstance = FindObjectOfType<EASServerMapManager>();
                }
                return _sharedInstance;
            }
        }

        public void Start()
        {
            animator = GetComponent<Animator>();
            StartCoroutine(Tween(true));
            currentMap = dragonPark;
            dragonPark.SetActive(true);
            kavatar.SetActive(false);
        }

        public void ShowDragonPark()
        {
            if (currentMap != dragonPark)
            {
                currentMap = dragonPark;
                HideKAvatar();
                StartCoroutine(Tween(false));
            }

            animator.SetBool("show_dragonpark", true);
            animator.SetBool("hide_dragonpark", false);
        }

        public void HideDragonPark()
        {
            animator.SetBool("hide_dragonpark", true);
            animator.SetBool("show_dragonpark", false);
        }

        public void ShowKAvatar()
        {
            if (currentMap != kavatar)
            {
                currentMap = kavatar;
                HideDragonPark();
                StartCoroutine(Tween(false));
            }

            animator.SetBool("show_kavatar", true);
            animator.SetBool("hide_kavatar", false);
        }

        public void HideKAvatar()
        {

            animator.SetBool("show_kavatar", false);
            animator.SetBool("hide_kavatar", true);
        }

        public IEnumerator Tween(bool start)
        {
            float time = 0.0f;

            tween.cachedGameObject.SetActive(true);
            tween.enabled = true;

            if (!start)
            {
                tween.color = Color.clear;
                do
                {
                    yield return null;
                    time += Time.deltaTime;
                    tween.color = new Color(0.0f, 0.0f, 0.0f, time);
                }
                while (time < 1.0f);
            }

            time = 0.0f;

            tween.color = Color.black;
            do
            {
                yield return null;
                time += Time.deltaTime;
                tween.color = new Color(0.0f, 0.0f, 0.0f, 1.0f - time);
            }
            while (time < 1.0f);

            tween.cachedGameObject.SetActive(!start);
        }
    }
}