using UnityEngine;
using UnityEngine.UI;
using System;

namespace QualiumSystems
{
    public abstract class NoteObject : MonoBehaviour
    {
        public NoteData noteData;

        protected Action<NoteObject> onNoteSelected;

        protected bool isHighlited = false;

        protected Color highlited, common;

        // Set the value of variables after object creating
        public virtual void NoteInit(NoteData noteData, Action<NoteObject> noteSelected)
        {
            onNoteSelected = noteSelected;
            this.noteData = noteData;
        }

        // Change image color after click on object
        public virtual void NoteHighlited()
        {
            GetComponent<Image>().color = isHighlited ? common : highlited;

            isHighlited = !isHighlited;
        }
    }
}
