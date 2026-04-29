using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;

public class FadeInOutParticles : MonoBehaviour
{
    private EffectSettings effectSettings;
    private ParticleSystem[] particles;
    private bool oldVisibleStat;

    private void GetEffectSettingsComponent(Transform tr)
    {
        var parent = tr.parent;
        if (parent != null)
        {
            effectSettings = parent.GetComponentInChildren<EffectSettings>();
            if (effectSettings == null)
                GetEffectSettingsComponent(parent.transform);
        }
    }

    void Start()
    {
        GetEffectSettingsComponent(transform);
        particles = effectSettings.GetComponentsInChildren<ParticleSystem>();
        oldVisibleStat = effectSettings.IsVisible;
    }

    void Update()
    {
        if (effectSettings.IsVisible != oldVisibleStat)
        {
            foreach (var particle in particles)
            {
                var emission = particle.emission; // EmissionModule을 가져옵니다
                if (effectSettings.IsVisible)
                {
                    particle.Play();
                    emission.enabled = true; // enableEmission 대신 emission.enabled 사용
                }
                else
                {
                    particle.Stop();
                    emission.enabled = false; // enableEmission 대신 emission.enabled 사용
                }
            }
        }
        oldVisibleStat = effectSettings.IsVisible;
    }
}
