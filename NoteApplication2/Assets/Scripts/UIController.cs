using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIController : MonoBehaviour
{
    public Action<string> OnNoteShared;
    public Action<NoteData, string> OnNoteEdited;
    public Action<NoteData> OnNoteDeleted;

    [SerializeField] private GameObject notePrefab;
    [SerializeField] private GameObject filePrefab;
    [SerializeField] private Transform previousNotesContainer;

    [SerializeField] private TMP_InputField inputNote;

    [SerializeField] private Button deleteButton, editButton, shareButton;

    private NoteObject selectedNote;

    private bool isEditing = false;

    public void OnShareNoteButtonPressed()
    {
        if (isEditing)
        {
            OnNoteEdited(selectedNote.noteData, inputNote.text);
            isEditing = false;
        }
        else
        {
            OnNoteShared(inputNote.text);
        }
        inputNote.text = "";

        UIAnimator.ElementTapResponse(shareButton.transform);
    }

    public void OnDeleteNoteButtonPressed()
    {
        if (selectedNote != null)
        {
            OnNoteDeleted(selectedNote.noteData);
        }

        UIAnimator.ElementTapResponse(deleteButton.transform);
    }

    public void OnEditNoteButtonPressed()
    {
        isEditing = true;

        UIAnimator.ElementTapResponse(editButton.transform);
    }

    public void DisplayNotes(List<NoteData> notes)
    {
        foreach (Transform child in previousNotesContainer)
        {
            Destroy(child.gameObject);
        }

        foreach(var note in notes)
        {
            GameObject noteGameobject;

            if (note.fileLink != "") noteGameobject = Instantiate(filePrefab, previousNotesContainer);
            else noteGameobject = Instantiate(notePrefab, previousNotesContainer);

            var noteObject = noteGameobject.GetComponent<NoteObject>();
            noteObject.NoteInit(note, SelectNote);
        }
    }

    private void SelectNote(NoteObject noteObject)
    {
        isEditing = false;

        if (selectedNote != null) selectedNote.NoteHighlited();

        if (selectedNote == noteObject)
        {
            selectedNote = null;

            deleteButton.interactable = false;
            editButton.interactable = false;
        }
        else
        {
            selectedNote = noteObject;
            selectedNote.NoteHighlited();

            deleteButton.interactable = true;
            editButton.interactable = true;
        }
    }
}
