using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class LocalizationBigboard : MonoBehaviour
{
    public LocalizationDataType dataType = LocalizationDataType.String;
    public LocalizationKey curKey = LocalizationKey.NONE;

    void Awake()
    {
        SetKey(curKey, dataType);
    }
    
    void OnEnable()
    {
        SetKey(curKey, dataType);
	}

    void OnDisable()
    {
        StopAllCoroutines();
    }

    void OnDestroy()
    {
        LocalizationManager.RemoveItme(this);
    }
    
    public void SetKey(LocalizationKey key, LocalizationDataType type)
    {
        LocalizationManager.AddLocalization(gameObject, key, type);
        Replace();
    }

	public void Replace()
    {
        if (gameObject.activeInHierarchy)
        {
            if (curKey == LocalizationKey.NONE)
            {
                Debug.LogError("LocalizationBigboard.cs -  LocalizationKey is None");
                return;
            }

            StartCoroutine(ReplaceCoroutine());
        }
    }

    IEnumerator ReplaceCoroutine()
    {
        yield return new WaitForEndOfFrame();

        switch (dataType)
        {
            case LocalizationDataType.String:
                UILabel label = GetComponent<UILabel>();
                if (label != null)
                    label.text = GetData();
                else
                    Debug.LogError("LocalizationBigboard.cs -  UILabel is Null");

                break;

            case LocalizationDataType.Image:
                UISprite sprite = GetComponent<UISprite>();
                if (sprite != null)
                    sprite.spriteName = GetData();
                else
                    Debug.LogError("LocalizationBigboard.cs -  UISprite is Null");
                // replace sprite.MakePixelPerfect?

                break;
        }
    }

    private string GetData()
    {
        return LocalizationManager.GetData(curKey);
    }
}
