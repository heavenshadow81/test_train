using UnityEngine;
using DG.Tweening; // DoTween을 사용하기 위해 추가

public class ParticleRepeat : MonoBehaviour
{
    public GameObject[] particleSystems; // 파티클 시스템들을 퍼블릭으로 선언

    private void Start()
    {
        //// 파티클 시스템을 9초 후에 비활성화
        //DOVirtual.DelayedCall(9f, () =>
        //{
        //    for (int i = 0; i < particleSystems.Length; i++)
        //    {
        //        if (particleSystems[i] != null)
        //        {
        //            particleSystems[i].SetActive(false);
        //        }
        //    }
        //});
        Invoke("StopParticle", 4f);
    }
    void StopParticle()
    {
        for (int i = 0; i < particleSystems.Length; i++)
        {
            if (particleSystems[i] != null)
            {
                particleSystems[i].SetActive(false);
            }
        }
    }


}

    //void Start()
    //{
    //    // 모든 파티클 시스템 반복 설정 초기화
    //    foreach (var ps in particleSystems)
    //    {
    //        if (ps != null)
    //        {
    //            ps.loop = false; // Looping off
    //            ps.Stop();
    //        }
    //    }

    //    // 첫 재생 시작
    //    PlayAllParticles();
    //}

    //void PlayAllParticles()
    //{
    //    foreach (var ps in particleSystems)
    //    {
    //        if (ps != null)
    //        {
    //            ps.Play();
    //        }
    //    }
    //}

    //void Update()
    //{
    //    // 모든 파티클 시스템이 멈췄는지 확인
    //    bool allStopped = true;
    //    foreach (var ps in particleSystems)
    //    {
    //        if (ps != null && ps.isPlaying)
    //        {
    //            allStopped = false;
    //            break;
    //        }
    //    }

    //    // 모든 파티클 시스템이 멈췄을 때
    //    if (allStopped && playCount < maxPlays)
    //    {
    //        playCount++;
    //        PlayAllParticles();
    //    }
    //}

