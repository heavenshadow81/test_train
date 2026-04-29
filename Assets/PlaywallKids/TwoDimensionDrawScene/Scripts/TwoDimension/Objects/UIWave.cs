using UnityEngine;
using UnityEngine.Video;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    /// <summary>
    /// 파도 영상 제어 클래스
    /// </summary>
    public class UIWave : MonoBehaviour
    {
        public UITexture wave;
        public VideoClip[] videos;
        public RenderTexture[] renderTextures;
        private VideoPlayer[] _videoPlayers;

        void OnEnable()
        {
            if (_videoPlayers == null)
            {
                _videoPlayers = new VideoPlayer[videos.Length];
                for (int i = 0; i < _videoPlayers.Length; i++)
                {
                    VideoPlayer vp = gameObject.AddComponent<VideoPlayer>();
                    vp.source = VideoSource.VideoClip;
                    vp.renderMode = VideoRenderMode.RenderTexture;
                    vp.isLooping = true;
                    vp.playOnAwake = false;
                    _videoPlayers[i] = vp;
                }
            }

            for (int i = 0; i < videos.Length; i++)
            {
                _videoPlayers[i].targetTexture = renderTextures[i];
                _videoPlayers[i].clip = videos[i];
                _videoPlayers[i].Play();
            }
        }

        void OnDisable()
        {
            foreach (var vp in _videoPlayers)
            {
                vp.Stop();
                vp.clip = null;
                vp.targetTexture = null;
            }
        }
    }
}