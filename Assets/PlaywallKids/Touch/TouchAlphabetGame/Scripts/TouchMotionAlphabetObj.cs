using UnityEngine;
using System.Collections;

public class TouchMotionAlphabetObj : MonoBehaviour
{
    public GameObject particlePrefab;
    public AudioClip alphabetSound;

    public void ShootingDown()
    {
        if (particlePrefab != null)
        {
            GameObject particle = (GameObject)Instantiate(particlePrefab);
            particle.SetActive(true);
            Camera cam = GetComponent<TouchMotionSmallObject>().cam;
            particle.transform.parent = cam.transform;
            particle.transform.position = transform.position;
            particle.transform.rotation = transform.rotation;
            particle.transform.localScale = Vector3.one;

            ParticleSystem ps = particle.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                var mainModule = ps.main;  // main 모듈을 가져옵니다.
                mainModule.startSize = transform.localScale.x;  // startSize를 설정합니다.
            }
        }

        if (alphabetSound != null)
        {
            AudioSource.PlayClipAtPoint(alphabetSound, Vector3.zero);
        }
    }
}
