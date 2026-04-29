using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;


public class KG_GameManager : TouchManager_3DTouch
{
    [SerializeField] Animator dragonAnim; //드래곤 애니메이션
    [SerializeField] Animator playerAnim; //플레이어 애니메이션

    [SerializeField] Transform swordParticle; //파티클 생성될 위치

    [SerializeField] Transform sword; //소드 오브젝트 트랜스폼
    [SerializeField] Transform shield; //쉴드 오브젝트 트랜스폼

    [SerializeField] MagicLife heart; //라이프 스크립트
    [SerializeField] PlayerAnimation anim; //플레이어 스크립트

    public bool isNotPlaying; //게임 진행중인지 체크
    public int score; //게임 스코어

    private void OnEnable()
    { 
        heart.OnTimerEnd += EndingAnimation;
        isNotPlaying = true;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        heart.OnTimerEnd -= EndingAnimation;
    }

    public override void HandleInput(Vector2 pos)
    {
        isTouchable = true;

        // 마우스 또는 터치 입력을 사용하여 Ray 생성
        Ray ray = Camera.main.ScreenPointToRay(pos);
        RaycastHit hit;

        // Raycast로 오브젝트 감지
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null & !isNotPlaying) //콜라이더가 존재하고 게임이 실행중일때
            {
                if (hit.collider.tag == "Enemy") //레이에 맞은 콜라이더의 태그가 Enemy라면
                {
                    anim.Sword();
                    SoundMGR.Instance.SoundPlay("sword");
                }
                else if (hit.collider.tag == "Keeper") //레이에 맞은 콜라이더의 태그가 Keeper라면
                {
                    anim.Shield();
                    SoundMGR.Instance.SoundPlay("shield");
                }
            }
            else
            {
                isTouchable = true;
                //Debug.Log("콜라이더 없음");
            }
        }
        else
        {
            isTouchable = true;
        }
    }

    public void SwordParticle()
    {
        //칼에 파티클 생성 후 2초뒤 삭제
        GameObject particle = Instantiate(effect[0], swordParticle);
        Destroy(particle, 2f);
    }

    public void EndingLauncher()
    {
        Debug.Log("나야, 엔딩");
        StartCoroutine(Ending());
    }

    public void LifeDelete()
    {
        heart.LifeDelete(); //하트 감소
    }

    public void EndingAnimation()
    {
        if (!isNotPlaying)
        {
            //중력으로 창과 방패 떨어트림
            sword.GetComponent<Rigidbody>().useGravity = true;
            shield.GetComponent<Rigidbody>().useGravity = true;

            //플레이어 쓰러짐
            Transform player = playerAnim.gameObject.transform;
            player.DOLocalRotateQuaternion(new Quaternion(0, 0, 90, 0), 3f).OnComplete(() => gameCanvas.SetActive(true));
            SoundMGR.Instance.SoundPlay("fail");

            isNotPlaying = true;
        }
    }

    IEnumerator Ending()
    {
        //엔딩 애니메이션 재생 후
        yield return new WaitForSeconds(1f);
        playerAnim.SetTrigger("Sword");
        SoundMGR.Instance.SoundPlay("sword");

        yield return new WaitForSeconds(1f);
        dragonAnim.SetTrigger("Death");
        SoundMGR.Instance.SoundPlay("down");

        yield return new WaitForSeconds(3f);

        victoryUI.SetActive(true); //3초 뒤 빅토리UI 활성화
    }
}
