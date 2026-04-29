using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ML.T_Sports.BaseBall
{
    public class FloatingCloud : MonoBehaviour
    {
        public GameObject[] CloudsObject;
        public Transform CloudOP;
        public int CloudCount;
        public float start;
        void Awake()
        {
            StartCoroutine(CloudCreat());
        }

        IEnumerator CloudCreat()
        {
            for (int i = 0; i < CloudCount; i++)
            {
                int rand = Random.Range(0, CloudsObject.Length);
                Transform cloud = Instantiate(CloudsObject[rand], new Vector3(start, 45f, Random.Range(50, 180)), CloudsObject[rand].transform.rotation).transform;
                cloud.parent = CloudOP;
                cloud.name = "Cloud" + i.ToString();
                Cloud cld = cloud.GetComponent<Cloud>();
                cld.StartX = start;
                cld.end = start - 190;
                yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
            }
        }
    }
}
