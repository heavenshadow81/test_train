using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Fish
{
    private bool active;
    public GameObject fishObj;  // 물고기 오브젝트
    public Collider2D collider; // 물고기 콜라이더2D
    public AnimationCurve speed;
    private bool revers;
    private SpriteRenderer[] spriteRenderer;

    public bool Active
    {
        get
        {
            if (fishObj.activeSelf && collider.enabled)
                active = true;
            else
                active = false;

            return active;
        }
        set
        {
            fishObj.SetActive(value);
            collider.enabled = value;
        }
    }
    public bool Revers
    {
        set
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = fishObj.transform.
                    GetComponentsInChildren<SpriteRenderer>();
            }
            foreach(var v in spriteRenderer)
            {
                v.flipX = value;
            }
        }
    }
}

public class FishController : MonoBehaviour
{
    public static FishController Instance;
    public int level;   // 현재 진화단계
    public int growthCount;     // 진화에 필요한 먹이 갯수
    public int currentCount;    // 현재 먹은 먹이 갯수
    public Fish[] growthType;
    public bool chase = false;
    public Transform target;    // 추격할 대상
    public RayObjInfo2D info = new RayObjInfo2D();  // 클릭한 오브젝트 정보
    public AnimationCurve moveSpeed; // 이동 속도
    private float timer = 0;
    private float tick;
    private SpriteRenderer[] spriteRenderer;
    public GameObject gameOverUI;
    public GameObject gameClearUI;
    public GameObject feedEffect;   //먹이 먹었을때 효과
    private bool notTrigger = false;
    private AudioSource audioSource;

    public List<GameObject> sharks;

    private void Awake()
    {
        Instance = this;
        spriteRenderer = GetComponentsInChildren<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    public void UpdateLogic()
    {
        timer += Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && !notTrigger)
        {
            // UI Layer를 갖고있는 오브젝트만 읽어옴
            if (InputManager.Instance.RayCastObj2D(
                        info, LayerMask.NameToLayer("Back Ground")))
            {
                timer = 0;
                chase = true;
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
                target.position = info.point;   // 마우스 충돌 위치로 target 이동               
            }
        }

        tick = Time.deltaTime * moveSpeed.Evaluate(timer);

        Vector3 pos = transform.position;
        Vector3 dis = TargetToA_Dis(info.point, pos);

        if (chase)
        {
            // 현재 레벨의 이미지 뒤집기
            growthType[level].Revers = (dis.x < 0 ? true : false);
            pos.x += tick * dis.x;
            pos.y += tick * dis.y;
            transform.position = pos;
        }
        growthCount = FishGameManager.Instance.count;
    }

    // 진화
    private void GrowEvent(int currentLevel)
    {
        FishGameManager manager = FishGameManager.Instance;
        // 경험치(먹이)를 일정량 획득 시 레벨업
        if (currentCount >= growthCount)
        {
            level++;
            foreach(var shark in sharks)
            {
                shark.GetComponent<Animator>().Rebind();
            }
        }

        if (level >= 3)
        {
            notTrigger = true;
            FishGameManager.Instance.stateClass.resultState = GameResult.Success;
            FishGameManager.Instance.zozo.Change(GameState.GameResult);
            //gameClearUI.SetActive(true);
        }
        else if (currentCount >= growthCount)  
        {
            // 진화 이벤트
            Grow_Appearance_Change(currentLevel);
            Grow_Status_Change(currentLevel);
            StartCoroutine(manager.ILevelUp());
            manager.CreateRandomFeeds(manager.feedMin, manager.feedMax);
            currentCount = 0;
        }
    }

    // 외형을 다음 레벨로 변경
    public void Grow_Appearance_Change(int currentLevel)
    {
        growthType[currentLevel + 1].Active = true;
        growthType[currentLevel].Active = false;
    }

    // 스테이터스 정보를 다음 레벨로 변경
    public void Grow_Status_Change(int currentLevel)
    {
        transform.position = Vector3.zero;
        moveSpeed = growthType[currentLevel + 1].speed;
    }

    // 타겟과 a 사이의 노멀라이즈 좌표 값
    public Vector3 TargetToA_Dis(Vector3 target, Vector3 a)
    {
        return (target - a).normalized;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!notTrigger)
        {
            if (collision.CompareTag("Feed"))
            {
                chase = false;
                currentCount++;
                Instantiate(feedEffect, collision.transform.position, Quaternion.identity);
                GrowEvent(level);
                Destroy(collision.gameObject);
            }
            else if (collision.CompareTag("Enemy"))
            {
                //Time.timeScale = 0;
                // gameOverUI.SetActive(true);

                FishGameManager.Instance.stateClass.resultState = GameResult.Fail;
                FishGameManager.Instance.zozo.Change(GameState.GameResult);

            }
            else if (collision.CompareTag("Point"))
            {
                chase = false;
            }
        }
    }
}
