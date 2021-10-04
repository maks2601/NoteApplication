using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class NoteController : MonoBehaviour
{
    public Action<DataSnapshot> OnNotesChanged;

    private FirebaseAuth auth;
    private FirebaseUser user;

    private void Awake()
    {
        FirebaseAuth.DefaultInstance.StateChanged += HandleAuthStateChanged;
        CheckUser();

        DontDestroyOnLoad(this);
    }

    private void HandleAuthStateChanged(object sender, EventArgs e)
    {
        CheckUser();
    }

    private void CheckUser()
    {
        if(FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
            auth = FirebaseAuth.DefaultInstance;
            user = auth.CurrentUser;

            SetUser();
        }
        else
        {
            auth = FirebaseAuth.GetAuth(Firebase.FirebaseApp.DefaultInstance);
            auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(OnSignedIn);
        }
    }

    private void OnUserDataChanged(object sender, ValueChangedEventArgs e)
    {
        GetPreviousNotes();
    }

    private void OnSignedIn(Task<FirebaseUser> signInTask)
    {
        user = signInTask.Result;
        if (signInTask.IsFaulted || signInTask.IsCanceled) Debug.Log("Failed authorization");

        SetUser();
    }

    private void SetUser()
    {
        var reference = FirebaseDatabase.DefaultInstance.GetReference("notes");
        reference.ValueChanged += OnUserDataChanged;

        AppController.username = auth.CurrentUser.DisplayName;
        AppController.userId = user.UserId;
    }

    public void ShareNote(NoteData note)
    {
        var jsonNote = JsonUtility.ToJson(note);

        FirebaseDatabase.DefaultInstance.GetReference("notes").Child(note.noteId).SetRawJsonValueAsync(jsonNote);
    }

    public void DeleteNote(string noteId)
    {
        FirebaseDatabase.DefaultInstance.GetReference("notes").Child(noteId).RemoveValueAsync();
    }

    public void EditNote(NoteData newNote)
    {
        FirebaseDatabase.DefaultInstance.GetReference("notes").Child(newNote.noteId).RemoveValueAsync();

        ShareNote(newNote);
    }

    public void GetPreviousNotes()
    {
        FirebaseDatabase.DefaultInstance.GetReference("notes").OrderByChild("counter").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("Failed to get notes");
            }
            else if (task.IsCompleted)
            {
                OnNotesChanged(task.Result);
            }
        });
    }
}
