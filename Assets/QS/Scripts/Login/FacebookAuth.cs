using Facebook.Unity;
using Firebase.Auth;
using Firebase.Extensions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace QualiumSystems
{
    public class FacebookAuth : MonoBehaviour
    {
        private FirebaseAuth auth;
        private FirebaseUser user;

        private List<string> perms = new List<string>() { "public_profile", "email" };

        private void Awake()
        {
            if (FB.IsInitialized)
            {
                FB.ActivateApp();

                return;
            }

            // Initialize the Facebook SDK
            FB.Init(InitCallback);
        }

        private void Start()
        {
            auth = FirebaseAuth.DefaultInstance;
        }

        // Processing the result of Facebook initialization
        private void InitCallback()
        {
            if (FB.IsInitialized)
            {
                FB.ActivateApp();

                return;
            }

            Debug.LogError("Failed to Initialize the Facebook SDK");
        }

        // Login via facebook when user clicks on Facebook login button
        public void OnFacebookLoginButtonPressed() => FB.LogInWithReadPermissions(perms, AuthCallback);

        // Processing the result of the login via Facebook
        private void AuthCallback(ILoginResult result)
        {
            if (FB.IsLoggedIn)
            {
                var aToken = AccessToken.CurrentAccessToken;
                FirebaseConnect(aToken.TokenString);

                return;
            }

            Debug.LogError("User cancelled login");
        }

        // Using Facebook access token of logged account to login in Firebase 
        private void FirebaseConnect(string accessToken)
        {
            Credential credential = FacebookAuthProvider.GetCredential(accessToken);

            auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
            {
                if (task.Exception is null)
                {
                    user = task.Result;
                    SceneManager.LoadScene(1);

                    return;
                }

                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
            });
        }
    }
}
