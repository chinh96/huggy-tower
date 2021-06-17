using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Spine.Unity;
using UnityEngine.UI;

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
    [SerializeField] private Image background;
    [SerializeField] private Sprite spriteCoinClaimed;
    [SerializeField] private Sprite spriteCoinCurrent;
    [SerializeField] private Sprite spriteCoinNotClaimed;
    [SerializeField] private Sprite spriteSkinClaimed;
    [SerializeField] private Sprite spriteSkinCurrent;
    [SerializeField] private Sprite spriteSkinNotClaimed;

    private int day;
    private int coin;
    private int dayTotal;
    private bool isDayLoop;
    private DailyRewardPopup dailyRewardPopup;
    private SkinData skinData;
    private DailyRewardType dailyRewardType = DailyRewardType.NotClaimed;

    private bool isDay7 => day % 7 == 6;
    private bool isSkin => !isDayLoop && isDay7;

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
        dayText.text = $"Day {day + 1}";
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
        CheckDailyCoin();
        CheckDailySkin();
        CheckBackground();
    }

    private void CheckDailyCoin()
    {
        if (day > dayTotal)
        {
            claimDisableButton.SetActive(true);

            dailyRewardType = DailyRewardType.NotClaimed;
        }
        else if (day < Data.DailyRewardCurrent)
        {
            doneIcon.SetActive(true);

            dailyRewardType = DailyRewardType.Claimed;
        }
        else if (day > Data.DailyRewardCurrent)
        {
            claimPendingButton.SetActive(true);

            dailyRewardType = DailyRewardType.NotClaimed;
        }
        else
        {
            claimButton.SetActive(true);
            dailyRewardPopup.SetCoinCurrent(coin);

            dailyRewardType = DailyRewardType.Current;
            dailyRewardPopup.SetX5Text(coin);
        }
    }

    private void CheckDailySkin()
    {
        if (isSkin)
        {
            skinData = ResourcesController.Hero.SkinsDailyReward[day / 7];
            hero.ChangeSkin(skinData.SkinName);
            hero.gameObject.SetActive(true);
        }
        else
        {
            coinIcon.SetActive(true);
            coinText.gameObject.SetActive(true);
        }
    }

    private void CheckBackground()
    {
        switch (dailyRewardType)
        {
            case DailyRewardType.Claimed:
                background.sprite = isDay7 ? spriteSkinClaimed : spriteCoinClaimed;
                break;
            case DailyRewardType.Current:
                background.sprite = isDay7 ? spriteSkinCurrent : spriteCoinCurrent;
                break;
            case DailyRewardType.NotClaimed:
                background.sprite = isDay7 ? spriteSkinNotClaimed : spriteCoinNotClaimed;
                break;
        }
    }

    public void OnClickClaim()
    {
        if (isSkin)
        {
            Data.CurrentSkinHero = skinData.SkinName;
            skinData.IsUnlocked = true;
        }
        dailyRewardPopup.OnClickClaim(claimButton, isSkin);
    }

    private enum DailyRewardType
    {
        Claimed,
        Current,
        NotClaimed
    }
}
