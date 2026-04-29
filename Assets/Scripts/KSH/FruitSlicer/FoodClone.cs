using Bax.P0.Client.UnityWorld.SlicerGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// 과일이 쪼개지면 생성
/// </summary>
public class FoodClone : MonoBehaviour
{
    //쪼개진 이미지 로드용 스프라이트랜더러
    public SpriteRenderer spriteRenderer;
    //밑으로 떨어지게 만들용
    public Rigidbody2D rid2D;
    
    //제거
    public void Release()
    {
        Destroy(gameObject);
    }


    public void Update()
    {
        if (SlicerMgr.instance.stateClass.state == GameState.GamePlay && gameObject.activeSelf)
        {
            //쪼개질 경우 밑으로만 떨어지니까 y값이 50 아래로 떨어질경우 제거
            if ((transform.position.y <= -50f))
            {
                Release();
            }
        }
    }
}
