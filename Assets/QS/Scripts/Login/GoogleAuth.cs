using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Google;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace QualiumSystems
{
    public class GoogleAuth : MonoBehaviour
    {
        [SerializeField] private string webClientId;

        private FirebaseAuth auth;
        private FirebaseUser user;
        private GoogleSignInConfiguration configuration;

        private void Start()
        {
            configuration = new GoogleSignInConfiguration { WebClientId = webClientId, RequestEmail = true, RequestIdToken = true };
            CheckFirebaseDependencies();
        }

        // Setup Firebase auth
        private void CheckFirebaseDependencies()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                if (task.Exception is null && task.Result == DependencyStatus.Available)
                {
                    auth = FirebaseAuth.DefaultInstance;

                    return;
                }

                Debug.LogError(task.Exception.Message);
            });
        }

        // Sign in when click on button
        public void SignInWithGoogle() => OnSignIn();

        // Sign out when click on button
        public void SignOutFromGoogle() => OnSignOut();

        // Setup GoogleSignIn configuration then sign in and call OnAuthenticationFinished
        private void OnSignIn()
        {
            GoogleSignIn.Configuration = configuration;
            GoogleSignIn.Configuration.UseGameSignIn = false;
            GoogleSignIn.Configuration.RequestIdToken = true;

            GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
        }

        // Sign out
        private void OnSignOut()
        {
            GoogleSignIn.DefaultInstance.SignOut();
        }

        // Disconnect
        public void OnDisconnect()
        {
            GoogleSignIn.DefaultInstance.Disconnect();
        }

        // SignIn on Firebase or catch exception
        internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
        {
            if (task.Exception is null)
            {
                SignInWithGoogleOnFirebase(task.Result.IdToken);

                return;
            }

            Debug.LogError(task.Exception.Message);
        }

        // Firebase login using google id token
        private void SignInWithGoogleOnFirebase(string idToken)
        {
            Credential credential = GoogleAuthProvider.GetCredential(idToken, null);

            auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
            {
                // Successful login
                if (task.Exception is null)
                {
                    user = task.Result;
                    SceneManager.LoadScene(1);

                    return;
                }

                Debug.LogError(task.Exception.Message);
            });
        }

        // When button is clicked silently sign in with configuration and call OnAuthenticationFinished
        public void OnSignInSilently()
        {
            GoogleSignIn.Configuration = configuration;
            GoogleSignIn.Configuration.UseGameSignIn = false;
            GoogleSignIn.Configuration.RequestIdToken = true;

            GoogleSignIn.DefaultInstance.SignInSilently().ContinueWith(OnAuthenticationFinished);
        }
    }
}