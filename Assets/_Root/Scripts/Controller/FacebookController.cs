using System;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using UnityEngine;

public class FacebookController : Singleton<FacebookController>
{
    public Action onLoginComplete;
    public Action onLoginFaild;
    public Action onLogoutComplete;
    public Action onLoginError;
    public string UserId { get; set; }
    public string Token { get; set; }
    public bool IsInitialized => FB.IsInitialized;
    public bool IsLoggedIn => FB.IsLoggedIn;
    private IDisposable _disposableLogout;
    private Coroutine _coroutine;

    // Awake function from Unity's MonoBehavior
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        UserId = "";
        Token = "";

        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }

    #region login

    public void Login(
        Action onComplete = null,
        Action onFaild = null,
        Action onError = null)
    {
        onLoginComplete = onComplete;
        onLoginFaild = onFaild;
        onLoginError = onError;
        var perms = new List<string>() {"public_profile", "email"};
        FB.LogInWithReadPermissions(perms, AuthCallback);
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...

            if (FB.IsLoggedIn)
            {
                // todo load.
                var token = AccessToken.CurrentAccessToken;
                UserId = token.UserId;
                Token = token.TokenString;
            }
        }
        else
        {
            //todo Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(
        bool isGameShown)
    {
        Time.timeScale = !isGameShown ? 0 : 1;
    }

    private void AuthCallback(
        ILoginResult result)
    {
        if (result.Error != null)
        {
            onLoginError?.Invoke();
            // todo error login
            return;
        }

        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var token = AccessToken.CurrentAccessToken;
            UserId = token.UserId;
            Token = token.TokenString;
            // todo aToken.TokenString;
            onLoginComplete?.Invoke();
            onLoginComplete = null;
        }
        else
        {
            //todo User cancelled login
            onLoginFaild?.Invoke();
            onLoginFaild = null;
        }
    }

    #endregion

    #region logout

    public void Logout(
        Action onComplete = null)
    {
        onLogoutComplete = onComplete;
        if (FB.IsLoggedIn)
        {
            FB.LogOut();

            _coroutine = StartCoroutine(IeLogoutSuccess());
        }
    }

    private IEnumerator IeLogoutSuccess()
    {
        if (FB.IsLoggedIn)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            if (_coroutine != null) StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(IeLogoutSuccess());
        }
        else
        {
            onLogoutComplete?.Invoke();
            onLogoutComplete = null;
            Token = "";
            UserId = "";
        }
    }

    #endregion
}