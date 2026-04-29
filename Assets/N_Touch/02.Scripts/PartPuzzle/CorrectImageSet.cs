using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
//맞는 이미지를 골랐는가? 틀린 이미지를 골랐는가?
public class CorrectImageSet : MonoBehaviour
{
    public float aTime;
    #region 20221118 추가
    Image LevelSettingBtn;
    private void Awake()
    {
        LevelSettingBtn = GameObject.Find("LevelSettingBtn").GetComponent<Image>();
    }
    #endregion 
    void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOut());        
    }
    //그림 서서히 투명하게
    IEnumerator FadeOut()
    {
        Color shownImage = new Color(1,1,1,1);
        while (shownImage.a > 0)
        {
            shownImage.a -= Time.deltaTime / aTime;
            GetComponent<Image>().color = shownImage;
            yield return null;
        }
        // 20221118 추가
        LevelSettingBtn.raycastTarget = true;
        //

        gameObject.SetActive(false);
        yield break;
    }
    
}
