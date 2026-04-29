using UnityEngine;

namespace ML.PlaywallKids.Aquarium
{
    using AuthoringTool;
    public class SeaStoryClientAuthoringToolManager : MonoBehaviour
    {
        public GameObject prefab;
        public GameObject current;

        public int ncount = 0;

        public void Update()
        {
            if (current == null)
            {
                current = NGUITools.AddChild(gameObject, prefab);
            }
        }

        public void Setting()
        {
            current.transform.localPosition = new Vector3(0, 105, 0);
            int nInstanceID = current.GetComponent<UIPanel>().GetInstanceID();

            UserData userData = UserData.Instance();
            userData.SetInstanceID(nInstanceID, ++ncount);
            userData.SetState(nInstanceID, UserData.State.WAIT);
        }
    }
}