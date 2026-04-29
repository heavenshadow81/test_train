using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    /// <summary>
    /// 메모리 풀 Wrapping class,
    /// 그림자 이미지를 축구공 위치에 실시간으로 설정함
    /// </summary>
    [RequireComponent(typeof(ObjectPool))]
    public class TwoDimnesionBallManager : MonoBehaviour
    {
        public ObjectPool ballObjectPool;
        public ObjectPool shadowImgPool;
        public Vector3 shadowPos;
        public int maxNum;
        public float ballSize;

        UIImage[] shadows;

        void OnEnable()
        {
            ballObjectPool.Initialize();
            shadowImgPool.Initialize();
            ballObjectPool.CheckObjectsState();
            shadows = new UIImage[maxNum];
            for (int i = 0, len = shadows.Length; i < len; ++i)
            {
                shadows[i] = shadowImgPool.GetObejct().GetComponent<UIImage>();
            }
        }

        /// <summary>
        /// 축구공 개수 확인 및 일정 개수 유지
        /// 원래 계획은 골대에 들어간 축구공을 비활성 또는 동적으로 삭제 후 새로운 공을 생성 시키는 함수 인데 , 기획 변경으로 골대 기능을 비활성화 함
        /// </summary>
        void FixedUpdate()
        {
            if (maxNum > ballObjectPool.Count)
            {
                GameObject go = ballObjectPool.GetObejct();
                if (go == null)
                {
                    Debug.LogError("ball is null");
                    return;
                }
                BoidController.instance.SetBoid(go.GetComponent<BoidAgent>());
                go.layer = ML.PlaywallKids.Common.LayerConstants.INTERACTION_OBJECT;
                go.transform.localScale = new Vector3(ballSize, ballSize, ballSize);
                go.gameObject.SetActive(true);
                go.transform.localPosition = UtilityScript.RandomPostion(new Vector2(0.25f, 0.15f), new Vector2(0.65f, 0.55f));
            }
        }

        /// <summary>
        /// 그림자 이미지를 축구공 위치로 실시간으로 이동시킴
        /// /// </summary>
        void Update()
        {
            List<GameObject> balls = ballObjectPool.ActiveObjectsList;
            for (int i = 0, len = ballObjectPool.Count; i < len; ++i)
            {
                shadows[i].CachedTransform.localPosition = balls[i].transform.localPosition + shadowPos;
            }
        }
    }
}