using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TwitterKit.Unity;
using Firebase.Auth;

public class TwitterAuth : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseUser user;

    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;

        Twitter.Init();
    }

    public void StartLogin()
    {
        TwitterSession session = Twitter.Session;
        if (session == null)
        {
            Twitter.LogIn(LoginComplete, LoginFailure);
        }
        else
        {
            LoginComplete(session);
        }
    }

    public void LoginComplete(TwitterSession session)
    {
        FirebaseConnect(session.authToken.token, session.authToken.secret);
    }

    public void LoginFailure(ApiError error)
    {
        Debug.Log("code=" + error.code + " msg=" + error.message);
    }

    private void FirebaseConnect(string accessToken, string secret)
    {
        Credential credential = TwitterAuthProvider.GetCredential(accessToken, secret);
        auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            user = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                user.DisplayName, user.UserId);
        });
    }
}
