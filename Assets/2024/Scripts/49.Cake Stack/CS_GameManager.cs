using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CS_GameManager : TouchManager_3DTouch
{
    [SerializeField] GameObject cakeParent; //케이크가 생성될 부모 오브젝트
    [SerializeField] GameObject scoreParent; //케이크 갯수를 셀 부모 오브젝트
    [SerializeField] GameObject[] cakePrefabs; //케이크 프리팹
    [SerializeField] GameObject ladder; //사다리 오브젝트

    [SerializeField] TextMeshProUGUI scoreText; //점수 텍스트

    float posY; //사다리와 카메라 위치값
    float ladderY; //사다리의 위치값

    int score; //케이크 쌓은 수

    private void Awake()
    {
        CakeSpawn();
        ladderY = ladder.transform.position.y;
    }

    public override void HandleInput(Vector2 pos)
    {
        isTouchable = true;

        if (cakeParent.transform.childCount != 0)
        {
            //케이크 오브젝트 변수 지정
            GameObject cake = cakeParent.transform.GetChild(0).gameObject;

            //클릭하면 케이크 키네마틱 꺼지고 점수오브젝트 자녀로 만듦
            cake.transform.parent = scoreParent.transform;
            cake.GetComponent<Rigidbody>().isKinematic = false;

            //1초뒤 케이크 스폰
            Invoke("CakeSpawn", 1f);
            //0.5초뒤 케이크 사운드 재생
            Invoke("CakeSound", 0.5f);

            //떨어지고 있는 수 빼고 케이크 수 계산
            scoreText.text = score + "  pieces";
        }
    }

    void CakeSpawn()
    {
        //케이크 생성
        Instantiate(cakePrefabs[Random.Range(0, cakePrefabs.Length)],cakeParent.transform);  
    }

    void CakeSound()
    {
        SoundMGR.Instance.SoundPlay("띠융");
    }

    public void CameraUp()
    {
        //사다리와 카메라 transform.y 값 1.3f 상승
        ladderY += 1.3f;
        posY += 1.3f;

        Camera camera = Camera.main;

        camera.transform.DOMoveY(posY, 1f);
        ladder.transform.DOMoveY(ladderY, 1f);

        score++;

        print("상승");
    }
}
