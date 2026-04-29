using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//키넥트 오류를 방지하기 위한 기다림...
public class WaitforKinect : MonoBehaviour
{
    [SerializeField]
    GameObject SelectButtons;
    [SerializeField]
    UnityEngine.UI.Image waitKinect, kinim;
    [SerializeField]
    UnityEngine.UI.Text waitKine;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitForKinectActivation());
    }

    IEnumerator WaitForKinectActivation()
    {
        waitKinect.CrossFadeAlpha(0, 3, false);
        kinim.CrossFadeAlpha(0, 3, false);
        waitKine.CrossFadeAlpha(0, 3, false);
        yield return new WaitForSeconds(3);
        waitKinect.gameObject.SetActive(false);
        SelectButtons.SetActive(true);
        yield break;
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
