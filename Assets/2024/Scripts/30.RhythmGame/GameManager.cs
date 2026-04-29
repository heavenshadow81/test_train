using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RhythmGame.NoteManager;

namespace RhythmGame
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [SerializeField] GameObject gameCanvas = null;

        public bool isStartGame = false;

        [SerializeField] RhythmTimer rhythmTimer = null;
        [SerializeField] Record record = null;

        [Header("노래 설정")]
        [SerializeField] NoteGenerationMode generationMode = NoteGenerationMode.BPM;
        [SerializeField] TimeStampMode timeStampMode = TimeStampMode.Manual;
        [SerializeField] int bpm = 120;
        [SerializeField] SongList songName;

        // NoteManager를 찾기 위한 배열
        [SerializeField] NoteManager[] noteManagers;

        void Awake()
        {
            // Singleton 패턴 구현
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject); // 이미 Instance가 존재하면 중복 생성 방지
            }
        }

        void Start()
        {
            // NoteManager의 설정을 변경
            UpdateNoteManagerSettings(generationMode, timeStampMode, bpm, songName);
        }

        // NoteManager의 설정을 일괄적으로 업데이트하는 함수
        public void UpdateNoteManagerSettings(NoteManager.NoteGenerationMode newGenerationMode, NoteManager.TimeStampMode newTimeStampMode, int newBpm, SongList newSongName)
        {
            foreach (var noteManager in noteManagers)
            {
                noteManager.UpdateNoteManagerSettings(newGenerationMode, newTimeStampMode, newBpm, newSongName);
            }
        }

        void OnEnable()
        {
            CountDown.OnCountdownFinished += GameStart; // 이벤트 구독
        }

        void OnDisable()
        {
            CountDown.OnCountdownFinished -= GameStart; // 이벤트 구독 해제
        }

        void GameStart()
        {
            if (CountDown.gameStart) // CountDown 클래스가 정의되어 있어야 함
            {
                gameCanvas.SetActive(true);
                SoundMGR.Instance.bgmSource.Play();
                isStartGame = true;
                rhythmTimer.StartTimer();
                record.gameObject.SetActive(true); 
            }
        }
    }

}

