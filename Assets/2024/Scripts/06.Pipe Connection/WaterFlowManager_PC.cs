using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterFlowManager_PC : MonoBehaviour
{
    public static WaterFlowManager_PC Instance { get; private set; }

    private List<Image> fillImages = new List<Image>();
    private float fillDuration; // 채우는 데 걸리는 시간
    private float minfillDuration = 0.5f; // 채우는 데 걸리는 시간
    private float maxfillDuration = 2f; // 채우는 데 걸리는 시간
    private Coroutine fillCoroutine; // 채우기 코루틴을 저장할 변수
    private Pipe_PC currentPipe;
    private Pipe_PC nextPipe;
    public bool isConnectable = false;
    private bool isFlowable = false; // 실제 isFlowable 상태를 저장
    public bool IsFlowable
    {
        get => isFlowable;
        set
        {
            isFlowable = value;
            if (isFlowable) // isFlowable이 true로 변경될 때
            {
                StartFillingImages();
            }
        }
    }
    int currentIndex = 0;
    public Image failImage;

    private void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    public void Init()
    {
        // 현재 실행 중인 코루틴이 있다면 중단
        if (fillCoroutine != null)
        {
            StopCoroutine(fillCoroutine);
            fillCoroutine = null;
        }

        StartCoroutine(startCoroutine());
    }

    IEnumerator startCoroutine()
    {
        fillImages.Clear();

        fillDuration = maxfillDuration;

        currentIndex = 0;

        // 실패 UI 보이게 하기
        Color color = failImage.color;
        color.a = 1f;
        failImage.color = color;

        // StartPipe 초기화 및 자식 이미지 추가
        GameObject startPipe = GameObject.Find("StartPipe");
        if (startPipe != null)
        {
            Image image = startPipe.GetComponentInChildren<Image>(true);   //true 해주면 비활성화된 객체도 찾음
            image.fillAmount = 0;

            fillImages.Add(image);
        }

        // EndPipe 초기화
        GameObject endPipe = GameObject.Find("EndPipe");
        if (endPipe != null)
        {
            Image endImage = endPipe.GetComponentInChildren<Image>(true); // 비활성화된 객체도 찾음
            endImage.fillAmount = 0;
        }

        yield return new WaitForSeconds(1f);
    }

    private void StartFillingImages()
    {
        // fillCoroutine이 null일 때만 채우기 시작
        if (fillCoroutine == null)
        {
            fillCoroutine = StartCoroutine(FillImagesSequentially());
            SoundMGR.Instance.SoundPlay("물흐름");
        }
    }

    // 이미지를 순차적으로 채우는 코루틴
    private IEnumerator FillImagesSequentially()
    {
        currentIndex = 0;

        while (currentIndex < fillImages.Count)
        {
            // currentIndex가 3보다 크면 fillDuration을 1.5로 변경
            if (currentIndex > 3)
            {
                fillDuration = minfillDuration;
            }

            Image imageToFill = fillImages[currentIndex];
            yield return StartCoroutine(FillImage(imageToFill));
            currentIndex++;
        }

        // 모든 이미지가 채워진 후, 마지막으로 채운 이미지의 부모 이름 확인
        Image lastImage = fillImages[currentIndex - 1];
        Pipe_PC lastPipe = lastImage.GetComponentInParent<Pipe_PC>();

        // 마지막 파이프에서만 성공 여부 체크
        if (lastPipe != null)
        {
            CheckGameSuccess(lastPipe);
            SoundMGR.Instance.SoundStop("물흐름");
        }
    }

    // 개별 이미지를 채우는 코루틴
    private IEnumerator FillImage(Image image)
    {
        float elapsedTime = 0f;

        // 현재 흐르고 있는 Pipe_PC 컴포넌트 가져오기
        currentPipe = image.GetComponentInParent<Pipe_PC>();

        while (elapsedTime < fillDuration)
        {
            if (image == null) yield break; // 이미지가 null이면 종료
            image.fillAmount = Mathf.Lerp(0, 1, elapsedTime / fillDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (image != null)
        {
            image.fillAmount = 1f; // 마지막에 fillAmount를 1로 설정
                                   // 현재 이미지가 마지막 이미지인지 확인
            if (fillImages.IndexOf(image) == fillImages.Count - 1)
            {
                SetNextPipe(currentPipe); // 마지막 이미지일 때만 SetNextPipe 호출
            }
        }
    }

    public void SetNextPipe(Pipe_PC currentPipe)
    {
        Vector2 direction = Vector2.zero;

        // endDirection에 따라 방향 설정
        switch (currentPipe.endDirection)
        {
            case PipeDirection.North:
                direction = Vector2.up; // 위쪽
                break;
            case PipeDirection.South:
                direction = Vector2.down; // 아래쪽
                break;
            case PipeDirection.West:
                direction = Vector2.left; // 왼쪽
                break;
            case PipeDirection.East:
                direction = Vector2.right; // 오른쪽
                break;
        }

        // 현재 파이프의 위치
        Vector2 currentPosition = currentPipe.transform.position;

        // 방향으로 2 유닛 떨어진 위치 계산
        Vector2 nextPosition = currentPosition + direction * 2;

        // 해당 위치에 있는 nextPipe 컴포넌트 찾기
        Collider2D hit = Physics2D.OverlapCircle(nextPosition, 0.1f); // 작은 반경으로 탐색
        if (hit != null)
        {
            nextPipe = hit.GetComponent<Pipe_PC>();
            ImageCell_PC nextCell = nextPipe.GetComponentInParent<ImageCell_PC>();
            if (nextCell != null)
            {
                nextCell.UnableCell();

            }
            if (nextPipe != null)
            {
                nextPipe.CheckConnection(currentPipe);
                if(isConnectable)
                {
                    AddToPipe(nextPipe);             
                }
            }
        }
    }

    // 새로운 이미지를 채우기 
    public void AddToPipe(Pipe_PC pipe)
    {
        foreach (Image image in pipe.pipeImages)
        {
            if (!fillImages.Contains(image))
            {
                fillImages.Add(image);
            }
        }
    }

    private void CheckGameSuccess(Pipe_PC currentPipe)
    {
        // 현재 파이프의 부모 이름 확인
        string parentName = currentPipe.gameObject.name;

        if (parentName == "EndPipe")
        {
            GameManager_PC.Instance.GameSuccess(); // 게임 성공
        }
        else
        {
            GameManager_PC.Instance.GameOver(); // 게임 오버
        }
    }
}
