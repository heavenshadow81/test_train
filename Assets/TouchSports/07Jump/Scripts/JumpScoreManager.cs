using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ML.T_Sports.Common;
using UnityEngine.EventSystems;
using System.Diagnostics.Contracts;
namespace ML.T_Sports.Jump
{
    public enum PlayerState
    {
        Ready,
        Play,
        End
    }

    public class JumpScoreManager : MonoBehaviour
    {
        public PlayerState state;
        public GameObject[] hightBar;
        public GameObject[] hrzBar;
        float[] yfloat = new float[4] { -450, -450, -450, -450 };

        public Button[] playerBtn;
        public GameObject[] ReadyIMG;

        public Sprite[] rankImage;
        public GameObject[] rank;

        public EFMPlayer Hit;
        public EFMPlayer Cheers;

        int rankCount;
        int[] ranks = new int[4] { 0, 0, 0, 0 };


        private void OnEnable()
        {
            for (int i = 0; i < hightBar.Length; i++)
            {
                hightBar[i].GetComponent<Image>().fillAmount = 0;
            }

            rankCount = 0;
        }

        private void Update()
        {
            if (state == PlayerState.Ready)
            {
                for (int i = 0; i < playerBtn.Length; i++)
                {
                    playerBtn[i].gameObject.SetActive(false);
                }
            }

            for (int i = 0; i < hightBar.Length; i++)
            {
                hrzBar[i].transform.localPosition = new Vector3(0, yfloat[i], 0);

                if (ranks[i] == 0)
                {
                    if (hightBar[i].GetComponent<Image>().fillAmount == 1)
                    {
                        ranks[i] = 1;
                        rank[i].SetActive(true);

                        if (rankCount < 3)
                        {
                            rank[i].transform.localScale = new Vector3(1, 1, 1);
                            rank[i].GetComponent<Image>().sprite = rankImage[rankCount];
                        }
                        else
                        {
                            rank[i].transform.localScale = new Vector3(1, 0.5f, 1);
                            rank[i].GetComponent<Image>().sprite = rankImage[rankCount];
                        }

                        if (ranks[i] == 1)
                        {
                            rankCount++;
                            Cheers.EFMRandomPlay();
                            ranks[i] = 2;
                        }
                    }
                }

            }
        }

        public void IsPlay()
        {
            state = PlayerState.Play;
            rankCount = 0;

            for (int i = 0; i < hightBar.Length; i++)
            {
                playerBtn[i].gameObject.SetActive(true);
                rank[i].SetActive(false);
                hightBar[i].GetComponent<Image>().fillAmount = 0;
                ranks[i] = 0;
                ReadyIMG[i].SetActive(false);
                yfloat[i] = -450;
            }
            Hit.EFMRandomPlay();
        }


        public void Score()
        {
            for (int i = 0; i < hightBar.Length; i++)
            {
                if (EventSystem.current.currentSelectedGameObject.name == playerBtn[i].name)
                {
                    hightBar[i].GetComponent<Image>().fillAmount += 0.01f;
                    if (yfloat[i] < 505)
                        yfloat[i] += 9.5f;
                }
            }
            Hit.EFMRandomPlay();
        }

    }
}

