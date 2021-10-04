using Firebase.Extensions;
using Firebase.Storage;
using SimpleFileBrowser;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GalleryUpload : MonoBehaviour
{
	public Action<string> OnFileUploaded;

	[SerializeField] private GameObject preview;
	[SerializeField] private RawImage previewImage;

	private FirebaseStorage storage;
	private StorageReference storageReference;

	private string path;

	private void Start()
    {
		storage = FirebaseStorage.DefaultInstance;
		storageReference = storage.GetReferenceFromUrl("gs://noteapplication2.appspot.com/");
	}

    public void OnUploadButtonPressed()
    {
		PickImage(512);
    }

	private void PickImage(int maxSize)
	{
		NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
		{
			Debug.Log("Image path: " + path);
			if (path != null)
			{
				// Create Texture from selected image
				Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize);
				if (texture == null)
				{
					Debug.Log("Couldn't load texture from " + path);
					return;
				}

				this.path = path;
				SendPreview(texture);
			}
		});

		Debug.Log("Permission result: " + permission);
	}

	private void SendPreview(Texture2D texture)
    {
		preview.SetActive(true);
		previewImage.texture = texture;
	}

	public void OnSendButtonPressed()
    {
		UploadToDatabase();
		preview.SetActive(false);
	}

	public void OnCancelButtonPressed()
    {
		preview.SetActive(false);
	}

	private void UploadToDatabase()
    {
		byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(path);
		//Editing Metadata
		var newMetadata = new MetadataChange();
		newMetadata.ContentType = "image/jpeg";

		//Create a reference to where the file needs to be uploaded
		StorageReference uploadRef = storageReference.Child("uploads/" + path);
		Debug.Log("File upload started");

		uploadRef.PutBytesAsync(bytes, newMetadata).ContinueWithOnMainThread((task) => {
			if (task.IsFaulted || task.IsCanceled)
			{
				Debug.Log(task.Exception.ToString());
			}
			else
			{
				Debug.Log("File Uploaded Successfully!");
				OnFileUploaded(path);
			}
		});
	}
}
