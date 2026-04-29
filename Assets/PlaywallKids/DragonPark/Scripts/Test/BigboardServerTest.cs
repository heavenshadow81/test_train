using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.DragonPark.Test
{
    public class BigboardServerTest : MonoBehaviour
    {
        public AnimatablePanel contentsManagementPanel;

        private bool _loggingIn = false;

        private List<ContentsStoreItemInfo> contentsList = new List<ContentsStoreItemInfo>();

        void Start()
        {
            BigboardServerDataManager.TestLogin();
        }

        //void OnGUI()
        //{
        //    return;
        //    if (BigboardServer.userSeq < 0)
        //    {
        //        if (!_loggingIn)
        //        {
        //            if (GUI.Button(new Rect(20, 20, 200, 20), "Login"))
        //            {
        //                _loggingIn = true;

        //                BigboardServer.Login("admin", "admin", (conn, retCode, message, flag) =>
        //                {
        //                    _loggingIn = false;
        //                });
        //            }
        //        }
        //        else
        //        {
        //            GUI.Label(new Rect(20, 20, 200, 20), "Logging in...");
        //        }
        //    }
        //    else
        //    {
        //        GUI.Label(new Rect(20, 20, 200, 20), string.Format("Login success! (userSeq: {0})", BigboardServer.userSeq));
        //    }

        //    if (GUI.Button(new Rect(20, 50, 100, 20), "Contents"))
        //    {
        //        BigboardServer.GetContentsList((conn, retCode, message) =>
        //        {
        //            Debug.Log("ContentsList - (" + retCode + ", " + message + ")");

        //            contentsList = BigboardServerDataManager.GetListAllContentsStoreItemInfo();
        //        });
        //    }

        //    if (GUI.Button(new Rect(220, 50, 100, 20), "My Contents"))
        //    {
        //        BigboardServer.GetMyContents((conn, retCode, message) =>
        //        {
        //            Debug.Log("My Contents - (" + retCode + ", " + message + ")");
        //        });
        //    }

        //    if (GUI.Button(new Rect(340, 50, 100, 20), "States"))
        //    {
        //        BigboardServer.GetState(null);
        //    }

        //    for (int i = 0; i < contentsList.Count; i++)
        //    {
        //        var c = contentsList[i];
        //        GUI.Label(new Rect(20, 80 + i * 20, 380, 20), string.Format("{0} - {1},{2},{3}", c.name, c.description, c.price, c.regDate));
        //    }

        //    var myContents = BigboardServerDataManager.GetListMyContentsStoreItemInfo();
        //    for (int i = 0; i < myContents.Count; i++)
        //    {
        //        var c = myContents[i];
        //        GUI.Label(new Rect(420, 80 + i * 20, 380, 20), string.Format("{0} - {1},{2},{3}", c.name, c.description, c.price, c.regDate));
        //    }

        //    if (GUI.Button(new Rect(460, 50, 200, 20), "Show ContentsManagement"))
        //    {
        //        contentsManagementPanel.Show();
        //    }
        //}
    }
}