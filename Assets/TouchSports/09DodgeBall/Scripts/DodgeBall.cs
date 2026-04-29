using UnityEngine;

namespace ML.T_Sports.DodgeBall
{
    /// <summary>
    /// 피구공, 점수를 측정하여 manager에 보고한다.
    /// </summary>
    public class DodgeBall : MonoBehaviour
    {
        public AudioClip successSound, failSound;
        private bool _check = false;

        public void OnCollisionEnter(Collision collision)
        {
            _Check(collision.gameObject, collision.contacts[0].point);
        }

        private void _Check(GameObject other, Vector3 hit)
        {
            DodgeBallGameManager gm = (DodgeBallGameManager)Common.ContentsManagerBase.Current;

            if (!_check)
            {
                DodgeBallPlayer player = other.GetComponent<DodgeBallPlayer>();

                if (player != null && player.alive)
                {
                    player.Die(hit);
                    gm.AddScore(1, hit);
                    _check = true;

                    AudioSource.PlayClipAtPoint(successSound, Camera.main.transform.position, Common.ContentsManagerBase.Current.GetSharedPropertyValue(Common.ContentsPropertyType.SFX));
                }
                else
                {
                    gm.AddScore(0);
                    _check = true;

                    AudioSource.PlayClipAtPoint(failSound, Camera.main.transform.position, Common.ContentsManagerBase.Current.GetSharedPropertyValue(Common.ContentsPropertyType.SFX));
                }

                // Move target
                var boid = gm.prefabs[0].GetComponent<DodgeBallBoid>();
                var target = gm.boidTarget;
                Vector2 boundingBoxOrigin = boid.boundingBoxOrigin;
                Vector2 boundingBoxSize = boid.boundingBoxSize;
                Vector3 pos = target.position;
                Vector3 move = -(hit + Vector3.down * hit.y).normalized * Random.Range(8.0f, 20.0f);
                pos += move;
                pos.x = Mathf.Clamp(pos.x, boundingBoxOrigin.x - boundingBoxSize.x / 2, boundingBoxOrigin.x + boundingBoxSize.x / 2);
                pos.z = Mathf.Clamp(pos.z, boundingBoxOrigin.y - boundingBoxSize.y / 2, boundingBoxOrigin.y + boundingBoxSize.y / 2);
                target.position = pos;
            }
        }

        public void OnEnable()
        {
            Destroy(gameObject, 5);
        }
    }
}