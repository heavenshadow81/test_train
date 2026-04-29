using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
//using UnityEngine.UIElements;
using static ML.PlaywallKids.DragonPark.AToolStartPanel;

public class PlayerController_CT : MonoBehaviour
{
    [Header("인풋시스템")]
    public InputActionAsset actionAsset;
    private InputActionMap actionMap;
    private InputAction touchAction;
    private List<Image> answerCheckImages = new List<Image>(); // 이미지 리스트 추가

    [Header("애니메이션")]
    public float moveSpeed = 2f; // 이동 속도
    public float jumpHeight = 5f; // 점프 높이
    private bool isJumping = false;
    private Vector2 leftPosition;
    private Vector2 rightPosition;
    private bool movingLeft = true;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [Header("정답체크")]
    private GameObject answerObject;
    public GameObject particleObject;

    private void Awake()
    {
        // 인풋시스템 등록
        actionMap = actionAsset.FindActionMap("Touch");
        touchAction = actionMap.FindAction("Touch");
        touchAction.Enable();
        touchAction.started += CheckAnswer;

        // 애니메이션
        animator = GetComponent<Animator>(); 
        spriteRenderer = GetComponent<SpriteRenderer>();
        leftPosition = new Vector2(transform.position.x - 5, transform.position.y);   // 왼쪽 위치 지정
        rightPosition = new Vector2(transform.position.x + 5, transform.position.y); // 오른쪽 위치 지정
        StartCoroutine(Patrol());

        // 파티클 사이즈 조절
        particleObject.transform.localScale = Vector3.one * 0.3f;
    }

    void CheckAnswer(InputAction.CallbackContext context)
    {
        if (isJumping) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        // Raycast를 통해 오브젝트 감지
        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag("Option"))
        {
            // 감지된 오브젝트의 스프라이트 가져오기
            Sprite clickedSprite = hit.collider.GetComponent<Image>().sprite;

            // 정답을 클릭했을 때 추가 작업
            if (GameManager_CT.Instance.answerSettingsUI.GetAnswerSprite().Contains(clickedSprite))
            {
                isJumping = true;

                StartCoroutine(JumpToPosition(mousePos2D));

                GameManager_CT.Instance.correctAnswerUI.ChangeBoneImage();
                int boneIndex = GameManager_CT.Instance.correctAnswerUI.GetBoneIndex();
                PlaySound(clickedSprite.name);

                // 정답인 오브젝트 담기
                answerObject = hit.collider.gameObject;

                foreach (GameObject answer in GameManager_CT.Instance.answerSettingsUI.GetAnswerObject())
                {
                    Image answerImage = answer.GetComponent<Image>();

                    if (answerImage != null && answerImage.sprite == clickedSprite)
                    {
                        // "Answer Check"라는 이름을 가진 자식 이미지 찾기
                        Transform answerCheckTransform = answer.transform.Find("Answer Check");
                        if (answerCheckTransform != null)
                        {
                            Image answerCheckImage = answerCheckTransform.GetComponent<Image>();
                            if (answerCheckImage != null)
                            {
                                answerCheckImages.Add(answerCheckImage); // 이미지 리스트에 추가
                                // FillAmount를 채우는 코루틴 호출
                                StartCoroutine(FillImage(answerCheckImage, 1f)); // 1초 동안 채우기
                            }
                        }
                    }
                }

                // 현재 boneIndex에 따라 Init() 호출 및 성공 처리
                StartCoroutine(CheckGameStatus(boneIndex));
            }
            else
            {
                SoundMGR.Instance.SoundPlay("CatchingText_Fail");
            }
        }
    }

    IEnumerator CheckGameStatus(int boneIndex)
    {
        // 1초 대기
        yield return new WaitForSeconds(1f);



        if (boneIndex == 2 || boneIndex == 4)
        {
            GameManager_CT.Instance.StopBone();
            SetFillZero();
            GameManager_CT.Instance.answerSettingsUI.Init(); // Init() 호출        
            GameManager_CT.Instance.MoveBone();
        }
        else if (boneIndex == 6)
        {
            GameManager_CT.Instance.StopBone();
            GameManager_CT.Instance.GameSuccess();
        }
    }

    IEnumerator FillImage(Image image, float duration)
    {
        SoundMGR.Instance.SoundPlay("CatchingText_Circle");
        float elapsedTime = 0f;
        float startValue = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            image.fillAmount = Mathf.Lerp(startValue, 1f, elapsedTime / duration);
            yield return null; // 다음 프레임까지 대기
        }

        image.fillAmount = 1f; // 마지막 값을 정확히 설정
    }

    public void SetFillZero()
    {
        // answerCheckImages의 fillAmount를 0으로 초기화
        foreach (Image img in answerCheckImages)
        {
            img.fillAmount = 0f; // fillAmount를 0으로 설정
        }
        answerCheckImages.Clear(); // 리스트 초기화
    }

    void PlaySound(string soundName)
    {
        SoundMGR.Instance.SoundPlay(soundName);
    }

    private IEnumerator Patrol()
    {
        while (true)
        {
            Vector2 targetPosition = movingLeft ? leftPosition : rightPosition;

            animator.SetBool("isWalking", true); // 걷기 애니메이션 시작
            spriteRenderer.flipX = movingLeft; // 방향에 따라 스프라이트 플립 설정

            while ((Vector2)transform.position != targetPosition)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null; // 매 프레임 대기
            }

            animator.SetBool("isWalking", false); // 걷기 애니메이션 종료
            movingLeft = !movingLeft; // 방향 전환
            yield return new WaitForSeconds(1f); // 잠시 대기
        }
    }

    private IEnumerator JumpToPosition(Vector2 targetPosition)
    {
        animator.SetBool("isJumping", true); // 점프 애니메이션 시작

        // 점프
        float jumpStartTime = Time.time;
        Vector2 startPosition = transform.position;
        Vector2 jumpPosition = new Vector2(targetPosition.x, targetPosition.y);
        Vector2 endPosition = new Vector2(targetPosition.x, startPosition.y);

        while (Time.time < jumpStartTime + 0.5f) // 0.5초 동안 점프
        {
            transform.position = Vector2.Lerp(startPosition, jumpPosition, (Time.time - jumpStartTime) / 0.5f);
            yield return null; // 매 프레임 대기
        }

        // 파티클 오브젝트 인스턴스화
        Instantiate(particleObject, jumpPosition, Quaternion.identity);

        answerObject.SetActive(false);
        transform.position = targetPosition; // 정확한 위치로 설정

        // 떨어지는 과정
        float fallStartTime = Time.time;
        while (Time.time < fallStartTime + 0.5f) // 0.5초 동안 떨어짐
        {
            transform.position = Vector2.Lerp(jumpPosition, endPosition, (Time.time - fallStartTime) / 0.5f);
            yield return null; // 매 프레임 대기
        }

        isJumping = false;
        animator.SetBool("isJumping", false); // 점프 애니메이션 종료
    }
}
