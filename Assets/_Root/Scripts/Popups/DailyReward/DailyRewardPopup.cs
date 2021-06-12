using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DailyRewardPopup : Popup
{
    [SerializeField] private GameObject x5Button;
    [SerializeField] private TextMeshProUGUI x5Text;
    [SerializeField] private List<DailyRewardItem> dailyRewardItems;
    [SerializeField] private CoinGeneration coinGeneration;

    private int coinCurrent;
    private bool hasCoinCurrent;

    protected override void BeforeShow()
    {
        base.BeforeShow();

        Reset();
    }

    public void Reset()
    {
        int dayTotal = Data.TotalDays;
        int dayStart = 7 * (Data.DailyRewardCurrent / 7);
        bool isDayLoop = dayStart >= ResourcesController.DailyReward.DailyRewards.Count;
        int dayOffset = 0;

        hasCoinCurrent = false;
        dailyRewardItems.ForEach(item =>
        {
            int day = dayStart + dayOffset;
            int coin = isDayLoop ? ResourcesController.DailyReward.DailyRewardsLoop[day % 7] : ResourcesController.DailyReward.DailyRewards[day];

            item.Init(day, coin, dayTotal, isDayLoop, this);
            item.Reset();

            dayOffset++;
        });

        SetX5ButtonActive(hasCoinCurrent);
    }

    public void SetCoinCurrent(int coinCurrent)
    {
        hasCoinCurrent = Data.DailyRewardCurrent % 7 != 6;
        this.coinCurrent = coinCurrent;
    }

    public void OnClickClaim(GameObject claimButton)
    {
        int coinTotal = Data.CoinTotal + coinCurrent;
        coinGeneration.GenerateCoin(() =>
        {
            Data.CoinTotal++;
        }, () =>
        {
            Data.CoinTotal = coinTotal;
            Claim();
        }, claimButton);
    }

    public void OnClickClaimAds()
    {
        AdController.Instance.ShowRewardedAd(() =>
        {
            int coinTotal = Data.CoinTotal + coinCurrent * 5;
            coinGeneration.GenerateCoin(() =>
            {
                Data.CoinTotal++;
            }, () =>
            {
                Data.CoinTotal = coinTotal;
                Claim();
            }, x5Button);
        });
    }

    public void Claim()
    {
        Data.DailyRewardCurrent++;
        Reset();
    }

    public void SetX5ButtonActive(bool active)
    {
        x5Button.SetActive(active);
    }

    public void SetX5Text(int coin)
    {
        x5Text.text = (coin * 5).ToString();
    }
}
