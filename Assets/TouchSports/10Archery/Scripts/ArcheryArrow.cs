using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ML.T_Sports.Common;
using DG.Tweening;

namespace ML.T_Sports.Archery
{
    public class ArcheryArrow : MonoBehaviour
    {
        public Transform Mytr;
        public Transform ArrowTr;
        public Transform CamPos;
        public Vector3 TargetPos;
        public float Speed;
        public float TargetSpeed;
        public BoxCollider MyCollider;
        public ArcheryCameraMoving mov;
        public bool Go;
        public bool spin;
        public float scoreDist;
        public bool ScorePlus;
        public int TeamNumber;
        public ParticleSystem arrowparticle;
        
             
        private void Awake()
        {
            scoreDist = 0;
            Mytr = this.transform;
            Go = false;
            spin = true;
            ScorePlus = false;
        }
        private void Update()
        {
            if (Go)
            {
                float dist = Vector3.Distance(Mytr.position, TargetPos);
                //Mytr.Translate(Mytr.forward * Time.deltaTime * Speed);
                Mytr.position = Vector3.MoveTowards(Mytr.position, TargetPos, Speed);
                if (TargetSpeed >= Speed)
                {
                    Speed += Time.deltaTime * 2;
                }
                if (dist <= 0.1f)
                {
                    Go = false;
                    spin = false;
                    scoreDist = Vector3.Distance(Mytr.position, cent.position);
                    //결과
                    if (!ScorePlus)
                        SetScore();
                }
            }
        }
        public void SetScore()
        {
            GameObject target = GameObject.FindGameObjectWithTag("target");
            gameObject.transform.parent =  target.transform;

            arrowparticle.Stop();
            mov.ResetCamera();
            MyCollider.enabled = false;
            Common.SoundManager.instance.Strike.EFMRandomPlay();
            ScorePlus = true;
            int score = 0;
            if (scoreDist <= 0.43f)
            {
                score = 10;
                Common.SoundManager.instance.Cheers.EFMRandomPlay();
            }
            else if (scoreDist > 0.43f && scoreDist <= 0.78f)
            {
                score = 9;
                Common.SoundManager.instance.Cheers.EFMRandomPlay();
            }
            else if (scoreDist > 0.78f && scoreDist <= 1.16f)
            {
                score = 8;
                Common.SoundManager.instance.Cheers.EFMRandomPlay();
            }
            else if (scoreDist > 1.16f && scoreDist <= 1.54f)
            {
                score = 7;
                Common.SoundManager.instance.Cheers.EFMRandomPlay();
            }
            else if (scoreDist > 1.54f && scoreDist <= 1.9f)
            {
                score = 6;
                Common.SoundManager.instance.Bhoo.EFMRandomPlay();
            }
            else if (scoreDist > 1.9f && scoreDist <= 2.3f)
            {
                score = 5;
                Common.SoundManager.instance.Bhoo.EFMRandomPlay();
            }
            else if (scoreDist > 2.3f && scoreDist <= 2.65f)
            {
                score = 4;
                Common.SoundManager.instance.Bhoo.EFMRandomPlay();
            }
            else if (scoreDist > 2.65f && scoreDist <= 3.03f)
            {
                score = 3;
                Common.SoundManager.instance.Bhoo.EFMRandomPlay();
            }
            else if (scoreDist > 3.03f && scoreDist <= 3.41f)
            {
                score = 2;
                Common.SoundManager.instance.Bhoo.EFMRandomPlay();
            }
            else if (scoreDist > 3.41f && scoreDist <= 3.76f)
            {
                score = 1;
                Common.SoundManager.instance.Bhoo.EFMRandomPlay();
            }
            else if (scoreDist > 3.76f)
            {
                gameObject.transform.DOScale(0, 1);
                score = 0;
                Common.SoundManager.instance.Bhoo.EFMRandomPlay();
                Vector3 plusPos = Mytr.position;
                plusPos.z += 0.3f;
                Mytr.position = plusPos;
            }
            ArcheryTeamManager.instance.AddScore(TeamNumber, score);
        }


        public Transform cent;
        public void SetTarget(Vector3 target, float speed, Transform center, int TeamNumb, ArcheryCameraMoving moving)
        {
            mov = moving;
            TeamNumber = TeamNumb;
            cent = center;
            TargetPos = target;
            Speed = speed;
            TargetSpeed = speed * 2;
            Mytr.LookAt(TargetPos);
            Go = true;
            StartCoroutine(SpinArrow());
        }
        IEnumerator SpinArrow()
        {
            while (spin)
            {
                ArrowTr.Rotate(Vector3.left * Speed * 10);
                yield return new WaitForSeconds(0.01f);
            }
        }
    }
}
