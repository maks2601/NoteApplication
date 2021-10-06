using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;
using UnityEngine.SceneManagement;
using Firebase.Extensions;

namespace QualiumSystems
{
    public class EmailAuth : MonoBehaviour
    {
        [SerializeField] private GameObject loginUI;
        [SerializeField] private GameObject registerUI;

        [Header("Login")]
        [SerializeField] private TMP_InputField emailLoginField;
        [SerializeField] private TMP_InputField passwordLoginField;
        [SerializeField] private TextMeshProUGUI warningLoginText;
        [SerializeField] private GameObject loginButton;

        [Header("Register")]
        [SerializeField] private TMP_InputField usernameRegisterField;
        [SerializeField] private TMP_InputField emailRegisterField;
        [SerializeField] private TMP_InputField passwordRegisterField;
        [SerializeField] private TMP_InputField passwordRegisterVerifyField;
        [SerializeField] private TextMeshProUGUI warningRegisterText;
        [SerializeField] private GameObject registerButton;

        private FirebaseAuth auth;
        private FirebaseUser user;
        private FirebaseException firebaseEx;

        private void Start()
        {
            auth = FirebaseAuth.DefaultInstance;
            UIAnimator.ElementAppear(loginUI.transform);
        }

        // When click on login button - login with specified email and password
        public void LoginButtonPressed()
        {
            UIAnimator.ElementTapResponse(loginButton.transform);
            Login(emailLoginField.text, passwordLoginField.text);
        }

        // When click on register button - register with specified email, password and username
        public void RegisterButtonPressed()
        {
            UIAnimator.ElementTapResponse(registerButton.transform);
            Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text);
        }

        // When click on button - we will get to the login page
        public void LoginScreenButtonPressed()
        {
            loginUI.SetActive(true);
            registerUI.SetActive(false);
            UIAnimator.ElementAppear(loginUI.transform);
        }

        // When click on button - we will get to the register page
        public void RegisterScreenButtonPressed()
        {
            loginUI.SetActive(false);
            registerUI.SetActive(true);
            UIAnimator.ElementAppear(registerUI.transform);
        }

        // Login in Firebase with specified email and password
        private void Login(string email, string password)
        {
            auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => 
            {
                if (task.Exception is null)
                {
                    user = task.Result;
                    warningLoginText.text = default;
                    SceneManager.LoadScene(1);

                    return;
                }

                // If there are errors handle them
                firebaseEx = task.Exception.GetBaseException() as FirebaseException;
                warningLoginText.text = firebaseEx.Message;
                UIAnimator.ElementShake(warningLoginText.transform);
            });
        }

        // Register in Firebase with specified email, password and username
        private void Register(string email, string password, string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                warningRegisterText.text = "Missing Username";
                UIAnimator.ElementShake(warningRegisterText.transform);

                return;
            }
            if (!passwordRegisterField.text.Equals(passwordRegisterVerifyField.text))
            {
                warningRegisterText.text = "Password Does Not Match!";
                UIAnimator.ElementShake(warningRegisterText.transform);

                return;
            }

            auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
            {
                if (task.Exception is null)
                {
                    // User has now been created
                    user = task.Result;

                    if (user is null) return;

                    // Create a user profile and set the username
                    UserProfile profile = new UserProfile { DisplayName = username };
                    CreateProfile(profile);

                    return;
                }

                // If there are errors handle them
                firebaseEx = task.Exception.GetBaseException() as FirebaseException;
                warningRegisterText.text = firebaseEx.Message.Normalize();

                UIAnimator.ElementShake(warningRegisterText.transform);
            });
        }

        // Call the Firebase auth update user profile function passing the profile with the username
        private void CreateProfile(UserProfile profile)
        {
            user.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(task =>
            {
                if (task.Exception is null)
                {
                    LoginScreenButtonPressed();
                    warningRegisterText.text = default;

                    return;
                }

                //If there are errors handle them
                firebaseEx = task.Exception.GetBaseException() as FirebaseException;
                warningRegisterText.text = firebaseEx.Message;

                UIAnimator.ElementShake(warningRegisterText.transform);
            });
        }
    }
}
