using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class HammerMove : MonoBehaviour
{
    [SerializeField] MagicLife life; //라이프 스크립트
    [SerializeField] BoxSpawner spawner; //박스 스포너 스크립트
    [SerializeField] GameObject effect; //이펙트 프리팹
    [SerializeField] GameObject[] victoryObj; //빅토리 오브젝트
    [SerializeField] TextMeshProUGUI scoreText; //현재 남은 박스 수 표시

    int score; //스코어

    private void OnCollisionEnter(Collision collision)
    {
        //콜라이전의 태그가 target이라면
        if(collision.transform.tag=="target")
        {
            //이펙트 위치 지정 후 생성
            Vector3 effectPos = new Vector3(gameObject.transform.position.x-2f, gameObject.transform.position.y, gameObject.transform.position.z-5);
            Instantiate(effect, effectPos, Quaternion.identity);

            SoundMGR.Instance.SoundPlay("rocket");

            //콜라이전 애니메이터의 브로큰 애니메이션 실행 후 0.5초 뒤 파괴
            collision.transform.GetComponent<Animator>().SetTrigger("Broken");
            Destroy(collision.gameObject,0.5f);
            print("상자파괴");

            //박스콜라이더 비활성화, 자식 오브젝트 비활성화
            collision.transform.GetComponent<BoxCollider>().enabled = false;
            collision.transform.GetChild(0).transform.GetComponent<BoxCollider>().enabled = false;

            if (score<9)
            {
                score++; //스코어 상승
                scoreText.text = $"{score} / 10";
                spawner.BoxSpawn(); //박스 스폰
            }
            else
            {
                scoreText.transform.parent.gameObject.SetActive(false);
                StartCoroutine(Ending()); //엔딩씬 재생   
                SoundMGR.Instance.SoundPlay("select");     
            }
        }
        else if(collision.transform.tag=="Enemy")
        {
            //라이프 감소 함수 실행
            life.LifeDelete();
            print("라이프감소");   

            //박스 흔들림 애니메이션 재생
            collision.transform.parent.GetComponent<Animator>().SetTrigger("Shake");

            if (gameObject.activeInHierarchy) // 오브젝트가 활성화된 경우에만 코루틴 실행
            {
                //에너미 큐브 비활성화 후 1초 뒤 다시 활성화
                collision.gameObject.SetActive(false);
                StartCoroutine(StoneFalse(collision.gameObject));
            }

            //망치 작아졌다 커짐
            gameObject.transform.DOScale(0.6f, 0.2f).SetLoops(2, LoopType.Yoyo);

            SoundMGR.Instance.SoundPlay("pistol");
        }
    }

    public void Attack()
    {
        //망치 위아래 움직임 멈추면서 때리는 모션 실행
        gameObject.GetComponent<DOTweenAnimation>().DOPause();
        gameObject.transform.DORotateQuaternion(Quaternion.Euler(179, 270, 90), 0.3f).OnComplete(() =>
        {
            gameObject.transform.DOLocalMoveX(5f, 0.3f);
            gameObject.transform.DORotateQuaternion(Quaternion.Euler(0, 270, 90), 0.3f).OnComplete(() =>
            {
                gameObject.transform.DOLocalMoveX(6f, 0.3f).OnComplete(() =>
                {
                    if (gameObject.activeInHierarchy) // 오브젝트가 활성화된 경우에만 코루틴 실행
                    {
                        gameObject.GetComponent<DOTweenAnimation>().DOPlay();
                        StartCoroutine(Touchable()); //터치가능 true
                    }  
                });
            });
        });
    }

    IEnumerator Ending()
    {
        
        yield return new WaitForSeconds(1f);

        //스포너 및 망치 메쉬 비활성화 후 이펙트 생성
        spawner.gameObject.SetActive(false);
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        Instantiate(effect, gameObject.transform.position, Quaternion.identity);

        //박스콜라이더 활성화 다리 떨어지고 1초 뒤 전등, 체스말 오브젝트들 켜짐
        victoryObj[0].SetActive(true);
        victoryObj[1].SetActive(true);

        yield return new WaitForSeconds(0.5f);
        SoundMGR.Instance.SoundPlay("쾅");

        yield return new WaitForSeconds(1f);
        SoundMGR.Instance.SoundPlay("쾅");

        yield return new WaitForSeconds(0.5f);
        SoundMGR.Instance.SoundPlay("arrow");

        victoryObj[2].SetActive(true);
        victoryObj[3].SetActive(true);
        victoryObj[4].SetActive(true);

        //4초 뒤에 하트 이펙트 켜짐
        yield return new WaitForSeconds(4f);
        SoundMGR.Instance.SoundStop("arrow");

        victoryObj[5].SetActive(true);

        yield return new WaitForSeconds(1f);

        //win 팝업 활성화
        victoryObj[6].SetActive(true);

    }

    IEnumerator Touchable()
    {
        //1초 뒤에 터치가능
        yield return new WaitForSeconds(1.5f);
        BG_ClickHammer.touchOn = true;
    }

    IEnumerator StoneFalse(GameObject stoneCube)
    {
        yield return new WaitForSeconds(1f);
        stoneCube.gameObject.SetActive(true);
    }
}
