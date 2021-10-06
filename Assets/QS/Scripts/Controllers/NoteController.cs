using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace QualiumSystems
{
    public class NoteController : MonoBehaviour
    {
        public Action<DataSnapshot> onNotesChanged;

        private const string NoteDataPath = "notes";
        private const string OrderNotesParameter = "counter";

        private FirebaseAuth auth;
        private FirebaseUser user;

        private void Awake()
        {
            FirebaseAuth.DefaultInstance.StateChanged += HandleAuthStateChanged;
            CheckUser();

            DontDestroyOnLoad(this);
        }

        // When user is changed call CheckUser
        private void HandleAuthStateChanged(object sender, EventArgs e)
        {
            CheckUser();
        }

        // If user don't set - login anonymously, else get current user
        private void CheckUser()
        {
            if (FirebaseAuth.DefaultInstance.CurrentUser is null)
            {
                auth = FirebaseAuth.GetAuth(Firebase.FirebaseApp.DefaultInstance);
                auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(OnSignedIn);

                return;
            }

            auth = FirebaseAuth.DefaultInstance;
            user = auth.CurrentUser;

            SetUser();
        }

        // when any note is changed call GetPreviousNotes
        private void OnUserDataChanged(object sender, ValueChangedEventArgs e)
        {
            GetPreviousNotes();
        }

        // Check for exceptions, set user variable and call SetUser
        private void OnSignedIn(Task<FirebaseUser> signInTask)
        {
            if (signInTask.Exception is null)
            {
                user = signInTask.Result;

                SetUser();

                return;
            }

            Debug.LogError(signInTask.Exception.Message);
        }

        // Subscribe for changes in database, set username and user id
        private void SetUser()
        {
            var reference = FirebaseDatabase.DefaultInstance.GetReference(NoteDataPath);
            reference.ValueChanged += OnUserDataChanged;

            AppController.username = auth.CurrentUser.DisplayName;
            AppController.userId = user.UserId;
        }

        // Send note data to database
        public void ShareNote(NoteData note)
        {
            var jsonNote = JsonUtility.ToJson(note);

            FirebaseDatabase.DefaultInstance.GetReference(NoteDataPath).Child(note.noteId).SetRawJsonValueAsync(jsonNote);
        }

        // Delete note data from database by note id
        public void DeleteNote(string noteId)
        {
            FirebaseDatabase.DefaultInstance.GetReference(NoteDataPath).Child(noteId).RemoveValueAsync();
        }

        // Edit specific note in database by its id
        public void EditNote(NoteData newNote)
        {
            FirebaseDatabase.DefaultInstance.GetReference(NoteDataPath).Child(newNote.noteId).RemoveValueAsync();

            ShareNote(newNote);
        }

        // Get all notes from database, order them and invoke onNotesChanged
        public void GetPreviousNotes()
        {
            FirebaseDatabase.DefaultInstance.GetReference(NoteDataPath).OrderByChild(OrderNotesParameter).GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.Exception is null)
                {
                    onNotesChanged?.Invoke(task.Result);

                    return;
                }

                Debug.LogError(task.Exception.Message);
            });
        }
    }
}
