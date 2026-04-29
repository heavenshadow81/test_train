using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ML.PlaywallKids.Aquarium
{
    public class UserFish : MonoBehaviour
    {
        public Texture2D tex;
        public enum USER_INTERATION
        {
            NONE = -1,
            MOVE = 0,
            BACK,
            JUMP,
            SIT,
            HELLO,
            HEART,
            BUBBLE,
        }

        public USER_INTERATION cmd = USER_INTERATION.NONE;
        UserFishModeling fish;

        string basicPath = "/data/UserTemplate/0/Puffer/";
        List<Texture2D> textures = new List<Texture2D>();

        void Awake()
        {
            GameObject go = GameObject.Find("0408_bok");
            fish = go.GetComponent<UserFishModeling>();
        }

        public void Init(Texture2D[] list)
        {
            /*
            textures.Clear();
            string path = Application.dataPath;
            string[] split = path.Split('/');

            path = "";
            for(int i = 0; i < split.Length-2; ++i)
            {
                path += split[i];
            }

            path = path + basicPath;
            path = path.Replace("C:", "file//");
            for(int i = 0; i < 3; ++i)
            {
                string newPath = path + i.ToString();
                StartCoroutine("LoadTextures", newPath);
            }
            */

            Texture2D[] newList = new Texture2D[3];
            newList[0] = (Texture2D)Resources.Load("2", typeof(Texture2D));
            newList[1] = (Texture2D)Resources.Load("1", typeof(Texture2D));
            newList[2] = (Texture2D)Resources.Load("3", typeof(Texture2D));
            fish.Init(newList);
        }

        IEnumerator LoadTextures(string path)
        {
            Debug.Log(path);
            WWW www = new WWW(path);
            yield return www;
            textures.Add(www.texture);

            if (textures.Count >= 3)
            {
                fish.Init(textures.ToArray());
            }
        }

        public void Interation(EventData evt)
        {
            switch ((USER_INTERATION)evt.cmd)
            {
                case USER_INTERATION.MOVE:
                    Move(evt.user, 0, evt.val);
                    break;
                case USER_INTERATION.BACK:
                    Back(evt.user, 0, evt.val);
                    break;
                case USER_INTERATION.JUMP:
                    Jump(evt.user, 0, false);
                    break;
                case USER_INTERATION.SIT:
                    Sit(evt.user, 0, false);
                    break;
                case USER_INTERATION.HELLO:
                    break;
                case USER_INTERATION.HEART:
                    Heart(evt.user, 0, evt.val);
                    break;
                case USER_INTERATION.BUBBLE:
                    Bubble(evt.user, 0, evt.val);
                    break;
            }
        }

        void Move(int user, int fishIndex, float val)
        {
            fish.gameObject.transform.position += new Vector3(val, 0, 0);
        }

        void Back(int user, int fishIndex, float val)
        {
            fish.gameObject.transform.position += new Vector3(0, 0, val);
        }

        void Jump(int user, int fishIndex, bool finish)
        {
            TweenPosition tp = fish.GetComponent<TweenPosition>();
            tp.from = fish.transform.position;
            tp.to = tp.from + new Vector3(0, 20, 0);
            tp.enabled = true;
            tp.ResetToBeginning();

            if (!finish)
                Invoke("SitSelf", 1);
        }

        void SitSelf()
        {
            Sit(0, 0, true);
        }

        void Sit(int user, int fishIndex, bool finish)
        {
            TweenPosition tp = fish.GetComponent<TweenPosition>();
            tp.from = fish.transform.position;
            tp.to = tp.from + new Vector3(0, -20, 0);
            tp.enabled = true;
            tp.ResetToBeginning();

            if (!finish)
                Invoke("JumpSelf", 2);
        }

        void JumpSelf()
        {
            Jump(0, 0, true);
        }

        void Heart(int user, int fishIndex, float val)
        {
            fish.Heart();
        }

        void Bubble(int user, int fishIndex, float val)
        {
            fish.Bubble();
        }

        void LateUpdate()
        {
            if (Input.GetKey(KeyCode.F1))
                Move(0, 0, 1);
            else if (Input.GetKey(KeyCode.F2))
                Move(0, 0, -1);
            else if (Input.GetKeyDown(KeyCode.F3))
                Jump(0, 0, false);
            else if (Input.GetKeyDown(KeyCode.F4))
                Sit(0, 0, false);
            else if (Input.GetKeyDown(KeyCode.F5))
                Heart(0, 0, 0);
            else if (Input.GetKeyDown(KeyCode.F6))
                Bubble(0, 0, 0);
            else if (Input.GetKey(KeyCode.F7))
                Back(0, 0, -1);
            else if (Input.GetKey(KeyCode.F8))
                Back(0, 0, 1);
            else if (Input.GetKey(KeyCode.F10))
                Init(null);
        }
    }
}