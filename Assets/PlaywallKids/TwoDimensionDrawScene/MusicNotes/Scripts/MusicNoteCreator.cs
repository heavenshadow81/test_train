using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public class MusicNoteCreator : MonoBehaviour
    {
        #region Public variables
        public Camera noteCamera;

        public GameObject[] musicNotePrefabs;

        public float createCountPerSeconds = 20.0f;

        public int maxCount = 100;

        public TextAsset[] chords;

        public Texture2D[] chordTextures;

        /**
         * Outline box 3D colliders
         *    [3]
         * [0][4][1]
         *    [2]
         * */
        public BoxCollider[] colliders = new BoxCollider[5];

        public List<GameObject> notes = new List<GameObject>();

        public List<AudioClip> noteSounds = new List<AudioClip>();

        public string currentChordName = "";

        public Texture2D currentChordTexture = null;

        public System.Action onTouched;

        public System.Action onChordLoaded;
        #endregion

        #region Properties
        public int currentPosInChordTexture
        {
            get
            {
                if (_posListInChordTexture.Count > _currentNoteSoundListIndex)
                {
                    return _posListInChordTexture[_currentNoteSoundListIndex];
                }

                return 0;
            }
        }
        #endregion

        #region Private variables
        private Transform _collidersParent;

        private Transform _notesParent;

        private float _countForCreating = 0.0f;

        private Dictionary<int, bool> _touchStateDict = new Dictionary<int, bool>();
        private Dictionary<int, float> _elapsedTimeOfDrag;

        private int _selectedChordIndex = -1;

        private List<int> _noteSoundIndexList = new List<int>();

        private int _currentNoteSoundListIndex = 0;

        private int _destroyedObjectCount = 0;

        public List<int> _posListInChordTexture = new List<int>();
        #endregion

        void OnEnable()
        {
            _elapsedTimeOfDrag = new Dictionary<int, float>();
        }

        void OnDisable()
        {
            _elapsedTimeOfDrag.Clear();
            _elapsedTimeOfDrag = null;
        }

        public void Start()
        {
            // Find the note camera if it doesn't set. It attemps to main camera as the note camera.
            if (noteCamera == null) noteCamera = Camera.main;

            // Initialize the inner transforms
            _collidersParent = new GameObject("Colliders").transform;
            _collidersParent.parent = transform;
            _collidersParent.localPosition = Vector3.zero;
            _collidersParent.localRotation = Quaternion.identity;
            _collidersParent.localScale = Vector3.one;

            _notesParent = new GameObject("Notes").transform;
            _notesParent.parent = transform;
            _notesParent.localPosition = Vector3.zero;
            _notesParent.localRotation = Quaternion.identity;
            _notesParent.localScale = Vector3.one;

            // Initialize the colliders
            _InitColliders();

            // Initialize the chords
            if (chords.Length > 0)
            {
                SetChordsAt(0);
            }
            else
            {
                ResetDefaultNoteSoundIndices();
            }
        }

        private void _InitColliders()
        {
            colliders = new BoxCollider[5];

            string[] colliderNames = { "Left", "Right", "Bottom", "Top", "Back" };
            for (int i = 0; i < colliders.Length; i++)
            {
                GameObject go = new GameObject("Collider_" + colliderNames[i]);
                go.transform.parent = _collidersParent;
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;

                BoxCollider collider = go.AddComponent<BoxCollider>();
                colliders[i] = collider;

                // for preventing static collider move
                var rigidbody = go.AddComponent<Rigidbody>();
                rigidbody.isKinematic = true;
                rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            }
        }

        public void SetChordsAt(int idx)
        {
            if (chords.Length > idx)
            {
                _selectedChordIndex = idx;

                _ParseChords(chords[idx], out currentChordName);

                _FindChordTextureAndParse();

                _currentNoteSoundListIndex = 0;

                if (onChordLoaded != null) onChordLoaded();
            }
        }

        public void ResetDefaultNoteSoundIndices()
        {
            _noteSoundIndexList.Clear();
            _currentNoteSoundListIndex = 0;

            for (int i = 0; i < noteSounds.Count; i++)
            {
                _noteSoundIndexList.Add(i);
            }
        }

        private void _ParseChords(TextAsset text, out string chordName)
        {
            _noteSoundIndexList.Clear();
            _currentNoteSoundListIndex = 0;

            chordName = "";

            System.IO.StringReader sr = new System.IO.StringReader(text.text);

            chordName = sr.ReadLine();

            while (true)
            {
                string line = sr.ReadLine();
                if (line == null || line.Length < 1)
                    break;

                char prevChord = ' ';
                for (int i = 0; i < line.Length; i++)
                {
                    char c = line[i];

                    if (c >= 'a' && c <= 'z')
                        c = (char)(c - 'a' + 'A');

                    if (prevChord == ' ')
                    {
                        if (c >= 'A' && c <= 'G')
                        {
                            prevChord = c;

                            if (line.Length <= i + 1)
                            {
                                int at = c - 'C';
                                if (at < 0) at += 7;

                                _noteSoundIndexList.Add(at);
                            }
                        }
                    }
                    else
                    {
                        int at = prevChord - 'C';
                        if (at < 0) at += 7;

                        if (c > '3' && c < '9')
                        {
                            int l = c - '3';

                            at = (at + 7 * l);
                            if (at >= noteSounds.Count)
                                at = at % 7 + (at / 7) * (noteSounds.Count - 7);

                            _noteSoundIndexList.Add(at);

                            prevChord = ' ';
                        }
                        else if (c >= 'A' && c <= 'G')
                        {
                            _noteSoundIndexList.Add(at);

                            prevChord = c;

                            if (line.Length <= i + 1)
                            {
                                at = prevChord - 'C';
                                if (at < 0) at += 7;

                                _noteSoundIndexList.Add(at);
                            }
                        }
                        else
                        {
                            if (prevChord >= 'A' && prevChord <= 'G')
                                _noteSoundIndexList.Add(at);

                            prevChord = ' ';
                        }
                    }
                }
            }

            if (_noteSoundIndexList.Count == 0)
            {
                ResetDefaultNoteSoundIndices();
            }
        }

        private void _FindChordTextureAndParse()
        {
            _posListInChordTexture.Clear();
            currentChordTexture = null;

            const byte blackColorVal = 30;

            System.DateTime s = System.DateTime.UtcNow;
            //이 부분이 악보 길이에 따라 위치 지정하는 것으로 추정됨
            foreach (Texture2D tex in chordTextures)
            {
                if (tex.name.Equals(currentChordName) || tex.name.Equals(chords[_selectedChordIndex].name))
                {
                    currentChordTexture = tex;

                    Color32[] colors = currentChordTexture.GetPixels32();
                    bool prevIsChordOrSeparator = false;

                    for (int i = 0, width = currentChordTexture.width, height = currentChordTexture.height; i < width; i++)
                    {
                        bool start = true;
                        int chordHeight = 0, maxChordHeight = 0;
                        bool isMusicPaperStart = false, isMusicPaperEnd = false;
                        int bothSideCount = 0;
                        bool maybeConnectedChord = false;
                        bool prevIsBothSide = false;
                        if (i == 169)
                            Debug.Log("Debug Me!");

                        for (int j = 0; j < height; j++)
                        {
                            if (start)
                            {
                                if (colors[j * width + i].r < blackColorVal)
                                {
                                    start = false;

                                    if (i > 0 && colors[j * width + i - 1].r < blackColorVal &&
                                        i + 1 < width && colors[j * width + i + 1].r < blackColorVal)
                                    {
                                        bothSideCount = 1;
                                        isMusicPaperStart = true;
                                        prevIsBothSide = true;
                                    }

                                    chordHeight = 1;
                                }
                            }
                            else if (colors[j * width + i].r < blackColorVal)
                            {
                                chordHeight++;

                                if (i > 0 && colors[j * width + i - 1].r < blackColorVal &&
                                    i + 1 < width && colors[j * width + i + 1].r < blackColorVal && !prevIsBothSide)
                                {
                                    bothSideCount++;
                                    prevIsBothSide = true;
                                }
                                else if ((i > 0 && colors[j * width + i - 1].r < blackColorVal) ||
                                    (i + 1 < width && colors[j * width + i + 1].r < blackColorVal && !prevIsBothSide))
                                {
                                    maybeConnectedChord = true;
                                }
                                else
                                    prevIsBothSide = false;
                            }

                            if (!start && (colors[j * width + i].r >= blackColorVal || j + 1 == height))
                            {
                                j = j - 1;

                                if ((i - 1 < 0 || colors[j * width + i - 1].r < blackColorVal) &&
                                    (i + 1 >= width || colors[j * width + i + 1].r < blackColorVal))
                                {
                                    bothSideCount++;
                                    prevIsBothSide = true;
                                    isMusicPaperEnd = true;
                                }

                                j = j + 1;

                                maxChordHeight = Mathf.Max(chordHeight, maxChordHeight);

                                if (maxChordHeight * 4 >= height) break;

                                isMusicPaperStart = false;
                                isMusicPaperEnd = false;
                                bothSideCount = 0;
                                prevIsBothSide = false;
                                maybeConnectedChord = false;

                                start = true;
                            }
                        }

                        if (maxChordHeight * 4 >= height && !prevIsChordOrSeparator)
                        {

                            if (isMusicPaperStart && isMusicPaperEnd && bothSideCount >= 5 && !maybeConnectedChord)
                            {
                                Debug.Log(string.Format("MusicNoteCreator({0}) : Separator at {1}", tex.name, i));
                                prevIsChordOrSeparator = true;
                            }
                            else
                            {
                                _posListInChordTexture.Add(i);
                                prevIsChordOrSeparator = true;
                                Debug.Log(string.Format("MusicNoteCreator({0}) : Added chord pos {1}, chord height : {2}", tex.name, i, maxChordHeight));
                            }
                        }
                        else
                        {
                            prevIsChordOrSeparator = false;
                        }
                    }

                    System.DateTime f = System.DateTime.UtcNow;

                    Debug.Log(string.Format("MusicNoteCreator._FindChordTextureAndParse({0}) : Taked time : {1:#,##0}ms", tex.name, (f - s).Milliseconds));

                    break;
                }
            }
        }

        public void Update()
        {
            RepositionColliders();

            _PerformInput();

            _Flow();

            if (noteCamera.targetTexture != null)
                noteCamera.aspect = (float)Screen.width / (float)Screen.height;
        }

        private void _PerformInput()
        {
            for (int i = 0; i < CustomInput.touchCount; i++)
            {
                TouchInfo t = CustomInput.GetTouch(i);

                if (noteCamera.GetComponent<BarrelDistortionEffect>() != null)
                    t.position = noteCamera.GetComponent<BarrelDistortionEffect>().GetDistoredScreenPosFromOriginal(t.position);

                if (t.phase == TouchInfo.Phase.Begin)
                {
                    GameObject go = _CheckNoteIsTouched(t);

                    if (go != null)
                    {
                        MusicNoteObject musicNote = go.GetComponent<MusicNoteObject>();
                        if (musicNote == null)
                            musicNote = go.GetComponentInParent<MusicNoteObject>();

                        if (musicNote != null)
                        {
                            // Play sounds if needed
                            if (noteSounds != null && noteSounds.Count > 0)
                            {
                                AudioClip clip = noteSounds[_noteSoundIndexList[_currentNoteSoundListIndex++]];
                                if (_currentNoteSoundListIndex + 1 >= _noteSoundIndexList.Count)
                                    SetChordsAt((_selectedChordIndex + 1) % chords.Length);

                                if (clip != null)
                                {
                                    // Create one shot audio
                                    GameObject audioGo = new GameObject("one shot audio");
                                    audioGo.transform.position = go.transform.position;
                                    AudioSource ac = audioGo.AddComponent<AudioSource>();
                                    ac.clip = clip;
                                    ac.spatialBlend = 0.0f; // 2D audio
                                    ac.loop = false;
                                    ac.volume = 1;
                                    ac.pitch = 1;
                                    ac.Play();
                                    Destroy(audioGo, clip.length + 1.0f);
                                }
                            }

                            // Remove note from list
                            notes.Remove(musicNote.gameObject);

                            // Destroy the game object
                            musicNote.Explode();

                            // Increase the number of destroyed objects
                            _destroyedObjectCount += 1;

                            // Call the handler
                            if (onTouched != null) onTouched();

                            return;
                        }
                    }
                    else
                    {
                        if (!_elapsedTimeOfDrag.ContainsKey(t.id)) _elapsedTimeOfDrag.Add(t.id, 0f);
                        else _elapsedTimeOfDrag[t.id] = 0f;
                    }
                }
                else if (t.phase == TouchInfo.Phase.End || t.phase == TouchInfo.Phase.Cancel)
                {
                    // do nothing...

                    // Test : Wind
                    if (_elapsedTimeOfDrag.ContainsKey(t.id)) _elapsedTimeOfDrag[t.id] = 0f;
                    Vector3 pos = t.position;
                    if (noteCamera.targetTexture != null)
                    {
                        pos.x = pos.x / (float)Screen.width * noteCamera.targetTexture.width;
                        pos.y = pos.y / (float)Screen.height * noteCamera.targetTexture.height;
                    }
                    pos.z = -noteCamera.transform.position.z;
                    pos = noteCamera.ScreenToWorldPoint(pos);

                    foreach (GameObject note in notes)
                    {
                        Vector3 dir = note.transform.position - pos;
                        if (dir.magnitude >= 1.0f)
                        {
                            Rigidbody rigidbody = note.GetComponent<Rigidbody>();

                            if (rigidbody != null)
                            {
                                Vector2 force = dir.normalized * Random.Range(30000.0f, 40000.0f) * rigidbody.mass / dir.magnitude;

                                rigidbody.AddForce(force, ForceMode.Force);
                            }
                        }
                    }
                }
                else
                {
                    if (_touchStateDict.ContainsKey(t.id) && !_touchStateDict[t.id])
                    {

                        float _elapsedTime = _elapsedTimeOfDrag[t.id];
                        _elapsedTime += Time.deltaTime;
                        if (_elapsedTime > 0.07f)
                        {
                            _elapsedTime = 0f;
                            _CreateNotes(t);
                        }
                        _elapsedTimeOfDrag[t.id] = _elapsedTime;
                    }


                }
            }
        }

        private GameObject _CheckNoteIsTouched(TouchInfo t)
        {
            Vector3 pos = t.position;
            if (noteCamera.targetTexture != null)
            {
                pos.x = pos.x / (float)Screen.width * noteCamera.targetTexture.width;
                pos.y = pos.y / (float)Screen.height * noteCamera.targetTexture.height;
            }

            Vector3 worldPos = noteCamera.ScreenToWorldPoint(new Vector3(pos.x, pos.y, transform.position.z - noteCamera.transform.position.z));
            var hitInfo2D = Physics2D.Raycast(worldPos, Vector2.zero);

            if (hitInfo2D.collider != null)
            {
                Debug.Log(string.Format("Touch {0} - Hit : {1}", t.id, hitInfo2D.collider.name));

                _touchStateDict[t.id] = true;

                return hitInfo2D.collider.gameObject;
            }
            else
            {
                RaycastHit hitInfo3D;
                if (Physics.Raycast(noteCamera.ScreenPointToRay(pos), out hitInfo3D))
                {
                    Debug.Log(string.Format("Touch {0} - Hit : {1}", t.id, hitInfo3D.collider.name));

                    bool hitsOutlineCollider = false;
                    for (int i = 0; i < colliders.Length; i++)
                    {
                        var collider = colliders[i];
                        if (hitInfo3D.collider == collider)
                        {
                            hitsOutlineCollider = true;
                            break;
                        }
                    }

                    if (hitInfo3D.collider != null && !hitsOutlineCollider)
                    {
                        _touchStateDict[t.id] = true;

                        return hitInfo3D.collider.gameObject;
                    }
                }
            }

            _touchStateDict[t.id] = false;

            return null;
        }
        //음표 생성 함수
        private void _CreateNotes(TouchInfo t)
        {
            if (notes.Count >= maxCount)
                return;

            Vector3 pos = t.position;
            if (noteCamera.targetTexture != null)
            {
                pos.x = pos.x / (float)Screen.width * noteCamera.targetTexture.width;
                pos.y = pos.y / (float)Screen.height * noteCamera.targetTexture.height;
            }
            pos.z = (noteCamera.farClipPlane + noteCamera.nearClipPlane) * 0.5f;

            pos = noteCamera.ScreenToWorldPoint(pos);

            // NaN (Not a number) Check
            if (float.IsNaN(pos.x) || float.IsNaN(pos.y) || float.IsNaN(pos.z))
                return;

            _countForCreating += Time.deltaTime * createCountPerSeconds;

            if (_countForCreating >= 1.0f)
            {
                for (; _countForCreating >= 1.0f; _countForCreating -= 1.0f)
                {
                    if (maxCount <= notes.Count)
                    {
                        _countForCreating = 0.0f;
                        break;
                    }

                    int random = Random.Range(0, musicNotePrefabs.Length);

                    GameObject go = (GameObject)Instantiate(musicNotePrefabs[random]);
                    go.SetActive(true);
                    go.transform.parent = transform;
                    go.transform.position = pos;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.one;
                    go.layer = gameObject.layer;

                    notes.Add(go);
                }
            }
        }

        //
        private void _Flow()
        {
            foreach (GameObject note in notes)
            {
                var rigidbody = note.GetComponent<Rigidbody>();
                if (rigidbody != null)
                {
                    if (rigidbody.linearVelocity.sqrMagnitude < 100.0f)
                    {
                        Vector3 force = Random.insideUnitSphere * Random.Range(1000.0f, 2000.0f);
                        rigidbody.AddForce(force, ForceMode.Force);
                    }
                }
            }
        }

        public void RepositionColliders()
        {
            _collidersParent.position = noteCamera.transform.position;
            _collidersParent.rotation = noteCamera.transform.rotation;
            _collidersParent.localScale = Vector3.one;

            // fov, theta
            float aspectRatio = (float)Screen.width / (float)Screen.height;
            float fov = Mathf.Max(5.0f, noteCamera.fieldOfView) * Mathf.Deg2Rad;
            float theta = 2 * Mathf.Atan(aspectRatio * Mathf.Tan(fov * 0.5f));

            // camera pos, clip range
            float clipRange = noteCamera.farClipPlane;
            Vector3 clipCenter = Vector3.forward * clipRange * 0.5f;

            // width, height, thickness
            float width = clipRange * 2 * Mathf.Tan(theta * 0.5f);
            float height = clipRange * 2 * Mathf.Tan(fov * 0.5f);
            float colliderThickness = Mathf.Min(width * 0.05f, height * 0.05f);

            // positions
            colliders[0].transform.localPosition = clipCenter - Vector3.right * (width * 0.25f + colliderThickness * 0.5f * Mathf.Cos(theta * 0.5f)) - Vector3.forward * colliderThickness * 0.5f * Mathf.Sin(theta * 0.5f);
            colliders[1].transform.localPosition = clipCenter + Vector3.right * (width * 0.25f + colliderThickness * 0.5f * Mathf.Cos(theta * 0.5f)) - Vector3.forward * colliderThickness * 0.5f * Mathf.Sin(theta * 0.5f);
            colliders[2].transform.localPosition = clipCenter - Vector3.up * (height * 0.25f + colliderThickness * 0.5f * Mathf.Cos(fov * 0.5f)) - Vector3.forward * colliderThickness * 0.5f * Mathf.Sin(fov * 0.5f);
            colliders[3].transform.localPosition = clipCenter + Vector3.up * (height * 0.25f + colliderThickness * 0.5f * Mathf.Cos(fov * 0.5f)) - Vector3.forward * colliderThickness * 0.5f * Mathf.Sin(fov * 0.5f);
            colliders[4].transform.localPosition = clipCenter + Vector3.forward * (clipRange + colliderThickness) * 0.5f;

            // rotations
            colliders[0].transform.localRotation = Quaternion.AngleAxis(-theta * Mathf.Rad2Deg * 0.5f, Vector3.up);
            colliders[1].transform.localRotation = Quaternion.AngleAxis(theta * Mathf.Rad2Deg * 0.5f, Vector3.up);
            colliders[2].transform.localRotation = Quaternion.AngleAxis(fov * Mathf.Rad2Deg * 0.5f, Vector3.right);
            colliders[3].transform.localRotation = Quaternion.AngleAxis(-fov * Mathf.Rad2Deg * 0.5f, Vector3.right);
            colliders[4].transform.localRotation = Quaternion.identity;

            Vector3 backColliderSize = new Vector3(width, height, colliderThickness);

            colliders[4].size = backColliderSize;
            colliders[0].size = colliders[1].size = new Vector3(colliderThickness, backColliderSize.y, clipRange / Mathf.Cos(theta * 0.5f));
            colliders[2].size = colliders[3].size = new Vector3(backColliderSize.x, colliderThickness, clipRange / Mathf.Cos(fov * 0.5f));
        }

        public void Clear()
        {
            foreach (var note in notes)
            {
                Destroy(note.gameObject);
            }
            notes.Clear();

            _destroyedObjectCount = 0;
        }
    }
}