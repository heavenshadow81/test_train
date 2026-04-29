using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

namespace Dumpling
{

    public class GameManager : MonoBehaviour
    {
        public GameObject[] selectStage;
        public GameObject[] buttons;

        public GameObject explain;
        public GameObject popup;
        public TextMeshProUGUI popupText;
        public TextMeshProUGUI TopUIText;

        public GameObject[] ingredients;
        public GameObject ingredBtn;
        GameObject[] ingredBtns = new GameObject[10];

        public GameObject[] dough;
        public Sprite[] bucketImage;
        public Sprite[] liquidImage;
        public Sprite[] powderImage;
        public Sprite[] doughImage;

        public GameObject[] cook;
        public GameObject[] dumplings;
        public GameObject endingEffect;
        int dumplingNumber;

        Vector3[] ingredPostion = new Vector3[2];

        int stage = 0;
        int ingredCount;

        List<string> duplingList;
        string[] failList = new string[8] { "Apple", "Banana", "Cheese", "Cookie", "CoffeeBucket", "JuiceBucket", "MilkBucket", "CokeBucket" };


        //Start is called before the first frame update

        private void Awake()
        {
            for (int i = 0; i < ingredBtn.transform.childCount; i++)
            {
                ingredBtns[i] = ingredBtn.transform.GetChild(i).gameObject;
            }
        }

        void OnEnable()
        {
            ingredBtns = new GameObject[10];
            duplingList = new List<string>();

            for (int i = 0; i < ingredBtn.transform.childCount; i++)
            {
                ingredBtns[i] = ingredBtn.transform.GetChild(i).gameObject;
            }

            ingredPostion[0] = ingredients[0].transform.position;
            ingredPostion[1] = ingredients[11].transform.position;

            RandomActiveIngred(ingredBtns); //재료 랜덤하게 섞음
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void StartBtn() //시작 버튼
        {
            SoundMGR.Instance.SoundPlay("GameStart");

            ingredients[0].SetActive(true);

            selectStage[0].SetActive(false);
            selectStage[1].SetActive(true);

            buttons[0].SetActive(false);

            stage = 1;

            explain.SetActive(true);
            Invoke("ExplainFalse", 4.5f);

            TopUIText.text = "만두속 만들기";
        }
        
        void ExplainFalse() //설명 닫기
        {
            explain.SetActive(false);
            AtiveIngredBtn();
        }

        public void StageBtn() //다음 버튼
        {
            SoundMGR.Instance.SoundPlay("rapid-wind-sound-effect");

            if (stage==1)
            {

                if (ingredCount<4)
                {
                    popup.transform.DOScale(0.8f, 0.5f).OnComplete(() =>
                    {
                        DOVirtual.DelayedCall(1, () =>
                        {
                            popup.transform.DOScale(0, 0.5f);
                        });
                    });
                }
                else
                {
                    ingredCount = 0;

                    stage = 2;

                    selectStage[2].SetActive(false);
                    selectStage[3].SetActive(true);

                    ingredients[0].transform.DOMove(ingredPostion[1], 1);
                    ingredients[0].transform.DOScale(0.5f, 1);

                    buttons[0].SetActive(true);

                    TopUIText.text = "만두피 만들기";
                }

            }
            else if(stage==2) 
            {
                if (ingredCount < 1)
                {
                    popup.transform.DOScale(0.8f, 0.5f).OnComplete(() =>
                    {
                        DOVirtual.DelayedCall(1, () =>
                        {
                            popup.transform.DOScale(0, 0.5f);
                        });
                    });
                }
                else
                {
                    ingredCount = 0;

                    stage = 3;

                    selectStage[3].SetActive(false);
                    selectStage[4].SetActive(true);
                }

            }
            else if (stage == 3)
            {
                if (ingredCount < 1)
                {
                    popup.transform.DOScale(0.8f, 0.5f).OnComplete(() =>
                    {
                        DOVirtual.DelayedCall(1, () =>
                        {
                            popup.transform.DOScale(0, 0.5f);
                        });
                    });
                }
                else
                {
                    popupText.text = "방법을\r\n선택해줘";

                    ingredCount = 0;
                    stage = 4;

                    for (int i = 0; i < dough.Length; i++)
                    {
                        dough[i].SetActive(false);
                    }

                    dough[3].SetActive(true);
                    dough[4].SetActive(true);

                    ingredients[0].transform.DOMove(ingredPostion[0], 1);
                    ingredients[0].transform.DOScale(0, 1).OnComplete(() => dough[3].transform.DOScale(0, 1));
                    ingredients[0].transform.DORotate(new Vector3(0, 0, 180), 1).OnComplete(() =>
                    {
                        SoundMGR.Instance.SoundPlay("DumplingLand");
                        dough[4].transform.DOScale(1, 1).OnComplete(() => selectStage[5].SetActive(true));
                    });

                    selectStage[4].SetActive(false);


                    TopUIText.text = "요리하기";
                }
            }
            else if (stage == 4)
            {
                if (ingredCount < 1)
                {
                    popup.transform.DOScale(0.8f, 0.5f).OnComplete(() =>
                    {
                        DOVirtual.DelayedCall(1, () =>
                        {
                            popup.transform.DOScale(0, 0.5f);
                        });
                    });
                }
                else
                {
                    stage = 5;
                    ingredCount = 0;

                    for (int i = 0; i < cook.Length; i++)
                    {
                        cook[i].SetActive(false);
                        buttons[i].SetActive(false);
                    }

                    selectStage[5].SetActive(false);
                    selectStage[6].SetActive(false);
                    selectStage[7].SetActive(true);

                    endingEffect.SetActive(true);

                    for (int i = 0; i < failList.Length; i++)
                    {
                        if (duplingList.Contains(failList[i]))
                        {
                            dumplingNumber = 2;
                            SoundMGR.Instance.SoundPlay("Fail");
                        }
                    }

                    if (dumplingNumber != 2)
                    {
                        SoundMGR.Instance.SoundPlay("yay(SuccessTemp)");

                        if (duplingList.Contains("Kimchi"))
                            dumplingNumber = 0;
                        else if (duplingList.Contains("Meat"))
                            dumplingNumber = 1;
                        else if (duplingList.Contains("SesameOil"))
                            dumplingNumber = 3;
                    }

                    cook[0].transform.parent.gameObject.SetActive(false);
                    dumplings[dumplingNumber].SetActive(true);
                    //print(dumplingNumber);
                }
            }
        }

        void RandomActiveIngred(GameObject[] ingredients) //리스트에 중복안되게 랜덤 저장
        {   
            for (int i = ingredients.Length - 1; i > 0; i--)
            {
                // 0부터 i까지의 범위에서 랜덤 인덱스 선택
                int randomIndex = UnityEngine.Random.Range(0, i + 1);

                // 배열의 i번째 요소와 랜덤 인덱스 요소를 교환
                GameObject temp = ingredients[i];
                ingredients[i] = ingredients[randomIndex];
                ingredients[randomIndex] = temp;
            }
        }

        void AtiveIngredBtn() //저장한 버튼 활성화 
        {
                for (int i = 0; i < 8; i++)
                {
                    ingredBtns[i].SetActive(true);
                }
        }

        public void PreviousBtn()
        {
            SoundMGR.Instance.SoundPlay("rapid-wind-sound-effect");

            if (stage == 2)
            {
                stage = 1;

                selectStage[2].SetActive(true);
                selectStage[3].SetActive(false);

                buttons[0].SetActive(false);
                dough[0].SetActive(false);
                ingredCount = 0;

                duplingList.Clear();

                ingredients[0].transform.DOMove(ingredPostion[0], 0.00001f);
                ingredients[0].transform.DOScale(1f, 0.00001f);

                for (int i=1; i< ingredients.Length-1; i++)
                {
                    ingredients[i].SetActive(false);
                }

                TopUIText.text = "만두속 만들기";
            }
            else if (stage == 3)
            {
                stage = 2;

                selectStage[4].SetActive(false);
                selectStage[3].SetActive(true);

                for(int i=0;i< dough.Length; i++)
                {
                    dough[i].SetActive(false);
                }

                duplingList.RemoveAt(duplingList.Count - 1);

                ingredients[0].transform.DOMove(ingredPostion[1], 1);
                ingredients[0].transform.DOScale(0.5f, 1);

                dough[0].SetActive(true);
            }
            else if (stage == 4)
            {
                ingredients[0].transform.DORotate(new Vector3(0, 0, 0), 1);

                stage = 3;

                selectStage[5].SetActive(false);
                selectStage[4].SetActive(true);
                dough[4].transform.DOScale(0, 0.00001f);
                dough[3].transform.DOScale(1, 0.00001f);
                dough[3].SetActive(false);

                dough[0].SetActive(true);
                dough[1].SetActive(true);
                dough[2].SetActive(true);

                for (int i = 0; i < cook.Length; i++)
                {
                    cook[i].SetActive(false);
                }

                TopUIText.text = "만두피 만들기";

                popupText.text = "재료를\r\n추가해줘";
            }
        }

        public void ShowIngredients(int ingredNumber)
        {
            SoundMGR.Instance.SoundPlay("SelectSound");

            if (ingredients[ingredNumber].activeSelf)
            {
                ingredients[ingredNumber].SetActive(false);
                ingredCount--;

                duplingList.Remove(ingredients[ingredNumber].name);
            }
            else
            {
                ingredients[ingredNumber].SetActive(true);
                ingredCount++;

                duplingList.Add(ingredients[ingredNumber].name);
            }
        }

        public void ShowLiquid(int liquidNumber)
        {
            SoundMGR.Instance.SoundPlay("SelectSound");
            if (dough[0].activeSelf)
            {
                dough[0].GetComponent<SpriteRenderer>().sprite = bucketImage[liquidNumber];
                dough[1].GetComponent<SpriteRenderer>().sprite = liquidImage[liquidNumber];
                duplingList.RemoveAt(duplingList.Count - 1);
                duplingList.Add(dough[0].GetComponent<SpriteRenderer>().sprite.name);
                ingredCount++;

            }
            else
            {
                dough[0].SetActive(true);
                dough[0].GetComponent<SpriteRenderer>().sprite = bucketImage[liquidNumber];
                dough[1].GetComponent<SpriteRenderer>().sprite = liquidImage[liquidNumber];
                duplingList.Add(dough[0].GetComponent<SpriteRenderer>().sprite.name);
                ingredCount++;
            }
        }

        public void ShowPowder(int powderNumber)
        {
            SoundMGR.Instance.SoundPlay("SelectSound");
            if (dough[2].activeSelf)
            {
                dough[2].GetComponent<SpriteRenderer>().sprite = powderImage[powderNumber];
                dough[3].GetComponent<SpriteRenderer>().sprite = doughImage[powderNumber];
                ingredCount++;
            }
            else
            {
                dough[1].SetActive(true);
                dough[2].SetActive(true);
                dough[2].GetComponent<SpriteRenderer>().sprite = powderImage[powderNumber];
                dough[3].GetComponent<SpriteRenderer>().sprite = doughImage[powderNumber];
                ingredCount++;
            }
        }

        public void ShowCook(int cookNumber)
        {
            SoundMGR.Instance.SoundPlay("SelectSound");

            dough[4].transform.DOScale(0, 0.5f).OnComplete(() =>
            {
                if (!cook[cookNumber].activeSelf)
                {
                    for (int i = 0; i < cook.Length; i++)
                    {
                        cook[i].SetActive(false);
                    }
                    cook[cookNumber].SetActive(true);

                    ingredCount++;
                }
            });

            if (cook[cookNumber].activeSelf)
            {
                for (int i = 0; i < cook.Length; i++)
                {
                    cook[i].SetActive(false);
                }
                cook[cookNumber].SetActive(false);
                ingredCount++;
            }
        }

        public void Retry()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
