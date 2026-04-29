using UnityEngine;

namespace ML.PlaywallKids.Aquarium
{
    /// <summary>
    /// 아쿠아리움 posFood.posObj에 지정한 먹이 중 택일하는 클래스.
    /// </summary>
    public class SelectFood : MonoBehaviour
    {
        public Transform posObj;

        public void OnEnable()
        {
            int idx = Random.Range(0, posObj.childCount);
            for (int i = 0; i < posObj.childCount; i++)
            {
                posObj.GetChild(i).gameObject.SetActive(i == idx);
            }
        }
    }
}