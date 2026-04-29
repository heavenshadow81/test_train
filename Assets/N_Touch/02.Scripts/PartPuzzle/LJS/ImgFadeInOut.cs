using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ImgFadeInOut : MonoBehaviour {
	public void ImageFadeOut()
	{
		GetComponent<Image>().DOFade(1, 1).SetEase(Ease.InOutCubic).SetId("Fade");
		Invoke("KillDotween", 1);
	}
	void KillDotween()
	{
		DOTween.Kill("Fade");
	}

}
