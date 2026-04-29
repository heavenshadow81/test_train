using UnityEngine;
using System;

namespace ML.PlaywallKids.Aquarium
{
    using AuthoringTool;
    public class SetBrush : MonoBehaviour
    {
        protected UserData userData;
        protected int nInstanceId = 0;

        // Use this for initialization
        void Start()
        {
            nInstanceId = this.transform.parent.GetComponent<UIPanel>().GetInstanceID();

            userData = UserData.Instance();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnClick()
        {
            userData.SetBrushSize(nInstanceId, (Convert.ToInt32(this.name) * 2));
        }
    }
}