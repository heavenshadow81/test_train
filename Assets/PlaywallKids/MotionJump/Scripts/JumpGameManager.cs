//#define USE_TAG

using UnityEngine;

namespace ML.PlaywallKids.MotionJump
{
    /// <summary>
    /// 프리팹 복제 및 모션 데이터를 각각의 컨트롤러에 전달하는 클래스
    /// </summary>
    [RequireComponent(typeof(MotionManager))]
    public class JumpGameManager : MonoBehaviour
    {
        public enum EResolution { NONE = 0, TWO_BY_THREE = 3, TWO_BY_SIX = 6 }

        const string szFilePath = "Interaction/Jump/";
        const string szFileName = "InteractionGameJump";

        const float fGameControllerWidth = 15f;
        [HideInInspector]
        public GameObject gameControllerPrefab;
        [HideInInspector]
        public int numberOfPlayers;

        private JumpGameController[] controllers;
        private MotionManager motionManager;

        private int countOfStarting;

#if USE_TAG
    System.Collections.Generic.List<NFCUserInfo> userList = new System.Collections.Generic.List<NFCUserInfo>();
#endif

        void Awake()
        {
            // 인원 별 복제
            int ratio = (Screen.width / Screen.height);
            EResolution resolution = ratio == 2 ? EResolution.TWO_BY_THREE : ratio == 5 ? EResolution.TWO_BY_SIX : EResolution.NONE;

            motionManager = gameObject.GetComponent<MotionManager>();
            //해상도 비율- 스마트 빅보드 설치 된 모니터 수
            switch (resolution)
            {
                default:
                case EResolution.TWO_BY_THREE:
                    numberOfPlayers = 2;
                    motionManager.SetDefaultData((int)EResolution.TWO_BY_THREE);
                    break;
                case EResolution.TWO_BY_SIX:
                    numberOfPlayers = 4;
                    motionManager.SetDefaultData((int)EResolution.TWO_BY_SIX);
                    break;
            }

            if (gameControllerPrefab == null)
            { gameControllerPrefab = Resources.Load(szFilePath + szFileName) as GameObject; }

            controllers = new JumpGameController[numberOfPlayers];
            float camWidth = (float)(1 / (float)numberOfPlayers);

            // 인원별 카메라 및 index 설정 
            for (int i = 0; i < numberOfPlayers; ++i)
            {
                GameObject tempClone = Instantiate(gameControllerPrefab) as GameObject;
                tempClone.transform.position = new Vector3(fGameControllerWidth * i, 0, 0);
                controllers[i] = tempClone.GetComponent<JumpGameController>();
                tempClone.SetActive(true);
                controllers[i].camController.cam.rect = new Rect(camWidth * i, 0, camWidth, 1); //adjust game camera viewRect position & size
                UIRoot uiRoot = controllers[i].gameObject.GetComponentInChildren<UIRoot>();
                uiRoot.GetComponentInChildren<Camera>().rect = controllers[i].camController.cam.rect;//adjust ngui camera viewRect position & size
                JumpGameController controller = tempClone.GetComponent<JumpGameController>();
                controller.userIndex = i;
                controller.motionManager = GetComponent<MotionManager>();
                if (controller.motionManager == null) controller.motionManager = this.gameObject.AddComponent<MotionManager>();
                if (i != 0) controller.basicMenu.gameObject.SetActive(false);
            }

            countOfStarting = 0;
            // this.GetComponentInChildren<Camera>().rect = new Rect(0, 0, camWidth, 1f);
        }

        void Update()
        {
            // 사용자 모션 데이터 전달
            for (int i = 0; i < controllers.Length; ++i)
            {
                JumpJumpData kinectInfo = motionManager.GetCommand(i);
                if (kinectInfo != null)
                { controllers[i].SendCommand(kinectInfo.inUser, ref kinectInfo.isJump, kinectInfo.command, kinectInfo.body); }
            }

#if USE_TAG
        while (NFCClientSocket.instance.Count > 0)
        {
            NFCUserInfo _info = NFCUserInfo.GetNFCUserInfo( NFCClientSocket.instance.GetStringValue() );
            userList.Add(_info);
        }
    }

    void FixedUpdate()
    {
        if(userList.Count > 0)
        {
            NFCUserInfo _info = userList[0];
            int _index= 0;
            if(CheckEqualUser(_info.userName, _index))
            {
                for(int i = 0 ; i< controllers.Length ; ++i)
                {
                    if(! controllers[i].IsPlaying  && !controllers[i].IsTaged)
                    {
                        controllers[i].SetNFCValue(_info);
                        userList.RemoveAt(0);
                    }
                }
            }
            return;
        }
    }

    bool CheckEqualUser(string _name , int _index)
    {
        if(_index < controllers.Length)
        {
            if (controllers[_index].IsTaged || controllers[_index].IsPlaying)
            {
                if (controllers[_index++].EqualUser(_name))
                {
                    userList.RemoveAt(0);
                    return true;
                }
            }
            else
                return CheckEqualUser(_name, ++_index);
        }

        return false;
#endif

        }

        //public void SendCommand(int _playerIndex, Body _body,  string _message, string _counter = null)
        //{
        //    _playerIndex -=1;
        //    controllers[_playerIndex].SendCommand(_body, _message, _counter);
        //}
    }
}