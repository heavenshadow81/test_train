using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CorrectAnswerUI_CT : MonoBehaviour
{
    [SerializeField] Sprite[] changeImages;
    [SerializeField] Image[] bones;
    public int boneIdx;

    private void Awake()
    {
        bones = GetComponentsInChildren<Image>();
    }
    
    public void Init()
    {
        foreach(Image bone in bones)
        {
            bone.sprite = changeImages[0];
        }

        boneIdx = 0;
    }

    public void ChangeBoneImage()
    {
        Image boneImage =  bones[boneIdx].GetComponent<Image>();
        boneImage.sprite = changeImages[1];

        boneIdx++;
    }

    public int GetBoneIndex()
    {
        return boneIdx;
    }
}
