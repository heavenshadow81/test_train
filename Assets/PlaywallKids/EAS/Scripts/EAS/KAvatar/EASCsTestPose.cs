using UnityEngine;
using System.Collections;
using LibRBM;
using LibAP3;

namespace ML.PlaywallKids.EAS
{
    public class EASCsTestPose : MonoBehaviour
    {
        public TextAsset rbmText;

        public CsMotionControl[] motionControls;

        AP3 ap3;
        AP2 ap2;
        RBMFile rbmFile;

        bool playing = false;
        public float time = 0f;
        float kFramePerTime = 200;
        float curFrame { get { if (playing) return time * kFramePerTime; else return 0; } }

        const float MILLS_TO_TIME = 0.00100f;

        // 현재 조인트 값
        float[] joints = new float[16];

        // 기본 동작을 위한 초기 조인트 값
        float[] baseJointValues = new float[16] {
        125,
        179,
        199,
        88,
        108,
        126,
        72,
        49,
        163,
        141,
        51,
        47,
        49,
        199,
        205,
        205
    };

        // 실제 모델과의 로테이션 차이 값 (보정)
        float[] jointOffsetRotations = new float[16] {
        0.0f,
        -30.0f,
        64.0f,
        34.0f,
        0.0f,
        0.0f,
        30.0f,
        -64.0f,
        -34.0f,
        0.0f,
        -36.0f,
        0.0f,
        -46.0f,
        36.0f,
        0.0f,
        46.0f
    };

        // 파일에서 설정된 초기 조인트 값
        float[] initialSetJointValues = new float[16];

        // used in SendJoint()
        float[] innerJoints = new float[16];

        public float playTime = 0.0f;

        string[] apPaths = new string[0];
        string selected = "";

        public AudioClip audioClip = null;
        AudioSource audioSource
        {
            get
            {
                AudioSource audioSource = GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                }
                audioSource.playOnAwake = false;
                audioSource.loop = false;
                return audioSource;
            }
        }

        void Start()
        {
            for (int i = 0; i < joints.Length; i++)
            {
                joints[i] = baseJointValues[i];
            }

            SendJoint();

            string path = Application.streamingAssetsPath;

#if UNITY_ANDROID && !UNITY_EDITOR
        path = "/storage/sdcard0/";
#endif
            /*
            apPaths = System.IO.Directory.GetFiles(path, "*.ap?", System.IO.SearchOption.TopDirectoryOnly);

            if (apPaths.Length > 0)
            {
                selected = apPaths[0];
                OpenAP(apPaths[0]);
            }
             * */
            rbmFile = new RBMFile();
            rbmFile.ParseRBM(rbmText.text);

            playTime = 0.0f;
            foreach (var scene in rbmFile.Frames)
            {
                playTime += scene.SceneTime * MILLS_TO_TIME;
            }
            for (int i = 0; i < 16; i++)
            {
                initialSetJointValues[i] = rbmFile.Header.wCKSettingList[i].FileZeroPos;
                jointOffsetRotations[i] = 0;

                Debug.Log("Joint " + i + " file zero pos : " + initialSetJointValues[i]);
                //baseJointValues[i] = initialSetJointValues[i];
            }
            SendJoint();
        }

        void OpenAP(string fn)
        {
            if (GetComponent<AudioSource>() != null)
            {
                Destroy(GetComponent<AudioSource>());
                audioClip = null;
            }

            if (fn.EndsWith("ap2"))
            {
                ap2 = new AP2();
                ap2.openFile(fn);

                Debug.Log("Init OK");

                kFramePerTime = 60;
            }
            else
            {
                string filenameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(fn);

                ap3 = new AP3();
                ap3.openAP3File(fn);

                if (ap3.saveAP2File(Application.persistentDataPath + "/" + filenameWithoutExtension + ".ap2"))
                {
                    ap2 = new AP2();
                    ap2.openFile(Application.persistentDataPath + "/" + filenameWithoutExtension + ".ap2");

                    Debug.Log("Init OK");

                    kFramePerTime = ap2.MotionData.framecount / ap3.AP3Header.length_sec;
                }

                audioClip = AudioClip.Create(filenameWithoutExtension, ap3.left.Length, ap3.WAVHeader.NumChannels, 44100, false, false);
                short[] left = ap3.left;
                short[] right = ap3.right;
                float[] wavStreamFloats = new float[ap3.left.Length + ap3.right.Length];
                for (int i = 0, out_i = 0; i < left.Length; i++)
                {
                    wavStreamFloats[out_i++] = (float)left[i] / (float)short.MaxValue;
                    if (right.Length > 0)
                    {
                        wavStreamFloats[out_i++] = (float)right[i] / (float)short.MaxValue;
                    }
                }
                audioClip.SetData(wavStreamFloats, 0);

                audioSource.clip = audioClip;
            }
        }

        IEnumerator OpenWAVFromAP3(string path)
        {
            WWW www = new WWW(path);

            yield return www;

            audioClip = www.GetAudioClip(false);

            if (GetComponent<AudioSource>() != null)
            {
                Debug.Log("Audio Load Success");
            }
        }

        int prevScene = 0;

        void Update()
        {
            if (playing)
            {
                if (ap2 != null)
                {
                    int curFrameFrom = Mathf.FloorToInt(curFrame);
                    int curFrameTo = curFrameFrom + 1;

                    if (curFrameFrom + 1 >= ap2.MotionData.framecount)
                    {
                        playing = false;
                        audioSource.Stop();
                    }
                    else
                    {
                        var frameDataFrom = ap2.MotionData.frame[curFrameFrom];
                        var frameDataTo = ap2.MotionData.frame[curFrameTo];

                        for (int i = 0; i < ap2.MotionData.motorcount; i++)
                        {
                            var jointFrom = frameDataFrom.control[i];
                            var jointTo = frameDataTo.control[i];

                            float pos = Mathf.Lerp((float)jointFrom.pos, (float)jointTo.pos, curFrame % 1.0f);

                            pos -= (float)ap2.MotionData.motor[i].initial;

                            joints[i] = pos;
                        }

                        SendJoint();

                        time += Time.deltaTime;
                    }
                }
                else if (rbmFile != null)
                {
                    int currentScene = 0;
                    float tt = 0;
                    for (int i = 0; i < rbmFile.Header.SceneCount; i++)
                    {
                        tt += rbmFile.Frames[i].SceneTime * MILLS_TO_TIME;
                        if (tt >= time)
                        {
                            if (currentScene + 1 == rbmFile.Header.SceneCount)
                            {
                                playing = false;
                            }
                            break;
                        }
                        else
                        {
                            currentScene++;
                        }
                    }

                    if (currentScene > prevScene)
                    {
                        for (int i = prevScene; i < currentScene; i++)
                        {
                            var scene = rbmFile.Frames[i];
                            foreach (var wck in scene.wCKInfoList)
                            {
                                int joint = wck.wCKID;
                                float to = (float)wck.DPOS;

                                if (joint < joints.Length)
                                {
                                    joints[joint] = to;
                                }
                            }
                        }

                        prevScene = currentScene;
                    }

                    var scene2 = rbmFile.Frames[currentScene];
                    foreach (var wck in scene2.wCKInfoList)
                    {
                        int joint = wck.wCKID;
                        float from = (float)wck.SPOS;
                        float to = (float)wck.DPOS;
                        float dt = (scene2.SceneTime * MILLS_TO_TIME - (tt - time)) / (scene2.SceneTime * MILLS_TO_TIME /* / Mathf.Max(1.0f, (float)wck.Torque)*/);

                        if (joint < joints.Length)
                        {
                            joints[joint] = Mathf.Lerp(from, to, dt);
                        }
                    }

                    SendJoint();

                    time += Time.deltaTime;
                }
            }

        }

        public void Play()
        {
            playing = true;
            time = -2.0f;

            if (audioClip != null)
            {
                audioSource.clip = audioClip;
                audioSource.Stop();
                audioSource.time = 0;
                audioSource.Play();
            }

            prevScene = 0;
        }

        public void Stop()
        {
            playing = false;
            time = 0f;

            audioSource.Stop();

            for (int i = 0; i < 16; i++)
            {
                joints[i] = baseJointValues[i];
            }
            SendJoint();
        }

        /*
        void OnGUI()
        {
            float h = 20;
            foreach (string path in apPaths)
            {
                string filename = System.IO.Path.GetFileName(path);
                if (GUI.Toggle(new Rect(20, h, 200, 40), selected.Equals(path), filename))
                {
                    if (selected.Equals(path) == false)
                    {
                        selected = path;
                        OpenAP(path);
                    }
                }
                h += 40;
            }

            if (ap2 != null)
            {
                GUI.HorizontalSlider(new Rect(20, h + 20, 200, 20), curFrame, 0, ap2.MotionData.framecount);
                GUI.Label(new Rect(240, h + 20, 200, 20), curFrame + "/" + ap2.MotionData.framecount);
            }
            else if (rbmFile != null)
            {
                GUI.HorizontalSlider(new Rect(20, h + 20, 200, 20), (float)System.Math.Round(time, 1), 0, (float)System.Math.Round(playTime, 1));
                GUI.Label(new Rect(240, h + 20, 200, 20), string.Format("{0:0.00}/{1:0.00}", time, playTime));
            }

            if (GUI.Button(new Rect(20, h + 60, 80, 40), "Play"))
            {
                Play();
            }
            if (GUI.Button(new Rect(120, h + 60, 80, 40), "Stop"))
            {
                Stop();
            }

            for (int i = 0; i < 16; i++)
            {
                if (SliderSlider(i))
                {
                    SendJoint();
                }
            }
        }
         * */

        void SendJoint()
        {
            for (int i = 0; i < joints.Length; i++)
            {
                innerJoints[i] = joints[i] - baseJointValues[i] + jointOffsetRotations[i];
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < joints.Length; i++)
            {
                sb.AppendFormat("{0}{1}", (i == 0 ? "" : ","), Mathf.FloorToInt(innerJoints[i]));
            }

            foreach (var motionControl in motionControls)
            {
                motionControl.CommandPose(sb.ToString());
            }
        }

        bool SliderSlider(int idx)
        {
            float newJoint = GUI.HorizontalSlider(new Rect(400, 35 * (idx + 1), 200, 35), joints[idx],
                baseJointValues[idx] + jointOffsetRotations[idx] - 90,
                 baseJointValues[idx] + jointOffsetRotations[idx] + 90);
            bool ret = newJoint != joints[idx];
            joints[idx] = newJoint;

            return ret;
        }
    }
}