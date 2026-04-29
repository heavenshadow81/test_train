using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.Aquarium
{
    public class colorAction : MonoBehaviour
    {

        bool isSet = true;

        Color[] col;
        int nIdx = 0;

        Vector3 os;

        // Use this for initialization
        void Start()
        {

            col = new Color[] {
            new Color(1, 1, 1, 1),
            new Color(0.9f, 0.9f, 0.9f, 1),
            new Color(0.8f, 0.8f, 0.8f, 1),
            new Color(0.7f, 0.7f, 0.7f, 1),
            new Color(0.6f, 0.6f, 0.6f, 1),
            new Color(0.5f, 0.5f, 0.5f, 1),
            new Color(0.4f, 0.4f, 0.4f, 1),
            new Color(0.3f, 0.3f, 0.3f, 1),
            new Color(0.4f, 0.4f, 0.4f, 1),
            new Color(0.5f, 0.5f, 0.5f, 1),
            new Color(0.6f, 0.6f, 0.6f, 1),
            new Color(0.7f, 0.7f, 0.7f, 1),
            new Color(0.8f, 0.8f, 0.8f, 1),
            new Color(0.9f, 0.9f, 0.9f, 1)
        };

            os = transform.localScale;
        }

        // Update is called once per frame
        void Update()
        {
            if (isSet == true)
                StartCoroutine(SetColor());

        }

        IEnumerator SetColor()
        {
            isSet = false;

            yield return new WaitForSeconds(0.05f);

            isSet = true;

            gameObject.GetComponent<Renderer>().material.color = col[nIdx];

            gameObject.transform.localScale = new Vector3(os.x * col[nIdx].r, os.y * col[nIdx].g, os.z * col[nIdx].b);

            if (nIdx < 13) nIdx++;
            else nIdx = 0;
        }
    }
}