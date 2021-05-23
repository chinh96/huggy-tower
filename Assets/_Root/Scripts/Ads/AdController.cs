using System;
using DG.Tweening;

public class AdController : Singleton<AdController>
{
    [NonSerialized] public bool JustShowReward = false;
    public bool IsRewardLoaded => ad != null && ad.IsRewardLoaded;

    private IAd ad;
    private bool isRewardEarned = false;
    private Action handleRewardAfterEarned;
    private Action handleInterAfterClosed;
    private bool isShowBanner
    {
        get
        {
            return !Data.IsRemovedAds;
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

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Init()
    {
        if (RemoteConfigController.Instance.OnlyAdmob)
        {
            ad = GetComponent<AdMobController>();
        }
        else
        {
            ad = GetComponent<IronSourceController>();
        }

        ad.Init(OnInterClosed, OnInterLoaded, OnRewardLoaded, OnRewardClosed, OnRewardEarned);

        ad.RequestBanner();
        ad.RequestInterstitial();
        ad.RequestRewarded();

        ad.HideBanner();
    }

    public void RequestBanner()
    {
#if !UNITY_EDITOR
        if (ad != null)
        {
            ad.RequestBanner();
        }
#endif
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
            ad.RequestInterstitial();
        }
    }

    public void ShowInterstitial(Action action)
    {
#if UNITY_EDITOR
        action?.Invoke();
#else
        if (ad != null && isShowInter)
        {
            AnalyticController.RequestAdInterstitial();
            if (ad.IsInterLoaded)
            {
                AnalyticController.ImpressAdInterstitial();
                handleInterAfterClosed = action;
                ad.ShowInterstitial();
                GameManager.Instance.Root.ResetTotalTimePlay();
                GameManager.Instance.Root.ResetTotalLevelWin();
            }
            else {
                action?.Invoke();
            }
        }
        else
        {
            action?.Invoke();
        }
#endif
    }

    public void RequestRewarded()
    {
        if (ad != null && !ad.IsRewardLoaded)
        {
            ad.RequestRewarded();
        }
    }

    public void ShowRewardedAd(Action action)
    {
#if UNITY_EDITOR
        action?.Invoke();
#else
        if (ad != null)
        {
            AnalyticController.RequestAdReward();
            if (ad.IsRewardLoaded)
            {
                AnalyticController.ImpressAdReward();
                handleRewardAfterEarned = action;
                ad.ShowRewardedAd();
            }
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

        if (isRewardEarned)
        {
            isRewardEarned = false;
            DOTween.Sequence().AppendInterval(.1f).AppendCallback(() => handleRewardAfterEarned?.Invoke());
        }
    }

    public void OnRewardLoaded()
    {
    }

    public void OnRewardEarned()
    {
        JustShowReward = true;
        isRewardEarned = true;
    }
}
