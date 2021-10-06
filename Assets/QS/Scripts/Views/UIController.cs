using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace QualiumSystems
{
    public class UIController : MonoBehaviour
    {
        public Action<string> onNoteShared;
        public Action<NoteData, string> onNoteEdited;
        public Action<NoteData> onNoteDeleted;

        [SerializeField] private GameObject notePrefab;
        [SerializeField] private GameObject filePrefab;
        [SerializeField] private Transform previousNotesContainer;

        [SerializeField] private TMP_InputField inputNote;

        [SerializeField] private Button deleteButton, editButton, shareButton;

        private NoteObject selectedNote;

        private bool isEditing;

        // When we click on button send note to others
        public void OnShareNoteButtonPressed()
        {
            if (isEditing)
            {
                onNoteEdited?.Invoke(selectedNote.noteData, inputNote.text);
                isEditing = false;
            }
            else
            {
                onNoteShared?.Invoke(inputNote.text);
            }
            inputNote.text = default;

            UIAnimator.ElementTapResponse(shareButton.transform);
        }

        // When we click on button invoke onNoteDeleted to delete selected note
        public void OnDeleteNoteButtonPressed()
        {
            if (selectedNote != null)
            {
                onNoteDeleted?.Invoke(selectedNote.noteData);
            }

            UIAnimator.ElementTapResponse(deleteButton.transform);
        }

        // When button pressed set isEditing variable to true
        public void OnEditNoteButtonPressed()
        {
            isEditing = true;

            UIAnimator.ElementTapResponse(editButton.transform);
        }

        // Create object for every note in received list
        public void DisplayNotes(List<NoteData> notes)
        {
            foreach (Transform child in previousNotesContainer)
            {
                Destroy(child.gameObject);
            }

            foreach (var note in notes)
            {
                GameObject noteGameobject;

                if (string.IsNullOrEmpty(note.fileLink)) noteGameobject = Instantiate(notePrefab, previousNotesContainer);
                else noteGameobject = Instantiate(filePrefab, previousNotesContainer);

                var noteObject = noteGameobject.GetComponent<NoteObject>();
                noteObject.NoteInit(note, SelectNote);
            }
        }

        // Display selected note and enable interactions with it
        private void SelectNote(NoteObject noteObject)
        {
            isEditing = false;

            if (selectedNote != null) selectedNote.NoteHighlited();

            // If note already selected - remove selection
            if (selectedNote == noteObject)
            {
                selectedNote = null;

                EnableNoteActions(false);

                return;
            }

            // Select new note
            selectedNote = noteObject;
            selectedNote.NoteHighlited();

            EnableNoteActions(true);
        }

        // Enable/disable edit and delete buttons
        private void EnableNoteActions(bool enable)
        {
            deleteButton.interactable = enable;
            editButton.interactable = enable;
        }
    }
}
