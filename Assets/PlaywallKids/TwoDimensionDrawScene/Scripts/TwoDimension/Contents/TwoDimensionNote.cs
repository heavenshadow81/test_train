using UnityEngine;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public class TwoDimensionNote : TwoDimensionBase
    {
        public MusicNoteCreator creatorPrefab;

        // Music Note
        public RenderTexture musicNoteRT;
        public UITexture musicNoteUITexture;

        // Chord
        public UIPanel musicPaperPanel;
        public UIPanel chordPanel;
        public UITexture chordUITexture;
        public UILabel titleLabel;

        private MusicNoteCreator _creator;

        public void Start()
        {
            PlayStart();
        }

        public override bool PlayStart()
        {
            if (_creator == null)
            {
                _creator = FindObjectOfType<MusicNoteCreator>();
                if (_creator == null)
                {
                    _creator = (MusicNoteCreator)Instantiate(creatorPrefab, transform.position + new Vector3(0, 0, -200.0f), Quaternion.identity);
                    _creator.transform.localScale = Vector3.one;
                }
            }

            if (_creator.onTouched == null)
            {
                _creator.onTouched = () =>
                {
                    if (chordUITexture.mainTexture != null)
                    {
                        float scaleFactor = chordPanel.height / chordUITexture.mainTexture.height;
                        TweenPosition.Begin(chordUITexture.cachedGameObject, 0.5f, Vector3.left * scaleFactor * _creator.currentPosInChordTexture + new Vector3(chordPanel.baseClipRegion.x, chordPanel.baseClipRegion.y, 0));
                    }
                };
            }

            if (_creator.onChordLoaded == null)
            {
                _creator.onChordLoaded = () =>
                {
                    chordUITexture.mainTexture = _creator.currentChordTexture;
                    if (chordUITexture.mainTexture != null)
                    {
                        float scaleFactor = chordPanel.height / chordUITexture.mainTexture.height;
                        chordUITexture.width = Mathf.FloorToInt(scaleFactor * chordUITexture.mainTexture.width);
                        chordUITexture.height = Mathf.FloorToInt(chordPanel.height);

                        var tp = chordUITexture.GetComponent<TweenPosition>();
                        if (tp != null)
                        {
                            tp.enabled = false;
                        }
                        chordUITexture.cachedTransform.localPosition = Vector3.left * scaleFactor * _creator.currentPosInChordTexture + new Vector3(chordPanel.baseClipRegion.x, chordPanel.baseClipRegion.y, 0);

                        musicPaperPanel.alpha = 1;
                    }
                    else
                    {
                        musicPaperPanel.alpha = 0;
                    }

                    titleLabel.text = _creator.currentChordName;
                };

                _creator.onChordLoaded();
            }


            if (musicNoteRT == null)
            {
                musicNoteRT = new RenderTexture(4096, 2048, 16, RenderTextureFormat.ARGB32);
                _creator.noteCamera.targetTexture = musicNoteRT;
                _creator.noteCamera.clearFlags = CameraClearFlags.SolidColor;
                _creator.noteCamera.backgroundColor = Color.clear;
                _creator.noteCamera.aspect = (float)Screen.width / (float)Screen.height;
            }

            if (musicNoteUITexture != null)
                musicNoteUITexture.mainTexture = musicNoteRT;

            return true;
        }

        public override bool Play()
        {
            if (!_creator.gameObject.activeInHierarchy)
            { _creator.gameObject.SetActive(true); }

            return true;
        }

        public override bool PlayEnd()
        {
            _creator.Clear();
            _creator.onTouched = null;
            _creator.onChordLoaded = null;
            _creator.gameObject.SetActive(false);

            return true;
        }
    }
}