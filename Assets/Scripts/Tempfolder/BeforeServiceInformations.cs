using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeforeServiceInformations : MonoBehaviour
{
    [SerializeField]
    GameObject button;
    
    private void OnEnable()
    {
        StartCoroutine(Enactive());
    }

    IEnumerator Enactive()
    {
        yield return new WaitForSeconds(3);
        button.SetActive(true);
        gameObject.SetActive(false);
        yield break;
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
