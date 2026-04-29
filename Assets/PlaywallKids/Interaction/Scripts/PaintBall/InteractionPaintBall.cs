using UnityEngine;

namespace ML.PlaywallKids.Interaction
{
    public class InteractionPaintBall : MonoBehaviour
    {

        bool bLive = false;

        new Renderer renderer;
        new Rigidbody rigidbody;

        public delegate void ParticleActiveDelegate(InteractionPaintBall paintBall);
        ParticleActiveDelegate ParticleActive;



        void OnEnable()
        {
            if (renderer == null)
            {
                renderer = GetComponent<Renderer>();
                // Material Clone
                renderer.material = renderer.material;
            }
            if (rigidbody == null)
                rigidbody = gameObject.GetComponent<Rigidbody>();
        }

        // Update is called once per frame

        public void UnUsed()
        {
            bLive = false;
            gameObject.SetActive(false);
        }

        public bool IsUseable()
        {
            return bLive == false;
        }

        public void Shoot(ParticleActiveDelegate particleDelegate, Vector3 startPosition, Vector3 viewport, Vector3 left, Vector3 right)
        {
            _Init(startPosition);

            ParticleActive = particleDelegate;

            float x = Mathf.Lerp(left.x, right.x, viewport.x);
            Vector3 target = new Vector3(x, left.y, left.z);
            Vector3 dir = target - startPosition;
            dir += (Vector3.right * Random.Range(-0.05f, 0.05f) + Vector3.up * Random.Range(0.25f, 0.6f) * viewport.y + Vector3.forward * Random.Range(0.2f, 0.5f)) * 35f;
            dir = dir.normalized;
            rigidbody.AddForce((dir) * 100f, ForceMode.VelocityChange);
            //rigidbody.AddForce( ray.direction );
            //Ray ray = targetCamera.ScreenPointToRay(Input.mousePosition);
            //rigidbody.AddForce(ray.direction * 55f, ForceMode.VelocityChange);

            /*
            Vector3 dir = new Vector3(Mathf.Lerp(-0.75f, 0.75f, viewport.x ), 0, 0);
            dir += Vector3.right * Random.Range(-0.05f, 0.05f) + Vector3.up * Random.Range(0.2f, 0.5f) * viewport.y + Vector3.forward * Random.Range(0.2f, 0.5f);

            rigidbody.AddForce((Vector3.forward + dir) * 75f, ForceMode.VelocityChange);        
            rigidbody.AddTorque(new Vector3(Random.Range(-20f, 20f), Random.Range(-20f, 20f), Random.Range(-20f, 20f)) * 50);
            */
        }

        public void Shoot(ParticleActiveDelegate particleDelegate, Vector3 startPosition, Vector3 viewport, Vector3 left, Vector3 right, Vector3 navigate, float power)
        {
            _Init(startPosition);

            ParticleActive = particleDelegate;

            float x = Mathf.Lerp(left.x, right.x, viewport.x);
            Vector3 target = new Vector3(x, left.y, left.z);
            Vector3 dir = target - startPosition;

            dir = navigate;
            dir = dir.normalized;
            rigidbody.AddForce((dir) * (10f + (250f * power)), ForceMode.VelocityChange);

        }

        public Color GetColor()
        {
            return renderer.sharedMaterial.color;
        }

        void _Init(Vector3 startPosition)
        {
            ParticleActive = null;

            bLive = true;
            gameObject.SetActive(true);

            transform.position = startPosition;
            transform.localRotation = Quaternion.identity;

            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;

            renderer.material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("PaintBall_Wall"))
            {
                if (ParticleActive != null)
                    ParticleActive(this);
                UnUsed();
            }
        }
    }
}