using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace ML.T_Sports.BaseBall
{
    public class UI_Particle : MonoBehaviour
    {
        public Transform Mytr;
        public bool twinkling;
        public Image MyImage;
        RectTransform rectTr;
        private void Awake()
        {
            twinkling = false;
            Mytr = this.transform;
            MyImage = Mytr.GetComponent<Image>();
            rectTr = Mytr.GetComponent<RectTransform>();
        }
        public void TwinklePlay()
        {
            StartCoroutine(Twinkle());
        }

        IEnumerator Twinkle()
        {
            twinkling = true;
            MyImage.color = new Color(255, 255, 255, 0);

            float alpha = 0;
            float target = 1;
            bool fadeIn = true;
            float randomSize = Random.Range(70,110);
            float RandomSpeed = Random.Range(1.5f, 2.5f);
            rectTr.sizeDelta = new Vector2(randomSize, randomSize);
            while (twinkling)
            {
                if (fadeIn)
                {
                    alpha += Time.deltaTime * RandomSpeed;
                    if (alpha > target)
                    {
                        alpha = 1;
                        fadeIn = false;
                    }
                    MyImage.color = new Color(255, 255, 255, alpha);
                    Mytr.localScale = new Vector3(alpha, alpha, alpha);
                }
                else if (!fadeIn)
                {
                    alpha -= Time.deltaTime * RandomSpeed;
                    if (alpha < 0)
                    {
                        alpha = 0;
                        fadeIn = true;
                        randomSize = Random.Range(50, 100);
                        rectTr.sizeDelta = new Vector2(randomSize, randomSize);
                    }
                    MyImage.color = new Color(255, 255, 255, alpha);
                    Mytr.localScale = new Vector3(alpha, alpha, alpha);
                }

                yield return new WaitForSeconds(0.01f);
            }
        }
    }
}
