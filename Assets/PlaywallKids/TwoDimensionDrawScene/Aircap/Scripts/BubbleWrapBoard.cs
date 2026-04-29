using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    /// <summary>
    /// 뽁뽁이 터뜨리기 - 뽁뽁이 판 (board)
    /// </summary>
    public class BubbleWrapBoard : MonoBehaviour
    {
        public int row = 10;
        public int column = 10;

        public BubbleWrap bubbleWrapPrefab;
        public GameObject bubbleWrapEffectPrefab;
        public GameObject bubbleWrapWaterPrefab;

        public AudioClip[] bubbleWrapSounds;

        public new Camera camera;

        public bool is2DPanelMode = false;

        public List<BubbleWrap> bubbles = new List<BubbleWrap>();

        // 0 : pop
        // 1~5 : color
        int mode = 0;

        void OnEnable()
        {
            if (camera == null)
                camera = Camera.main;
            column = (int)(UtilityScript.width / 60);

            foreach (var bubble in bubbles)
            { Destroy(bubble.gameObject); }

            bubbles.Clear();

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    BubbleWrap go = Instantiate(bubbleWrapPrefab, Vector3.zero, Quaternion.identity);
                    go.gameObject.SetActive(true);
                    go.gameObject.layer = gameObject.layer;
                    go.transform.parent = transform;
                    go.transform.localPosition = new Vector3(-(float)(column - 1) * 0.5f + j, 0.0f, -(float)(row - 1) * 0.5f + i);
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.one;
                    bubbles.Add(go);

                    if (is2DPanelMode)
                    {
                        go.transform.localScale = Vector3.zero;

                        var ts = TweenScale.Begin(go.gameObject, 0.5f, Vector3.one);
                        ts.delay = Random.Range(0.0f, 1.0f) + 1.0f;

                        go.color = Random.Range(1, 5);
                        go.pop = false;
                    }
                }
            }
        }

        void Update()
        {
            if (Input.GetKeyDown("0"))
            {
                mode = 0;
            }
            else if (Input.GetKeyDown("1"))
            {
                mode = 1;
            }
            else if (Input.GetKeyDown("2"))
            {
                mode = 2;
            }
            else if (Input.GetKeyDown("3"))
            {
                mode = 3;
            }
            else if (Input.GetKeyDown("4"))
            {
                mode = 4;
            }
            else if (Input.GetKeyDown("5"))
            {
                mode = 5;
            }
            else if (Input.GetKeyDown("9"))
            {
                Reset(true);
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                Reset(false);
            }

            for (int i = 0; i < CustomInput.touchCount; i++)
                _PerformInput(CustomInput.GetTouch(i).position);
        }

        public void _PerformInput(Vector3 position)
        {
            RaycastHit hit;
            if (Physics.Raycast(camera.ScreenPointToRay(position), out hit))
            {
                if (hit.collider != null)
                {
                    BubbleWrap bubbleWrap = hit.collider.GetComponent<BubbleWrap>();
                    if (bubbleWrap != null && !bubbleWrap.pop)
                    {
                        switch (mode)
                        {
                            case 0:
                                Pop(bubbleWrap);
                                break;
                            default:
                                bubbleWrap.color = mode - 1;
                                break;
                        }
                    }
                }
            }
        }

        public void Reset(bool isRandomColor)
        {
            BubbleWrap[] bubbles = GetComponentsInChildren<BubbleWrap>();
            foreach (BubbleWrap bubble in bubbles)
            {
                bubble.color = isRandomColor ? Random.Range(1, 5) : 0;
                bubble.pop = false;
            }
        }

        public void Pop(BubbleWrap bubbleWrap)
        {
            bubbleWrap.pop = true;

            int random = Random.Range(0, bubbleWrapSounds.Length - (bubbleWrap.color == 0 ? 1 : 0));

            // play one shot audio
            GameObject audioObj = new GameObject("BubbleWrapAudio");
            audioObj.hideFlags = HideFlags.HideInHierarchy;
            AudioSource audioSource = audioObj.AddComponent<AudioSource>();
            audioSource.volume = 1;
            audioSource.spatialBlend = 0.0f;
            audioSource.clip = bubbleWrapSounds[random];
            audioSource.Play();
            Destroy(audioObj, audioSource.clip.length + 1.0f);

            // change particle colors
            if (bubbleWrap.color > 0)
            {
                Color[] colors = { Color.white, Color.blue, Color.green, Color.red, Color.yellow };

                GameObject go = Instantiate(bubbleWrapEffectPrefab);
                go.SetActive(true);
                go.transform.position = bubbleWrap.transform.position;
                ParticleSystem[] ps = go.GetComponentsInChildren<ParticleSystem>();
                foreach (var p in ps)
                {
                    var module = p.main;
                    module.startColor = colors[bubbleWrap.color];
                }
                MeshRenderer mr = go.GetComponentInChildren<MeshRenderer>();
                mr.transform.localPosition = new Vector3(0, 0, Random.Range(5.0f, 8.0f));

                Destroy(go, 2.0f);

                go = Instantiate(bubbleWrapWaterPrefab);
                go.SetActive(true);
                go.transform.position = bubbleWrap.transform.position + new Vector3(0, 2.0f, 0);
                ps = go.GetComponentsInChildren<ParticleSystem>();
                foreach (var p in ps)
                {
                    var module = p.main;
                    module.startColor = colors[bubbleWrap.color];
                }
                Destroy(go, 20.0f);
            }
        }

        public void OnGUI()
        {
            if (!is2DPanelMode)
            {
                if (mode > 0)
                {
                    GUI.Box(new Rect(20, 20, 80, 20), "Mode : Color");
                    GUI.DrawTexture(new Rect(20, 50, 80, 80), bubbleWrapPrefab.unpopColors[mode - 1]);
                }
                else
                {
                    GUI.Box(new Rect(20, 20, 80, 20), "Mode : Pop");
                }
            }
        }
    }
}