using UnityEngine;
using UnityEngine.UI;
namespace ML.PlaywallKids.Aquarium
{
    public class MouseAction : MonoBehaviour
    {

        private string msg;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            msg = "   mouseclick = " + Input.GetMouseButton(0);
        }

        
    }
}