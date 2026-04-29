using LGM.OXPlaneGame;
using ML.MLBKids;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmGame
{
    public class TimingManager : MonoBehaviour
    {
        int[] judgementRecord = new int[5];

        [SerializeField] Transform center = null; // 중앙 위치를 나타내는 Transform
        [SerializeField] RectTransform[] timingRect = null; // 타이밍을 위한 RectTransform 배열
        [SerializeField] Vector2[] timingBoxes = null; // 타이밍 박스의 위치를 저장할 배열

        [SerializeField] EffectManager effectManager = null;
        [SerializeField] ScoreManager scoreManager = null;
        [SerializeField] ComboManager comboManager = null;

        private void Start()
        {
            // 타이밍 박스의 위치 배열 초기화
            timingBoxes = new Vector2[timingRect.Length];

            // 각 RectTransform에 대해 박스의 위치를 계산
            for (int i = 0; i < timingRect.Length; i++)
            {
                // 중앙 위치에서 RectTransform의 너비의 반을 빼고 더하여 좌우 경계를 설정
                timingBoxes[i].Set(center.localPosition.y - timingRect[i].rect.height / 2,
                                   center.localPosition.y + timingRect[i].rect.height / 2);
            }
        }

        // 타이밍 체크 메서드
        public bool CheckTiming(Line line)
        {
            SoundMGR.Instance.SoundPlay("PlayGround_Hit");     // 사운드 재생

            List<GameObject> notes = line.GetNotes();  // 라인의 노트 리스트 가져오기

            // 노트 리스트를 순회하며 타이밍 체크
            for (int i = 0; i < notes.Count; i++)
            {
                GameObject note = notes[i];
                float t_notePosY = note.transform.localPosition.y;

                for (int j = 0; j < timingBoxes.Length; j++)
                {
                    // 현재 노트의 Y 위치가 타이밍 박스 범위 안에 있는지 확인
                    if (timingBoxes[j].x <= t_notePosY && t_notePosY <= timingBoxes[j].y)
                    {
                        // 노트를 숨기고 리스트에서 제거
                        note.GetComponent<Note>().HideNote();
                        line.RemoveNote(note);

                        if (j < timingBoxes.Length - 1) effectManager.NoteHitEffect();  // Hit 연출
                        effectManager.PlayJudgemnetEffect(j); // 판정 연출
                        scoreManager.IncreaseScore(j);  // 점수 증가

                        return true;
                    }
                }
            }

            comboManager.ResetCombo();
            effectManager.PlayJudgemnetEffect(timingBoxes.Length); // Bad 판정
            return false;
        }

        public int[] GetJudgementRecord()
        {
            return judgementRecord;
        }

        public void MissRecord()
        {
            judgementRecord[4]++; // 판정 기록
        }

        public void Init()
        {
            for (int i = 0; i < judgementRecord.Length; i++)
            {
                judgementRecord[i] = 0;
            }
        }
    }
}

