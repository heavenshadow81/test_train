using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

namespace ML.T_Sports.VolleyBall
{
    /// <summary>
    /// 배구공, 점수를 측정하여 manager에 보고한다.
    /// </summary>
    public class VolleyBall : MonoBehaviour
    {
        public static List<VolleyBall> list = new List<VolleyBall>();

        private bool _check = false;
        
        public void OnCollisionEnter(Collision collision)
        {
            _Check(collision.gameObject);
        }

        public void OnTriggerEnter(Collider other)
        {
            _Check(other.gameObject);
        }

        private void _Check(GameObject other)
        {
            if (VolleyBallGameManager.instance != null && VolleyBallGameManager.instance.IsPlaying)
            {
                if (!_check)
                {
                    if (other.name.Equals("Valid"))
                    {
                        VolleyBallGameManager.instance.AddScore(1);
                        _check = true;
                    }
                    else if (other.layer == LayerMask.NameToLayer("GoalKeeper"))
                    {
                        GetComponent<Rigidbody>().AddForce(Vector3.back * Random.Range(6.0f, 10.0f) + Vector3.up * 4.0f, ForceMode.VelocityChange);
                    }
                    else if (other.layer == LayerMask.NameToLayer("Ball") || other.name.Equals("VolleyballNet01"))
                    {
                        // 공끼리는 아무것도 안함.
                    }
                    else
                    {
                        VolleyBallGameManager.instance.AddScore(0);
                        _check = true;
                    }
                }
            }
        }

        public void OnEnable()
        {
            list.Add(this);
            transform.DOScale(0.05f, 0.25f).SetDelay(10.0f).OnComplete(() =>
            {
                Destroy(gameObject);
                list.Remove(this);
            });
        }

        public static void Clear()
        {
            for (int i = 0; i < list.Count; i++)
            {
                DestroyImmediate(list[i].gameObject);
            }
            list.Clear();
        }

    }
}