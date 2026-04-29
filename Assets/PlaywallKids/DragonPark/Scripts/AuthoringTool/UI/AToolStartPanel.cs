using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.DragonPark
{
    /// <summary>
    /// Authoring Tool Starter.
    /// Shows "Welcome to Playwall Kids" and user list with name.
    /// </summary>
    public class AToolStartPanel : MonoBehaviour
    {
        #region Public variables
        public GameObject balloon;
        public GameObject AToolPrefab;

        public int userCount = 0;
        public int[] userSeqs = null;

        public Dictionary<int, GameObject> balloons = new Dictionary<int, GameObject>();
        public Dictionary<int, AToolMainPanel> ATools = new Dictionary<int, AToolMainPanel>();
        public delegate Vector3 GetPosition(int id);
        #endregion

        #region Properties
        /*
        private string[] _userNames = null;
        public string[] userNames {
            get {
                return _userNames;
            }
            set {
                _userNames = value;
                if(balloons != null) {
                    for(int i = 0; i < balloons.Length; i++) {
                        GameObject b = balloons[i];
                        UILabel nameLabel = b.GetComponentInChildren<UILabel>();
                        if(_userNames != null && _userNames.Length > i) {
                            nameLabel.text = _userNames[i];
                        }
                        else {
                            break;
                        }
                    }
                }
            }
        }

        private Texture2D[] _userImages = null;
        public Texture2D[] userImages {
            get {
                return _userImages;
            }
            set {
                _userImages = value;
                if(balloons != null) {
                    for(int i = 0; i < balloons.Length; i++) {
                        GameObject b = balloons[i];
                        UITexture imageTexture = b.GetComponentInChildren<UITexture>();
                        if(_userImages != null && _userImages.Length > i && imageTexture != null) {
                            imageTexture.mainTexture = (Texture)_userImages[i];
                        }
                        else {
                            break;
                        }
                    }
                }
            }
        }
        */

        #endregion

        public void Start()
        {
            balloon.SetActive(false);
            //Show (4);
        }

        public void Update()
        {
            CheckTouchEvent();
        }

        public void OnDestroy()
        {
            for (int i = 0; i < userCount; i++)
            {
                var template = SimpleInstantiatedTemplateControl.GetCurrentTemplate((byte)i);
                if (template != null)
                {
                    DragonComeToFront comeToFront = template.GetComponent<DragonComeToFront>();
                    if (comeToFront != null)
                    {
                        if (comeToFront.isComing || comeToFront.came)
                        {
                            comeToFront.Back();
                        }
                    }
                }
            }
        }

        public void CheckTouchEvent()
        {
            float width = Screen.width;
            float height = Screen.height;
            float offset = width / userCount;

            for (int i = 0; i < CustomInput.touchCount; i++)
            {
                TouchInfo touch = CustomInput.GetTouch(i);
                if (touch.phase != TouchInfo.Phase.Begin || touch.type == TouchInfo.Type.Custom) continue;
                Vector3 pos = touch.position;

                // check ui touch
                Ray uiRay = UICamera.currentCamera.ScreenPointToRay(new Vector3(pos.x, pos.y));
                if (!Physics.Raycast(uiRay))
                {
                    // get user id
                    int userId = (int)(pos.x / offset);
                    if (userId >= userCount)
                    {
                        userId = userCount - 1;
                    }

                    // check
                    if (ATools.ContainsKey(userId))
                    {
                        if (ATools != null && userId < ATools.Count && ATools[userId] != null && !ATools[userId].gameObject.activeSelf)
                        {
                            var template = SimpleInstantiatedTemplateControl.GetCurrentTemplate((byte)userId);
                            DragonComeToFront comeToFront = template.GetComponent<DragonComeToFront>();
                            if (comeToFront == null)
                            {
                                comeToFront = template.gameObject.AddComponent<DragonComeToFront>();
                            }
                            if (!comeToFront.isComing && !comeToFront.came)
                            {
                                comeToFront.Come(DragonComeToFront.ComeReason.Interaction);
                            }
                            else if (comeToFront.came)
                            {
                                comeToFront.BackTimerInit();

                                if (BackgroundManager.sharedInstance != null && BackgroundManager.sharedInstance.mainCamera != null)
                                {
                                    BarrelDistortionEffect distortionEffect = BackgroundManager.sharedInstance.mainCamera.GetComponent<BarrelDistortionEffect>();

                                    if (distortionEffect != null)
                                    {
                                        pos = distortionEffect.GetDistoredScreenPosFromOriginal(pos);
                                    }
                                }
                                Ray ray = Camera.main.ScreenPointToRay(new Vector3(pos.x, pos.y));
                                RaycastHit hit;
                                if (Physics.Raycast(ray, out hit))
                                {
                                    if (hit.collider.gameObject == comeToFront.gameObject)
                                    {
                                        DragonAnimationControl dragonAnimation = comeToFront.GetComponent<DragonAnimationControl>();
                                        if (dragonAnimation.isIdle)
                                        {
                                            dragonAnimation.CharmingAct();
                                        }
                                    }
                                    else
                                    {
                                        comeToFront.Back();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        public void Show(int userCount, int[] userSeqs, Dictionary<int, DragonParkUserInfo> listUser, GetPosition getPosition)
        {
            this.userCount = userCount;
            this.userSeqs = new int[userCount];
            for (int i = 0; i < userCount; i++)
            {
                if (userSeqs != null && userSeqs.Length > i)
                {
                    this.userSeqs[i] = userSeqs[i];
                }
                else
                {
                    break;
                }
            }

            for (int i = 0; i < userCount; i++)
            {
                GameObject parent = Show(i, getPosition);
                if (listUser.ContainsKey(i))
                    listUser[i].SetBtnExitParent(parent);
            }

            /*
            float height = UIRoot.list[0].activeHeight;
            float width = height * ((float)Screen.width / (float)Screen.height);
            float areaX = width / userCount;

            for(int i = 0; i < userCount; i++) {
                int userId = i;
                GameObject newBalloon = NGUITools.AddChild(gameObject, balloon);
                newBalloon.SetActive(true);
                newBalloon.transform.localPosition = new Vector3(areaX * (i - (userCount - 1.14f) * 0.5f), newBalloon.transform.localPosition.y, 0.0f);
                balloons[i] = newBalloon;

                UIButton button = newBalloon.GetComponent<UIButton>();
                button.onClick.Add(new EventDelegate(()=>{ ShowATool(userId); }));

                newBalloon.GetComponent<AToolStartPanelBalloon>().userSeq = userSeqs[i];
            }
            */

            /*
                    userImages = _userImages;
                    if(_userNames == null) {
                        string[] newNames = new string[userCount];
                        for(int i = 0; i < userCount; i++) {
                            newNames[i] = string.Format("User {0}", i+1);
                        }
                        userNames = newNames;
                    }
                    else {
                        userNames = _userNames;
                    }
            */
            DragonComeToFront.userCount = userCount;
        }

        public GameObject Show(int userID, GetPosition getPosition)
        {
            return ShowATool(userID, getPosition);

            #region
            /*
            GameObject newBalloon = NGUITools.AddChild(gameObject, balloon);
            if (balloons.ContainsKey(userID))
                balloons[userID] = newBalloon;
            else
                balloons.Add(userID, newBalloon);

            userCount = 0; 
            foreach(KeyValuePair<int, GameObject> item in balloons)
                if( item.Value != null)
                    userCount++;

            // 임시로 적용 EAS 적용시 새로 적용이 필요함
            userCount = CustomInput.maxUserCount;

            float height = UIRoot.list[0].activeHeight;
            float width = height * ((float)Screen.width / (float)Screen.height);
            float areaX = width / userCount;

            newBalloon.SetActive(true);
            newBalloon.transform.localPosition = new Vector3(areaX * (userID - (userCount - 1.14f) * 0.5f), newBalloon.transform.localPosition.y, 0.0f);


            UIButton button = newBalloon.GetComponent<UIButton>();
            button.onClick.Add(new EventDelegate(() => { ShowATool(userID); }));

            newBalloon.GetComponent<AToolStartPanelBalloon>().userSeq = userID;//userSeqs[userID];

            // 풍선 바로 터지기
            return ShowATool(userID);
            */
            #endregion
        }

        public void DeleteATool(int userID)
        {
            if (balloons.ContainsKey(userID))
            {
                if (balloons[userID] != null)
                {
                    Destroy(balloons[userID]);
                    balloons[userID] = null;
                }
            }

            if (ATools.ContainsKey(userID))
            {
                if (ATools[userID] != null)
                {
                    Destroy(ATools[userID].gameObject);
                    ATools[userID] = null;
                }
            }
        }

        public GameObject ShowATool(int user, GetPosition getPosition)
        {
            #region
            /*
            GameObject balloon = balloons[user];
            ParticleSystem particle = balloon.GetComponentInChildren<ParticleSystem>();
            if(particle != null) {
                if(particle.gameObject != balloon) {
                    particle.transform.parent = this.transform;
                    particle.gameObject.SetActive(true);
                }
            }
            */
            #endregion
            if (ATools.ContainsKey(user) == false)
                ATools.Add(user, null);

            if (ATools[user] == null)
            {
                AToolMainPanel atool = NGUITools.AddChild(gameObject, AToolPrefab).GetComponent<AToolMainPanel>();
                atool.gameObject.SetActive(true);
                atool.SetDelegate(DeleteATool);
                atool.userId = (byte)user;
                atool.userSeq = user;//userSeqs[user];

                float height = UIRoot.list[0].activeHeight;
                float width = height * ((float)Screen.width / (float)Screen.height);
                float areaX = width / CustomInput.maxUserCount;

                atool.transform.localPosition = getPosition(user);// ScreenManager.ViewportToNGUIScreen(((1f / (SettingsManager.maxUserCount + 1)) * (user + 1)), 0.3f);
                                                                  //new Vector3(areaX * (user - (userCount - 1.14f) * 0.5f), -UIRoot.list[0].activeHeight * 0.1f, 0.0f);

                ATools[user] = atool;

                balloon.GetComponent<Animator>().SetTrigger("pop");

                AToolStartPanelBalloon b = balloon.GetComponent<AToolStartPanelBalloon>();
                b.Pop(0.25f);

                return atool.gameObject;
            }
            Debug.LogError("WORNG");
            return null;
        }
    }
}