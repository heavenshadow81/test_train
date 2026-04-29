using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LetterSlot_PP : MonoBehaviour
{
    public bool check; //이미 정답리스트 안에 있는 행성인지 체크
    public static List<GameObject> Orange = new List<GameObject>();
    public static List<GameObject> Green = new List<GameObject>();

    public void Init()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Orange")
        {
            if (check)
            {
                //체크가 된 오브젝트가 아니고 리스트에 없으면 추가
                if (!collision.GetComponent<LetterSlot_PP>().check)
                {
                    if (!Orange.Contains(collision.transform.gameObject))
                    {
                        Orange.Add(collision.transform.gameObject);
                    }
                }
            }
        }
        else if (collision.tag == "Green")
        {
            if (check)
            {
                //체크가 된 오브젝트가 아니고 리스트에 없으면 추가
                if (!collision.GetComponent<LetterSlot_PP>().check)
                {
                    if (!Green.Contains(collision.transform.gameObject))
                        Green.Add(collision.transform.gameObject);
                }
            }
        }
    }
}

