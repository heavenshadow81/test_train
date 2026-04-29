using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System.Text;
//이젠 커뮤니케이션 파일 위치...!
public class EzeneFileWay : MonoBehaviour
{
    //싱글톤 설정
    static EzeneFileWay _instance;
    public static EzeneFileWay Instance { get => _instance; }

    [SerializeField]
    string filepath, directories, folderpath, rootfoldername;
    private void Awake()
    {
        if (!_instance)
        {
            _instance = this;
        }
    }

    private void Start()
    {
        directories = Application.dataPath + @"\영상"+ directories;
        folderpath = directories;
        if (File.Exists(filepath))
        {
            var data = File.ReadAllText(filepath);
            var da = JsonConvert.DeserializeObject<EzeneFiles>(data);
            directories = da.directories;
        }
    }

    private void OnEnable()
    {
        folderpath = directories;
    }
    //파일 패스 지정 전에 하위 폴더 지정
    public void SetPath(int index)
    {
        var pa = Directory.GetDirectories(folderpath)[index];
        folderpath = pa;
    }
    //이전 폴더 위치로 되돌리기!
    public void SetRevert()
    {
        var pa = Directory.GetParent(folderpath).FullName;
        
        print($"상위 폴더: {pa}, {pa.Contains(rootfoldername)}");
        if (pa.Contains(rootfoldername))
        {
            folderpath = pa;
        }
    }
    //파일 패스로 지정된 곳의 파일 부르기!!
    public void SetFile(int index)
    {
        var pa = Directory.GetFiles(folderpath)[index];
        filepath = pa;
    }
    //파일 위치
    public string Path => filepath;
    //폴더 위치
    public string FolderPaths => folderpath;
}
