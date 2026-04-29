using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//폭탄 스크립트
public class Bomber : MonoBehaviour
{
    #region 변수
    //폭발 이펙트
    [SerializeField]
    ParticleSystem bombEffect;
    [SerializeField]
    GameObject bombmesh;
    [SerializeField]
    int soundindex;
    public System.Action TouchAction;
    #endregion
    
    #region 함수
    //폭탄 터지기..
    IEnumerator BombEffect()
    {
        bombEffect.Play();
        Coding.ContentsController.Instance.SoundEffect(soundindex);
        yield return new WaitWhile(() => bombEffect.isPlaying);
        gameObject.SetActive(false);
        yield break;
    }
    //폭탄 메시 잠시 나중에 활성화
    IEnumerator BombShow()
    {
        yield return new WaitForSeconds(1);
        bombmesh.SetActive(true);
        yield break;
    }
    //상자 열기 함수
    IEnumerator BoxAction()
    {
        bombmesh.GetComponent<Animator>().SetBool("Open", true);
        yield return new WaitForSeconds(1);
        bombmesh.GetComponent<Animator>().SetBool("Open", false);
        yield break;
    }
    //오브젝트 비활성화
    void Disa()
    {
        gameObject.SetActive(false);
    }
    //폭발
    void Explode()
    {
        bombmesh.SetActive(false);
        bombEffect.Play();
        Coding.ContentsController.Instance.SoundEffect(soundindex);
        StartCoroutine(BombShow());
    }
    //상자 열기
    void Open()
    {
        StartCoroutine(BoxAction());
        Coding.ContentsController.Instance.SoundEffect(soundindex);
    }
    
    #endregion

    #region 유니티 함수
    private void OnEnable()
    {
        StartCoroutine(BombShow());
        Coding.ContentsController.Instance.Final += Disa;
        if(soundindex == 2)
        {
            TouchAction += Explode;
        }
        else
        {
            TouchAction += Open;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TouchAction?.Invoke();
            StartCoroutine(BombEffect());
        }
        if (TotalParameter.Instance.persons == 1 && !gameObject.tag.Contains("Costume"))
        {
            Coding.UIController.Instance.fail.gameObject.SetActive(true);
            Coding.UIController.Instance.uibackground[0].gameObject.SetActive(false);
            Coding.UIController.Instance.gamesceneUI.gameObject.SetActive(false);
            Coding.UIController.Instance.characterView[0].gameObject.SetActive(false);
            Coding.UIController.Instance.movingCharacterView[0].gameObject.SetActive(false);
        }
    }
    private void FixedUpdate()
    {
        bombmesh.transform.localScale = Vector3.one * (1 + Mathf.PingPong(Time.time, 1.5f) * 0.13f);
    }

    private void OnDisable()
    {
        Coding.ContentsController.Instance.Final -= Disa;
        TouchAction -= Open;
        TouchAction -= Explode;
    }

    private void OnMouseDown()
    {
        print("클릭 실현");
        TouchAction?.Invoke();
    }
    
    #endregion
}
