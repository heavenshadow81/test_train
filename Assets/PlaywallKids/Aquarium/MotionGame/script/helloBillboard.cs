using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.Aquarium
{
    public class helloBillboard : MonoBehaviour
    {

        Vector3 OS;
        Quaternion OQ;

        int nIdx = 0;
        Vector3[] vSet;

        bool isSet = true;

        // Use this for initialization
        void Start()
        {

            GameObject org = GameObject.Find("planeHello");

            OQ = org.transform.rotation;
            OS = transform.localScale;

            vSet = new Vector3[] {
            new Vector3(1,1,1),
            new Vector3(1.1f, 1.1f, 1.1f),
            new Vector3(1.2f, 1.2f, 1.2f),
            new Vector3(1.3f, 1.3f, 1.3f),
            new Vector3(1.2f, 1.2f, 1.2f),
            new Vector3(1.1f, 1.1f, 1.1f)
        };
        }

        // Update is called once per frame
        void Update()
        {
            transform.rotation = OQ;

            if (isSet == true)
                StartCoroutine(SetScale());
        }

        IEnumerator SetScale()
        {
            isSet = false;

            yield return new WaitForSeconds(0.05f);

            isSet = true;

            gameObject.transform.localScale = new Vector3(OS.x * vSet[nIdx].x, OS.y * vSet[nIdx].y, OS.z * vSet[nIdx].z);

            if (nIdx < 5) nIdx++;
            else nIdx = 0;
        }
    }
}