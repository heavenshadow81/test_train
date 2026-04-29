using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.IO;


#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(TimeStamper))]
public class TimeStamperEditor : Editor
{
    const string INFO = "TimeStamper 사용법:\n" +
        "1. TimeStamper는 Manual과 Auto 두 가지 모드를 지원합니다.\n" +
        "   - Manual 모드: Q, W, E, R 키를 눌러 특정 시점에 노드를 수동으로 추가합니다.\n" +
        "   - Auto 모드: 오디오의 스펙트럼 데이터를 분석하여 자동으로 노드를 추가합니다.\n" +
        "     - threshold 값을 설정하여 특정 임계값 이상일 때만 노드를 생성합니다.\n" +
        "     - minIntervalRange와 maxIntervalRange를 설정하여 노드 생성 간격을 랜덤하게 설정합니다.\n\n" +
        "2. 파일 이름 설정:\n" +
        "   - SongList enum을 사용하여 파일 이름을 설정합니다.\n" +
        "   - 오타 방지를 위해 SongList에 미리 정의된 노래 이름을 사용합니다.\n" +
        "   - Auto 모드일 때는 파일 이름 앞에 'Auto_' 접두사가 자동으로 붙습니다.\n\n" +
        "3. 노드 기록은 오디오 재생이 종료되면 자동으로 JSON 파일로 저장됩니다.\n" +
        "   - 저장된 JSON 파일은 'Assets/2024/Json/' 경로에 저장됩니다.\n\n" +
        "4. JSON 파일을 Addressables 그룹에 등록해야 합니다.\n" +
        "   - 생성된 JSON 파일을 Addressables 그룹에 수동으로 추가하여 불러올 수 있도록 설정해야 합니다.\n\n" +
        "5. Auto와 Manual 두 가지 모드를 사용하여 JSON 파일을 생성한 후, 수동으로 수정하여 더 정확한 타이밍을 설정할 수 있습니다.\n" +
        "   - Auto 모드로 생성된 파일을 수동으로 조정하면 더욱 정확한 결과를 얻을 수 있습니다.\n";



    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox(INFO, MessageType.Info);
        base.OnInspectorGUI();
    }
}
#endif

public enum SongList
{
    OldMacdonald
}


public class TimeStamper : MonoBehaviour
{
    public enum TimeStampMode { Manual, Auto }
    public TimeStampMode mode = TimeStampMode.Manual; // 모드를 Manual로 기본 설정

    public List<NodeInfo> nodeArray = new List<NodeInfo>();
    public AudioSource audioSource;
    public SongList songName;

    private float[] spectrumData = new float[1024]; // Auto 모드를 위한 주파수 데이터를 저장할 배열
    private double audioStartTime = 0;
    public float threshold = 0.05f; // 스펙트럼 데이터 임계값을 설정
    public float minIntervalRange = 0.3f; // 랜덤 최소 간격
    public float maxIntervalRange = 1.0f; // 랜덤 최대 간격
    private double lastNoteTime = 0f;     // 마지막으로 노드를 생성한 시간
    private float nextInterval = 0f;     // 다음 노트가 생성될 때까지의 간격
    private bool hasSaved = false;
    private int nodeNum = 1;

    void Start()
    {
        StartCoroutine(PrepareAndPlayAudio());
        nextInterval = Random.Range(minIntervalRange, maxIntervalRange); // 첫 노트 간격 설정
    }

    IEnumerator PrepareAndPlayAudio()
    {
        // 오디오 소스가 준비될 때까지 대기
        while (!audioSource.isActiveAndEnabled || !audioSource.clip.loadState.Equals(AudioDataLoadState.Loaded))
        {
            yield return null;
        }

        audioStartTime = AudioSettings.dspTime + 1.0; // 1초 후 오디오 재생
        audioSource.PlayScheduled(audioStartTime);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            SaveJson();
        }

        if (audioSource.isPlaying && AudioSettings.dspTime >= audioStartTime)
        {
            double currTime = AudioSettings.dspTime - audioStartTime;

            switch (mode)
            {
                case TimeStampMode.Manual:
                    ManualModeUpdate((float)currTime);
                    break;
                case TimeStampMode.Auto:
                    AutoModeUpdate((float)currTime);
                    break;
            }

            hasSaved = false;  // 재생 중일 때는 저장을 다시 허용
        }
        else if (!audioSource.isPlaying && nodeArray.Count > 0 && !hasSaved)
        {
            SaveJson();
            hasSaved = true;  // 저장이 한 번 되면 다시 저장하지 않도록 플래그 설정
        }
    }

    // Manual 모드에서 키 입력에 따른 노드 추가
    void ManualModeUpdate(float currTime)
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            AddNode(nodeNum, 0, currTime, 0f); // Manual 모드에서는 spectrumData를 0으로 설정
            nodeNum++;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            AddNode(nodeNum, 1, currTime, 0f);
            nodeNum++;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            AddNode(nodeNum, 2, currTime, 0f);
            nodeNum++;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            AddNode(nodeNum, 3, currTime, 0f);
            nodeNum++;
        }
    }

    // Auto 모드에서 오디오 스펙트럼 분석하여 자동으로 노드 추가
    void AutoModeUpdate(double currTime)
    {
        // 2초 이후에만 노드를 생성하도록 조건 추가
        if (currTime < 2.0)
        {
            return; // 2초 이전에는 아무것도 하지 않음
        }

        // 오디오의 스펙트럼 데이터를 가져옵니다.
        audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);

        double voiceRangeSum = 0f;
        int lowerBound = 5; // 약 85Hz에 해당하는 주파수 대역 (0~1024로 나뉘기 때문에 대략적인 인덱스)
        int upperBound = 30; // 약 255Hz에 해당하는 주파수 대역

        // 해당 주파수 범위의 스펙트럼 데이터를 모두 합산하여 음성 주파수 대역의 에너지를 계산합니다.
        for (int i = lowerBound; i <= upperBound; i++)
        {
            voiceRangeSum += spectrumData[i];
        }

        // 주파수 대역의 에너지가 설정한 임계값을 넘고, 마지막 노드 생성 시간 이후 일정 시간이 경과했을 때 노드를 생성
        if (voiceRangeSum > threshold && (currTime - lastNoteTime) > nextInterval)
        {
            int randType = Random.Range(0, 4);
            AddNode(nodeNum, randType, currTime, voiceRangeSum); // 새로운 노드를 추가
            nodeNum++;
            lastNoteTime = currTime; // 마지막으로 노드를 생성한 시간을 갱신
            nextInterval = Random.Range(minIntervalRange, maxIntervalRange); // 새로운 랜덤 간격을 설정
        }
    }

    // 노드를 추가하고 spectrumData 값을 포함시킴
    void AddNode(int number, int type, double time, double spectrum)
    {
        NodeInfo info = new NodeInfo();
        info.number = number;
        info.type = type;
        info.time = time;
        info.spectrumData = spectrum;
        nodeArray.Add(info);
    }

    // JSON 파일 저장하기
    void SaveJson()
    {
        string folderPath = Path.Combine(Application.dataPath, "2024", "Json");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // 파일명 앞에 모드에 따라 'auto_'를 추가하거나 기본 이름으로 설정
        string filePrefix = mode == TimeStampMode.Auto ? "Auto_" : "";
        string filePath = Path.Combine(folderPath, $"{filePrefix}{songName}.json");

        if (File.Exists(filePath))
        {
            Debug.LogWarning("파일이 이미 존재합니다. 덮어쓰기를 합니다: " + filePath);
        }

        string json = JsonUtility.ToJson(new Wrapper<NodeInfo> { Items = nodeArray }, true);
        File.WriteAllText(filePath, json);

        Debug.Log("Saved JSON to: " + filePath);
    }
}
