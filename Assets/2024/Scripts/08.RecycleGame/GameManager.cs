using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RecycleGame
{
    public class GameManager : MonoBehaviour
    {
        public GameObject endPopup;
        public GameObject[] result;
        public TextMeshProUGUI countText;
        public TextMeshProUGUI[] resultText;

        // Update is called once per frame
        void Update()
        {
            countText.text = $"{RecycleButton.count}/20";

            if(RecycleButton.count>=20)
            {
                for (int i = 0; i < RecycleButton.trashType.Length; i++)
                {
                    resultText[i].text = RecycleButton.trashType[i].ToString();
                }

                if(RecycleButton.total >= 15)
                {
                    result[1].SetActive(false);
                    result[0].SetActive(true);
                }
                else if (RecycleButton.total < 15)
                {
                    result[0].SetActive(false);
                    result[1].SetActive(true);
                }

                endPopup.SetActive(true);
            }
        }

        public void Retry()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}