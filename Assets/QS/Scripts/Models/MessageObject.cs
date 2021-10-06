using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace QualiumSystems
{
    public class MessageObject : NoteObject, IPointerDownHandler
    {
        [SerializeField] private TextMeshProUGUI authorText, messageText;

        // Set the value of variables after object creating
        public override void NoteInit(NoteData noteData, Action<NoteObject> noteSelected)
        {
            base.NoteInit(noteData, noteSelected);

            authorText.text = noteData.username;
            messageText.text = noteData.message;

            highlited = Color.gray;
            common = Color.white;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            onNoteSelected?.Invoke(this);
        }
    }
}
