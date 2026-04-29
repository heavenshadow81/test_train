using UnityEngine;

namespace ML.PlaywallKids.Aquarium
{
    public class cameraTest : MonoBehaviour
    {

        private Vector3 pos;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            //if (Input.GetMouseButtonDown(0))
            {
                pos = Input.mousePosition;
            }
        }

        //void OnDrawGizmosSelected() 
        void OnDrawGizmos()
        {
            Vector3 p = GetComponent<Camera>().ScreenToWorldPoint(new Vector3(pos.x, pos.y, GetComponent<Camera>().nearClipPlane + 500));
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(p, 60.0F);
            //Gizmos.DrawWireSphere(p, 60.0F);
        }
    }
}