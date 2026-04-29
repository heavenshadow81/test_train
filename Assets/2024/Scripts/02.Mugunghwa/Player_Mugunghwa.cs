using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Mugunghwa : MonoBehaviour
{
    float moveSpeed = 1.2f;
    float minScale = 0.3f; // 최소 스케일
    float maxScale = 0.7f; // 최대 스케일
    public bool isMoving = false;
    Vector3 startPosition; // 시작 위치
    float currentScale = 0.3f; // 현재 스케일

    public Transform endPosition; // 도착 위치

    int soundState = 0;

    [Header("인풋시스템")]
    public InputActionAsset actionAsset;
    private InputActionMap actionMap;
    private InputAction touchAction;

    [Header("애니메이션")]
    public Sprite[] sprites;
    public SpriteRenderer spriteRenderer;
    public GameObject gameOverUI;
    public Tagger_Mugunghwa tagger;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        actionMap = actionAsset.FindActionMap("Touch");
        touchAction = actionMap.FindAction("Touch");
    }
    private void OnEnable()
    {
        // 터치 입력을 활성화
        touchAction.Enable();
        touchAction.started += OnTouchPerformed;
        touchAction.canceled += OnTouchCanceled;
    }

    private void OnDisable()
    {
        // 터치 입력 비활성화
        touchAction.Disable();
        touchAction.started -= OnTouchPerformed;
        touchAction.canceled -= OnTouchCanceled;
    }
    private void Start()
    {
        // 시작 위치 설정
        startPosition = transform.position;
    }

    void Update()
    {
        //if (!GameManager_Mugunghwa.Instance.isPlaying) return;

        //if (Input.GetMouseButtonDown(0))
        //{
        //    isMoving = true;

        //    if (GameManager_Mugunghwa.Instance.isGameOver)
        //    {
        //        spriteRenderer.sprite = sprites[2];
        //    }
        //    else
        //    {
        //        spriteRenderer.sprite = sprites[1];
        //        SoundMGR.Instance.SoundPlay("무궁화_걷기");
        //    }
        //}
        //else if (Input.GetMouseButtonUp(0))
        //{
        //    isMoving = false;

        //    if (GameManager_Mugunghwa.Instance.isGameOver)
        //    {
        //        spriteRenderer.sprite = sprites[2];
        //        gameOverUI.gameObject.SetActive(true);
        //    }
        //    else
        //    {
        //        spriteRenderer.sprite = sprites[0];
        //        SoundMGR.Instance.SoundStop("무궁화_걷기");
        //    }
        //}
    }

    private void FixedUpdate()
    {
        MoveAndScale();
    }

    private void OnTouchPerformed(InputAction.CallbackContext context)
    {
        if (!GameManager_Mugunghwa.Instance.isPlaying) return;

        isMoving = true;

        if (GameManager_Mugunghwa.Instance.isGameOver)
        {
            spriteRenderer.sprite = sprites[2];
        }
        else
        {
            spriteRenderer.sprite = sprites[1];
            SoundMGR.Instance.SoundPlay("무궁화_걷기");
        }
    }

    private void OnTouchCanceled(InputAction.CallbackContext context)
    {
        isMoving = false;

        if (GameManager_Mugunghwa.Instance.isGameOver)
        {
            spriteRenderer.sprite = sprites[2];
            gameOverUI.SetActive(true);
        }
        else
        {
            spriteRenderer.sprite = sprites[0];
            SoundMGR.Instance.SoundStop("무궁화_걷기");
        }
    }
    private void MoveAndScale()
    {
        if (isMoving)
        {
            // 라디안으로 변환
            float angle = Mathf.Deg2Rad * 23f;

            // 기울어진 방향의 x, y 오프셋 계산
            // Mathf.Cos(angle)은 x 방향 오프셋, Mathf.Sin(angle)은 y 방향 오프셋
            float xOffset = Mathf.Cos(angle) * moveSpeed * Time.deltaTime;
            float yOffset = Mathf.Sin(angle) * moveSpeed * Time.deltaTime;

            // 도착점을 향해 이동
            transform.Translate(new Vector3(-xOffset, -yOffset, 0f));
        }

        // 현재 위치와 도착 위치의 거리 계산
        float distance = Vector3.Distance(transform.position, endPosition.position);
        float totalDistance = Vector3.Distance(startPosition, endPosition.position);

        // 거리 비율에 따라 스케일 조정
        currentScale = Mathf.Lerp(maxScale, minScale, distance / totalDistance);
        transform.localScale = new Vector3(currentScale, currentScale, currentScale);

        if (distance <= 1f)
        {
            // 도착했다는 처리 작업 수행
            
            if(soundState == 0)
            {
                SoundMGR.Instance.SoundStop("무궁화_걷기");
                SoundMGR.Instance.SoundStop("무궁화_술래");
                SoundMGR.Instance.SoundPlay("무궁화_게임성공");
                spriteRenderer.sprite = sprites[3];
                tagger.spriteRenderer.sprite = tagger.sprites[3];
                GameManager_Mugunghwa.Instance.GameStop();
                soundState++;
            }          
        }
    }
    public bool GetIsMoving()
    {
        return isMoving;
    }
}
