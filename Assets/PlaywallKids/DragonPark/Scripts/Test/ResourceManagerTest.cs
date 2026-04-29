using UnityEngine;

namespace ML.PlaywallKids.DragonPark.Test
{
    public class ResourceManagerTest : MonoBehaviour
    {
        public GameObject go;
        public Camera anotherCamera;
        // Use this for initialization
        void Start()
        {
            Template3D template = go.AddComponent<Template3D>();
            template.identifier = "20140321";


            Vector3 originalPos = template.transform.position;

            Vector3 viewportPos = GetComponent<Camera>().WorldToViewportPoint(originalPos);

            Vector3 newPos = anotherCamera.ViewportToWorldPoint(viewportPos);

            Debug.Log(newPos);

            template.transform.position = newPos;
            template.transform.rotation = anotherCamera.transform.rotation;
            template.transform.Rotate(template.transform.up, 180.0f);

            /*
            Vector3 v1 = camera.transform.position - originalPos;
            Vector3 v2 = anotherCamera.transform.position - newPos;
            v1.y = v2.y = 0;

            Vector3 cross = Vector3.Cross(v1, v2);
            float angle = Vector3.Angle(v1, v2);

            Debug.Log (v1 + ", " + v2);
            Debug.Log (cross);

            //template.transform.Rotate(cross, angle);
            //template.transform.rotation *= Quaternion.Inverse(camera.transform.rotation);
            //template.transform.rotation *= Quaternion.Inverse(anotherCamera.transform.rotation);
            //template.transform.Rotate(Vector3.up, angle);
            */
            /*
            Matrix4x4 p1 = camera.projectionMatrix * camera.worldToCameraMatrix;
            Matrix4x4 p2 = anotherCamera.cameraToWorldMatrix * anotherCamera.projectionMatrix.inverse;

            Debug.Log ("p1 : " + p1);
            Debug.Log ("p2 : " + p2);
            Vector3 originalPos = template.transform.position;
            yield return new WaitForSeconds(2.0f);

            originalPos = camera.worldToCameraMatrix * originalPos;
            template.transform.position = originalPos;
            yield return new WaitForSeconds(2.0f);

            originalPos = camera.projectionMatrix * originalPos;
            template.transform.position = originalPos;
            yield return new WaitForSeconds(2.0f);

            originalPos = anotherCamera.projectionMatrix.inverse * originalPos;
            template.transform.position = originalPos;
            yield return new WaitForSeconds(2.0f);

            originalPos = anotherCamera.cameraToWorldMatrix * originalPos;
            template.transform.position = originalPos;
    */
            GetComponent<Camera>().enabled = false;
        }
    }
}