using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace LGM
{
    namespace OXPlaneGame
    {
        public class LifeManager : Singleton<LifeManager>
        {
            public List<GameObject> life;   // 목숨 오브젝트
            public AnimationCurve shake;

            public int Count
            {
                get { return life.Count; }
            }
            public void LifeDelete(int count = 1)
            {
                for(int i = 0; i < count; i++)
                {
                    LifeEvent(life[life.Count - 1]);
                    Destroy(life[life.Count - 1], 5f);
                    life.RemoveAt(life.Count - 1);
                }
                if (Count <= 0)
                {
                    GameManager.Instance.GameOver();
                }
            }
            public void LifeEvent(GameObject _obj)  // 목숨 감소 시 사라지는 애니메이션
            {
                Sequence sequence = DOTween.Sequence().SetAutoKill(true).Pause().
                Append(_obj.transform.DOShakePosition(1f, Vector3.left * 0.1f).SetEase(shake)).
                Append(_obj.transform.DOLocalMoveY(transform.position.y + 3, 2f)).
                Join(_obj.transform.DOScale(0.85f, 2f));
                sequence.Restart();
            }
        }
    }
}