using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace RecycleGame
{
    public class HandMove : MonoBehaviour
    {
        bool buttonOn;
        public GameObject handPos;
        public GameObject[] wastebasket;
        public GameObject[] trash;
        Vector3 startPos;

        [SerializeField] Button[] buttons; //사용할 버튼들

        // Start is called before the first frame update
        void OnEnable()
        {
            buttonOn = true;
            startPos = gameObject.transform.position;
            CreateTrash();
            gameObject.transform.DOMove(handPos.transform.position, 1);
        }

        public void RecycleBtn(int trashType)
        {
            if (buttonOn)
            {
                for(int i = 0; i < buttons.Length; i++)
                {
                    buttons[i].enabled= false;
                }

                buttonOn = false;
                SoundMGR.Instance.SoundPlay("24.스윙");
                GameObject trash = gameObject.transform.GetChild(0).gameObject;
                trash.transform.DOMove(wastebasket[trashType].transform.position, 1);
                trash.transform.DORotate(new Vector3(0, 0, 180), 1);
                trash.transform.DOScale(0.2f, 1);
                gameObject.transform.DOMove(startPos, 1).OnComplete(CreateTrash);
            }
        }

        public void CreateTrash()
        {
            if (gameObject.transform.childCount == 0)
            {
                Instantiate(trash[Random.Range(0, trash.Length)], gameObject.transform);
                gameObject.transform.DOMove(handPos.transform.position, 1).OnComplete(() =>
                {
                    for (int i = 0; i < buttons.Length; i++)
                    {
                        buttons[i].enabled = true;
                    }
                    buttonOn = true;
                });
            }
        }
    }
}
