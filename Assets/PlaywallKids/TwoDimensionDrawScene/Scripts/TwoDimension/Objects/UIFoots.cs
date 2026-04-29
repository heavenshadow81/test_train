using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public class UIFoots : UIObject
    {
        public UISprite[] imgFoots;
        Coroutine coroutine;
        int _indexFoot;
        public override float Value
        {
            get { return (float)_indexFoot; }
            set
            {
                if ((int)value < 2)
                {
                    _indexFoot = (int)value;
                    imgFoots[_indexFoot].alpha = 1f;
                    imgFoots[_indexFoot].cachedGameObject.SetActive(true);
                    imgFoots[(_indexFoot + 1) % imgFoots.Length].gameObject.SetActive(false);

                    coroutine = StartCoroutine(ActiveProcess());
                }
            }
        }

        public UISprite imgFoot
        {
            get { return imgFoots[_indexFoot]; }
        }

        void OnEnable()
        {
            _indexFoot = 0;

            for (int i = 0, len = imgFoots.Length; i < len; ++i)
            {
                imgFoots[i].alpha = 1f;
                imgFoots[i].cachedGameObject.SetActive(false);
            }
        }

        void OnDisable()
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
        }

        IEnumerator ActiveProcess()
        {
            float scale = 1.01f;
            bool bAscending = true;
            do
            {
                scale += Time.fixedDeltaTime * (bAscending ? 2f : -2f);
                if (bAscending && scale > 1.5f) bAscending = false;
                if (!bAscending && scale < 1) scale = 1.0f;
                CachedTransform.localScale = new Vector3(scale, scale, 1f);
                yield return new WaitForFixedUpdate();
            } while (scale != 1f);

            do
            {
                scale -= Time.fixedDeltaTime * 2f; ;
                CachedTransform.localScale = new Vector3(scale, scale, 1f);
                yield return new WaitForFixedUpdate();
            } while (scale > 1f);

            CachedTransform.localScale = Vector3.one;
            float time = 0f;
            do
            {
                time += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            } while (time < 1f);

            float alpha = 1f;
            do
            {
                float _time = Time.fixedDeltaTime * 0.05f;
                alpha = imgFoot.alpha;
                if (alpha - _time > 0) alpha -= _time;
                else { alpha = 0; }
                imgFoot.alpha = alpha;
                yield return new WaitForFixedUpdate();
            } while (alpha > 0);
            yield return new WaitForEndOfFrame();

            gameObject.SetActive(false);
        }

    }
}