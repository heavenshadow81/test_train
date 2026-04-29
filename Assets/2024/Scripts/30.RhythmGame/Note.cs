using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RhythmGame
{
    public class Note : MonoBehaviour
    {
        [SerializeField] float noteSpeed = 5;

        Image noteImage = null;

        public int assignedLine;

        private void OnEnable()
        {
            if (noteImage == null) noteImage = GetComponent<Image>();

            noteImage.enabled = true;
        }

        private void Update()
        {
            transform.position += Vector3.down * noteSpeed * Time.deltaTime;
        }

        public void HideNote()
        {
            noteImage.enabled = false;
        }

        public bool GetNoteFlag()
        {
            return noteImage.enabled;
        }
    }
}
