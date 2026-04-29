using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAction : MonoBehaviour
{
    Animator ani;

    [Tooltip("animator의 총 animation 개수")]
    public int animationCount;

    // 공격 가능여부
    public bool isAttack;

    void Start()
    {
        // animator 들고오기
        ani = GetComponent<Animator>();

        // animator 안에 존재하는 animation 개수를 들고 온다.
        animationCount = ani.runtimeAnimatorController.animationClips.Length;

        // 시작시 몬스터가 카메라를 바라보도록 rotation값 변경
        transform.Rotate(new Vector3(0.0f, -180.0f, 0.0f));

        MonsterPlayAnimation();
    }

    public void MonsterPlayAnimation()
    {
        if(gameObject.activeSelf)
            StartCoroutine(MonsterAnimation());
    }

    // 몬스터가 플레이어를 마주쳤을때 발생하는 애니메이션
    IEnumerator MonsterAnimation()
    {
        yield return new WaitForSeconds(0.1f);

        // 공격 상태가 아닐때
        if (!isAttack)
        {
            ani.SetInteger("animations", 0);
        }

        float ranTime = Random.Range(2.0f, 4.0f);

        yield return new WaitForSeconds(ranTime);

        MonsterPlayAnimation();
    }

    // 몬스터 터치시 반응 애니매이션
    IEnumerator MonsterTouchAnimation()
    {
        yield return new WaitForSeconds(0.1f);

        int ranNum = Random.Range(1, animationCount);

        // 공격 상태가 아닐때
        if (!isAttack)
        {
            ani.SetInteger("animations", ranNum);
        }

        // 현재 실행중인 애니메이션 클립 길이를 들고 온다.
        RuntimeAnimatorController rac = ani.runtimeAnimatorController;

        float playTime = rac.animationClips[ranNum].length;

        // 클립 길이에 따라 지연 시간 할당
        yield return new WaitForSeconds(playTime);

        // 다시 Idle 상태로 변경
        ani.SetInteger("animations", 0);
    }


    // 무기를 들고 있는 캐릭터에게 몬스터가 죽을때 이벤트 메소드
    public void MonsterDie(bool attack, GameObject player)
    {
        // 몬스터가 플에이어를 바라보도록
        transform.LookAt(player.transform);

        // 공격 가능 여부를 받아온다
        isAttack = attack;

        // 공격 가능 여부에 따라 애니메이션 실행
        if (isAttack)
        {
            ani.SetInteger("animations", 2);

            print("몬스터 죽음");
        }
    }

    // 무기가 없는 캐릭터를 공격 할 때 이벤트 메소드
    public void CharacterDie(bool attack, GameObject player)
    {
        GetComponent<BoxCollider>().enabled = true;

        // 몬스터가 플레이어를 바라보게 한다.
        transform.LookAt(player.transform);

        // 공격 가능 여부를 받아온다
        isAttack = attack;

        if (isAttack)
        {
            // 공격 가능 여부에 따라 애니메이션 실행
            ani.SetInteger("animations", 1);

            print("몬스터 공격");
        }
    }

    public void MonsterTouch()
    {
        StartCoroutine(MonsterTouchAnimation());
    }

}
