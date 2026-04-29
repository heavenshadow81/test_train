using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace ML.T_Sports.Main
{

    public class FlipBackGround : MonoBehaviour
    {
        public Image BackGround;
        public Image BackGround_flip;
        public GameObject Flips;
        public Sprite Single, Team;

        public void FlipToSingle()
        {
            BackGround_flip.sprite = Single;
            Flips.SetActive(true);
        }
        public void ChangeModeToSingle()
        {
            BackGround.sprite = Single;
            Flips.SetActive(false);
        }

        public void FlipToTeam()
        {
            BackGround_flip.sprite = Team;
            Flips.SetActive(true);
        }
        public void ChangeModeToTeam()
        {
            BackGround.sprite = Team;
            Flips.SetActive(false);
        }
    }
}
