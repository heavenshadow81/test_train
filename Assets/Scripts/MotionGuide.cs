using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace ML.SportsMiniGame.KinectSkating
{
    public class MotionGuide : MonoBehaviour
    {
        public Image GuideImage;
        public Sprite[] GuideSprites;
        public GameObject GuideObj;
        public bool isActive;
        
        public void GuideStart(float GuideTimer)
        {
            isActive = true;
            GuideObj.SetActive(true);
            StartCoroutine(GuideLoop(GuideTimer));
        }

        IEnumerator GuideLoop(float GuideTime)
        {
            int GuideIdx = 0;
            while (GuideTime > 0)
            {
                GuideTime -= Time.deltaTime;
                GuideImage.sprite = GuideSprites[GuideIdx];
                GuideIdx++;
                if (GuideIdx >= GuideSprites.Length)
                    GuideIdx = 0;

                yield return new WaitForSeconds(0.1f);
            }
            isActive = false;
            GuideObj.SetActive(false);
        }
    }
}

