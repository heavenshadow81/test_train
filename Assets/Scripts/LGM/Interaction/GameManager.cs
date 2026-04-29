using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace LGM
{
    namespace InteractionGame
    {

        public class GameManager : Singleton<GameManager>
        {
            public GameObject flowerPrefab; // ²É ÇÁ¸®ÆÕ
            public int setCount;    // ²É ¼³Ä¡ÇÒ °¹¼ö
            public List<GameObject> flowers = new();    // ²É¿ÀºêÁ§Æ® ¹­À½ °ü¸®

            private void Awake()
            {
                
                for(int i = 0; i < setCount; i++)
                {
                    Vector2 cam = Camera.main.Size();
                    float x = Random.Range(-cam.x, cam.x);
                    float y = Random.Range(-cam.y, cam.y);
                    flowers.Add(Instantiate(flowerPrefab, new Vector2(x, y), Quaternion.Euler(0, 0, Random.Range(0, 361)), gameObject.transform));
                }
            }
        }
    }
}