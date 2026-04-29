using com.Loxwell.File;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace RhythmGame
{
    public class NoteManager : MonoBehaviour
    {
        public enum NoteGenerationMode { BPM, TimeStamp }
        public enum TimeStampMode { Manual, Auto }

        [SerializeField] NoteGenerationMode generationMode = NoteGenerationMode.BPM; // 기본 모드를 BPM으로 설정
        [SerializeField] TimeStampMode timeStampMode = TimeStampMode.Manual; // TimeStamp 모드일 경우 Manual로 기본 설정

        [SerializeField] int bpm = 120; // BPM 방식에서 사용할 BPM 값
        string addressableKey = "Assets/2024/Json/"; // TimeStamp 방식에서 사용할 JSON 파일의 경로
        [SerializeField] SongList songName; // TimeStamp JSON 파일 이름

        [SerializeField] List<NodeInfo> nodeArray = new List<NodeInfo>(); // JSON에서 로드한 노드 배열 (TimeStamp 방식)

        private int currentNodeIndex = 0; // TimeStamp 모드에서 현재 생성할 노드 인덱스
        private double currentTime = 0d;    // 높은 정밀도를 요하므로 double로 처리 (BPM 방식)
        private double audioStartTime = 0d; // TimeStamp 방식에서 오디오 재생 시간 시작
        private double dspCurrentTime = 0d; // TimeStamp 방식에서 DSP 시간 기반의 현재 게임 시간

        [SerializeField] Transform[] noteSpawners = null;
        [SerializeField] Line[] lines = null;
        [SerializeField] ObjectPool objectPool = null;
        [SerializeField] TimingManager timingManager = null;
        //[SerializeField] EffectManager effectManager = null;
        [SerializeField] ComboManager comboManager = null;

        [SerializeField] Transform startPos;
        [SerializeField] Transform endPos;
        [SerializeField] double offset;


        private void Start()
        {
            if (generationMode == NoteGenerationMode.TimeStamp)
            {
                LoadJsonFromAddressables(); // TimeStamp Auto 모드에서 JSON 파일을 로드

                float distance = Vector3.Distance(startPos.position, endPos.position);
                offset = distance / 5; // 이동 속도가 5일 때
            }
        }

        private void Update()
        {
            if (GameManager.Instance.isStartGame && audioStartTime == 0)
            {
                audioStartTime = AudioSettings.dspTime; // 게임 시작 시점에 오디오 시작 시간 기록
            }

            if (GameManager.Instance.isStartGame)
            {
                switch (generationMode)
                {
                    case NoteGenerationMode.BPM:
                        UpdateBPMMode();
                        break;
                    case NoteGenerationMode.TimeStamp:
                        UpdateTimeStampAutoMode();
                        break;
                }
            }
        }

        // BPM 방식 업데이트
        private void UpdateBPMMode()
        {
            currentTime += Time.deltaTime; // 프레임 간 경과 시간을 currentTime에 누적

            double beatTime = 60d / bpm;  // 1 Beat에 해당하는 시간

            if (currentTime >= beatTime)
            {
                if (objectPool.noteQueue.Count > 0)
                {
                    GameObject note = objectPool.noteQueue.Dequeue(); // 노트를 큐에서 꺼냄
                    Note noteComponent = note.GetComponent<Note>();

                    // 랜덤한 노트 스포너 선택
                    int randNum = Random.Range(0, noteSpawners.Length);

                    // 선택된 위치로 노트 이동
                    note.transform.position = noteSpawners[randNum].position;
                    lines[randNum].AddNote(note);

                    // 노트에 속한 라인 정보 저장
                    noteComponent.assignedLine = randNum;
                    note.SetActive(true); // 노트 활성화

                    // 현재 시간을 1 비트의 시간만큼 줄여서 다음 생성 시간 조정
                    currentTime -= beatTime;
                }
            }
        }

        // TimeStamp 방식의 업데이트
        private void UpdateTimeStampAutoMode()
        {
            dspCurrentTime = AudioSettings.dspTime - audioStartTime; // 오디오 재생 이후 경과 시간 계산

            while (currentNodeIndex < nodeArray.Count && dspCurrentTime >= nodeArray[currentNodeIndex].time - offset)
            {
                SpawnNote(nodeArray[currentNodeIndex]); // 현재 노트를 생성
                currentNodeIndex++; // 다음 노트로 이동
            }
        }

        // 노트 오프셋 타임 계산 함수
        private double CalculateOffsetTime(double noteTime)
        {
            // 현재 DSP 시간을 이용해 음악이 시작된 후 경과한 시간을 구합니다.
            double currentMusicTime = (double)(AudioSettings.dspTime - audioStartTime);

            // 오프셋 = 노트가 발생할 예정인 시간 - 현재 음악 경과 시간
            offset = noteTime - currentMusicTime;

            return offset;
        }

        // 노트 스폰 함수 (BPM과 TimeStamp에서 공통으로 사용)
        private void SpawnNote(NodeInfo node)
        {
            if (objectPool.noteQueue.Count > 0)
            {
                GameObject note = objectPool.noteQueue.Dequeue(); // 노트를 큐에서 꺼냄
                Note noteComponent = note.GetComponent<Note>();

                // 노트 스폰할 라인 선택
                int assignedLine = node.type % noteSpawners.Length;

                // 선택된 위치로 노트 이동
                note.transform.position = noteSpawners[assignedLine].position;
                lines[assignedLine].AddNote(note);

                // 노트에 속한 라인 정보 저장
                noteComponent.assignedLine = assignedLine;
                note.SetActive(true); // 노트 활성화
            }
        }

        // JSON 파일을 Addressables을 통해 로드하는 함수 (TimeStamp Auto 모드에서 사용)
        private void LoadJsonFromAddressables()
        {
            // 모드에 따라 파일명 앞에 'Auto_' 접두사를 추가 (Auto 모드일 때)
            string filePrefix = timeStampMode == TimeStampMode.Auto ? "Auto_" : "";
            string jsonFileName = $"{filePrefix}{songName}.json";

            // Addressables을 통해 파일 로드
            Addressables.LoadAssetAsync<TextAsset>(addressableKey + jsonFileName).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    string json = handle.Result.text;

                    // JSON 데이터를 NodeInfo 리스트로 변환
                    nodeArray = JsonUtility.FromJson<Wrapper<NodeInfo>>(json).Items;

                    // JSON 파일이 정상적으로 로드되었을 경우 첫 번째 노트 생성 인덱스를 초기화
                    currentNodeIndex = 0;
                    dspCurrentTime = 0d;
                }
                else
                {
                    Debug.LogError("Failed to load JSON from Addressables: " + jsonFileName);
                }
            };
        }

        // 노트 제거 및 충돌 처리
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Note"))
            {
                Note noteComponent = collision.GetComponent<Note>();

                if (noteComponent.GetNoteFlag())
                {
                    timingManager.MissRecord();
                    //effectManager.PlayJudgemnetEffect(4);   // 실패 이펙트
                    comboManager.ResetCombo();
                }

                // 충돌한 노트를 해당 라인에서 제거
                lines[noteComponent.assignedLine].RemoveNote(collision.gameObject);

                // 노트 풀에 다시 추가
                objectPool.noteQueue.Enqueue(collision.gameObject);
                collision.gameObject.SetActive(false);
            }
        }

        public void UpdateNoteManagerSettings(NoteGenerationMode newGenerationMode, TimeStampMode newTimeStampMode, int newBpm, SongList newSongName)
        {
            generationMode = newGenerationMode;
            timeStampMode = newTimeStampMode;
            bpm = newBpm;
            songName = newSongName;
        }
    }
}
