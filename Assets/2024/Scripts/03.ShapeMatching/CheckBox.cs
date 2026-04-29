using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShapeMatching
{
    public class CheckBox : MonoBehaviour
    {

        public static int correct;
        public GameObject[] effect;
        public string shapeName;
        public static bool end;

        // Start is called before the first frame update
        void OnEnable()
        {
            end = false;
            correct = 0;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.name == shapeName)
            {
                SoundMGR.Instance.SoundPlay("ShapeMatching_정답");
                Instantiate(effect[0],gameObject.transform);
                correct++;
                print(correct);
                //print("정답");

                if (correct >= 27)
                {
                    end = true;
                    SoundMGR.Instance.SoundPlay("ShapeMatching_클리어");
                }
            }
            else
            {
                SoundMGR.Instance.SoundPlay("ShapeMatching_오답");
                Instantiate(effect[1],gameObject.transform);
                other.gameObject.SetActive(false);
                //print("오답");
            }
        }
    }
}