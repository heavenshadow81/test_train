using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ShapeMatching
{
    public class EndButton : MonoBehaviour
    {
       public void EndBtn()
        {
            SoundMGR.Instance.SoundPlay("ShapeMatching_Click");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}