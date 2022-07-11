using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Spine.Unity;
using UnityEngine.UI;
using I2.Loc;

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
    [SerializeField] private TextMeshProUGUI skinName;

    private int day;
    private int coin;
    private int dayTotal;
    private bool isDayLoop;
    private DailyRewardPopup dailyRewardPopup;
    private SkinData skinData;
    private DailyRewardType dailyRewardType = DailyRewardType.NotClaimed;

    private bool isDay7 => day % 7 == 6;
    private bool isSkin => !isDayLoop && ResourcesController.DailyReward.DailyRewardsSkin.Contains(day);
    private bool isUnlocked
    {
        get
        {
            Data.IdCheckUnlocked = Constants.DAILY_REWARD + day;
            return Data.IsUnlocked;
        }

        set
        {
            Data.IdCheckUnlocked = Constants.DAILY_REWARD + day;
            Data.IsUnlocked = value;
        }
    }

    public int Day => day;

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
        if (isDay7) dayText.text = $"Day\n{day + 1}";
        // dayText.GetComponent<LocalizationParamsManager>().SetParameterValue("VALUE", (day + 1).ToString(), true);
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
        skinName.gameObject.SetActive(false);

        background.sprite = spriteCoinClaimed;
    }

    private void Check()
    {
        CheckDailyCoin();
        CheckDailySkin();
        // CheckBackground();
    }

    private void CheckDailyCoin()
    {
        // if (day > dayTotal)
        // {
        //     // claimDisableButton.SetActive(true);

        //     dailyRewardType = DailyRewardType.NotClaimed;
        // }
        // else 
        if (day < Data.DailyRewardCurrent)
        {
            doneIcon.SetActive(isUnlocked);

            dailyRewardType = DailyRewardType.Claimed;
            if (Data.LastDayClaimedReward == Data.TotalDays && day == Data.DailyRewardCurrent - 1) background.sprite = spriteCoinCurrent;
        }
        else if (day > Data.DailyRewardCurrent)
        {
            // claimPendingButton.SetActive(true);

            dailyRewardType = DailyRewardType.NotClaimed;
        }
        else // current daily reward
        {
            if (Data.LastDayClaimedReward < Data.TotalDays)
            {
                claimButton.SetActive(true);
                dailyRewardPopup.SetCoinCurrent(coin);

                dailyRewardPopup.SetX5Text(coin);
                dailyRewardType = DailyRewardType.Current;

                background.sprite = spriteCoinCurrent;

                if (isDay7)
                {
                    skinName.color = new Color(0, 0.2980392f, 0.6f, 1);
                    dayText.color = new Color(0, 0.2980392f, 0.6f, 1);
                    coinText.color = new Color(0, 0.2980392f, 0.6f, 1);
                }
            }
            else
            {
                claimButton.SetActive(false);
                // doneIcon.SetActive(true);
                dailyRewardType = DailyRewardType.NotClaimed;
            }

        }
    }

    private void CheckDailySkin()
    {
        if (isUnlocked) return;
        if (isSkin)
        {
            int index = ResourcesController.DailyReward.DailyRewardsSkin.IndexOf(day);
            skinData = ResourcesController.Hero.SkinsDailyReward[index];
            hero.ChangeSkin(skinData.SkinName);
            hero.gameObject.SetActive(true);

            skinName.text = skinData.Name;
            if (isDay7) skinName.gameObject.SetActive(true);
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
                background.sprite = spriteCoinClaimed;
                break;
                // case DailyRewardType.Current:
                //     background.sprite = spriteCoinCurrent;
                //     if (isDay7)
                //     {
                //         skinName.color = new Color(0, 0, 102, 1);
                //         dayText.color = new Color(0, 0, 102, 1);
                //         coinText.color = new Color(0, 0, 102, 1);
                //     }
                //     break;
                // case DailyRewardType.NotClaimed:
                //     background.sprite = isDay7 ? spriteSkinNotClaimed : spriteCoinNotClaimed;
                //     break;
        }
    }

    public void OnClickClaim()
    {   
        isUnlocked = true;
        if (isSkin)
        {
            Data.currentSkinHeroId = skinData.Id;
            Data.CurrentSkinHero = skinData.SkinName;
            skinData.IsUnlocked = true;
            EventController.SkinPopupReseted?.Invoke();
        }
        Debug.Log("Done Claimed Skin");
        dailyRewardPopup.OnClickClaim(claimButton, isSkin);
        ResourcesController.Achievement.IncreaseByType(AchievementType.ClaimDailyReward);
    }

    public void SetDoneAfterClaimedAds()
    {
        isUnlocked = true;
    }

    public string DailyRewardTypeOfItem()
    {
        return dailyRewardType.ToString();
    }
    private enum DailyRewardType
    {
        Claimed,
        Current,
        NotClaimed
    }
}
