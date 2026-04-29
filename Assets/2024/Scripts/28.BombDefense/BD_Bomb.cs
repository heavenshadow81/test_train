using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class BD_Bomb : MonoBehaviour
{
    int direction; //시작할 때 폭탄의 방향을 정할 변수

    [SerializeField] TextMeshProUGUI[] scoreText; //색상별 점수 텍스트
    [SerializeField] BD_Manger green; //색상별 점수
    [SerializeField] BD_Manger orange; //색상별 점수
    [SerializeField] GameObject bomb; //폭탄 파티클
    Vector3 originPos;

    // Start is called before the first frame update
    void OnEnable()
    {
        //폭탄의 처음 위치 저장
        originPos = gameObject.transform.position;

        //숫자 1 이나 2로 랜덤하게 지정
        direction = Random.Range(1, 3);

        //디렉션이 1이면 오른쪽으로 이동 2면 왼쪽으로 이동
        if (direction == 1)
        {
            gameObject.transform.DOMoveX(7, 3);
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }
        else
        {
            gameObject.transform.DOMoveX(-7, 3);
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //닿은 트리거의 태그가 Player1이거나 Player2면 스코어 상승
        if (collision.tag== "Player1")
        {
            SoundMGR.Instance.SoundPlay("22.풍선터짐");

            //폭탄 비활성화
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            gameObject.transform.DOKill();
            gameObject.transform.DOMove(originPos, 0);

            //스코어 상승 후 표기 하고 폭탄 새로 셋팅
            green.score++;
            scoreText[0].text = green.score.ToString();
            StartCoroutine("BombSetting");

            //터지는 효과 생성
            GameObject particle = Instantiate(bomb, collision.transform.position, Quaternion.identity);
            Destroy(particle, 1f);

        }
        else if(collision.tag == "Player2")
        {
            SoundMGR.Instance.SoundPlay("22.풍선터짐");

            //폭탄 비활성화
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            gameObject.transform.DOKill();
            gameObject.transform.DOMove(originPos, 0);

            //스코어 상승 후 표기 하고 폭탄 새로 셋팅
            orange.score++;
            scoreText[1].text = orange.score.ToString();
            StartCoroutine("BombSetting");

            //터지는 효과 생성
            GameObject particle = Instantiate(bomb, collision.transform.position, Quaternion.identity);
            Destroy(particle, 1f);
        }
    }

    IEnumerator BombSetting()
    {
        yield return new WaitForSeconds(0.3f);

        //숫자 1 이나 2로 랜덤하게 지정
        direction = Random.Range(1, 3);
        int speed = Random.Range(2, 5);

        //디렉션이 1이면 오른쪽으로 이동 2면 왼쪽으로 이동
        if (direction == 1)
        {
            gameObject.transform.DOMoveX(7, speed);
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }
        else
        {
            gameObject.transform.DOMoveX(-7, speed);
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }

        //오브젝트 활성화
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
    }
}
