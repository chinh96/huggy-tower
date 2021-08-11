using UnityEngine;
using System;

public class IronSourceController : MonoBehaviour, IAd
{
#if UNITY_EDITOR
    private string appKey = "85460dcd";
#elif UNITY_ANDROID
    private string appKey = "fa796329";
#elif UNITY_IOS
    private string appKey = "fb7da059";
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
        IronSourceEvents.onImpressionSuccessEvent += ImpressionSuccessEvent;

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
    
    private void ImpressionSuccessEvent(IronSourceImpressionData impressionData)
    {
        com.adjust.sdk.AdjustAdRevenue adjustAdRevenue = new com.adjust.sdk.AdjustAdRevenue(com.adjust.sdk.AdjustConfig.AdjustAdRevenueSourceIronSource);
        if (impressionData.revenue != null)
        {
            adjustAdRevenue.setRevenue(impressionData.revenue.Value, "USD");
        }
        // optional fields
        adjustAdRevenue.setAdRevenueNetwork(impressionData.adNetwork);
        adjustAdRevenue.setAdRevenueUnit(impressionData.adUnit);
        adjustAdRevenue.setAdRevenuePlacement(impressionData.placement);
        // track Adjust ad revenue
        com.adjust.sdk.Adjust.trackAdRevenue(adjustAdRevenue);
    }
}