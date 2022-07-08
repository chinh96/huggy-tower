using System;
using com.adjust.sdk;

using UnityEngine;

public class ApplovinManager : MonoBehaviour
{
    private Action<bool> _acRewarded;
    private Action _acInter;

    public void InitAds()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
        {
            Debug.Log("[Max] init completed!");
            // AppLovin SDK is initialized, start loading ads
            InitializeBannerAds();
            InitializeInterstitialAds();
            InitializeRewardedAds();
        };

        MaxSdk.SetSdkKey("-feJa9bEGOmZW95XxkyfhE2R_yHQ4poWZofsvPWhIw_es2dT16vUIDRoHKX63m6a9JD7wX1Q0PZ0Qng8EukpFT");
        MaxSdk.InitializeSdk();
    }

    #region banner

    public string bannerAdUnitId = "374b0eca17857934"; // Retrieve the ID from your account

    public void InitializeBannerAds()
    {
        // Banners are automatically sized to 320×50 on phones and 728×90 on tablets
        // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
        MaxSdk.CreateBanner(bannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);

        // Set background or background color for banners to be fully functional
        MaxSdk.SetBannerBackgroundColor(bannerAdUnitId, Color.black);
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        Debug.Log("[Max] banner init!");
    }

    public void ShowBanner()
    {
        Debug.Log("[Max] banner show!");
        MaxSdk.ShowBanner(bannerAdUnitId);

    }

    public void HideBanner()
    {
        MaxSdk.HideBanner(bannerAdUnitId);
    }

    #endregion

    #region intertitial

    public string intertitialAdUnit = "966f9f092c3d91c3";
    private int _retryAttempt;

    public void InitializeInterstitialAds()
    {
        // Attach callback
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;

        // Load the first interstitial
        LoadInterstitial();
    }

    private void LoadInterstitial()
    {
        MaxSdk.LoadInterstitial(intertitialAdUnit);
        //MyAnalytic.AdInterstitalRequest();
        Debug.Log("[Max] intertitial init!");

    }

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'

        // Reset retry attempt
        _retryAttempt = 0;
    }

    private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Interstitial ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)

        _retryAttempt++;
        double retryDelay = Mathf.Pow(2, Mathf.Min(6, _retryAttempt));

        Invoke("LoadInterstitial", (float)retryDelay);
        Debug.Log("[Max] intertitial load fail!");
    }

    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
        LoadInterstitial();
    }

    private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is hidden. Pre-load the next ad.
        //AdjustLog.AdjustLogEventAdsInterFL();
        _acInter?.Invoke();

        LoadInterstitial();
    }

    public void ShowIntertitial(Action acInter)
    {
        Debug.Log("[Max] intertitial show!");
        if (MaxSdk.IsInterstitialReady(intertitialAdUnit))
        {
            _acInter = acInter;
            MaxSdk.ShowInterstitial(intertitialAdUnit);
        }
    }

    #endregion

    #region reward

    public string rewardAdUnit = "e2ca96221a19fc79";
    private int _rewardRetryAttempt;

    private bool _isLoadedRewardAds = false;
    public bool IsLoadedRewardAds => _isLoadedRewardAds;
    public void InitializeRewardedAds()
    {
        // Attach callback
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

        // Load the first rewarded ad
        LoadRewardedAd();
    }

    private void LoadRewardedAd()
    {
        MaxSdk.LoadRewardedAd(rewardAdUnit);
        //MyAnalytic.AdRewardRequest();
        Debug.Log("[Max] reward init!");


    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.

        // Reset retry attempt
        _rewardRetryAttempt = 0;
        _isLoadedRewardAds = true;
    }

    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).

        _rewardRetryAttempt++;
        double retryDelay = Mathf.Pow(2, Mathf.Min(6, _rewardRetryAttempt));

        Invoke("LoadRewardedAd", (float)retryDelay);
        Debug.Log("[Max] reward load fail!");
        _isLoadedRewardAds = false;
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
        LoadRewardedAd();
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is hidden. Pre-load the next ad
        _acRewarded?.Invoke(true);
        LoadRewardedAd();
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        // The rewarded ad displayed and the user should receive the reward.
    }

    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Ad revenue paid. Use this callback to track user revenue.
        // send ad revenue info to Adjust
        AdjustAdRevenue adRevenue = new AdjustAdRevenue(AdjustConfig.AdjustAdRevenueSourceAppLovinMAX);
        adRevenue.setRevenue(adInfo.Revenue, "USD");
        adRevenue.setAdRevenueNetwork(adInfo.NetworkName);
        adRevenue.setAdRevenuePlacement(adInfo.Placement);
        adRevenue.setAdRevenueUnit(adInfo.AdUnitIdentifier);
        Adjust.trackAdRevenue(adRevenue);
    }


    public void ShowReward(Action<bool> onClaimReward)
    {
        Debug.Log("[Max] reward show!");
        if (MaxSdk.IsRewardedAdReady(rewardAdUnit))
        {
            _acRewarded = onClaimReward;
            MaxSdk.ShowRewardedAd(rewardAdUnit);
        }
    }

    public bool IsRewardedReady()
    {
        return MaxSdk.IsRewardedAdReady(rewardAdUnit);
    }

    public bool IsInterstitialReady(){
        return MaxSdk.IsInterstitialReady(intertitialAdUnit);
    }
    #endregion
}