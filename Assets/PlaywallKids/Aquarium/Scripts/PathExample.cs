using System.Collections.Generic;
using UnityEngine;

namespace ML.PlaywallKids.Aquarium
{
    public enum STATUS
    {
        WAIT = 0,
        SWIM,
        FIND,
        COME,
        MOVE,
        PLAYCOME,
        HELLO,
        FEEDING,
        SLEEP,
        STUNT,
        TICKLE,
    }

    public enum COMMAND
    {
        NO_CMD = 0,
        RIGHTHAND,
        LEFTHAND,
        HEART
    }
    
    public enum FISH_SIZE
    {
        SMALL = 0,
        MEDIUM,
        BIG,
        GIANT
    }

    /// <summary>
    /// 아쿠아리움 물고기 오브젝트 움직임을 정의한 클래스.
    /// </summary>
    public class PathExample : MonoBehaviour
    {
        public float aniSpeed = 1;
        public Animation ani;
        public float startTime = 0;
        public bool draw = false;
        public Color pathColor = Color.white;
        public float time = 30;
        public float comeAniDelay;
        public Transform[] path;

        public STATUS status { get; private set; }

        private Vector3 inputPos;
        private Vector3 curPos;
        private Vector3 lookPos;
        private Vector3 positionMount = new Vector3(1.0f, 1.0f, 1.0f);
        private Vector3 scaleMount = new Vector3(0.1f, 0.1f, 0.1f);
        private Vector3 rotateMount = new Vector3(3.0f, 3.0f, 3.0f);

        private Vector3[] stuntPath1 = new Vector3[5] { new Vector3(0, 0, 0), new Vector3(0, 20, 20), new Vector3(0, 40, 0), new Vector3(0, 20, -20), new Vector3(0, 0, 0) };
        private Vector3[] stuntPath2 = new Vector3[5] { new Vector3(0, 0, 0), new Vector3(-50, -10, 50), new Vector3(0, -20, 100), new Vector3(50, -10, 50), new Vector3(0, 0, 0) };
        private Vector3[] stuntPath3 = new Vector3[10] { new Vector3(0, 0, 0), new Vector3(0, -45, 0), new Vector3(-100, -45, 0), new Vector3(-150, 0, 0), new Vector3(-100, 45, 0),
                                                    new Vector3(0, 45, 0), new Vector3(100, 45, 0), new Vector3(150, 0, 0), new Vector3(100, -45, 0), new Vector3(0, -45, 0)};
        
        private string inputAction = "";
        
        public bool isReset = true;
        private COMMAND preCmd = COMMAND.NO_CMD;
        private FISH_SIZE fishSize = FISH_SIZE.SMALL;
        private static Dictionary<FISH_SIZE, List<PathExample>> listDict = new Dictionary<FISH_SIZE, List<PathExample>>();

        private void Awake()
        {
            status = STATUS.SWIM;

            // Distinguishes fish size via tag.
            switch (gameObject.tag)
            {
                case "SmallFish":
                    fishSize = FISH_SIZE.SMALL;
                    break;
                case "MediumFish":
                    fishSize = FISH_SIZE.MEDIUM;
                    break;
                case "BigFish":
                    fishSize = FISH_SIZE.BIG;
                    break;
                case "GiantFish":
                    fishSize = FISH_SIZE.GIANT;
                    break;
                case "AToolFishes":
                    fishSize = FISH_SIZE.SMALL;
                    break;
                default:
                    fishSize = FISH_SIZE.MEDIUM;
                    break;
            }
        }
        
        void Start()
        {
            if (ani != null)
                ani["Swim"].speed = aniSpeed;
            if (path != null && path.Length > 0)
                transform.position = path[0].position;
            Invoke("tween", startTime);
        }

        void OnDrawGizmos()
        {
            if (draw)
                iTween.DrawPath(path, pathColor);
        }

        void OnEnable()
        {
            List<PathExample> list = _GetList(fishSize);
            list.Add(this);
        }

        void OnDisable()
        {
            List<PathExample> list = _GetList(fishSize);
            list.Remove(this);
        }

        void Update()
        {
            if (transform.localScale.sqrMagnitude >= 0.99f)
                transform.localScale = Vector3.Max(Vector3.one, transform.localScale - Time.deltaTime * 0.01f * Vector3.one);
        }

        public static List<PathExample> GetList(FISH_SIZE fishSize)
        {
            return new List<PathExample>(_GetList(fishSize));
        }

        private static List<PathExample> _GetList(FISH_SIZE fishSize)
        {
            List<PathExample> list = null;
            if (!listDict.TryGetValue(fishSize, out list))
            {
                list = new List<PathExample>();
                listDict[fishSize] = list;
            }
            return list;
        }

        void tween()
        {
            //Debug.Log("start swim");
            if (path != null && path.Length > 0)
                iTween.MoveTo(gameObject, iTween.Hash("path", path, "time", time, "orienttopath", true, "easetype", "easeInOutSine", "oncomplete", "Reset"));
        }

        Vector3[] makePath(int startPoint, int endPoint)
        {
            if (endPoint <= startPoint)
            {
                Debug.Log("wrong point");
                return null;
            }

            Vector3[] neoPath = new Vector3[endPoint - startPoint + 1];

            for (int i = startPoint; i <= endPoint; i++)
            {
                //Debug.Log("neop = " + (i - startPoint) + " oldp = " + i);
                neoPath[i - startPoint] = path[i].position;
            }
            return neoPath;
        }

        int FindClosePoint()
        {
            int size = path != null ? path.Length : 0;
            float dist = path != null ? iTween.PathLength(path) : 0;
            float compare = 0;
            int rt = 0;

            for (int i = 0; i < size; i++)
            {
                compare = Vector3.Distance(curPos, path[i].position);
                if (dist > compare)
                {
                    dist = compare;
                    rt = i;
                }
            }
            //Debug.Log("rt = " + rt);
            return rt;
        }

        public void FoundIt(string cmd, Vector3 pos, string size)
        {
            if (status != STATUS.SWIM)
                return;

            status = STATUS.WAIT;

            switch (size)
            {
                case "SmallFish":
                    fishSize = FISH_SIZE.SMALL;
                    break;
                case "MediumFish":
                    fishSize = FISH_SIZE.MEDIUM;
                    break;
                case "BigFish":
                    fishSize = FISH_SIZE.BIG;
                    break;
                case "GiantFish":
                    fishSize = FISH_SIZE.GIANT;
                    break;
            }

            //Debug.Log(" cmd=" + cmd + " status=" + status + " fishsize=" + fishSize);
            //ksy debug
            //preCmd = COMMAND.RIGHTHAND;
            //preCmd = COMMAND.LEFTHAND;
            //preCmd = COMMAND.HEART;

            float findDelay = 0.5f + Random.value;

            gameObject.BroadcastMessage("FadeColor", findDelay, SendMessageOptions.DontRequireReceiver);

            //Select Action by FishSize
            switch (cmd)
            {
                case "RightHand":
                    if (preCmd == COMMAND.NO_CMD)
                    {
                        if (fishSize == FISH_SIZE.GIANT)
                        {
                            preCmd = COMMAND.RIGHTHAND;
                            Invoke("ReadyCommand", 3.0f);
                            return;
                        }
                    }
                    else if (preCmd == COMMAND.RIGHTHAND)
                    {
                        if (fishSize == FISH_SIZE.BIG || fishSize == FISH_SIZE.GIANT)
                        {
                            preCmd = COMMAND.NO_CMD;
                            Invoke("ReadyCommand", 3.0f);
                            return;
                        }
                    }
                    else if (preCmd == COMMAND.LEFTHAND)
                    {
                    }
                    else if (preCmd == COMMAND.HEART)
                    {
                        if (fishSize == FISH_SIZE.GIANT)
                        {
                            preCmd = COMMAND.NO_CMD;
                            Invoke("ReadyCommand", 3.0f);
                            return;
                        }
                    }
                    break;
                case "LeftHand":
                    if (preCmd == COMMAND.NO_CMD)
                    {
                        if (fishSize == FISH_SIZE.BIG || fishSize == FISH_SIZE.GIANT)
                        {
                            preCmd = COMMAND.LEFTHAND;
                            Invoke("ReadyCommand", 3.0f);
                            return;
                        }
                    }
                    else if (preCmd == COMMAND.RIGHTHAND)
                    {
                        if (fishSize == FISH_SIZE.BIG || fishSize == FISH_SIZE.GIANT)
                        {
                            preCmd = COMMAND.NO_CMD;
                            Invoke("ReadyCommand", 3.0f);
                            return;
                        }
                    }
                    else if (preCmd == COMMAND.LEFTHAND)
                    {
                    }
                    else if (preCmd == COMMAND.HEART)
                    {
                        if (fishSize == FISH_SIZE.GIANT)
                        {
                            preCmd = COMMAND.NO_CMD;
                            Invoke("ReadyCommand", 3.0f);
                            return;
                        }
                    }
                    break;
                case "Heart":
                    if (preCmd == COMMAND.NO_CMD)
                    {
                        //if (fishSize == FISH_SIZE.GIANT || fishSize == FISH_SIZE.BIG){
                        if (fishSize == FISH_SIZE.BIG)
                        {
                            preCmd = COMMAND.HEART;
                            if (ani != null)
                                ani.CrossFade("Hello");
                            GameMgr.Ins().SetItem_Hello(transform, (int)fishSize);
                            Invoke("ReadyCommand", 3.0f);
                            return;
                        }
                        //only whale
                        else if (fishSize == FISH_SIZE.GIANT)
                        {
                            preCmd = COMMAND.NO_CMD;
                            if (ani != null)
                                ani.CrossFade("Hello");
                            GameMgr.Ins().SetItem_Hello(transform, (int)fishSize);
                            Invoke("ReadyCommand", 10.0f);
                            return;
                        }
                    }
                    else if (preCmd == COMMAND.RIGHTHAND)
                    {
                        if (fishSize == FISH_SIZE.GIANT)
                        {
                            preCmd = COMMAND.NO_CMD;
                            Invoke("ReadyCommand", 3.0f);
                            return;
                        }
                    }
                    else if (preCmd == COMMAND.LEFTHAND)
                    {
                        if (fishSize == FISH_SIZE.GIANT || fishSize == FISH_SIZE.BIG)
                        {
                            preCmd = COMMAND.NO_CMD;
                            Invoke("ReadyCommand", 3.0f);
                            return;
                        }
                    }
                    else if (preCmd == COMMAND.HEART)
                    {
                        if (fishSize == FISH_SIZE.GIANT || fishSize == FISH_SIZE.BIG || fishSize == FISH_SIZE.MEDIUM)
                        {
                            preCmd = COMMAND.NO_CMD;
                            Invoke("ReadyCommand", 3.0f);
                            return;
                        }
                    }
                    break;
                case "TwoHand":  //tickle
                    if (fishSize == FISH_SIZE.GIANT || fishSize == FISH_SIZE.BIG || fishSize == FISH_SIZE.MEDIUM)
                    {
                        preCmd = COMMAND.NO_CMD;
                        Invoke("ReadyCommand", 3.0f);
                        return;
                    }
                    break;
            }


            inputPos = pos;

            if (inputPos.y < 100)
                inputPos.y = 100;

            iTween.ShakePosition(gameObject, positionMount, findDelay);

            //Debug.Log(cmd + pos);

            switch (cmd)
            {
                case "RightHand":
                    if (preCmd == COMMAND.NO_CMD)
                    {
                        preCmd = COMMAND.RIGHTHAND;

                        //Come here
                        inputPos.z = 0;
                        inputAction = "PlayCome";
                        GameMgr.Ins().SetPos_ComeHere(transform.position);
                        GameMgr.Ins().SetSoundEffect(gameObject, SOUNDEFFECT.CLICK);
                        Invoke("Come", findDelay);
                    }
                    else if (preCmd == COMMAND.RIGHTHAND)
                    {
                        preCmd = COMMAND.NO_CMD;

                        //Big Circle Z
                        inputPos.x = -5;
                        inputPos.y = 139;
                        inputPos.z = 0;
                        inputAction = "BigCircleZ";
                        GameMgr.Ins().SetSoundEffect(gameObject, SOUNDEFFECT.CLICK);
                        GameMgr.Ins().SetPos_ComeHere(transform.position);
                        Invoke("GoPosition", findDelay);
                    }
                    else if (preCmd == COMMAND.LEFTHAND)
                    {
                        preCmd = COMMAND.NO_CMD;

                        //Fart
                        GameMgr.Ins().SetPos_ComeHere(transform.position);
                        GameMgr.Ins().SetItem_FartParticle(transform);
                        GameMgr.Ins().SetSoundEffect(gameObject, SOUNDEFFECT.FART);
                    }
                    else if (preCmd == COMMAND.HEART)
                    {
                        preCmd = COMMAND.NO_CMD;

                        //Move Right Conner
                        inputPos.Set(-90, 100, 120);
                        GameMgr.Ins().SetPos_ComeHere(transform.position);
                        GameMgr.Ins().SetSoundEffect(gameObject, SOUNDEFFECT.CLICK);
                        Invoke("RapidMove", findDelay);
                    }
                    break;
                case "LeftHand":
                    if (preCmd == COMMAND.NO_CMD)
                    {
                        preCmd = COMMAND.LEFTHAND;

                        //Feeding
                        inputPos.z = 50;
                        inputAction = "Feeding";
                        GameMgr.Ins().SetPos_Food(inputPos);
                        GameMgr.Ins().SetSoundEffect(gameObject, SOUNDEFFECT.FOOD);
                        Invoke("GoPosition", findDelay);
                    }
                    else if (preCmd == COMMAND.RIGHTHAND)
                    {
                        preCmd = COMMAND.NO_CMD;

                        //laugh
                        GameMgr.Ins().SetPos_ComeHere(transform.position);
                        GameMgr.Ins().SetSoundEffect(gameObject, SOUNDEFFECT.HAHA);
                    }
                    else if (preCmd == COMMAND.LEFTHAND)
                    {
                        preCmd = COMMAND.NO_CMD;

                        //Sleep
                        GameMgr.Ins().SetPos_ComeHere(transform.position);
                        GameMgr.Ins().SetSoundEffect(gameObject, SOUNDEFFECT.CLICK);
                        GameMgr.Ins().SetItem_SleepParticle(transform);
                        Invoke("Sleep", findDelay);
                    }
                    else if (preCmd == COMMAND.HEART)
                    {
                        preCmd = COMMAND.NO_CMD;

                        //Move Left Conner
                        inputPos.Set(90, 100, 120);
                        GameMgr.Ins().SetPos_ComeHere(transform.position);
                        GameMgr.Ins().SetSoundEffect(gameObject, SOUNDEFFECT.CLICK);
                        Invoke("RapidMove", findDelay);
                    }
                    break;
                case "Heart":
                    if (preCmd == COMMAND.NO_CMD)
                    {
                        preCmd = COMMAND.HEART;

                        //love and hi
                        inputPos.z = 0;
                        inputAction = "Hello";
                        GameMgr.Ins().SetPos_Heart(inputPos);
                        GameMgr.Ins().SetSoundEffect(gameObject, SOUNDEFFECT.POP);
                        Invoke("Come", findDelay);
                    }
                    else if (preCmd == COMMAND.RIGHTHAND)
                    {
                        preCmd = COMMAND.NO_CMD;

                        //Come Closer
                        inputPos.z = 230;
                        GameMgr.Ins().SetPos_ComeHere(transform.position);
                        GameMgr.Ins().SetSoundEffect(gameObject, SOUNDEFFECT.CLICK);
                        Invoke("ComeCloser", findDelay);
                    }
                    else if (preCmd == COMMAND.LEFTHAND)
                    {
                        preCmd = COMMAND.NO_CMD;

                        //circle axis y
                        inputPos.z = 0;
                        inputAction = "CircleY";
                        GameMgr.Ins().SetSoundEffect(gameObject, SOUNDEFFECT.CLICK);
                        GameMgr.Ins().SetPos_ComeHere(transform.position);
                        Invoke("GoPosition", findDelay);
                    }
                    else if (preCmd == COMMAND.HEART)
                    {
                        preCmd = COMMAND.NO_CMD;

                        //jump
                        inputPos.z = 0;
                        inputAction = "Jump";
                        GameMgr.Ins().SetSoundEffect(gameObject, SOUNDEFFECT.CLICK);
                        GameMgr.Ins().SetPos_ComeHere(transform.position);
                        Invoke("GoPosition", findDelay);
                    }
                    break;
                case "TwoHand":  //tickle
                    preCmd = COMMAND.NO_CMD;

                    inputPos.z = 0;
                    inputAction = "Tickle";
                    GameMgr.Ins().SetPos_ComeHere(transform.position);
                    GameMgr.Ins().SetSoundEffect(gameObject, SOUNDEFFECT.HERE);
                    Invoke("GoPosition", findDelay);
                    break;
            }
        }

        public void RapidMove()
        {
            status = STATUS.MOVE;

            GameMgr.Ins().SetItem_MoveParticle(transform, 2);
            GameMgr.Ins().SetSoundEffect(gameObject, SOUNDEFFECT.MOVETO);

            Vector3 tmpPos = inputPos + (Random.insideUnitSphere * 22);

            curPos = transform.position;

            iTween.MoveTo(gameObject, iTween.Hash("position", tmpPos, "time", 2, "easetype", "easeInOutSine", "oncomplete", "Back"));
            iTween.LookTo(gameObject, iTween.Hash("looktarget", tmpPos, "time", 0.3, "easetype", "easeInOutSine"));
            
            Boid boid = GetComponent<Boid>();
            if (boid != null) boid.enabled = false;
        }

        public void GoPosition()
        {
            status = STATUS.MOVE;

            Vector3 tmpPos = inputPos;
            if (name.Contains("Dolphin"))
                tmpPos += (Vector3)(Random.insideUnitCircle * 26.0f);
            else
                tmpPos += (Vector3)(Random.insideUnitCircle * 22.0f);
            lookPos = inputPos;

            //Debug.Log("goPos=" + tmpPos);
            curPos = transform.position;

            iTween.MoveTo(gameObject, iTween.Hash("position", tmpPos, "time", 3, "easetype", "easeInOutSine", "oncomplete", inputAction));
            iTween.LookTo(gameObject, iTween.Hash("looktarget", tmpPos, "time", 0.5, "easetype", "easeInOutSine"));

            Boid boid = GetComponent<Boid>();
            if (boid != null) boid.enabled = false;
        }

        public void Feeding()
        {
            status = STATUS.FEEDING;

            curPos = transform.position;
            //Debug.Log("curPos=" + curPos);
            //Debug.Log("look target=" + lookPos);

            iTween.LookTo(gameObject, iTween.Hash("looktarget", lookPos, "time", 0.5, "easetype", "easeInOutSine"));

            if (ani != null)
                ani.CrossFade("Swim");

            Invoke("Back", 3.0f);
        }

        public void Jump()
        {
            status = STATUS.STUNT;

            //in GameMgr, Turn particle eff delay.
            GameMgr.Ins().SetItem_MoveParticle(transform, 1.5f);
            GameMgr.Ins().SetSoundEffect(gameObject, SOUNDEFFECT.MOVETO);

            Vector3[] jumpPath = new Vector3[5];

            for (int i = 0; i < 5; i++)
            {
                jumpPath[i] = stuntPath1[i] + transform.position;
                //Debug.Log("path point[" + i + "]  " + stuntPath[i]);
                //Debug.Log("exer point[" + i + "]  " + stuntPath1[i]);
            }

            iTween.MoveTo(gameObject, iTween.Hash("path", jumpPath, "time", 1.5, "orienttopath", true, "easetype", "easeInOutSine", "oncomplete", "Back"));

            Boid boid = GetComponent<Boid>();
            if (boid != null) boid.enabled = false;
        }

        public void CircleY()
        {
            status = STATUS.STUNT;

            //in GameMgr, Turn particle eff delay.
            GameMgr.Ins().SetItem_MoveParticle(transform, 4);
            GameMgr.Ins().SetSoundEffect(gameObject, SOUNDEFFECT.MOVETO);

            Vector3[] jumpPath = new Vector3[5];

            for (int i = 0; i < 5; i++)
            {
                jumpPath[i] = stuntPath2[i] + transform.position;
            }

            iTween.MoveTo(gameObject, iTween.Hash("path", jumpPath, "time", 4, "orienttopath", true, "easetype", "easeInOutSine", "oncomplete", "Back"));

            Boid boid = GetComponent<Boid>();
            if (boid != null) boid.enabled = false;
        }

        public void BigCircleZ()
        {
            status = STATUS.STUNT;

            //in GameMgr, Turn particle eff delay.
            float time = 8 + (Random.value * 8);
            GameMgr.Ins().SetItem_MoveParticle(transform, time);
            GameMgr.Ins().SetSoundEffect(gameObject, SOUNDEFFECT.MOVETO);

            Vector3[] jumpPath = new Vector3[10];

            for (int i = 0; i < 10; i++)
            {
                jumpPath[i] = stuntPath3[i] + transform.position;
            }

            iTween.MoveTo(gameObject, iTween.Hash("path", jumpPath, "time", time, "orienttopath", true, "easetype", "easeInOutSine", "oncomplete", "Back"));

            Boid boid = GetComponent<Boid>();
            if (boid != null) boid.enabled = false;
        }

        public void Come()
        {
            status = STATUS.COME;

            Vector3 tmpPos = inputPos + (Random.insideUnitSphere * 22);

            curPos = transform.position;
            //Debug.Log("curPos = " + curPos);

            iTween.MoveTo(gameObject, iTween.Hash("position", tmpPos, "time", 3, "easetype", "easeInOutSine", "oncomplete", inputAction));
            iTween.LookTo(gameObject, iTween.Hash("looktarget", tmpPos, "time", 0.5, "easetype", "easeInOutSine"));

            Boid boid = GetComponent<Boid>();
            if (boid != null) boid.enabled = false;
        }

        public void ComeCloser()
        {
            status = STATUS.COME;

            curPos = transform.position;
            //Debug.Log("curPos = " + curPos);
            Vector3 tmpPos = inputPos + (Random.insideUnitSphere * 22);

            iTween.MoveTo(gameObject, iTween.Hash("position", tmpPos, "time", 3, "easetype", "easeInOutSine", "oncomplete", "Back"));
            iTween.LookTo(gameObject, iTween.Hash("looktarget", tmpPos, "time", 0.5, "easetype", "easeInOutSine"));

            Boid boid = GetComponent<Boid>();
            if (boid != null) boid.enabled = false;
        }

        void Hello()
        {
            status = STATUS.HELLO;
            if (ani != null)
                ani.CrossFade("Hello");

            GameMgr.Ins().SetItem_Hello(transform, (int)fishSize);
            
            AnimationClip clip = ani != null ? ani.GetClip("Hello") : null;
            Invoke("Back", clip != null ? clip.length : 10.0f);
        }

        void PlayCome()
        {
            //Debug.Log("playcome!");
            status = STATUS.PLAYCOME;
            if (ani != null)
                ani.CrossFade("Come");

            //AnimationClip clip = ani.GetClip("Come");
            Invoke("Back", comeAniDelay);
        }

        void Sleep()
        {
            status = STATUS.SLEEP;

            curPos = transform.position;
            //Debug.Log("curPos = " + curPos);
            Vector3 tmpPos = curPos + (Random.insideUnitSphere * 20);
            iTween.MoveTo(gameObject, iTween.Hash("position", tmpPos, "time", 7, "easetype", "easeInOutSine", "oncomplete", "Back"));

            Boid boid = GetComponent<Boid>();
            if (boid != null) boid.enabled = false;
        }

        void Tickle()
        {
            //Debug.Log("Tickle!!!");

            status = STATUS.TICKLE;

            if (ani != null)
                ani.CrossFade("Hello");

            AnimationClip clip = ani != null ? ani.GetClip("Hello") : null;
            Invoke("Back", clip != null ? clip.length : 10.0f);

            //iTween.ShakePosition(gameObject, positionMount, clip.length);
            //iTween.ShakeRotation(gameObject, rotateMount, clip.length);
            iTween.ShakeScale(gameObject, scaleMount, clip != null ? clip.length : 10.0f);

            GameMgr.Ins().SetSoundEffect(gameObject, SOUNDEFFECT.SMILE);
        }

        void Back()
        {
            status = STATUS.WAIT;
            if (ani != null)
                ani.CrossFade("Swim");

            //Debug.Log("back");
            
            // 이전 위치로 돌아가도록 지정
            Boid boid = GetComponent<Boid>();
            if (boid != null)
                boid.enabled = true;
            if (path != null && path.Length > 0)
            {
                iTween.MoveTo(gameObject, iTween.Hash("position", curPos, "time", 8, "easetype", "easeInOutSine", "oncomplete", "RemainMove"));
                iTween.LookTo(gameObject, iTween.Hash("looktarget", curPos, "time", 3, "easetype", "easeInOutSine"));
            }

            // 3초 후 command를 받을 수 있도록 지정
            Invoke("ReadyCommand", 3.0f);
        }

        void RemainMove()
        {
            if (path != null && path.Length > 0)
            {
                int start = FindClosePoint();
                int end = path.Length - 1;
                //Debug.Log("remain Move from " + start + " to " + end);
                Vector3[] neoPath = makePath(start, end);

                iTween.MoveTo(gameObject, iTween.Hash("path", neoPath, "time", 5 + (time * (end - start) / end), "orienttopath", true, "easetype", "easeInOutSine", "oncomplete", "Reset"));
            }
        }

        void ReadyCommand()
        {
            //Debug.Log("Ready!");
            status = STATUS.SWIM;
            if (ani != null)
                ani.CrossFade("Swim");
        }

        public void Reset()
        {
            status = STATUS.SWIM;
            if (isReset)
            {
                if (path != null && path.Length > 0)
                    transform.position = path[0].position;
            }
            Invoke("tween", startTime);
        }

        public void FeedAction(Vector3 pos)
        {
            if (status != STATUS.SWIM)
                return;

            status = STATUS.WAIT;

            inputPos = pos;
            curPos = transform.position;
            
            //Feeding2
            inputAction = "Feeding2";
            GameMgr.Ins().SetPos_Food(inputPos);
            GameMgr.Ins().SetSoundEffect(gameObject, SOUNDEFFECT.FOOD);
            Invoke("GoPosition", 0);

            Boid boid = GetComponent<Boid>();
            if (boid != null) boid.enabled = false;
        }

        public void RandomAction()
        {
            int actionNumber = Random.Range(1, 5);
            switch(actionNumber)
            {
                case 1:
                    Tickle();
                    break;
                case 2:
                    Hello();
                    break;
                case 3:
                    Jump();
                    break;
                case 4:
                    BigCircleZ();
                    break;
            }
        }

        public void Feeding2()
        {
            status = STATUS.FEEDING;
            
            // 물고기 오브젝트 pivot이 바닥에 있으므로 먹이를 제대로 바라보게 하려면
            // 대상보다 조금 더 밑으로 보정해야 함.
            iTween.LookTo(gameObject, iTween.Hash("looktarget", lookPos - transform.up * 5.0f, "time", 0.5, "easetype", "easeInOutSine"));
            if (ani != null)
                ani.CrossFade("Swim");

            Invoke("Feeding2_Scale", 3.0f);
        }

        public void Feeding2_Scale()
        {
            transform.localScale = transform.localScale * 1.2f;
            iTween.PunchScale(gameObject, Vector3.one * 1.2f, 1.0f);
            Invoke("Back", 1.0f);

            GameMgr.Ins().SetSoundEffect(gameObject, SOUNDEFFECT.EAT);
        }
    }
}