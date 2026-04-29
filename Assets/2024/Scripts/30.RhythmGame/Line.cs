using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // DoTween을 사용하려면 필요합니다.

namespace RhythmGame
{
    public class Line : MonoBehaviour
    {
        private Image img = null;
        [SerializeField] List<GameObject> notes = new List<GameObject>(); // 라인에 속한 노트 리스트

        private void Awake()
        {
            img = GetComponent<Image>();
        }

        public void LineAnim()
        {
            img.DOFillAmount(1, 0.1f).OnComplete(() => img.DOFillAmount(0, 0.1f));
        }

        // 노트 리스트 반환
        public List<GameObject> GetNotes()
        {
            return notes;
        }

        // 새로운 노트 추가
        public void AddNote(GameObject note)
        {
            notes.Add(note);
        }

        // 노트 제거
        public void RemoveNote(GameObject note)
        {
            notes.Remove(note);
        }
    }
}
