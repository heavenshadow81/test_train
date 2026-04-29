using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class SW_GameManager : PlayManager_PlayGround
{
    // Start is called before the first frame update

    [SerializeField] GameObject ufoObject; //기지 오브젝트
    [SerializeField] GameObject[] spawnPosition; //스포너 위치

    [SerializeField] GameObject[] ailens; //에일리언 프리팹들
    List<GameObject> ailenlist = new List<GameObject>(); //에일리언 리스트
    [SerializeField] GameObject bomb;

    int timer = 60;
    [SerializeField] TextMeshProUGUI timerText; //타이머 텍스트

    [SerializeField] GameObject success; //성공 오브젝트
    void OnEnable()
    {
        SW_UFO.gameOver = false;
        timer = 60;
        Invoke("AilensSpawn", 4);
    }

    protected override void Init()
    {
        base.Init();
    }

    void AilensSpawn()
    {
        if (!SW_UFO.gameOver)
        {
            ufoObject.transform.DOScale(1, 0.5f);

            if (timer > 0)
            {
                int speed = Random.Range(4, 7);

                if (timer > 30)
                {
                    speed = Random.Range(5, 9);
                }

                //타이머 텍스트 현재 남은 시간 표시
                timerText.text = timer.ToString();
                timer--;

                //속도 랜덤하게
                int time = Random.Range(4, 7);
                Vector3 pos = spawnPosition[Random.Range(0, spawnPosition.Length)].transform.position;

                //랜덤한 위치에 랜덤한 에일리언 생성
                GameObject enemy = Instantiate(ailens[Random.Range(0, ailens.Length)], pos, Quaternion.identity);
                enemy.transform.DOMove(ufoObject.transform.position, speed);
                ailenlist.Add(enemy);

                Invoke("AilensSpawn", 1);
            }
            else
            {
                for(int i = 0; i < ailenlist.Count; i++)
                {
                    if (ailenlist[i] != null)
                    {
                        SoundMGR.Instance.SoundPlay("PlayGround_Touch");
                        GameObject particle = Instantiate(bomb, ailenlist[i].transform.position, Quaternion.identity);
                        Destroy(particle, 1);

                        Destroy(ailenlist[i]);
                    }
                }
                SoundMGR.Instance.SoundPlay("PlayGround_RightAnswer");
                success.SetActive(true);
            }    
        }
        else
        {
            for (int i = 0; i < ailenlist.Count; i++)
            {
                if (ailenlist[i] != null)
                {
                    SoundMGR.Instance.SoundPlay("PlayGround_Touch");
                    GameObject particle = Instantiate(bomb, ailenlist[i].transform.position, Quaternion.identity);
                    Destroy(particle, 1);

                    Destroy(ailenlist[i]);
                }
            }
        }
    }

    public override void HandleInput(Vector2 inputPosition)
    {
        // 터치 활성화
        isTouchable = true;

        // 터치/마우스 위치를 월드 좌표로 변환
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(inputPosition);

        // 터치/마우스 위치에서 카드 찾기
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                SoundMGR.Instance.SoundPlay("PlayGround_Touch");

                GameObject particle = Instantiate(effect, hit.collider.transform.position, Quaternion.identity);
                Destroy(particle, 1);

                Destroy(hit.collider.gameObject);
            }
        }
    }

    public override void CorrectAnswer(GameObject touched)
    {

    }
    public override void WrongAnswer(GameObject touched)
    {

    }
}
