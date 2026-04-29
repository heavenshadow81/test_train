using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EPrimaryMode = ContentsStoreItemType.Mode;

public class CommonButtonManufacturer : MonoBehaviour
{
    public CommonButtonManager buttonSpawner;
    private EPrimaryMode currentMode;
    public EPrimaryMode TestMode;

    void Awake()
    {
#if UNITY_EDITOR
        if (BigboardGlobal.currentMode != EPrimaryMode.None)
        {
            currentMode = BigboardGlobal.currentMode;
        }
        else
        {
            currentMode = TestMode;

        }
#else
          currentMode = BigboardGlobal.currentMode;
#endif
        StartCoroutine(LoginProcess());
    }

    IEnumerator LoginProcess()
    {

#if UNITY_EDITOR
        bool bLogin = false;

        //if (BigboardServerDataManager.GetListAllContentsStoreItemInfo().Count == 0)
        {
            BigboardServerDataManager.TestLogin(
               (bLoginSuccess) =>
               { bLogin = bLoginSuccess; }
               );
        }

        do
        {
            yield return new WaitForEndOfFrame();
        } while (!bLogin);
#endif

        List<ContentsStoreItemInfo> contents = BigboardServerDataManager.GetListMyContentsStoreItemInfo(currentMode);

        if (contents != null)
        {
            for (int i = 0; i < contents.Count; ++i)
            {
                if (contents[i].useYn)
                {
                    //ESubContents mode = BigboardServerDataManager.GetContentModeFromSequenceNumber(contents[i].seq);
                    BigboardContentMode mode = (BigboardContentMode)contents[i].seq;
                    string content = mode.ToString();
                    buttonSpawner.GenerateButton(contents[i].type.typeCode, content, contents[i].name, contents[i].seq);
                }
            }
            buttonSpawner.menuGrid.repositionNow = true;
        }
        else
        {
            yield return null;
        }
    }
}