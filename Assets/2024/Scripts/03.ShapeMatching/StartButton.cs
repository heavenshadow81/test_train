using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShapeMatching
{
    public class StartButton : MonoBehaviour
    {
       public void StartBtn()
        {
            SoundMGR.Instance.SoundPlay("ShapeMatching_Click");
            gameObject.SetActive(false);
        }
    }
}