using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CookiesParty
{
    public class BasketCookie : MonoBehaviour
    {
        private Rigidbody rb;
        private MeshRenderer render;

        private float forceAmount = 100f; // 가할 힘의 크기

        private SaveCookie saveCookie;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            render = GetComponent<MeshRenderer>();
            saveCookie = FindObjectOfType<SaveCookie>();
        }
        private void OnEnable()
        {
            Setup(saveCookie.GetCookieMat());

            // 아래 방향으로 힘을 가합니다.
            if (rb != null)
            {
                rb.AddForce(Vector3.down * forceAmount, ForceMode.Impulse);
            }
        }

        private void OnDisable()
        {
            ObjectPooler.ReturnToPool(gameObject);    // 한 객체에 한번만
            CancelInvoke();    // Monobehaviour에 Invoke가 있다면
        }

        public void Setup(Material newMat)
        {
            render.material = newMat;
        }
    }
}


