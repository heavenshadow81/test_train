using UnityEngine;

namespace ML.PlaywallKids.Aquarium
{
    /// <summary>
    /// Boid agent 터치 인터렉션 클래스
    /// </summary>
    public class BoidInteraction : MonoBehaviour
    {
        public void Update()
        {
            Camera cam = Camera.main;

            for (int i = 0; i < CustomInput.touchCount; i++)
            {
                TouchInfo t = CustomInput.GetTouch(i);

                RaycastHit hit;
                if (Physics.Raycast(cam.ScreenPointToRay(t.position), out hit))
                {
                    Boid b = hit.collider.GetComponent<Boid>();
                    if (b != null && b.type == Boid.Type.Agent)
                    {
                        StartCoroutine(b.Interaction());
                    }
                }
            }
        }
    }
}