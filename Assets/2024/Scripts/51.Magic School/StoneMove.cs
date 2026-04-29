using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneMove : MonoBehaviour
{
    Transform cube; //사이즈 안에서 움직일 큐브

    // Start is called before the first frame update
    void Start()
    {
        //생성되면 큐브 오브젝트 찾기
        cube = GameObject.FindWithTag("target").transform;
        StartCoroutine(StoneMoving());
    }

    IEnumerator StoneMoving()
    {
        Vector3 cubeSize = cube.GetComponent<MeshRenderer>().bounds.size;
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            //큐브 내부의 랜덤 위치를 목표 위치로 설정
            Vector3 targetPosition = cube.position + new Vector3(
                Random.Range(-cubeSize.x / 2, cubeSize.x / 2),
                Random.Range(-cubeSize.y / 2, cubeSize.y / 2),
                Random.Range(-cubeSize.z / 2, cubeSize.z / 2)
            );

            //DOTween을 사용하여 스무스하게 이동
            transform.DOLocalMove(targetPosition, 1.5f).SetEase(Ease.Linear);

            // 이동이 완료될 때까지 대기
            yield return new WaitForSeconds(1.5f);
        }
    }
}
