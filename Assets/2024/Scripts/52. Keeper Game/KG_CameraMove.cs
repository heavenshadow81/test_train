using ML.MLBKids;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class KG_CameraMove : MonoBehaviour
{
    [SerializeField] GameObject fireParticle; //제단 불 파티클
    Camera cam; //메인 카메라

    [SerializeField] KG_GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        StartCoroutine(CameraMoving());
    }

    IEnumerator CameraMoving()
    {
        yield return new WaitForSeconds(2.5f); //2.5초 뒤에

        cam.DOFieldOfView(7, 2); //카메라 필오뷰 2초동안 7로 만듦

        //카메라 각도 내리고 다 내려오면 제단불 켜지면서 줌아웃
        cam.transform.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, 0), 3f).OnComplete(()=>
        {
            SoundMGR.Instance.SoundPlay("dragon");
            fireParticle.SetActive(true);
            cam.DOFieldOfView(40, 2).SetDelay(2f).OnComplete(()=> gameManager.isNotPlaying = false);
        });
    }
}