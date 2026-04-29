using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.DragonPark
{
    /// <summary>
    /// Simple Weather information class.
    /// This class is temporary (for fast implement). This will be replaced with advanced class soon!
    /// </summary>
    public class SimpleWeatherInfo : MonoBehaviour
    {
        public Color fogColor;
        public FogMode fogMode;
        public float density;
        public float linearStart;
        public float linearEnd;
        public Color ambientLight;
        public Color directionalLight;
        public float directionalLightIntensity;
        public Color Ev_Light;
        public float Ev_LightIntensity;
        public Color backgroundColor;

        public static SimpleWeatherInfo Lerp(SimpleWeatherInfo from, SimpleWeatherInfo to, float t)
        {
            SimpleWeatherInfo info = new SimpleWeatherInfo();
            info.fogColor = Color.Lerp(from.fogColor, to.fogColor, t);
            info.fogMode = to.fogMode;
            info.density = Mathf.Lerp(from.density, to.density, t);
            info.linearStart = Mathf.Lerp(from.linearStart, to.linearStart, t);
            info.linearEnd = Mathf.Lerp(from.linearEnd, to.linearEnd, t);
            info.ambientLight = Color.Lerp(from.ambientLight, to.ambientLight, t);
            info.directionalLight = Color.Lerp(from.directionalLight, to.directionalLight, t);
            info.directionalLightIntensity = Mathf.Lerp(from.directionalLightIntensity, to.directionalLightIntensity, t);
            info.Ev_Light = Color.Lerp(from.Ev_Light, to.Ev_Light, t);
            info.Ev_LightIntensity = Mathf.Lerp(from.Ev_LightIntensity, to.Ev_LightIntensity, t);
            info.backgroundColor = Color.Lerp(from.backgroundColor, to.backgroundColor, t);
            return info;
        }
    }
}