using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class AdApplovinController : MonoBehaviour, IAd
{
    public bool IsBannerLoaded => false;
    public bool IsInterLoaded => true;
    public bool IsRewardLoaded => false;
    ApplovinManager ad;

    public void Init(Action OnInterClosed, Action OnInterLoaded, Action OnRewardLoaded, Action OnRewardClosed, Action OnRewardEarned)
    {
        ad.InitAds();
        this.OnInterClosed = OnInterClosed;
        this.OnInterLoaded = OnInterLoaded;
        this.OnRewardLoaded = OnRewardLoaded;
        this.OnRewardClosed = OnRewardClosed;
        this.OnRewardEarned = OnRewardEarned;
    }
    public void RequestBanner()
    {

    }
    public void ShowBanner()
    {
        ad.ShowBanner();
    }
    public void HideBanner()
    {
        ad.HideBanner();
    }
    public void RequestInterstitial()
    {

    }
    public void ShowInterstitial()
    {
        ad.ShowIntertitial(null);
    }
    public void RequestRewarded()
    {

    }
    public void ShowRewardedAd()
    {
        ad.ShowReward(null);
    }
    public Action OnInterClosed { get; set; }
    public Action OnInterLoaded { get; set; }
    public Action OnRewardClosed { get; set; }
    public Action OnRewardLoaded { get; set; }
    public Action OnRewardEarned { get; set; }
}
