using UnityEngine;

namespace ML.PlaywallKids.DragonPark.Test
{
    public class NavTest : MonoBehaviour
    {
        UnityEngine.AI.NavMeshAgent Agent;
        public GameObject Target;
        public GameObject[] Coming;
        public Camera MainCam;
        public bool b_Coming = false;



        // Use this for initialization
        void Start()
        {
            Agent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();
            Agent.speed = 2.0f;

            MainCam = GameObject.Find("Main Camera").GetComponent<Camera>();
        }

        // Update is called once per frame
        void Update()
        {
            //Agent.SetDestination (Target.transform.position);

            //				Ray ray = MainCam.ScreenPointToRay (Input.mousePosition);
            //				RaycastHit hit;
            //
            //				if (Input.GetMouseButton (0)) {
            //						if (Physics.Raycast (ray, out hit)) {
            //								if (hit.collider.gameObject.tag == "Player1") {
            //										b_Coming = true;
            //								}
            //						}
            //				}

            if (!b_Coming)
            {
                notcome();
            }
            else
            {
                come();
            }
        }
        void come()
        {
            Agent.SetDestination(Coming[0].transform.position);
        }

        void notcome()
        {
            Agent.SetDestination(Target.transform.position);
        }
    }
}