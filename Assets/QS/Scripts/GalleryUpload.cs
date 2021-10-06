using Firebase.Extensions;
using Firebase.Storage;
using SimpleFileBrowser;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace QualiumSystems
{
	public class GalleryUpload : MonoBehaviour
	{
		public const string StorageUrl = "gs://noteapplication2.appspot.com/";
		public const string StorageDataPath = "uploads/";

		public Action<string> onFileUploaded;

		[SerializeField] private GameObject preview;
		[SerializeField] private RawImage previewImage;

		private FirebaseStorage storage;
		private StorageReference storageReference;

		private string path;
		
		private const string ContentType = "image/jpeg";
		private const int MaxSizeOfUploadedFile = 512;

		private void Start()
		{
			storage = FirebaseStorage.DefaultInstance;
			storageReference = storage.GetReferenceFromUrl(StorageUrl);
		}

		// When we press on button - open file explorer to choose the file
		public void OnUploadButtonPressed()
		{
			PickImage(MaxSizeOfUploadedFile);
		}

		// Create texture using image from path, save path and call SendPreview with created texture
		private void PickImage(int maxSize)
		{
			NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
			{
				if (path is null) return;

				// Create Texture from selected image
				Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize);
				if (texture is null)
				{
					Debug.LogWarning("Couldn't load texture from " + path);
					return;
				}

				this.path = path;
				SendPreview(texture);
			});
		}

		// Show preview of image to user
		private void SendPreview(Texture2D texture)
		{
			preview.SetActive(true);
			previewImage.texture = texture;
		}

		// Close preview and call UploadToDatabase
		public void OnSendButtonPressed()
		{
			UploadToDatabase();
			preview.SetActive(false);
		}

		// Close preview
		public void OnCancelButtonPressed() => preview.SetActive(false);

		// Get bytes from path, create link and upload file to storage
		private void UploadToDatabase()
		{
			byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(path);

			//Editing Metadata
			var newMetadata = new MetadataChange();
			newMetadata.ContentType = ContentType;

			//Create a reference to where the file needs to be uploaded
			StorageReference uploadRef = storageReference.Child(StorageDataPath + path);

			uploadRef.PutBytesAsync(bytes, newMetadata).ContinueWithOnMainThread((task) =>
			{
				if (task.Exception is null)
				{
					onFileUploaded(path);

					return;
				}

				Debug.LogError(task.Exception.Message);
			});
		}
	}
}
