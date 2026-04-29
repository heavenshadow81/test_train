using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] Animator anim;

    public void Sword()
    {
        anim.SetTrigger("Sword");
        gameObject.transform.DOLocalMoveX(0.01f, 0.5f).SetLoops(2, LoopType.Yoyo);//¸ö Èçµé¸²
    }
    public void Shield()
    {
        anim.SetTrigger("Shield");
    }
}
