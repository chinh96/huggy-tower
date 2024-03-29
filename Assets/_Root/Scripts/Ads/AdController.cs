﻿using System;
using DG.Tweening;
using UnityEngine;
#if UNITY_IOS
using Unity.Advertisement.IosSupport;
using com.adjust.sdk;
#endif

public class AdController : Singleton<AdController>
{
    [NonSerialized] public bool JustShowReward = false;
    public bool inShowAds = false;
    public bool IsRewardLoaded => ad != null && ad.IsRewardLoaded;

    private IAd ad;
    private bool isRewardEarned = false;
    private Action handleRewardAfterEarned;
    private Action handleInterAfterClosed;
    private bool isShowBanner
    {
        get
        {
            return !Data.IsRemovedAds && RemoteConfigController.Instance.isShowBanner;
        }
    }
    private bool isShowInter
    {
        get
        {
            bool canShowInter = Data.CurrentLevel >= RemoteConfigController.Instance.FirstOpenCountLevelWinTurnOnAds;

            return canShowInter &&
                    GameController.Instance.Root.GetTotalLevelWin() >= RemoteConfigController.Instance.CountLevelWinShowAds &&
                    GameController.Instance.Root.GetTotalTimesPlay() >= RemoteConfigController.Instance.InterstitalTimeLevelCompleted &&
                    !JustShowReward &&
                    !Data.IsRemovedAds;
        }
    }
    private bool isShowInter2
    {
        get
        {
            bool canShowInter = Data.CurrentLevel >= RemoteConfigController.Instance.FirstOpenCountLevelWinTurnOnAds;

            return canShowInter &&
                    GameController.Instance.Root.GetTotalLevelWin() >= RemoteConfigController.Instance.CountLevelWinShowAds &&
                    GameController.Instance.Root.GetTotalTimesLose() >= RemoteConfigController.Instance.InterstitalTimeOnLoseCompleted &&
                    !JustShowReward &&
                    !Data.IsRemovedAds;
        }
    }
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
#if UNITY_EDITOR
        Init();
#elif UNITY_IOS
        // if (ATTrackingStatusBinding.GetAuthorizationTrackingStatus() == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
        // {
        //     ATTrackingStatusBinding.RequestAuthorizationTracking();
        // }
        
        if (Adjust.getAppTrackingAuthorizationStatus() == 0)
        {
            Adjust.requestTrackingAuthorizationWithCompletionHandler(_=>{
                
            });
        }
#endif
    }

    public void Init()
    {

#if UNITY_EDITOR
        ad = GetComponent<AdMobController>();
#else
        if (RemoteConfigController.Instance.OnlyAdmob)
        {
            ad = GetComponent<AdMobController>();
        }
        else
        {
            ad = (IAd)GetComponent<AdApplovinController>();
        }
#endif

        ad.Init(OnInterClosed, OnInterLoaded, OnRewardLoaded, OnRewardClosed, OnRewardEarned);

        Request();
        HideBanner();
    }

    public void Request()
    {
        RequestBanner();
        RequestInterstitial();
        RequestRewarded();
    }

    public void RequestBanner()
    {
        if (ad != null)
        {
            ad.RequestBanner();
        }
    }

    public void ShowBanner()
    {
        if (ad != null && ad.IsBannerLoaded)
        {
            if (isShowBanner)
            {
                ad.ShowBanner();
            }
            else
            {
                ad.HideBanner();
            }
        }
    }

    public void HideBanner()
    {
        if (ad != null && ad.IsBannerLoaded)
        {
            ad.HideBanner();
        }
    }

    public void RequestInterstitial()
    {
        if (ad != null && !ad.IsInterLoaded)
        {
            AnalyticController.RequestAdInterstitial();

            ad.RequestInterstitial();
        }
    }

    public void ShowInterstitial(Action action, bool isWin = true)
    {
        void Show()
        {
            bool check = false;
            if (isWin)
            {
                check = isShowInter;
            }
            else
            {
                check = isShowInter2;
            }
            if (ad != null && check)
            {
                if (ad.IsInterLoaded)
                {
                    AnalyticController.ImpressAdInterstitial();

                    handleInterAfterClosed = action;
                    ad.ShowInterstitial();
                    GameController.Instance.Root.ResetTotalTimesPlay();
                    GameController.Instance.Root.RestTotalTimesLose();
                    GameController.Instance.Root.ResetTotalLevelWin();
                    inShowAds = true;
                }
                else
                {
                    action?.Invoke();
                }
            }
            else
            {
                action?.Invoke();
            }
        }
#if UNITY_EDITOR
        if (ResourcesController.Config.EnableAds)
        {
            Show();
        }
        else
        {
            action?.Invoke();
        }
#else
        if (ResourcesController.Config.EnableTest)
        {
            action?.Invoke();
        }
        else
        {
            Show();
        }
#endif
    }

    public void RequestRewarded()
    {
        if (ad != null && !ad.IsRewardLoaded)
        {
            EventController.AdsRewardRequested?.Invoke();
            AnalyticController.RequestAdReward();

            ad.RequestRewarded();
        }
    }

    public void ShowRewardedAd(Action action)
    {
        void Show()
        {
            if (ad != null && ad.IsRewardLoaded)
            {
                AnalyticController.ImpressAdReward();

                handleRewardAfterEarned = action;
                ad.ShowRewardedAd();
                inShowAds = true;
            }
        }
#if UNITY_EDITOR
        if (ResourcesController.Config.EnableAds)
        {
            Show();
        }
        else
        {
            action?.Invoke();
        }
#else
        if (ResourcesController.Config.EnableTest)
        {
            action?.Invoke();
        }
        else
        {
            Show();
        }
#endif
    }

    public void OnInterClosed()
    {
        ad.RequestInterstitial();
        DOTween.Sequence().AppendInterval(.1f).AppendCallback(() => handleInterAfterClosed?.Invoke());
    }

    public void OnInterLoaded()
    {

    }

    public void OnRewardClosed()
    {
        RequestRewarded();

        DOTween.Sequence().AppendInterval(.1f).AppendCallback(() =>
        {
            if (isRewardEarned)
            {
                isRewardEarned = false;
                handleRewardAfterEarned?.Invoke();
            }
        });
    }

    public void OnRewardLoaded()
    {
        EventController.AdsRewardLoaded?.Invoke();
    }

    public void OnRewardEarned()
    {
        JustShowReward = true;
        isRewardEarned = true;

        ResourcesController.DailyQuest.IncreaseByType(DailyQuestType.WatchVideoReward);
    }
}
