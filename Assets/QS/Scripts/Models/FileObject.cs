using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Storage;
using UnityEngine.Networking;
using Firebase.Extensions;
using System;
using UnityEngine.EventSystems;

namespace QualiumSystems
{
    public class FileObject : NoteObject, IPointerDownHandler
    {
        [SerializeField] private RawImage rawImage;
        [SerializeField] private TextMeshProUGUI authorText;

        private const float colorTransparency = 0.6f;

        // Set the value of variables after object creating and then call LoadImage
        public override void NoteInit(NoteData noteData, Action<NoteObject> noteSelected)
        {
            base.NoteInit(noteData, noteSelected);

            highlited = Color.white * colorTransparency;
            common = Color.clear;

            FirebaseStorage storage = FirebaseStorage.DefaultInstance;
            StorageReference storageReference = storage.GetReferenceFromUrl(GalleryUpload.StorageUrl);

            StorageReference image = storageReference.Child(GalleryUpload.StorageDataPath + noteData.fileLink);

            //Get the download link of file
            image.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
            {
                if (task.Exception is null)
                {
                    StartCoroutine(LoadImage(Convert.ToString(task.Result))); //Fetch file from the link

                    return;
                }

                Debug.LogError(task.Exception.Message);
            });

            authorText.text = noteData.username;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            onNoteSelected?.Invoke(this);
        }

        // Load image from url and assign it to rawImage texture
        private IEnumerator LoadImage(string MediaUrl)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl); //Create a request

            yield return request.SendWebRequest(); //Wait for the request to complete

            if (request.error is null)
            {
                rawImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;

                yield break;
            }

            Debug.LogError(request.error);
        }
    }
}
