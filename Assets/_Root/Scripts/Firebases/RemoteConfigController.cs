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
    [NonSerialized] public bool isShowBanner = false;
    [NonSerialized] public bool isShowAppOpen = false;


    public static bool versionCompare(string v1, string v2)
    {
        // vnum stores each numeric
        // part of version
        int vnum1 = 0, vnum2 = 0;

        // loop until both string are
        // processed
        for (int i = 0, j = 0; (i < v1.Length || j < v2.Length);)
        {

            // storing numeric part of
            // version 1 in vnum1
            while (i < v1.Length && v1[i] != '.')
            {
                vnum1 = vnum1 * 10 + (v1[i] - '0');
                i++;
            }

            // storing numeric part of
            // version 2 in vnum2
            while (j < v2.Length && v2[j] != '.')
            {
                vnum2 = vnum2 * 10 + (v2[j] - '0');
                j++;
            }

            if (vnum1 > vnum2)
                return false;
            if (vnum2 > vnum1)
                return true;

            // if equal, reset variables and
            // go for next numeric part
            vnum1 = vnum2 = 0;
            i++;
            j++;
        }
        return false;
    }
    public bool HasNewUpdate => Application.version.CompareTo(CurrentVersion.Replace("_",".")) == -1;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        Debug.Log("App version: " + Application.version);
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
        defaults.Add(Constants.IS_SHOW_BANNER, true);
        defaults.Add(Constants.IS_SHOW_APP_OPEN, true);
        Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults);
    }

    public Task FetchDataAsync()
    {
        Task fetchTask = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
        return fetchTask.ContinueWith(FetchComplete);
    }

    private void FetchComplete(Task fetchTask)
    {
        var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
        switch (info.LastFetchStatus)
        {
            case Firebase.RemoteConfig.LastFetchStatus.Success:
                    Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync();
                    Debug.Log(String.Format("Remote data loaded and ready (last fetch time {0}).",
                    info.FetchTime));
                    break;
                case Firebase.RemoteConfig.LastFetchStatus.Failure:
                    switch (info.LastFetchFailureReason)
                    {
                        case Firebase.RemoteConfig.FetchFailureReason.Error:
                            Debug.Log("Fetch failed for unknown reason");
                            break;
                        case Firebase.RemoteConfig.FetchFailureReason.Throttled:
                            Debug.Log("Fetch throttled until " + info.ThrottledEndTime);
                            break;
                    }
                    break;
                case Firebase.RemoteConfig.LastFetchStatus.Pending:
                    Debug.Log("Latest Fetch call still pending.");
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
        Debug.Log("Cont Level win show Ads: " + CountLevelWinShowAds);
        
        InterstitalTimeLevelCompleted = int.Parse(GetConfig(Constants.INTERSTITIAL_TIME_LEVEL_COMPLETED));
        InterstitalTimeOnLoseCompleted = int.Parse(GetConfig(Constants.INTERSTITIAL_TIME_LEVEL_ON_LOSE_COMPLETED));
        HasCrossAds = bool.Parse(GetConfig(Constants.HAS_CROSS_ADS));
        IsShowInterLose = bool.Parse(GetConfig(Constants.IS_SHOW_INTER_LOSE));
        isShowBanner = bool.Parse(GetConfig(Constants.IS_SHOW_BANNER));
        isShowAppOpen = bool.Parse(GetConfig(Constants.IS_SHOW_APP_OPEN));
        Debug.Log(isShowBanner + "showban");
        Debug.Log(IsShowInterLose + "show interlose");
#if !UNITY_EDITOR
        HasIntro = bool.Parse(GetConfig(Constants.HAS_INTRO));
#endif
#if UNITY_ANDROID
        OnlyAdmob = bool.Parse(GetConfig(Constants.ONLY_ADMOB_ANDROID));
        Debug.Log(OnlyAdmob + " only admod");
        CurrentVersion = GetConfig(Constants.CURRENT_VERSION_ANDROID);
        Debug.Log("Current version firebase: " + CurrentVersion);
        UpdateDescription = GetConfig(Constants.ANDROID_UPDATE_DESCRIPTION);
        EnableFbLogin = true;
#elif UNITY_IOS
        OnlyAdmob = bool.Parse(GetConfig(Constants.ONLY_ADMOB_IOS));
        CurrentVersion = GetConfig(Constants.CURRENT_VERSION_IOS);
        UpdateDescription = GetConfig(Constants.IOS_UPDATE_DESCRIPTION);
        EnableFbLogin = bool.Parse(GetConfig(Constants.ENABLE_FB_LOGIN_IOS));
#endif

        DOTween.Sequence().SetDelay(0.1f).OnComplete(() => {AdController.Instance.Init();}); 
    }

    public string GetConfig(string name)
    {
        return Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(name).StringValue;
    }
}