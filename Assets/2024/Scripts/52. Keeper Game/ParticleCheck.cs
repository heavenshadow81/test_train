using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ParticleCheck : MonoBehaviour
{
    Camera cam; //메인 카메라
    Transform player; //플레이어 오브젝트
    KG_GameManager manager; //게임매니저 오브젝트
    float playerZ;// 플레이어 위치 값

    private void Start()
    {
        cam = Camera.main;
        player = FindObjectOfType<PlayerAnimation>().transform;
        manager = FindObjectOfType<KG_GameManager>();
        playerZ = player.localPosition.z;
    }

    private void OnTriggerEnter(Collider other)
    {
        string objectTag = gameObject.tag;
        string collisionTag = other.gameObject.tag;

        if (objectTag == "Green")
        {
            if (collisionTag == "target")
            {
                //파이어볼 커지면서 삭제, 닿는 순간 콜라이더 비활성화
                gameObject.GetComponent<BoxCollider>().enabled = false;
                gameObject.transform.DOLocalMoveZ(-10, 2f).OnComplete(() => Destroy(gameObject));

                Debug.Log("라이프 감소");
                DamageSound();
                manager.LifeDelete();

                cam.transform.DOLocalRotateQuaternion(Quaternion.Euler(355, 0, 0), 0.4f).SetLoops(2, LoopType.Yoyo);
            }
            else if (collisionTag == "Green")
            {
                manager.score++; //플레이어 전진 수
                manager.SwordParticle();//불꽃 파티클 생성

                //파이어볼 작아지면서 삭제, 닿는 순간 콜라이더 비활성화
                gameObject.GetComponent<BoxCollider>().enabled = false;
                gameObject.transform.DOLocalMoveX(5, 1f).OnComplete(() =>
                {
                    Forward();
                    Destroy(gameObject);
                });
                Debug.Log("전진");
            }
            else if (collisionTag == "Orange")
            {
                //파이어볼 커지면서 삭제, 닿는 순간 콜라이더 비활성화
                gameObject.GetComponent<BoxCollider>().enabled = false;
                gameObject.transform.DOLocalMoveZ(-10, 2f).OnComplete(() => Destroy(gameObject));

                Debug.Log("라이프 감소");
                DamageSound();
                manager.LifeDelete();

                cam.transform.DOLocalRotateQuaternion(Quaternion.Euler(355, 0, 0), 0.4f).SetLoops(2, LoopType.Yoyo);
            }
        }
        else if (objectTag == "Orange")
        {
            if (collisionTag == "target")
            {
                Debug.Log("라이프 감소");
                manager.LifeDelete();

                //회오리 커지면서 삭제, 닿는 순간 콜라이더 비활성화
                gameObject.GetComponent<BoxCollider>().enabled = false;
                gameObject.transform.DOScale(4, 2f).OnComplete(() => Destroy(gameObject));
                DamageSound();

                cam.transform.DOLocalRotateQuaternion(Quaternion.Euler(355, 0, 0), 0.4f).SetLoops(2, LoopType.Yoyo);
            }
            else if (collisionTag == "Green")
            {
                Debug.Log("라이프 감소");
                manager.LifeDelete();

                //회오리 커지면서 삭제, 닿는 순간 콜라이더 비활성화
                gameObject.GetComponent<BoxCollider>().enabled = false;
                gameObject.transform.DOScale(4, 2f).OnComplete(() => Destroy(gameObject));

                DamageSound();
                cam.transform.DOLocalRotateQuaternion(Quaternion.Euler(355, 0, 0), 0.4f).SetLoops(2, LoopType.Yoyo);
            }
            else if (collisionTag == "Orange")
            {
                manager.score++; //플레이어 전진 수
                player.DOLocalMoveX(0.005f, 0.15f).SetLoops(4, LoopType.Yoyo); //캐릭터 흔들림

                //회오리 작아지면서 삭제, 닿는 순간 콜라이더 비활성화
                Invoke("Forward", 1f);
                gameObject.GetComponent<BoxCollider>().enabled = false;
                gameObject.transform.DOScale(0, 1f).OnComplete(() =>
                {
                    Forward();
                    Destroy(gameObject);
                });
                Debug.Log("전진"); 
            }
        }
    }
    void Forward()
    {
        SoundMGR.Instance.SoundPlay("move");

        //위아래로 움직이면 앞으로 조금 전진
        playerZ += 0.3f;
        player.DOLocalMoveY(-0.14f,0.25f).SetLoops(4, LoopType.Yoyo);
        player.DOLocalMoveZ(playerZ, 1.2f);

        if (manager.score >10)
        {
            manager.EndingLauncher();
            manager.isNotPlaying = true;
        }
    }

    void DamageSound()
    {
        if(!manager.isNotPlaying)
        {
            SoundMGR.Instance.SoundPlay("윽");
        }
    }
}
