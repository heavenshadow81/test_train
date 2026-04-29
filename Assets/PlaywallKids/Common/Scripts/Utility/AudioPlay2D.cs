using System.Collections.Generic;
using UnityEngine;

namespace ML.PlaywallKids.Common
{
    public static class AudioPlay2D
    {
        public static float GlobalVolume
        {
            get { return _globalVolume; }
            set { _globalVolume = Mathf.Clamp01(value); }
        }
        private static float _globalVolume = 1.0f;

        private static List<AudioSource> _audios = new List<AudioSource>();
        private static Transform _parent;

        /// <summary>
        /// 2D 오디오를 1회 재생한다.
        /// </summary>
        public static void PlayClip(AudioClip clip, float volume = 1, float pitch = 1)
        {
            if (clip == null) return;

            if (_parent == null || _parent.gameObject == null)
            {
                GameObject parent = new GameObject("_audiopool_2d");
                Object.DontDestroyOnLoad(parent);
                //parent.hideFlags = HideFlags.HideInHierarchy;
                _parent = parent.transform;

                // 기본 object pool 생성
                for (int i = 0; i < 8; i++)
                    _NewAudio();
            }

            // 사용하지 않는 object를 가져온다. 없을 경우 새로 생성.
            AudioSource audio = null;
            for (int i = 0; i < _audios.Count; i++)
            {
                if (!_audios[i].isPlaying)
                {
                    audio = _audios[i];
                    break;
                }
            }
            if (audio == null)
                audio = _NewAudio();

            // 오디오 재생
            audio.clip = clip;
            audio.volume = volume * _globalVolume;
            audio.pitch = pitch;
            audio.Play();
        }

        private static AudioSource _NewAudio()
        {
            // 새로운 object를 생성하여 pool에 추가한다.
            GameObject go = new GameObject("_audio");
            go.transform.parent = _parent;
            AudioSource audio = go.AddComponent<AudioSource>();
            audio.playOnAwake = false;
            audio.loop = false;
            audio.spatialBlend = 0;
            _audios.Add(audio);
            return audio;
        }
    }
}