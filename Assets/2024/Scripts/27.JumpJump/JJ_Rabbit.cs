using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Diagnostics.Tracing;

public class JJ_Rabbit : MonoBehaviour
{
    [SerializeField] GameObject victory;
    [SerializeField] GameObject victoryParticle;
    [SerializeField] GameObject effect;

    Animator animator;
    BoxCollider2D boxCollider;
    Rigidbody2D rd;
    bool isJumping;


    // Start is called before the first frame update
    void OnEnable()
    {
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        rd = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    public void Jump()
    {
        //점프중이 아니라면
        if (!isJumping)
        {
            SoundMGR.Instance.SoundPlay("24.다음가지로이동");

            //점프 중 체크
            isJumping = true;

            //점프 애니메이션 실행하고 콜라이더 비활성화
            animator.SetTrigger("Jump");
            boxCollider.enabled = false;

            //포지션x값과 로테이션z값 고정
            rd.constraints = RigidbodyConstraints2D.FreezePositionX;
            rd.constraints = RigidbodyConstraints2D.FreezeRotation;

            //현재 포지션값에서 550만큼 위로 올라감
            float currentY = gameObject.transform.position.y;
            gameObject.transform.DOMoveY(currentY + 450, 1).OnComplete(() =>
            {
                Ground();
            });

            //버튼의 위치 지정
            Vector3 buttonPos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y-30f, gameObject.transform.position.z);

             //파티클 생성 후 삭제
            GameObject particle = Instantiate(effect, buttonPos, Quaternion.identity);
            Destroy(particle, 1f);
        }
    }

    public void Ground()
    {
        //포지션 x값 고정 해제 콜라이더 활성화
        rd.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
        boxCollider.enabled = true;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        //밟고 있는 오브젝트의 물체가 그라운드라면 점프 할 수 있게
        if (collision.transform.tag == "Ground")
        {
            isJumping = false;
        }

        if (collision.transform.tag == "Finish")
        {
            collision.transform.tag = "Untagged";
            SoundMGR.Instance.SoundPlay("PlayGround_Victory");
            victory.SetActive(true);
            victoryParticle.SetActive(true);
        }
    }
}
