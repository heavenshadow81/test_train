using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_1 : MonoBehaviour
{
    GameObject player;

    void Start()
    {
        player = transform.gameObject;

        StartCoroutine(IPlayerMoving());
    }

    void Update()
    {
        
    }

    IEnumerator IPlayerMoving()
    {
        yield return new WaitForSeconds(1.5f);
        // 앞으로
        player.transform.Translate(new Vector3(0.0f, 0.0f, -20.0f));
        yield return new WaitForSeconds(1.5f);
        // 뒤로
        player.transform.Translate(new Vector3(0.0f, 0.0f, 20.0f));
        yield return new WaitForSeconds(1.5f);
        // 왼쪽
        player.transform.Translate(new Vector3(20.0f, 0.0f, 0.0f));
        yield return new WaitForSeconds(1.5f);
        // 오른쪽
        player.transform.Translate(new Vector3(-20.0f, 0.0f, 0.0f));


    }
}
