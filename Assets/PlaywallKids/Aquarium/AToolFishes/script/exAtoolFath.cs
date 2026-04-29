using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.Aquarium
{
    public class exAtoolFath : MonoBehaviour
    {
        // initStart()에 넘겨줄 때 사용할 초기화 모드
        public enum InitMode : int
        {
            None = 0,           // 아무 동작 없음
            PresentAndWait,     // 화면 앞 이동 후 대기
            PresentAndDontWait, // 화면 앞 이동 후 바로 헤엄치기
            Swim                // 화면 앞 표시 없이 바로 헤엄치기
        }

        public string identifier { get { return info != null ? info.identifier : ""; } }
        public string templateName { get { return info != null ? info.templateName : ""; } }
        public int userId { get { return info != null ? info.userId : -1; } }
        

        public float startTime = 0;
        public float time = 30;

        public AToolFishInfo info;
        
        public bool draw = false;
        public Color pathColor = Color.white;

        public Vector3[] path;
        public Vector3[] rePath;

        Vector3 orgPos;

        Vector3 startPos;

        BoxCollider Coll;
        
        void Start()
        {
            orgPos = transform.position;
            Coll = gameObject.GetComponent<BoxCollider>();
        }

        void OnDestroy()
        {
            ResourceManager.ReleaseAToolFishInfo(info);
            info = null;
        }

        public void reFoundPath()
        {
            /* Boid AI 기반으로 움직이게 하기 */
            Boid b = GetComponent<Boid>();
            if (b == null)
                b = gameObject.AddComponent<Boid>();
            b.enabled = true;
            b.SetMaxNumberOfNeighbors(24);
            b.target = BoidAFishPath.GetDummy(string.Format("AFishPath_{0}_{1}", Random.Range(1, 4), Random.Range(1, 4)));
        }

        public void Reset()
        {
            connectMgr.Instance().sendTexPath(userId, templateName);
            Destroy(gameObject);
        }

        public void initStart(AToolFishInfo newInfo, InitMode newMode)
        {
            if (newInfo == null)
            {
                Debug.LogError("info is null.");
                return;
            }

            info = newInfo;
            orgPos = transform.position;
            transform.localRotation = Quaternion.identity;

            setTextures();

            if (newMode == InitMode.None)
            {
                transform.position = AquariumDummyUtils.GetSpawnPos(userId, "4");
                transform.rotation = Quaternion.LookRotation(-Camera.main.transform.right, transform.up);
                initScale1();
            }
            else if (newMode == InitMode.PresentAndWait)
            {
                path = new Vector3[3];
                path[0] = AquariumDummyUtils.GetSpawnPos(userId, "1");
                path[1] = (AquariumDummyUtils.GetSpawnPos(userId, "1") + AquariumDummyUtils.GetSpawnPos(userId, "4")) * 0.5f;
                path[2] = AquariumDummyUtils.GetSpawnPos(userId, "4");
                transform.position = path[0];
                time = 6;
                Invoke("tweenFront", startTime);
            }
            else if (newMode == InitMode.PresentAndDontWait)
            {
                path = new Vector3[12];
                path[0] = AquariumDummyUtils.GetSpawnPos(userId, "1");
                path[1] = AquariumDummyUtils.GetSpawnPos(userId, "4");
                path[2] = AquariumDummyUtils.GetSpawnPos(userId, "4") + Camera.main.transform.right * -0.04f;    // to show left side of fish
                path[3] = AquariumDummyUtils.GetSpawnPos(userId, "1");
                path[4] = AquariumDummyUtils.GetSpawnPos(userId, "3");
                path[5] = AquariumDummyUtils.GetSpawnPos(userId, "2");
                path[6] = AquariumDummyUtils.GetSpawnPos(userId, "1");
                path[7] = AquariumDummyUtils.GetSpawnPos(userId, "3");
                path[8] = AquariumDummyUtils.GetSpawnPos(userId, "2");
                path[9] = AquariumDummyUtils.GetSpawnPos(userId, "1");
                path[10] = AquariumDummyUtils.GetSpawnPos(userId, "4");
                path[11] = AquariumDummyUtils.GetSpawnPos(userId, "4") + Camera.main.transform.right * -0.04f;    // to show left side of fish
                transform.position = path[0];
                time = 30;
                Invoke("tween", startTime);
            }
            else
            {
                reFoundPath();
            }
        }

        private void initScale1()
        {
            transform.localScale = Vector3.one * 0.01f;
            iTween.ScaleTo(gameObject, iTween.Hash("scale", Vector3.one * 0.2f, "time", 0.66f, "oncomplete", "initScale2"));
        }

        private void initScale2()
        {
            transform.localScale = Vector3.one;
            iTween.PunchScale(gameObject, iTween.Hash("amount", Vector3.one * -0.8f, "time", 1.0f));
            GameMgr.Ins().SetSoundEffect(gameObject, SOUNDEFFECT.POP);
        }

        public void release()
        {
            reFoundPath();
        }

        public void setTextures()
        {
            // 1
            if (info.textures.Count > 0)
            {
                Texture texture = info.textures[0];
                if (templateName == "Jellyfish")
                {
                    transform.Find("A").GetComponent<Renderer>().materials[0].mainTexture = texture;
                    transform.Find("A").GetComponent<Renderer>().materials[1].mainTexture = texture;
                }
                else
                {
                    transform.Find("A").GetComponent<Renderer>().material.mainTexture = texture;
                }
            }

            // 2
            if (info.textures.Count > 1)
            {
                Texture texture = info.textures[1];
                transform.Find("B").GetComponent<Renderer>().material.mainTexture = texture;
            }

            // 3
            if (info.textures.Count > 2)
            {
                Texture texture = info.textures[2];
                transform.Find("C").GetComponent<Renderer>().material.mainTexture = texture;
            }
        }
    }
}