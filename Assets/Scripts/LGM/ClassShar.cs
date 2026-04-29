using LGM.OXPlaneGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace LGM
{
    public class GameView
    {
        public float width;
        public float height;

        public void InitSize()
        {
            width = Camera.main.orthographicSize;
            height = width * Camera.main.aspect;
        }
    }
    namespace OXPlaneGame
    {
        [Serializable]
        public class Line
        {
            public List<PlaneEvent> x;

            public void LineActive(bool active)
            {
                for(int i = 0; i < x.Count; i++)
     
                {
                    x[i].gameObject.Parent().SetActive(active);
                }
            }
            // 오브젝트로 선택 삭제
            public void Clear(PlaneEvent plane)
            {
                GameObject obj = plane.obj;
                x.Remove(plane);
                MonoBehaviour.Destroy(obj);
                MonoBehaviour.Destroy(plane);
            }
            // 인덱스 선택 삭제
            public void Clear(int index)
            {
                MonoBehaviour.Destroy(x[index].obj);
                MonoBehaviour.Destroy(x[index]);
                x.RemoveAt(index);
            }
            // 전부 삭제
            public void ClearAll()
            {
                int count = x.Count;
                for (int i = 0; i < count; i++)
                {
                    Clear(0);
                }
            }
        }
    }

    namespace AnimalMatch
    {
        public class PairCard
        {
            public CardType type;   // 카드 종류
            public GameObject A;    // 상체 카드 오브젝트
            public GameObject B;    // 하체 카드 오브젝트

            public PairCard(CardType _tpye, GameObject _a, GameObject _b)
            {
                type = _tpye;
                A = _a;
                B = _b;
            }
        }
    }
}
