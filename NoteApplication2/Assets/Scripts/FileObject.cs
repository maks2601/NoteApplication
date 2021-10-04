using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Storage;
using UnityEngine.Networking;
using Firebase.Extensions;
using System;
using UnityEngine.EventSystems;

public class FileObject : NoteObject, IPointerDownHandler
{
    [SerializeField] private RawImage rawImage;
    [SerializeField] private TextMeshProUGUI authorText;

    public override void NoteInit(NoteData noteData, Action<NoteObject> noteSelected)
    {
        base.NoteInit(noteData, noteSelected);

        highlited = new Color(0.6f, 0.6f, 0.6f, 0.5f);
        common = new Color(0.6f, 0.6f, 0.6f, 0);

        FirebaseStorage storage = FirebaseStorage.DefaultInstance;
        StorageReference storageReference = storage.GetReferenceFromUrl("gs://noteapplication2.appspot.com/");

        StorageReference image = storageReference.Child("uploads/" + noteData.fileLink);

        //Get the download link of file
        image.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
        {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                StartCoroutine(LoadImage(Convert.ToString(task.Result))); //Fetch file from the link
            }
            else
            {
                Debug.Log(task.Exception);
            }
        });

        authorText.text = noteData.username;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        noteSelected(this);
    }

    public override void NoteHighlited()
    {
        base.NoteHighlited();
    }

    IEnumerator LoadImage(string MediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl); //Create a request
        yield return request.SendWebRequest(); //Wait for the request to complete
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            // setting the loaded image to our object
            rawImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
    }
}
