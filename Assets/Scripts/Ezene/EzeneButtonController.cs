using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

//버튼 Icon 지정하기!
public class EzeneButtonController : MonoBehaviour
{
    [SerializeField]
    List<Texture2D> current = new List<Texture2D>();
    string iconPath;
    [SerializeField]
    List<Image> buttons = new List<Image>();
    [SerializeField]
    List<byte[]> curr = new List<byte[]>();
    
    //활성화 될 때마다 아이콘 바꾸기!!!
    private void OnEnable()
    {
        current.Clear();
        curr.Clear();
        if (Directory.Exists(EzeneFileWay.Instance.FolderPaths))
        {
            //과학에선 맞는 것...
            iconPath = EzeneFileWay.Instance.FolderPaths.Replace(@"\영상", "");
            //영어에선...?폴더, 아이콘 복사로 해결...
        }
        print(iconPath);
        print(Directory.GetFiles(iconPath)[0]);
        print(Directory.GetFiles(iconPath).Length);
        for(int i = 0; i<Directory.GetFiles(iconPath).Length; i++)
        {
            if (!Directory.GetFiles(iconPath)[i].Contains(".meta"))
            {
                print(Directory.GetFiles(iconPath)[i]);
                var bytes = File.ReadAllBytes(Directory.GetFiles(iconPath)[i]);
                //Texture2D tex = new Texture2D(816, 630);
                //tex.LoadImage(bytes);
                //current.Add(tex);
                curr.Add(bytes);
            }
        }

        //버튼 활성화!!
        for (int i = 0; i < buttons.Count; i++)
        {
            if(Directory.GetFiles(EzeneFileWay.Instance.FolderPaths).Length - 1 >= i)
            {
                buttons[i].sprite.texture.LoadImage(curr[i]);
            }
            buttons[i].enabled = Directory.GetFiles(EzeneFileWay.Instance.FolderPaths).Length - 1 >= i;
            print(buttons[i]);
        }
    }
    
}
