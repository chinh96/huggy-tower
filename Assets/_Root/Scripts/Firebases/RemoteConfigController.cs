using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Firebase.RemoteConfig;

public class RemoteConfigController : Singleton<RemoteConfigController>
{
    [NonSerialized] public int FirstOpenCountLevelWinTurnOnAds = 2;
    [NonSerialized] public int InterstitalTimeLevelCompleted = 30;
    [NonSerialized] public int CountLevelWinShowAds = 2;
    [NonSerialized] public string CurrentVersion = "";
    [NonSerialized] public string UpdateDescription = "";
    [NonSerialized] public bool OnlyAdmob = true;
    [NonSerialized] public bool EnableCastle = false;

    private FirebaseRemoteConfig firebaseRemoteConfig;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
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
    }

    private void InitalizeFirebase()
    {
        Dictionary<string, object> defaults = new Dictionary<string, object>();
        defaults.Add(Constants.FIRST_OPEN_COUNT_LEVEL_WIN_TURN_ON_ADS, 2);
        defaults.Add(Constants.INTERSTITIAL_TIME_LEVEL_COMPLETED, 30);
        defaults.Add(Constants.COUNT_LEVEL_WIN_SHOW_ADS, 2);
        defaults.Add(Constants.CURRENT_VERSION_ANDROID, "");
        defaults.Add(Constants.CURRENT_VERSION_IOS, "");
        defaults.Add(Constants.IOS_UPDATE_DESCRIPTION, "");
        defaults.Add(Constants.ANDROID_UPDATE_DESCRIPTION, "");
        defaults.Add(Constants.ONLY_ADMOB, true);
        defaults.Add(Constants.ONLY_ADMOB_ANDROID, true);
        defaults.Add(Constants.ONLY_ADMOB_IOS, true);

        firebaseRemoteConfig.SetDefaultsAsync(defaults);
    }

    public Task FetchDataAsync()
    {
        Task fetchTask = firebaseRemoteConfig.FetchAsync(TimeSpan.Zero);
        return fetchTask.ContinueWith(FetchComplete);
    }

    private void FetchComplete(Task fetchTask)
    {
        var info = firebaseRemoteConfig.Info;
        switch (info.LastFetchStatus)
        {
            case Firebase.RemoteConfig.LastFetchStatus.Success:
                firebaseRemoteConfig.ActivateAsync();
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
#if UNITY_ANDROID
        OnlyAdmob = bool.Parse(GetConfig(Constants.ONLY_ADMOB_ANDROID));
        CurrentVersion = GetConfig(Constants.CURRENT_VERSION_ANDROID);
        UpdateDescription = GetConfig(Constants.ANDROID_UPDATE_DESCRIPTION);
#elif UNITY_IOS
        onlyAdmob = bool.Parse(GetConfig(Constants.ONLY_ADMOB_IOS));
        currentVersion = GetConfig(Constants.CURRENT_VERSION_IOS);
        updateDescription = GetConfig(Constants.IOS_UPDATE_DESCRIPTION);
#endif
        AdController.Instance.Init();
    }

    public string GetConfig(string name)
    {
        return firebaseRemoteConfig.GetValue(name).StringValue;
    }
}