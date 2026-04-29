using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] boxes; //생성될 박스들
    [SerializeField] Transform spawnPosition;

    private void Start()
    {
        BoxSpawn();
        StartCoroutine(Touchable());
    }

    public void BoxSpawn()
    {
        //박스 중 랜덤하게 한가지 스폰위치에 소환
        Instantiate(boxes[Random.Range(0,boxes.Length)],spawnPosition);
        //사운드 1초 후에 재생
        Invoke("boxSound", 1f);
    }
    IEnumerator Touchable()
    {
        //1초 뒤에 터치가능
        yield return new WaitForSeconds(1.5f);
        BG_ClickHammer.touchOn = true;
    }

    void boxSound()
    {
        SoundMGR.Instance.SoundPlay("kick");
        CancelInvoke("boxSound");
    }
}
