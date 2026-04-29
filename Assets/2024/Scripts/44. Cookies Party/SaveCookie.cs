using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CookiesParty
{
    public class SaveCookie : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;

        private Material touchedCookieMat;

        public void SetCookieMat(Material newMat)
        {
            touchedCookieMat = newMat;
        }

        public Material GetCookieMat()
        {
            return touchedCookieMat;
        }
    }
}

