using System;

interface IAd
{
    bool IsBannerLoaded { get; }
    bool IsInterLoaded { get; }
    bool IsRewardLoaded { get; }
    void Init(Action OnInterClosed, Action OnInterLoaded, Action OnRewardLoaded, Action OnRewardClosed, Action OnRewardEarned);
    void RequestBanner();
    void ShowBanner();
    void HideBanner();
    void RequestInterstitial();
    void ShowInterstitial();
    void RequestRewarded();
    void ShowRewardedAd();
    Action OnInterClosed { get; set; }
    Action OnInterLoaded { get; set; }
    Action OnRewardClosed { get; set; }
    Action OnRewardLoaded { get; set; }
    Action OnRewardEarned { get; set; }
}
