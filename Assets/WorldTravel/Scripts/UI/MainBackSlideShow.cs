using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ML.MapoContents.WorldTravel;
public class MainBackSlideShow : MonoBehaviour
{
    public Sprite[] SlideImage;
    int _idx;
    public Image MainBackGround;
    public Animation Fade;
    bool _slideShow;
    public GameObject BackBlack;
    public GameObject ForwardBlack;
    // Use this for initialization
    void Start()
    {
        _idx = Random.Range(0, SlideImage.Length);
        StartSlideShow();
    }
    
    public void StartSlideShow()
    {
        _idx = Random.Range(0, SlideImage.Length);
        _slideShow = true;
        BackBlack.SetActive(true);
        ForwardBlack.SetActive(true);
        StartCoroutine(SlideShow());
    }
    void OnDestroy()
    {
        _slideShow = false;
    }
    IEnumerator SlideShow()
    {
        while (_slideShow)
        {
            if (TravelManager.Instance._state == TravelManager.TravelState.Intro)
            {
                Fade.Play("MainBackFadeOut");
                yield return new WaitForSeconds(1f);
                MainBackGround.sprite = SlideImage[_idx];
                _idx++;
                if (_idx >= SlideImage.Length)
                    _idx = 0;
                yield return new WaitForSeconds(0.1f);
                Fade.Play("MainBackFadeIn");
                yield return new WaitForSeconds(9f);
            }
            else
                _slideShow = false;


        }
        BackBlack.SetActive(false);
        ForwardBlack.SetActive(false);
    }
}
