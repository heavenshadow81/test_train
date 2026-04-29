using System.Collections.Generic;
using UnityEngine;

namespace ML.PlaywallKids.Aquarium
{
    using AuthoringTool;
    public class UIControl : MonoBehaviour
    {
        public GameObject laodingBarPanel;
        public GameObject mainPanel;
        //	public UISprite loadingBarSprite;
        public GameObject _AuthoringTool;
        protected Dictionary<int, UISprite> loadingBarSprite = new Dictionary<int, UISprite>();
        protected Vector3 startPos;
        protected Vector3 endPos;
        Dictionary<int, int> frameCount = new Dictionary<int, int>();
        float updateRate = 1.0f / 24f; // 25 updates per sec.
        public UserInfo userInfo;
        Dictionary<int, float> startTime = new Dictionary<int, float>();
        protected UserData userData;
        Camera guiCam;
        Dictionary<int, bool> bClick = new Dictionary<int, bool>();
        int ncount = 0;
        private int touchId;
        Rect cameraRect;
        protected Dictionary<int, GameObject> loadingBarprefab = new Dictionary<int, GameObject>();
        int nIndex = 0;

        void Start()
        {
            userData = UserData.Instance();
            
            //GUI객체의 카메라 객체입니다.
            guiCam = NGUITools.FindCameraForLayer(gameObject.layer);

            cameraRect = new Rect(0, 0, Screen.width, Screen.height);

            //타겟의 포지션을 월드좌표에서 ViewPort좌표로 변환하고 다시 ViewPort좌표를 NGUI월드좌표로 변환합니다.
            startPos = guiCam.ScreenToWorldPoint(new Vector3(0f, 0f, 0f));

            endPos = guiCam.ScreenToWorldPoint(new Vector3((float)Screen.width, (float)Screen.height, 0f));
        }
        
        void Update()
        {
            if (ButtonUI.GetState() == 1 && userData.PutCheck())
            {
                if (touchId < 0)
                {
                    if (bClick[0])
                    {
                        LoadingBar(0, Input.mousePosition.x, Input.mousePosition.y);
                    }
                }
                else
                {
                    TouchInfo[] touches = CustomInput.touches;

                    for (int i = 0; i < touches.Length; i++)
                    {
                        var t = touches[i];
                        if ((t.phase == TouchInfo.Phase.Stay || t.phase == TouchInfo.Phase.Move) && bClick.ContainsKey(t.id) && bClick[t.id])
                        {
                            LoadingBar(t.id, t.axisX, t.axisY);
                        }
                    }
                }
            }
        }

        void SetTouchId(int id)
        {
            touchId = id;
        }

        void OnPress(bool isDown)
        {
            if (ButtonUI.GetState() == 1)
            {
                touchId = UICamera.currentTouchID;
                if (touchId < 0)
                {
                    if (!isDown)
                    {
                        frameCount[0] = 0;
                        startTime[0] = 0;
                        if (loadingBarprefab.ContainsKey(0))
                        {
                            Destroy(loadingBarprefab[0]);
                            loadingBarprefab.Remove(0);
                        }
                    }
                    bClick[0] = isDown;
                }
                else
                {
                    TouchInfo t = default(TouchInfo);
                    for (int i = 0; i < CustomInput.touchCount; i++)
                    {
                        t = CustomInput.GetTouch(i);
                        if (t.id == touchId)
                        {
                            if (t.phase == global::TouchInfo.Phase.End && !isDown)
                            {
                                frameCount[touchId] = 0;
                                startTime[touchId] = 0;
                                if (loadingBarprefab.ContainsKey(touchId) && loadingBarprefab[touchId] != null)
                                {
                                    Destroy(loadingBarprefab[touchId]);
                                    loadingBarprefab.Remove(touchId);
                                }
                                bClick[touchId] = false;
                            }
                            if (t.phase == global::TouchInfo.Phase.Begin)
                                bClick[touchId] = true;

                            break;
                        }
                    }
                }
            }
        }

        void LoadingBar(int touchId, float x, float y)
        {
            if (startTime.ContainsKey(touchId))
                startTime[touchId] += Time.deltaTime;
            else
                startTime[touchId] = Time.deltaTime;

            if (startTime[touchId] >= updateRate)
            {
                if (!frameCount.ContainsKey(touchId))
                    frameCount[touchId] = 0;

                if (frameCount[touchId] < 12)
                {
                    frameCount[touchId]++;
                    startTime[touchId] = 0;
                }
                else if (frameCount[touchId] == 12)
                {
                    loadingBarprefab[touchId] = NGUITools.AddChild(mainPanel, laodingBarPanel);
                    loadingBarprefab[touchId].transform.position = guiCam.ScreenToWorldPoint(new Vector3(x, y, 0f));

                    foreach (UISprite sprite in loadingBarprefab[touchId].GetComponentsInChildren<UISprite>())
                    {
                        if (sprite.name.Equals("LoadingBar Sprite"))
                        {
                            loadingBarSprite[touchId] = sprite;
                            loadingBarSprite[touchId].spriteName = "0";
                        }
                    }

                    frameCount[touchId]++;
                    startTime[touchId] = 0;

                }
                else if (frameCount[touchId] < 36)
                {

                    loadingBarSprite[touchId].spriteName = string.Format("{0}", frameCount[touchId] % 12);
                    frameCount[touchId]++;
                    startTime[touchId] = 0;
                }
                else
                {
                    frameCount[touchId] = 0;
                    startTime[touchId] = 0;

                    Vector2 pos = guiCam.WorldToScreenPoint(new Vector3(loadingBarprefab[touchId].transform.position.x, loadingBarprefab[touchId].transform.position.y - 0.15f, 0f));

                    loadingBarSprite[touchId].spriteName = "0";
                    loadingBarSprite.Remove(touchId);
                    if (loadingBarprefab.ContainsKey(touchId) && loadingBarprefab[touchId] != null)
                    {
                        Destroy(loadingBarprefab[touchId]);
                        loadingBarprefab.Remove(touchId);
                    }
                    
                    GameObject prefab = NGUITools.AddChild(mainPanel, _AuthoringTool);

                    int nInstanceID = prefab.GetComponent<UIPanel>().GetInstanceID();

                    userData.SetInstanceID(nInstanceID, ++ncount);
                    //userData.SetInstanceID (nInstanceID, userInfo.GetUserID ());
                    userData.SetState(nInstanceID, UserData.State.WAIT);

                    Vector2[] posArea = new Vector2[2];
                    foreach (UISprite sprite in prefab.GetComponentsInChildren<UISprite>())
                    {
                        if (sprite.name.Equals("BackGround Sprite"))
                        {
                            posArea[0] = new Vector2(pos.x + (sprite.transform.localScale.x / 2), pos.y + (sprite.transform.localScale.y * 0.5f));
                            posArea[1] = new Vector2(pos.x - (sprite.transform.localScale.x / 2), pos.y - (sprite.transform.localScale.y * 0.9f));

                            Debug.Log(cameraRect.xMin + " / " + cameraRect.xMax);
                            if (cameraRect.Contains(posArea[0]) && cameraRect.Contains(posArea[1]))
                            {
                            }
                            else
                            {
                                if (posArea[1].x < cameraRect.xMin)
                                {
                                    pos = new Vector2(cameraRect.xMin + (sprite.transform.localScale.x * 0.7f), pos.y);
                                }
                                else if (posArea[0].x > cameraRect.xMax)
                                {
                                    pos = new Vector2(cameraRect.xMax - (sprite.transform.localScale.x * 0.7f), pos.y);
                                }

                                if (posArea[1].y < cameraRect.yMin)
                                {
                                    pos = new Vector2(pos.x, cameraRect.yMin + (sprite.transform.localScale.y * 0.7f));
                                }
                                else if (posArea[0].y > cameraRect.yMax)
                                {
                                    pos = new Vector2(pos.x, cameraRect.yMax - (sprite.transform.localScale.y * 0.7f));
                                }
                            }
                        }
                    }
                    prefab.transform.position = guiCam.ScreenToWorldPoint(new Vector3(pos.x, pos.y, 0f));
                    bClick[touchId] = false;
                }
            }
        }
    }
}