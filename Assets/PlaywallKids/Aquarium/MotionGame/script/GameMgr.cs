using UnityEngine;

namespace ML.PlaywallKids.Aquarium
{
    public enum SOUNDEFFECT
    {
        SMILE,
        FART,
        HAHA,
        HERE,
        CLICK,
        FOOD,
        FEAR,
        POP,
        MOVETO,
        EAT
    }

    public class GameMgr : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Q))
            {
                SetPos_Heart(new Vector3(Random.Range(-270, 270), Random.Range(50, 255), -200));
            }
            if (Input.GetKeyUp(KeyCode.W))
            {
                SetPos_Food(new Vector3(Random.Range(-270, 270), Random.Range(50, 255), -200));
            }

            if (Input.GetKeyUp(KeyCode.E))
            {
                SetItem_FartParticle(GameObject.Find("Moorish1").transform);
            }
        }

        //SetItem
        public void SetItem_Hello(Transform tParent, int type)
        {
            GameObject Obj = Instantiate(GameObject.Find("itmHello"), tParent.transform.position,
                GameObject.Find("itmHello").transform.localRotation) as GameObject;

            Obj.transform.rotation = tParent.transform.rotation;
            Obj.transform.parent = tParent;

            switch (type)
            {
                case 0:
                    Obj.transform.localScale *= 1.0f;
                    break;
                case 1:
                    Obj.transform.localScale *= 2.0f;
                    break;
                case 2:
                    Obj.transform.localScale *= 3.5f;
                    break;
                case 3:
                    Obj.transform.localScale *= 7.0f;
                    break;
            }

            Destroy(Obj, 10.0f);
        }

        public void SetItem_FartParticle(Transform tParent)
        {
            GameObject Obj = Instantiate(GameObject.Find("itmParticle_Fart"), tParent.transform.position,
                GameObject.Find("itmParticle_Fart").transform.localRotation) as GameObject;

            Obj.transform.rotation = tParent.transform.rotation;
            Obj.transform.parent = tParent;

            Destroy(Obj, 6.0f);
        }

        public void SetItem_MoveParticle(Transform tParent, float time)
        {
            GameObject Obj = Instantiate(GameObject.Find("itmParticle_Move"), tParent.transform.position,
                GameObject.Find("itmParticle_Move").transform.localRotation) as GameObject;

            Obj.transform.rotation = tParent.transform.rotation;
            Obj.transform.parent = tParent;

            Destroy(Obj, time);
        }

        public void SetItem_SleepParticle(Transform tParent)
        {
            GameObject Obj = Instantiate(GameObject.Find("itmParticle_Sleep"), tParent.transform.position,
                GameObject.Find("itmParticle_Sleep").transform.localRotation) as GameObject;

            Obj.transform.rotation = tParent.transform.rotation;
            Obj.transform.parent = tParent;

            Destroy(Obj, 12.0f);
        }

        //SetPos
        public void SetPos_ComeHere(Vector3 vPos)
        {
            var ins = Instantiate(GameObject.Find("posComeHere"), vPos, GameObject.Find("posComeHere").transform.localRotation);

            Destroy(ins, 1.5f);
        }

        public void SetPos_Food(Vector3 vPos)
        {
            var ins = Instantiate(GameObject.Find("posFood"), vPos, GameObject.Find("posFood").transform.localRotation);

            Destroy(ins, 6.0f);
        }

        public void SetPos_Heart(Vector3 vPos)
        {
            var ins = Instantiate(GameObject.Find("posHeart"), vPos, GameObject.Find("posHeart").transform.localRotation);

            Destroy(ins, 3.0f);
        }

        //SetSound
        public void SetSoundEffect(GameObject DestObj, SOUNDEFFECT SndEff)
        {
            if (DestObj.GetComponent<AudioSource>() == null)
            {
                DestObj.AddComponent<AudioSource>();
            }

            DestObj.GetComponent<AudioSource>().clip = Resources.Load("Sound/" + GetSoundName(SndEff)) as AudioClip;

            if (DestObj.GetComponent<AudioSource>().clip != null)
            {
                DestObj.GetComponent<AudioSource>().loop = false;
                DestObj.GetComponent<AudioSource>().Play();
            }
        }

        string GetSoundName(SOUNDEFFECT SndEff)
        {
            string SoundName = string.Empty;

            switch (SndEff)
            {
                case SOUNDEFFECT.SMILE:
                    SoundName = "smile";
                    break;
                case SOUNDEFFECT.FART:
                    SoundName = "fart";
                    break;
                case SOUNDEFFECT.HAHA:
                    SoundName = "haha";
                    break;
                case SOUNDEFFECT.HERE:
                    SoundName = "here";
                    break;
                case SOUNDEFFECT.CLICK:
                    SoundName = "click";
                    break;
                case SOUNDEFFECT.FOOD:
                    SoundName = "food";
                    break;
                case SOUNDEFFECT.FEAR:
                    SoundName = "fear";
                    break;
                case SOUNDEFFECT.POP:
                    SoundName = "bubblePop";
                    break;
                case SOUNDEFFECT.MOVETO:
                    SoundName = "moveTo";
                    break;
                case SOUNDEFFECT.EAT:
                    SoundName = "eat";
                    break;
            }

            return SoundName;
        }

        private static GameMgr s_data = null;

        public static GameMgr Ins()
        {
            if (s_data == null)
            {
                s_data = FindObjectOfType(typeof(GameMgr)) as GameMgr;
            }
            if (s_data == null)
            {
                GameObject obj = new GameObject("GameMgr");
                s_data = obj.AddComponent(typeof(GameMgr)) as GameMgr;
            }
            return s_data;
        }
    }
}