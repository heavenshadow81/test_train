using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;
using static Settings;

public class FishGameManager : MonoBehaviour/*, Game.IMyGameActions*/
{
   /* private Game inputGame; // New Input System*/

    public EnumClass stateClass;
    public ZoZoBasePatton<FishGameManager> zozo;
    public ScreenProsess screenProsess;

    public static FishGameManager Instance;
    public GameObject[] sharks;
    public Image level;
    public GameObject prefab;   // 먹이 오브젝트
    [HideInInspector] public int count;   // 먹이 갯수
    public List<Sprite> levelSprites = new List<Sprite>();
    public float correctionX;
    public float correctionY;

    public Curve[] feedMove;

    public int feedMin;
    public int feedMax;

    private float camX;
    private float camY;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        
        stateClass = new EnumClass();

        ActionProcess.Enter_StateListener(() => 
        {
            SetRandomFeedCount(feedMin, feedMax);
            levelSprites.Add(Resources.Load<Sprite>("Fish/Level1"));
            levelSprites.Add(Resources.Load<Sprite>("Fish/Level2"));
            levelSprites.Add(Resources.Load<Sprite>("Fish/Level3"));

        }, null, play, null);

        zozo = new ZoZoBasePatton<FishGameManager>();
        zozo.Init(stateClass, screenProsess, new ReadyProcess(screenProsess), new ResultProcess(screenProsess));
    }
    private void play()
    {
        camY = Camera.main.orthographicSize;
        camX = camY * Camera.main.aspect;

        for (int i = 0; i < count; i++)
        {
            CreateRandomFeed();
        }
        StartCoroutine(ILevelUp());
    }
    private int randomA;
    private bool checkA;
    private int randomB;
    private bool checkB;

    /*private void OnEnable()
    {
        // New Input System 사용하기 위한 초기화
        inputGame = new Game();
        inputGame.Enable();
        inputGame.MyGame.AddCallbacks(this);
        EnhancedTouchSupport.Enable();
        // Down 이벤트 사용하기 위해 입력 이벤트에 등록
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown += TouchDownEvent;
    }
    // 삭제 시 터치 이벤트 삭제
    private void OnDisable()
    {
        // Down 이벤트 반환하기 위해 입력 이벤트에서 제거
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown -= TouchDownEvent;
        EnhancedTouchSupport.Disable();
        inputGame.Disable();
    }// 터치 이벤트
    public void TouchDownEvent(Finger finger)
    {
        // 마우스 포인터에 Ray를 쏴 Sheep클래스를 보유한 오브젝트가 있을 시 Sheep의 ClickEvent 함수 실행
        RaycastHit2D hit = (Physics2D.Raycast(Camera.main.ScreenToWorldPoint(finger.currentTouch.screenPosition), Vector2.zero));
        if (hit)
        {
            if (hit.collider.CompareTag("Back Ground"))
            {
                
            }
        }
    }

    // 마우스 이벤트
    public void OnDown(InputAction.CallbackContext context)
    {
        if (Settings.instance.mouseToggle.isOn == false)
            return;
        // 한번만 클릭되도록 체크 값이 1일떄만 실행
        //if (context.ReadValue<float>() == 1f)
        {
            // 마우스 포인터에 Ray를 쏴 Sheep클래스를 보유한 오브젝트가 있을 시 Sheep의 ClickEvent 함수 실행
            RaycastHit2D hit = (Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Settings.instance.MousePos()*//*Input.mousePosition*//*), Vector2.zero, 0));
            if (hit)
            {
                if (hit.collider.CompareTag("Back Ground"))
                {
                    
                }
            }
        }
    }
    public void OnTouch(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnIsDown(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }*/

    // ------------

    private void Update()
    {
        if (zozo != null)
        {
            zozo.MGR.Excute(() =>
            {
                FishController.Instance.UpdateLogic();
                if (!checkA && FishController.Instance.level == 1)
                {
                    checkA = true;
                    randomA = UnityEngine.Random.Range(0, sharks.Length);

                    sharks[randomA].SetActive(true);
                }
                if (!checkB && FishController.Instance.level == 2)
                {
                    checkB = true;
                    do
                    {
                        randomB = UnityEngine.Random.Range(0, sharks.Length);
                    } while (randomB == randomA);

                    sharks[randomB].SetActive(true);
                }
            });
        }
    }


    public void SetRandomFeedCount(int min, int max)
    {
        while (true)
        {
            count = UnityEngine.Random.Range(min, max);
            if (count % 2 == 1)
            {
                return;
            }
        }
    }

    public void CreateRandomFeeds(int min, int max)
    {
        SetRandomFeedCount(min, max);
        for (int i = 0; i < count; i++)
        {
            CreateRandomFeed();
        }
    }

    public void CreateRandomFeed()
    {
        float x = UnityEngine.Random.Range(-camX + correctionX, camX - correctionX);
        float y = UnityEngine.Random.Range(-camY + correctionY, camY - correctionY);

        GameObject temp = Instantiate(prefab,
            new Vector3(x, y, transform.position.z),
            Quaternion.identity, transform);

        int random;
        random = UnityEngine.Random.Range(0, feedMove.Length);
        temp.GetComponent<ConvertMove>().xCurve = feedMove[random];
        random = UnityEngine.Random.Range(0, feedMove.Length);
        temp.GetComponent<ConvertMove>().yCurve = feedMove[random];
    }
    
    public IEnumerator ILevelUp()
    {
        Debug.Log("\nUp");
        level.gameObject.SetActive(true);
        level.sprite = levelSprites[FishController.Instance.level];
        yield return new WaitForSeconds(2);
        level.gameObject.SetActive(false);
    }
}
