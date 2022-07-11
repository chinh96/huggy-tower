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
    [SerializeField] private GameObject ClaimButton;
    private int coinCurrent;
    private bool hasCoinCurrent;
    private DailyRewardItem currentDailyRewardItem;
    protected override void BeforeShow()
    {
        base.BeforeShow();

        // Data.DailyRewardCurrent = Data.DailyRewardCurrent < Data.TotalDays ? Data.TotalDays : Data.DailyRewardCurrent;
        Reset();
    }

    public void Reset()
    {
        int dayTotal = Data.TotalDays;
        int dailyDayCurrent = Data.LastDayClaimedReward < dayTotal ? Data.DailyRewardCurrent : Data.DailyRewardCurrent - 1;

        int dayStart = 7 * (dailyDayCurrent / 7);
        bool isDayLoop = dayStart >= ResourcesController.DailyReward.DailyRewards.Count;
        int dayOffset = 0;

        hasCoinCurrent = false;
        dailyRewardItems.ForEach(item =>
        {
            int day = dayStart + dayOffset;
            int coin = isDayLoop ? ResourcesController.DailyReward.DailyRewardsLoop[day % 7] : ResourcesController.DailyReward.DailyRewards[day];
            item.Init(day, coin, dayTotal, isDayLoop, this);
            item.Reset();
            if(day == Data.DailyRewardCurrent) currentDailyRewardItem = item;
            dayOffset++;
        });

        if(currentDailyRewardItem.DailyRewardTypeOfItem() != "Current") ClaimButton.SetActive(false);
        SetX5ButtonActive(hasCoinCurrent);
    }

    public void SetCoinCurrent(int coinCurrent)
    {
        hasCoinCurrent = Data.DailyRewardCurrent % 7 != 6;
        this.coinCurrent = coinCurrent;
    }

    // old code
    public void OnClickClaim(GameObject claimButton, bool isSkin)
    {
        if (isSkin)
        {
            Claim();
            Data.CoinTotal = Data.CoinTotal; // ?
        }
        else
        {
            int coinTotal = Data.CoinTotal + coinCurrent;
            coinGeneration.GenerateCoin(() =>
            {
                Data.CoinTotal++;
            }, () =>
            {
                Claim();
                Data.CoinTotal = coinTotal;
            }, claimButton);
        }
        ClaimButton.SetActive(false);
        AnalyticController.AdjustLogEventClaimDailyReward();
    }

    public void OnClickClaimButton(){
        currentDailyRewardItem.OnClickClaim();
    }

    public void OnClickClaimAds()
    {
        AdController.Instance.ShowRewardedAd(() =>
        {
            int coinTotal = Data.CoinTotal + coinCurrent * 2;
            coinGeneration.GenerateCoin(() =>
            {
                Data.CoinTotal++;
            }, () =>
            {
                foreach(var dailyRewardItem in dailyRewardItems){
                    if(dailyRewardItem.Day == Data.DailyRewardCurrent){
                        dailyRewardItem.SetDoneAfterClaimedAds();
                        break;
                    }
                }
                Claim();
                Data.CoinTotal = coinTotal;
            }, x5Button);
            
            AnalyticController.AdjustLogEventClaimDailyRewardByAds();
        });
    }

    
    public void Claim()
    {
        Data.DailyRewardCurrent++;
        Data.LastDayClaimedReward = Data.TotalDays;
        Reset();
    }

    public void SetX5ButtonActive(bool active)
    {
        x5Button.SetActive(active);
    }

    public void SetX5Text(int coin)
    {
        x5Text.text = (coin * 2).ToString();
    }
}
