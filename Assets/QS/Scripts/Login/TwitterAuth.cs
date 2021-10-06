using UnityEngine;
using TwitterKit.Unity;
using Firebase.Auth;
using UnityEngine.SceneManagement;
using Firebase.Extensions;

namespace QualiumSystems
{
    public class TwitterAuth : MonoBehaviour
    {
        private FirebaseAuth auth;
        private FirebaseUser user;

        private void Start()
        {
            auth = FirebaseAuth.DefaultInstance;

            Twitter.Init();
        }

        // When we press on button to login via Twitter
        public void StartLogin()
        {
            TwitterSession session = Twitter.Session;
            if (session is null)
            {
                Twitter.LogIn(LoginComplete, LoginFailure);
            }
            else
            {
                LoginComplete(session);
            }
        }

        // Login success handling
        private void LoginComplete(TwitterSession session) => FirebaseConnect(session.authToken.token, session.authToken.secret);

        // Failed login handling
        private void LoginFailure(ApiError error) => Debug.LogError("code=" + error.code + " msg=" + error.message);

        // Using Twitter login data to login in Firebase
        private void FirebaseConnect(string accessToken, string secret)
        {
            Credential credential = TwitterAuthProvider.GetCredential(accessToken, secret);
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
