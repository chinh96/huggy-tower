using UnityEngine;
using System;

public class IronSourceController : MonoBehaviour, IAd
{
#if UNITY_EDITOR
    private string appKey = "85460dcd";
#elif UNITY_ANDROID
    private string appKey = "ea7b3ad9";
#elif UNITY_IOS
    private string appKey = "ecb15411";
#endif

    public bool IsBannerLoaded => true;

    public bool IsInterLoaded => IronSource.Agent.isInterstitialReady();

    public bool IsRewardLoaded => isLoadedReward;

    public Action OnInterClosed { get; set; }
    public Action OnInterLoaded { get; set; }
    public Action OnRewardLoaded { get; set; }
    public Action OnRewardClosed { get; set; }
    public Action OnRewardEarned { get; set; }

    private bool isLoadedReward = false;

    public void Init(Action OnInterClosed, Action OnInterLoaded, Action OnRewardLoaded, Action OnRewardClosed, Action OnRewardEarned)
    {
        IronSourceConfig.Instance.setClientSideCallbacks(true);
        IronSource.Agent.validateIntegration();
        IronSource.Agent.init(appKey);

        IronSourceEvents.onInterstitialAdClosedEvent += () => OnInterClosed();

        IronSourceEvents.onRewardedVideoAdClosedEvent += () => OnRewardClosed();
        IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += (available) =>
        {
            isLoadedReward = available;
            if (isLoadedReward)
            {
                OnRewardLoaded();
            }
        };
        IronSourceEvents.onRewardedVideoAdRewardedEvent += (arg) => OnRewardEarned();
        IronSourceEvents.onInterstitialAdReadyEvent += () => OnInterLoaded();

        this.OnInterClosed = OnInterClosed;
        this.OnRewardLoaded = OnRewardLoaded;
        this.OnRewardClosed = OnRewardClosed;
        this.OnRewardEarned = OnRewardEarned;
    }

    public void ShowBanner()
    {
        IronSource.Agent.displayBanner();
    }

    public void HideBanner()
    {
        IronSource.Agent.hideBanner();
    }

    public void ShowInterstitial()
    {
        IronSource.Agent.showInterstitial();
    }

    public void ShowRewardedAd()
    {
        IronSource.Agent.showRewardedVideo();
    }

    public void RequestBanner()
    {
        IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
    }

    public void RequestInterstitial()
    {
        IronSource.Agent.loadInterstitial();
    }

    public void RequestRewarded()
    {

    }

    private void OnApplicationPause(bool pauseStatus)
    {
        IronSource.Agent.onApplicationPause(pauseStatus);
    }
}