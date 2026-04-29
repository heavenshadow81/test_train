using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace ML.SportsMiniGame.KinectSkating
{
    public class QuizZoneGUI : MonoBehaviour
    {
        public GUIManager guimanager;
        public Text QuizString;
        public Text QuizBoxString;

        public Image QuizImage;
        public Image QuizImage_Cent;
        public GameObject[] QuizTexts;

        public Text Select1;
        public Text Select2;
        public GameObject[] QuizImages;

        public Image SelectImage1;
        public Image SelectImage2;
        public Text QuizText;

        public Text QuizText_Cent;
        public GameObject QuizBoxImage;
        public SelectedAnserCheck[] selectedAble;

        public ExplosionIce expL, expR;
        public Image Holographic_Select1;
        public Image Holographic_Select2;

        public Text Holographic_Text1;
        public Text Holographic_Text2;
        public Animation HolographicsAnimation;

        public string QuizSession1;
        public string QuizSession2;
        public JsonHelper MyJsonData;

        private void Awake()
        {
            InitValues();
        }
        private void OnEnable()
        {
            InitValues();
        }
        public void InitValues()
        {
            if (MyJsonData.QuizLevel == 1)
            {
                QuizSession1 = "이 국기의 나라를 맞춰보세요";
                QuizSession2 = "이 나라의 국기를 맞춰보세요";
            }
            else if (MyJsonData.QuizLevel == 2)
            {
                QuizSession1 = "이 전통의상의 국가를 맞춰보세요";
                QuizSession2 = "이 국가의 전통의상를 맞춰보세요";
            }
            else if (MyJsonData.QuizLevel == 3)
            {
                QuizSession1 = "이 랜드마크의 이름을 맞춰보세요";
                QuizSession2 = "이 랜드마크의 사진을 맞춰보세요";
            }
        }
        public void QuizZoneSet(Quiz quiz)
        {
            string tmp = quiz.ImageName;
            string[] path = tmp.Split('#');
            string[] selects = path[1].Split('&');
            if (quiz.QuizType == 0)
            {
                QuizString.text = QuizSession1;
                QuizBoxString.text = QuizSession1;
                QuizBoxImage.SetActive(true);
                QuizImages[0].SetActive(false);
                QuizImages[1].SetActive(false);
                QuizTexts[0].SetActive(true);
                QuizTexts[1].SetActive(true);

                string totalPath = "";
                if (quiz.Answer == 1)
                {
                    totalPath = string.Format("QuizImage/" + path[0] + "/" + selects[0]);
                }
                else
                {
                    totalPath = string.Format("QuizImage/" + path[0] + "/" + selects[1]);
                }
                //Debug.Log("totalPath : QuizImage/" + path[0] + "/" + selects[0]);
                //Debug.Log("Load Path : "+totalPath);
                Sprite quizspt = Resources.Load<Sprite>(totalPath);
                QuizImage.sprite = quizspt;
                QuizImage_Cent.sprite = quizspt;
                Select1.text = quiz.Select1;
                Holographic_Text1.text = quiz.Select1;
                Select2.text = quiz.Select2;
                Holographic_Text2.text = quiz.Select2;
                selectedAble[0].touchAble = true;
                selectedAble[1].touchAble = true;

                if (this.gameObject.name == "QuizZone1")
                    HolographicsAnimation.Play("Holographic_Text1");
                else
                    HolographicsAnimation.Play("Holographic_Text");
            }
            else
            {
                QuizString.text = QuizSession2;
                QuizBoxString.text = QuizSession2;
                QuizBoxImage.SetActive(false);
                QuizTexts[0].SetActive(false);
                QuizTexts[1].SetActive(false);
                QuizImages[0].SetActive(true);
                QuizImages[1].SetActive(true);

                string totalPath1 = string.Format("QuizImage/" + path[0] + "/" + selects[0]);
                string totalPath2 = string.Format("QuizImage/" + path[0] + "/" + selects[1]);
                Sprite quizspt1 = Resources.Load<Sprite>(totalPath1);
                Sprite quizspt2 = Resources.Load<Sprite>(totalPath2);

                SelectImage1.sprite = quizspt1;
                Holographic_Select1.sprite = quizspt1;
                SelectImage2.sprite = quizspt2;
                Holographic_Select2.sprite = quizspt2;
                selectedAble[0].touchAble = true;
                selectedAble[1].touchAble = true;

                if (quiz.Answer == 1)
                {
                    QuizText.text = string.Format("<"+quiz.Select1+">");
                    QuizText_Cent.text = QuizText.text;
                }
                else
                {
                    QuizText.text = string.Format("<" + quiz.Select2 + ">");
                    QuizText_Cent.text = QuizText.text;
                }

                if(this.gameObject.name == "QuizZone1")
                    HolographicsAnimation.Play("Holographic_Image1");
                else
                    HolographicsAnimation.Play("Holographic_Image");
            }
            expL.ResetObj();
            expR.ResetObj();
        }
        
    }
}

