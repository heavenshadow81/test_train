using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    using Common;

    public class TwoDimensionMode
    {
        public int iID { get; set; }
        public string szMode { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TwoDimensionTable
    {
        public virtual string getFileName()
        {
            return "TwoDimensionMode";
        }

        public virtual string getFilePath()
        {
            return "TwoDimensionContents/File/";
        }

        private Hashtable _hashTable = new Hashtable();

        public ICollection keys
        {
            get
            { return _hashTable.Keys; }
        }

        public TwoDimensionTable()
        {
            // if (!LoadTable())
            // { Debug.Log("Load TwoDimensionTable Error"); }
        }

        public TwoDimensionTable(string szFileName)
        {
            if (!LoadTable(szFileName))
            { Debug.Log("Load TwoDimensionTable Error"); }
        }

        public bool LoadTable()
        {
            LoadTableContentsModeFromServer();
            return true;
        }

        void CallbackLoading(object _obj)
        {
            LoadTableContentsModeFromServer();
        }

        public void LoadTableContentsModeFromServer()
        {
            List<ContentsStoreItemInfo> contents = BigboardServerDataManager.GetListMyContentsStoreItemInfo(ContentsStoreItemType.Mode.Drawing2D);
            //List<ContentsStoreItemInfo> contents = BigboardServerDataManager.GetListUseContentsStoreItemInfo(ContentsStoreItemType.Mode.Drawing2D);
            if (contents != null)
            {
                for (int i = 0; i < contents.Count; ++i)
                {
                    if (contents[i].useYn)
                    {
                        TwoDimensionMode _mode = new TwoDimensionMode();
                        _mode.iID = contents[i].seq;
                        _mode.szMode = ((BigboardContentMode)contents[i].seq).ToString();

                        Debug.Log((BigboardContentMode)contents[i].seq);

                        if (!_hashTable.ContainsKey(_mode.iID))
                        {
                            Debug.Log((BigboardContentMode)contents[i].seq + " Add To HashTable ");
                            _hashTable.Add(_mode.iID, _mode);
                        }
                    }
                }
            }
            else
            {
                Debug.Log(" TwoDimensionTable.LoadTableContensModeFromServer is null");
            }
        }

        private void LoadTableContentsMode()
        {
            string resPath = Path.Combine(getFilePath(), getFileName()).Replace("\\", "/");

            TextAsset textAsset = (TextAsset)Resources.Load(resPath, typeof(TextAsset));

            string[] szArrayLlines = textAsset.text.Split("\n"[0]);
            for (int iCnt = 0; iCnt < szArrayLlines.Length; iCnt++)
            {
                string[] szArrayRow = CSVUtil.SplitCsvLine(szArrayLlines[iCnt]);
                if (szArrayRow.Length == 0)
                    continue;

                TwoDimensionMode _mode = new TwoDimensionMode();
                _mode.iID = System.Convert.ToInt32(szArrayRow[0]);
                _mode.szMode = System.Convert.ToString(szArrayRow[1]);
                Debug.Log(_mode.szMode);
                _hashTable.Add(_mode.iID, _mode);
            }
        }

        public bool LoadTable(string szFileName)
        {
            string resPath = Path.Combine(getFilePath(), szFileName).Replace("\\", "/");
            // string resPath = Path.Combine(getFilePath(), getFileName()).Replace("\\", "/");
            TextAsset textAsset = (TextAsset)Resources.Load(resPath, typeof(TextAsset));

            string[] szArrayLlines = textAsset.text.Split("\n"[0]);
            for (int iCnt = 0; iCnt < szArrayLlines.Length; iCnt++)
            {
                string[] szArrayRow = CSVUtil.SplitCsvLine(szArrayLlines[iCnt]);
                if (szArrayRow.Length == 0)
                    continue;

                TwoDimensionMode _mode = new TwoDimensionMode();
                _mode.iID = System.Convert.ToInt32(szArrayRow[0]);
                _mode.szMode = System.Convert.ToString(szArrayRow[1]);
                Debug.Log(_mode.szMode);
                _hashTable.Add(_mode.iID, _mode);
            }
            return true;
        }

        public void SaveTableTwoDimensionContents()
        {

        }

        public int TableCount()
        {
            return _hashTable.Count;
        }

        public TwoDimensionMode GetTwoDimensionMode(int iID)
        {
            if (_hashTable.ContainsKey(iID) == false)
                Debug.LogWarning("GetID( int iID ) " + iID);

            return (TwoDimensionMode)_hashTable[iID];
        }

        public void Destroy()
        {
            _hashTable.Clear();
            _hashTable = null;

        }
    }

    /// <summary>
    /// 2D Table for EAS Server admin(<see cref="EASRemoteTwoDimensionAdmin"/>).
    /// </summary>
    public class TwoDimensionTable_EASRemote : TwoDimensionTable
    {
        public override string getFileName()
        {
            return "TwoDimensionMode_EASRemote";
        }
    }

    /// <summary>
    /// 2D Table for EAS Client admin(<see cref="EASClientTwoDimensionAdmin"/>).
    /// </summary>
    public class TwoDimensionTable_EASClient : TwoDimensionTable
    {
        public TwoDimensionTable_EASClient() : base("TwoDimensionMode_EASClient")
        {

        }

        public override string getFileName()
        {
            return "TwoDimensionMode_EASClient";
        }
    }
}