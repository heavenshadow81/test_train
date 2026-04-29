using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ML.T_Sports.Common;

namespace ML.T_Sports.ShotPut
{
    public class ShotPutBall : MonoBehaviour
    {
        public Rigidbody MyRigid;
        public Transform MyTr;
        public TextMesh DistanceText;

        public GameObject FlagObj;
        public Transform Flag;
        public bool OutCheck;

        Vector3 vStartPos;
        Vector3 vEndPos;
        Vector3 vPos;


        float fb_x;
        float fb_y;
        float fb_z;

        float g;
        float EndTime;
        float MaxHeight;
        float Height;
        float EndHeight;
        float _Time;
        float MaxTime;
        public float dist;
        public int playerIdx;
        public bool go;
        public bool Scoring;
        public ParticleSystem drop;
        public Transform DropTr;
        private void Awake()
        {
            FlagObj = Flag.gameObject;
            FlagObj.SetActive(false);
            go = false;
            MyTr = this.transform;
            MyRigid = MyTr.GetComponent<Rigidbody>();
            StartCoroutine(SpinBall());
            vStartPos = MyTr.position;
            Scoring = true;
        }

        private void Update()
        {
            if (go)
            {
                _Time += Time.deltaTime;
                vPos.x = vStartPos.x + fb_x * _Time;
                vPos.y = vStartPos.y + (fb_y * _Time) - (0.5f * g * _Time * _Time);
                vPos.z = vStartPos.z + fb_z * _Time;
                MyTr.position = vPos;

                if (MyTr.position.y <= 0.2f)
                {
                    go = false;
                    MyRigid.useGravity = false;
                    MyRigid.isKinematic = true;
                    spin = false;
                    StartCoroutine(FlagOn());
                    if (Scoring)
                    {
                        Scoring = false;
                        if (OutCheck)
                            ShotPutManager.instance.SetScore(playerIdx, dist);
                        else
                            ShotPutManager.instance.SetOut(playerIdx);
                        Common.SoundManager.instance.EFMs[2].EFMRandomPlay();
                        DropTr.rotation = Quaternion.Euler(new Vector3(-90,0,0));
                        drop.Play();
                    }


                    Vector3 tmp = MyTr.position;
                    tmp.y = 0.2f;
                    MyTr.position = tmp;
                }
            }            
        }
        public void SetBallShot(Vector3 target, float maxheight, float maxtime, float Distance, int idx, bool outcheck)
        {
            OutCheck = outcheck;
            playerIdx = idx;
            dist = Distance;
            DistanceText.text = Distance.ToString("N1")+"m";
            vEndPos = target;
            MaxHeight = maxheight;
            MaxTime = maxtime;
            ShotBall();
        }
        public void ShotBall()
        {
            EndHeight = vEndPos.y - vStartPos.y;
            Height = MaxHeight - vStartPos.y;
            g = 2 * Height / (MaxTime * MaxTime);

            fb_y = Mathf.Sqrt(2 * g * Height);
            float a = g;
            float b = -2 * fb_y;
            float c = 2 * EndHeight;

            EndTime = (-b + Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);
            fb_x = -(vStartPos.x - vEndPos.x) / EndTime;
            fb_z = -(vStartPos.z - vEndPos.z) / EndTime;
            go = true;
        }
        public void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.name == "Stage")
            {
                go = false;
                MyRigid.useGravity = false;
                MyRigid.isKinematic = true;
                StartCoroutine(FlagOn());
                spin = false;
                if (Scoring)
                {
                    Scoring = false;
                    if (OutCheck)
                        ShotPutManager.instance.SetScore(playerIdx, dist);
                    else
                        ShotPutManager.instance.SetOut(playerIdx);
                    Common.SoundManager.instance.EFMs[2].EFMRandomPlay();
                    DropTr.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
                    drop.Play();
                }
            }
        }

        IEnumerator FlagOn()
        {
            Vector3 flagpos = MyTr.position;
            flagpos.y += 4;
            Flag.position = flagpos;

            FlagObj.SetActive(true);
            Vector3 target = MyTr.position;
            target.y += 0.2f;
            while (true)
            {
                float dist = Vector3.Distance(Flag.position, target);
                if (dist > 0.1f)
                {
                    Flag.position = Vector3.Lerp(Flag.position, target, Time.deltaTime * 8);
                    Flag.rotation = Quaternion.Euler(new Vector3(0, 27.78f, 0));
                }
                else
                {
                    break;
                }
                yield return new WaitForSeconds(0.01f);                
            }
            Flag.position = target;
            Flag.rotation = Quaternion.Euler(new Vector3(0, 27.78f, 0));
        }
        public bool spin;
        IEnumerator SpinBall()
        {
            spin = true;
            Vector3 RandomRot = new Vector3(Random.Range(-120f, 120f), Random.Range(-120f, 120f), Random.Range(-120f, 120f));
            while (spin)
            {
                MyTr.Rotate(RandomRot* Time.deltaTime*10);
                yield return new WaitForSeconds(0.01f);
            }
        }
    }
}

