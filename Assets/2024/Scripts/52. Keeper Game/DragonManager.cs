using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonManager : MonoBehaviour
{
    [SerializeField] GameObject[] particles;
    [SerializeField] Transform player;
    Animator anim;

    [SerializeField] KG_GameManager gameManager;
    

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        StartCoroutine(PerformAction());
        //SoundMGR.Instance.SoundPlay("fly");
    }

    IEnumerator PerformAction()
    {
        yield return new WaitForSeconds(9);

        while (gameManager.score<=10)
        {
            RandomAttack();

            yield return new WaitForSeconds(7);
        }
    }

    void RandomAttack()
    {
        int attackType = Random.Range(0, 2); //0: 불 공격, 1:토네이도 공격
        if(attackType == 0)
        {
            anim.SetTrigger("FireBall");

            Invoke("FireBall", 1f);
        }
        else
        {
            anim.SetTrigger("Tornado");

            Invoke("Tornado", 1f);
        }
    }
    
    void FireBall()
    {
        SoundMGR.Instance.SoundPlay("fireball");
        GameObject particle = Instantiate(particles[0], gameObject.transform.position, Quaternion.identity);
        particle.transform.DOMove(player.position, 2f);
        CancelInvoke("FireBall");
    }

    void Tornado()
    {
        SoundMGR.Instance.SoundPlay("wind");
        GameObject particle = Instantiate(particles[1], gameObject.transform.position, Quaternion.identity);
        particle.transform.DOMove(player.position, 2f);
        CancelInvoke("Tornado");
    }
}
