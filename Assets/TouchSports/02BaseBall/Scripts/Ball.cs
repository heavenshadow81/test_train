using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ML.T_Sports.BaseBall;
using ML.T_Sports.BasketBall;
namespace ML.T_Sports.Common
{
    public class Ball : MonoBehaviour
    {
        public SphereCollider myCollider;
        public Transform mytr;
        public GameObject Glove;
        public bool CollisionCheck;
        public Vector3 TargetPosition;
        public float PowerSpeed;
        public float RotateSpeed;
        public bool tracking;
        public bool Goal;
        public bool BasketBall_Clean;
        void Awake()
        {
            BasketBall_Clean = true;
            Goal = false;
            mytr = this.transform;
            myCollider = this.GetComponent<SphereCollider>();
            CollisionCheck = false;            
            SoundManager.instance.Pitching.EFMRandomPlay();
            StartCoroutine(DestroySelf(3));
        }

        public void SetRandomSpin()
        {
            tracking = true;
            StartCoroutine(RandomSpin());
            // Debug.LogError("ball!");
        }
        IEnumerator RandomSpin()
        {
            while (tracking)
            {
                Quaternion randomspin;
                Vector3 rand = new Vector3(Random.Range(-40f, -90f), Random.Range(-40f, -90f), Random.Range(40f, 90f));
                randomspin = Quaternion.Euler(rand);
                mytr.Rotate(rand * Time.deltaTime * 3);
                yield return new WaitForSeconds(0.01f);
            }
        }

        private void OnTriggerEnter(Collider collider)
        {
            //게임모드는 0이 싱글 / 1이 팀
          //  int gameMode = ContentsManagerBase.Current.GetPropertyValueInt(ContentsPropertyType.GameMode);
            string gameName = ContentsManagerBase.Current.ContentsName;
            Debug.Log(gameName);
            //BaseBallSingle   BasketBallSingle
            if (gameName == "BasketBallSingle")
            {
                if (collider.tag == "Goal")
                {
                    //Debug.LogError(collider.name);
                    Goal = true;
                    BasketBallSingleManager.instance.scores.SetScoreBoxUI(true, BasketBall_Clean);
                    SoundManager.instance.Cheers.EFMRandomPlay();
                    Destroy(this.gameObject,3);
                    CollisionCheck = true;
                }
            }
            else if (gameName == "BasketBallTeam")
            {
                Debug.Log(collider.tag);
                if (collider.tag == "GoalA")
                {
                    Debug.LogError(collider.name);
                    Debug.LogError("GoalA");
                    Goal = true;
                    BasketBallTeamManager.instance.scores[0].SetScoreBoxUI(true, BasketBall_Clean);
                    SoundManager.instance.Cheers.EFMRandomPlay();
                    Destroy(this.gameObject, 3);
                    CollisionCheck = true;
                }
                else if (collider.tag == "GoalB")
                {
                    Debug.LogError(collider.name);
                    Debug.LogError("GoalB");

                    Goal = true;
                    BasketBallTeamManager.instance.scores[1].SetScoreBoxUI(true, BasketBall_Clean);
                    SoundManager.instance.Cheers.EFMRandomPlay();
                    Destroy(this.gameObject, 3);
                    CollisionCheck = true;
                }
            }

            //중복 충돌 체크에 대해 거부하는 스포츠
            if (CollisionCheck)
                return;
            if (gameName == "BaseBallSingle")
            {
                CollisionCheck = true;
                //충돌체크한곳이 스트라이크존 이라면
                if (collider.tag == "PlayerA")
                {
                    myCollider.enabled = false;
                    string stZone = "BonusZone";
                    int idx = BaseBallSingleManager.instance.scores[0].GetActivateZone();
                    idx += 1;
                    stZone += idx.ToString();
                    Debug.Log("collider.transform.name : " + collider.transform.name);
                    Debug.Log("stZone : " + stZone);
                    if (collider.transform.name == stZone)
                    {
                        Debug.Log("GetBonus!!");
                        BaseBallSingleManager.instance.scores[0].SetScoreBoxUI(true, 5);
                    }
                    else
                    {
                        BaseBallSingleManager.instance.scores[0].SetScoreBoxUI(true, 0);
                    }

                    Instantiate(Glove, mytr.position, Quaternion.identity);
                    SoundManager.instance.Strike.EFMRandomPlay();
                    SoundManager.instance.Cheers.EFMRandomPlay();
                    Destroy(this.gameObject);
                }
                else if (collider.tag == "PlayerB")
                {
                    myCollider.enabled = false;
                    string stZone = "BonusZone";
                    int idx = BaseBallSingleManager.instance.scores[1].GetActivateZone();
                    idx += 1;
                    stZone += idx.ToString();
                    Debug.Log("collider.transform.name : " + collider.transform.name);
                    Debug.Log("stZone : " + stZone);
                    if (collider.transform.name == stZone)
                    {
                        Debug.Log("GetBonus!!");
                        BaseBallSingleManager.instance.scores[1].SetScoreBoxUI(true, 5);
                    }
                    else
                    {
                        BaseBallSingleManager.instance.scores[1].SetScoreBoxUI(true, 0);
                    }
                    Instantiate(Glove, mytr.position, Quaternion.identity);                    
                    SoundManager.instance.Strike.EFMRandomPlay();
                    SoundManager.instance.Cheers.EFMRandomPlay();
                    Destroy(this.gameObject);
                }
                else if (collider.tag == "PlayerC")
                {
                    myCollider.enabled = false;
                    string stZone = "BonusZone";
                    int idx = BaseBallSingleManager.instance.scores[2].GetActivateZone();
                    idx += 1;
                    stZone += idx.ToString();
                    Debug.Log("collider.transform.name : " + collider.transform.name);
                    Debug.Log("stZone : " + stZone);
                    if (collider.transform.name == stZone)
                    {
                        Debug.Log("GetBonus!!");
                        BaseBallSingleManager.instance.scores[2].SetScoreBoxUI(true, 5);
                    }
                    else
                    {
                        BaseBallSingleManager.instance.scores[2].SetScoreBoxUI(true, 0);
                    }
                    Instantiate(Glove, mytr.position, Quaternion.identity);
                    SoundManager.instance.Strike.EFMRandomPlay();
                    SoundManager.instance.Cheers.EFMRandomPlay();
                    Destroy(this.gameObject);
                }
                //충돌체크한곳이 볼 지점이라면
                else if (collider.tag == "PlayerA_Ball")
                {
                    BaseBallSingleManager.instance.scores[0].SetScoreBoxUI(false,0);
                    SoundManager.instance.Bhoo.EFMRandomPlay();
                }
                else if (collider.tag == "PlayerB_Ball")
                {
                    BaseBallSingleManager.instance.scores[1].SetScoreBoxUI(false, 0);
                    SoundManager.instance.Bhoo.EFMRandomPlay();
                }
                else if (collider.tag == "PlayerC_Ball")
                {
                    BaseBallSingleManager.instance.scores[2].SetScoreBoxUI(false, 0);
                    SoundManager.instance.Bhoo.EFMRandomPlay();
                }
                tracking = false;
            }
            else if (gameName == "BaseBallTeam")
            {
                CollisionCheck = true;
                //야구 팀모드
                if (collider.tag == "PlayerA")
                {
                    string stZone = "BonusZone";
                    int idx = BaseBallTeamManager.instance.scores.GetActivateZone();
                    idx += 1;
                    stZone += idx.ToString();
                    Debug.Log("collider.transform.name : " + collider.transform.name);
                    Debug.Log("stZone : " + stZone);
                    if (collider.transform.name == stZone)
                    {
                        Debug.Log("GetBonus!!");
                        BaseBallTeamManager.instance.scores.SetScoreBoxUI(true, 10);
                    }
                    else
                    {
                        BaseBallTeamManager.instance.scores.SetScoreBoxUI(true, 0);
                    }

                    Instantiate(Glove, mytr.position, Quaternion.identity);
                    myCollider.enabled = false;
                    SoundManager.instance.Strike.EFMRandomPlay();
                    SoundManager.instance.Cheers.EFMRandomPlay();
                    Destroy(this.gameObject);
                }
                else
                    BaseBallTeamManager.instance.scores.SetScoreBoxUI(false, 0);
            }                     
        }
        private void OnCollisionEnter(Collision collision)
        {
            string gameName = ContentsManagerBase.Current.ContentsName;
            Debug.Log(gameName);
            if (gameName == "BasketBallSingle")
            {
                if (Goal)
                    return;
                if (collision.gameObject.tag == "BackBoard")
                    BasketBall_Clean = false;
                if (collision.gameObject.tag == "NoGoal_Area")
                {                    
                    CollisionCheck = true;
                    BasketBallSingleManager.instance.scores.SetScoreBoxUI(false, true);
                    Destroy(this.gameObject, 2);
                }
            }
            else if (gameName == "BasketBallTeam")
            {
                if (Goal)
                    return;
                if(collision.gameObject.tag == "BackBoard")
                    BasketBall_Clean = false;
                if (collision.gameObject.tag == "NoGoal_AreaA")
                {
                    CollisionCheck = true;
                    BasketBallTeamManager.instance.scores[0].SetScoreBoxUI(false, BasketBall_Clean);
                    Destroy(this.gameObject, 2);
                }
                else if (collision.gameObject.tag == "NoGoal_AreaB")
                {
                    CollisionCheck = true;
                    BasketBallTeamManager.instance.scores[1].SetScoreBoxUI(false, BasketBall_Clean);
                    Destroy(this.gameObject, 2);
                }
            }

            if (CollisionCheck)
                return;
            if (gameName == "BaseBallSingle")
            {
                CollisionCheck = true;
                if (collision.gameObject.tag == "PlayerA_Ball")
                {
                    BaseBallSingleManager.instance.scores[0].SetScoreBoxUI(false, 0);
                }
                else if (collision.gameObject.tag == "PlayerB_Ball")
                {
                    BaseBallSingleManager.instance.scores[1].SetScoreBoxUI(false, 0);
                }
                else if (collision.gameObject.tag == "PlayerC_Ball")
                {
                    BaseBallSingleManager.instance.scores[2].SetScoreBoxUI(false, 0);
                }
            }
            if (gameName == "BaseBallTeam")
            {
                CollisionCheck = true;
                BaseBallTeamManager.instance.scores.SetScoreBoxUI(false, 0);
            }

        }
        IEnumerator DestroySelf(float sec)
        {
            yield return new WaitForSeconds(sec);
            Destroy(this.gameObject);
        }
    }
}
