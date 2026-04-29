using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CenterRPS : MonoBehaviour
{
    public static CenterRPS instance;

    public Image RPSIMG; //가위바위보 이미지
    public GameObject CenterBG; //가운데 이미지 배경
    public Sprite[] CenterIMG; //가운데 이미지

    public bool speedCheck; //누가 빨랐는지 체크하기 위한 값
    public bool gameCheck; //게임 진행중인지 체크
    public bool isCoroutineRunning;

    void Awake()
    {
        // 인스턴스가 이미 존재하는지 확인
        if (instance == null)
        {
            instance = this; // 인스턴스가 없으면 현재 오브젝트를 인스턴스로 설정
            //DontDestroyOnLoad(gameObject); // 게임 오브젝트가 씬이 로드될 때 파괴되지 않도록 설정
        }
        else if (instance != this)
        {
            Destroy(gameObject); // 인스턴스가 이미 존재하면 새로운 인스턴스를 파괴
        }

        //StartCoroutine(RPSStart());
    }

    public IEnumerator RPSStart()
    {
        isCoroutineRunning = true;

        SoundMGR.Instance.SoundPlay("PlayGround_Card(2)");
        //가위 바위 보 중에 하나 랜덤하게 지정하기 위한 변수
        int random = Random.Range(3, 6);

        //이미지 3으로 바꾸고 활성화
        RPSIMG.sprite = CenterIMG[0];
        RPSIMG.gameObject.SetActive(true);

        //1초 뒤 이미지 2로 바꿈
        yield return new WaitForSeconds(1f);
        RPSIMG.sprite = CenterIMG[1];

        yield return new WaitForSeconds(1f);
        RPSIMG.sprite = CenterIMG[2];

        yield return new WaitForSeconds(1f);

        //묵찌빠 랜덤 이미지 생성 후 스피드 체크 값 true
        CenterBG.SetActive(true);
        RPSIMG.sprite = CenterIMG[random];
        speedCheck = true;
        gameCheck = true;

        isCoroutineRunning = false;
    }
}
