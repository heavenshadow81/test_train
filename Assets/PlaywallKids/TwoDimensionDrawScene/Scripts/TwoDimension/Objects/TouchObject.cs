using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public class TouchObject : MonoBehaviour
    {
        public Animator ani;

        public void Touch()
        {
            ani.SetTrigger("Touch");
        }
    }
}