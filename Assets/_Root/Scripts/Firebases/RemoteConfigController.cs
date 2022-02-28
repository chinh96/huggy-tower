using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

public class RemoteConfigController : Singleton<RemoteConfigController>
{
    [NonSerialized] public int FirstOpenCountLevelWinTurnOnAds = 2;
    [NonSerialized] public int InterstitalTimeLevelCompleted = 30;
    [NonSerialized] public int InterstitalTimeOnLoseCompleted = 30;
    [NonSerialized] public int CountLevelWinShowAds = 2;
    [NonSerialized] public string CurrentVersion = "0";
    [NonSerialized] public string UpdateDescription = "0";
    [NonSerialized] public bool OnlyAdmob = true;
    [NonSerialized] public bool EnableFbLogin = false;
    [NonSerialized] public bool HasIntro = true;
    [NonSerialized] public bool HasCrossAds = false;
    [NonSerialized] public bool IsShowInterLose = false;

    public bool HasNewUpdate => float.Parse(Application.version) < float.Parse(CurrentVersion);

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        DOTween.Sequence().AppendInterval(.5f).AppendCallback(() =>
        {
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                switch (dependencyStatus)
                {
                    case Firebase.DependencyStatus.Available:
                        Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;
                        InitalizeFirebase();
                        FetchDataAsync();
                        break;
                    default:
                        Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                        break;
                }
            });
        });
    }

    private void InitalizeFirebase()
    {
        Dictionary<string, object> defaults = new Dictionary<string, object>();
        defaults.Add(Constants.FIRST_OPEN_COUNT_LEVEL_WIN_TURN_ON_ADS, 2);
        defaults.Add(Constants.INTERSTITIAL_TIME_LEVEL_COMPLETED, 30);
        defaults.Add(Constants.INTERSTITIAL_TIME_LEVEL_ON_LOSE_COMPLETED, 30);
        defaults.Add(Constants.COUNT_LEVEL_WIN_SHOW_ADS, 2);
        defaults.Add(Constants.CURRENT_VERSION_ANDROID, "0");
        defaults.Add(Constants.CURRENT_VERSION_IOS, "0");
        defaults.Add(Constants.IOS_UPDATE_DESCRIPTION, "");
        defaults.Add(Constants.ANDROID_UPDATE_DESCRIPTION, "");
        defaults.Add(Constants.ONLY_ADMOB, true);
        defaults.Add(Constants.ONLY_ADMOB_ANDROID, true);
        defaults.Add(Constants.ONLY_ADMOB_IOS, true);
        defaults.Add(Constants.ENABLE_FB_LOGIN_IOS, false);
        defaults.Add(Constants.HAS_INTRO, true);
        defaults.Add(Constants.HAS_CROSS_ADS, false);
        defaults.Add(Constants.IS_SHOW_INTER_LOSE, false);
        Firebase.RemoteConfig.FirebaseRemoteConfig.SetDefaults(defaults);
    }

    public Task FetchDataAsync()
    {
        Task fetchTask = Firebase.RemoteConfig.FirebaseRemoteConfig.FetchAsync(TimeSpan.Zero);
        return fetchTask.ContinueWith(FetchComplete);
    }

    private void FetchComplete(Task fetchTask)
    {
        var info = Firebase.RemoteConfig.FirebaseRemoteConfig.Info;
        switch (info.LastFetchStatus)
        {
            case Firebase.RemoteConfig.LastFetchStatus.Success:
                Firebase.RemoteConfig.FirebaseRemoteConfig.ActivateFetched();
                break;
            case Firebase.RemoteConfig.LastFetchStatus.Failure:
                switch (info.LastFetchFailureReason)
                {
                    case Firebase.RemoteConfig.FetchFailureReason.Error:
                        Debug.LogError("Fetch failed for unknown reason");
                        break;
                    case Firebase.RemoteConfig.FetchFailureReason.Throttled:
                        Debug.LogError("Fetch throttled until " + info.ThrottledEndTime);
                        break;
                }

                break;
            case Firebase.RemoteConfig.LastFetchStatus.Pending:
                Debug.LogError("Latest Fetch call still pending.");
                break;
        }

        if (fetchTask.IsCanceled)
        {
        }
        else if (fetchTask.IsFaulted)
        {
        }
        else if (fetchTask.IsCompleted)
        {
        }

        FirstOpenCountLevelWinTurnOnAds = int.Parse(GetConfig(Constants.FIRST_OPEN_COUNT_LEVEL_WIN_TURN_ON_ADS));
        CountLevelWinShowAds = int.Parse(GetConfig(Constants.COUNT_LEVEL_WIN_SHOW_ADS));
        InterstitalTimeLevelCompleted = int.Parse(GetConfig(Constants.INTERSTITIAL_TIME_LEVEL_COMPLETED));
        InterstitalTimeOnLoseCompleted = int.Parse(GetConfig(Constants.INTERSTITIAL_TIME_LEVEL_ON_LOSE_COMPLETED));
        HasCrossAds = bool.Parse(GetConfig(Constants.HAS_CROSS_ADS));
        IsShowInterLose = bool.Parse(GetConfig(Constants.IS_SHOW_INTER_LOSE));

#if !UNITY_EDITOR
        HasIntro = bool.Parse(GetConfig(Constants.HAS_INTRO));
#endif
#if UNITY_ANDROID
        OnlyAdmob = bool.Parse(GetConfig(Constants.ONLY_ADMOB_ANDROID));
        CurrentVersion = GetConfig(Constants.CURRENT_VERSION_ANDROID);
        UpdateDescription = GetConfig(Constants.ANDROID_UPDATE_DESCRIPTION);
        EnableFbLogin = true;
#elif UNITY_IOS
        OnlyAdmob = bool.Parse(GetConfig(Constants.ONLY_ADMOB_IOS));
        CurrentVersion = GetConfig(Constants.CURRENT_VERSION_IOS);
        UpdateDescription = GetConfig(Constants.IOS_UPDATE_DESCRIPTION);
        EnableFbLogin = bool.Parse(GetConfig(Constants.ENABLE_FB_LOGIN_IOS));
#endif

        AdController.Instance.Init();
    }

    public string GetConfig(string name)
    {
        return Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(name).StringValue;
    }
}