using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class AdApplovinController : MonoBehaviour, IAd
{
    public bool IsBannerLoaded => true;
    public bool IsInterLoaded => true;
    public bool IsRewardLoaded => ad != null && ad.IsRewardedReady();
    public Action OnInterClosed { get; set; }
    public Action OnInterLoaded { get; set; }
    public Action OnRewardClosed { get; set; }
    public Action OnRewardLoaded { get; set; }
    public Action OnRewardEarned { get; set; }
    [SerializeField] private ApplovinManager ad;


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
        ad.ShowIntertitial(this.OnInterClosed);
    }
    public void RequestRewarded()
    {

    }
    public void ShowRewardedAd()
    {
        ad.ShowReward((isShow) =>
        {   
            Debug.Log("isShow applovin : " + isShow);
            if (isShow)
            {
                this.OnRewardEarned?.Invoke();
            }
            this.OnRewardClosed?.Invoke();
        });
    }

}
