using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Photo_Background : MonoBehaviour {
    public static Photo_Background Instance;
    public Image Backgroundimg;
    public Sprite[] Backgrounds;
    // Use this for initializatio
    int idx;
    void Start () {
        Instance = this;
        idx = Random.Range(0, Backgrounds.Length); ;
	}
    public void NextSlides()
    {
        Backgroundimg.sprite = Backgrounds[idx];

        idx++;
        if (idx >= Backgrounds.Length)
            idx = 0;
    }
}
