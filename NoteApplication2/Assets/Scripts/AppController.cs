using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppController : MonoBehaviour
{
    public static string username;
    public static string userId;
    public static int maxCount;

    [SerializeField] private NoteController noteController;
    [SerializeField] private UIController uIController;
    [SerializeField] private GalleryUpload galleryUpload;

    private List<NoteData> notes = new List<NoteData>();

    private void Awake()
    {
        DontDestroyOnLoad(this);

        noteController.OnNotesChanged = SetAllNotes;
        uIController.OnNoteShared = TryToShareNote;
        uIController.OnNoteEdited = TryToEditNote;
        uIController.OnNoteDeleted = TryToDeleteNote;
        galleryUpload.OnFileUploaded = TryToShareFile;
    }

    private void Start()
    {
        noteController.GetPreviousNotes();
    }

    private void TryToShareNote(string message)
    {
        if (message.Length > 0)
        {
            var note = new NoteData(userId, message, username, maxCount + 1);
            notes.Add(note);
            noteController.ShareNote(note);
        }
    }

    private void TryToShareFile(string link)
    {
        var note = new NoteData(userId, link, "", username, maxCount + 1);
        notes.Add(note);
        noteController.ShareNote(note);
    }

    private void TryToDeleteNote(NoteData currentNote)
    {
        if(currentNote.userId == userId) noteController.DeleteNote(currentNote.noteId);
    }

    private void TryToEditNote(NoteData currentNote, string newMessage)
    {
        if(newMessage.Length > 0 && currentNote.userId == userId)
        {
            NoteData note = new NoteData(userId, newMessage, username, currentNote.counter);
            noteController.EditNote(note);
        }
    }

    private void SetAllNotes(DataSnapshot dataSnapshot)
    {
        notes.Clear();

        foreach (var ds in dataSnapshot.Children)
        {
            var noteJson = ds.GetRawJsonValue();
            NoteData note = (NoteData)JsonUtility.FromJson(noteJson, typeof(NoteData));
            notes.Add(note);

            maxCount = Mathf.Max(maxCount, note.counter);
        }

        uIController.DisplayNotes(notes);
    }
}
