using Firebase.Database;
using System.Collections.Generic;
using UnityEngine;

namespace QualiumSystems
{
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

            SetAllActions();
        }

        private void Start()
        {
            noteController.GetPreviousNotes();
        }

        // Set functions to react on actions with notes
        private void SetAllActions()
        {
            noteController.onNotesChanged = SetAllNotes;
            uIController.onNoteShared = TryToShareNote;
            uIController.onNoteEdited = TryToEditNote;
            uIController.onNoteDeleted = TryToDeleteNote;
            galleryUpload.onFileUploaded = TryToShareFile;
        }

        // Verify note then create NoteData and call ShareNote to send message to database
        private void TryToShareNote(string message)
        {
            if (string.IsNullOrEmpty(message)) return;

            var note = new NoteData(userId, message, username, maxCount + 1);
            notes.Add(note);
            noteController.ShareNote(note);
        }

        // Create NoteData and call ShareNote to send file link to database
        private void TryToShareFile(string link)
        {
            var note = new NoteData(userId, link, default, username, maxCount + 1);
            notes.Add(note);
            noteController.ShareNote(note);
        }

        // If user is author of the note, call DeleteNote to delete note by id
        private void TryToDeleteNote(NoteData currentNote)
        {
            if (currentNote.userId.Equals(userId)) noteController.DeleteNote(currentNote.noteId);
        }

        // Verify note and if user is author of the note, create edited NoteData and call EditNote
        private void TryToEditNote(NoteData currentNote, string newMessage)
        {
            if (string.IsNullOrEmpty(newMessage) || !currentNote.userId.Equals(userId)) return;

            NoteData note = new NoteData(userId, newMessage, username, currentNote.counter);
            noteController.EditNote(note);
        }

        // Unpack data from database and write notes to list, then call DisplayNotes to show them to user
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
}
