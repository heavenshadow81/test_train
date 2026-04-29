using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace RecycleGame
{
    public class RecycleButton : MonoBehaviour
    {
        public GameObject[] check;
        public int trashCheck;
        public static int total;
        public static int count;
        public static int[] trashType = new int[4];
        string[] trashName = new string[4] { "plastic", "can", "glass", "paper" };
        string[] soundName = new string[4] { "플라스틱 소리", "캔 소리", "유리 소리", "종이 소리" };

        // Start is called before the first frame update
        void OnEnable()
        {
            for (int i = 0; i < trashType.Length; i++) //쓰레기 분류 초기화
            {
                trashType[i] = 0;
            }

            count = 0;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            count++;

            for (int i = 0; i < trashType.Length; i++)
            {
                if (collision.transform.name.Contains(trashName[i]))
                    SoundMGR.Instance.SoundPlay(soundName[i]);

                if (trashCheck == i)
                {
                    if (collision.transform.name.Contains(trashName[i])) //쓰레기 이름이 포함되어 있다면
                    {
                        trashType[i]++;
                        Destroy(collision.gameObject);
                        total++;
                        //print("성공");
                        SoundMGR.Instance.SoundPlay("18.정답");
                        check[0].transform.DOScale(1, 1f).OnComplete(() => check[0].transform.DOScale(0, 1f));
                    }
                    else
                    {
                        SoundMGR.Instance.SoundPlay("18.틀림");
                        check[1].transform.DOScale(1, 1f).OnComplete(() => check[1].transform.DOScale(0, 1f));
                    }
                }
                else
                {
                    Destroy(collision.gameObject);
                    //print(trashName[i]);
                    //print("실패");
                }
                    
                    //print(trashType[i]);
            }
        }
    }
}