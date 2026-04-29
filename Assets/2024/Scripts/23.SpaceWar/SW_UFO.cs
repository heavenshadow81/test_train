using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SW_UFO : MonoBehaviour
{
    public static bool gameOver;

    [SerializeField] GameObject bomb;
    [SerializeField] GameObject fail;

    float hpValue = 1f;
    [SerializeField] Slider hp;
    [SerializeField] Image hpFill;

    private void OnTriggerEnter2D(Collider2D collision)
    {
            SoundMGR.Instance.SoundPlay("08.과일자르기");

            //폭발 이펙트 생겼다가 사라짐
            GameObject particle = Instantiate(bomb, collision.transform.position, Quaternion.identity);
            Destroy(particle, 1);
            Destroy(collision.gameObject);

            if (hp.value > 0)
            {
                if (collision.transform.tag == "Enemy")
                {
                    //hp 줄어듬
                    hpValue -= 0.1f;
                    hp.value = hpValue;

                    //남은 hp에 따라 UI색상변화
                    if (hpValue < 0.8f)
                        hpFill.color = Color.yellow;
                    if (hpValue < 0.3f)
                        hpFill.color = Color.red;


                    //유에프오 흔들리고 색 변했다가 돌아옴
                    gameObject.GetComponent<Image>().color = Color.magenta;
                    gameObject.transform.DOMoveX(0.1f, 0.1f).OnComplete(() =>
                    {
                        gameObject.transform.DOMoveX(0, 0.1f);
                        gameObject.GetComponent<Image>().color = Color.white;
                    });
                }
            }
            else
            {
                SoundMGR.Instance.SoundPlay("PlayGround_WrongAnswer");
                gameOver = true;
                fail.SetActive(true);
            }
        }
    }
