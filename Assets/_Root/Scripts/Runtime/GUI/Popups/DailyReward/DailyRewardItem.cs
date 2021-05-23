using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Spine.Unity;

public class DailyRewardItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private GameObject coinIcon;
    [SerializeField] private GameObject doneIcon;
    [SerializeField] private SkeletonGraphic hero;
    [SerializeField] private GameObject claimButton;
    [SerializeField] private GameObject claimPendingButton;
    [SerializeField] private GameObject claimDisableButton;

    private int day;
    private int coin;
    private int dayTotal;
    private bool isDayLoop;
    private DailyRewardPopup dailyRewardPopup;
    private SkinData skinData;

    private bool isSkin => !isDayLoop && day % 7 == 6;

    public void Init(int day, int coin, int dayTotal, bool isDayLoop, DailyRewardPopup dailyRewardPopup)
    {
        this.day = day;
        this.coin = coin;
        this.dayTotal = dayTotal;
        this.isDayLoop = isDayLoop;
        this.dailyRewardPopup = dailyRewardPopup;
    }

    public void Reset()
    {
        dayText.text = $"Day {day}";
        coinText.text = $"{coin}";
        Hide();
        Check();
    }

    private void Hide()
    {
        doneIcon.SetActive(false);
        claimButton.SetActive(false);
        claimPendingButton.SetActive(false);
        claimDisableButton.SetActive(false);
        coinIcon.SetActive(false);
        coinText.gameObject.SetActive(false);
        hero.gameObject.SetActive(false);
    }

    private void Check()
    {
        if (day > dayTotal)
        {
            claimDisableButton.SetActive(true);
        }
        else if (day < Data.DailyRewardCurrent)
        {
            doneIcon.SetActive(true);
        }
        else if (day > Data.DailyRewardCurrent)
        {
            claimPendingButton.SetActive(true);
        }
        else
        {
            claimButton.SetActive(true);
            dailyRewardPopup.SetCoinCurrent(coin);
        }

        if (isSkin)
        {
            skinData = ResourcesController.Instance.Hero.SkinDailyRewards[day / 7];
            hero.ChangeSkin(skinData.SkinName);
            hero.gameObject.SetActive(true);
        }
        else
        {
            coinIcon.SetActive(true);
            coinText.gameObject.SetActive(true);
        }
    }

    public void OnClickClaim()
    {
        if (isSkin)
        {
            Data.CurrentSkinHero = skinData.SkinName;
            skinData.IsUnlocked = true;
        }
        dailyRewardPopup.OnClickClaim();
    }
}
