using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using TMPro;
using Facebook.Unity;
using System;

public class SkinItem : MonoBehaviour
{
    [SerializeField] private SkeletonGraphic skeletonGraphic;
    [SerializeField] private GameObject buttonBuy;
    [SerializeField] private GameObject buttonDisableBuy;
    [SerializeField] private GameObject buttonAds;
    [SerializeField] private GameObject buttonDailyReward;
    [SerializeField] private GameObject buttonFacebook;
    [SerializeField] private GameObject buttonAchievement;
    [SerializeField] private GameObject buttonGiftcode;
    [SerializeField] private TextMeshProUGUI cost;
    [SerializeField] private TextMeshProUGUI costDisable;
    [SerializeField] private GameObject dockActive;
    [SerializeField] private GameObject usedLabel;
    [SerializeField] private GameObject fx;

    private SkinData skinData;
    private SkinPopup skinPopup;

    public void Init(SkinData skinData, SkinPopup skinPopup)
    {
        this.skinData = skinData;
        this.skinPopup = skinPopup;
    }

    public void Reset()
    {
        skeletonGraphic.ChangeSkin(skinData.SkinName);

        cost.text = skinData.Coin.ToString();
        costDisable.text = skinData.Coin.ToString();

        HideAll();

        if (!skinData.IsUnlocked && !Data.IsUnlockAllSkins)
        {
            switch (skinData.SkinType)
            {
                case SkinType.Coin:
                    if (Data.CoinTotal >= skinData.Coin)
                    {
                        buttonBuy.SetActive(true);
                    }
                    else
                    {
                        buttonDisableBuy.SetActive(true);
                    }
                    break;
                case SkinType.Ads:
                    buttonAds.SetActive(true);
                    break;
                case SkinType.Daily:
                    buttonDailyReward.SetActive(true);
                    break;
                case SkinType.Facebook:
                    buttonFacebook.SetActive(true);
                    break;
                case SkinType.Achievement:
                    buttonAchievement.SetActive(true);
                    break;
                case SkinType.Giftcode:
                    buttonGiftcode.SetActive(true);
                    break;
            }
        }

        CheckCurrent();
    }

    private void CheckCurrent()
    {
        string currentSkinName = "";
        switch (skinPopup.EUnitType)
        {
            case EUnitType.Hero:
                currentSkinName = Data.CurrentSkinHero;
                break;
            case EUnitType.Princess:
                currentSkinName = Data.CurrentSkinPrincess;
                break;
        }

        if (skinData.SkinName == currentSkinName)
        {
            SetActiveDock(true);
            SetActiveUsedLabel(true);
            SetActiveFX(true);
        }
    }

    private void HideAll()
    {
        buttonBuy.SetActive(false);
        buttonAds.SetActive(false);
        buttonDailyReward.SetActive(false);
        buttonDisableBuy.SetActive(false);
        buttonFacebook.SetActive(false);
        buttonAchievement.SetActive(false);
        buttonGiftcode.SetActive(false);
        usedLabel.SetActive(false);
        dockActive.SetActive(false);
        fx.SetActive(false);
    }

    public void OnClickButtonBuy()
    {
        AnalyticController.UnlockSkinCoins();

        Data.CoinTotal -= skinData.Coin;
        Done();
    }

    public void OnClickButtonAds()
    {
        AnalyticController.UnlockSkinVideo();

        AdController.Instance.ShowRewardedAd(() =>
        {
            Done();
        });
    }

    public void OnClickButtonDailyReward()
    {
        PopupController.Instance.Show<DailyRewardPopup>(null, ShowAction.DoNothing);
    }


    public void OnClickButtonAchievement()
    {
        PopupController.Instance.Show<AchievementDailyQuestPopup>(null, ShowAction.DoNothing);
    }


    public void OnClickButtonGiftcode()
    {
        PopupController.Instance.Show<GiftcodePopup>(null, ShowAction.DoNothing);
    }

    public void OnClickFacebookButton()
    {
        string uri = "https://play.google.com/store/apps/details?id=com.gamee.herotowerwar";
#if UNITY_IOS
            uri = "https://apps.apple.com/us/app/id1570840391";
#endif
        FB.FeedShare(
            link: new Uri(uri),
            callback: (IShareResult result) =>
            {
                if (!result.Cancelled && String.IsNullOrEmpty(result.Error))
                {
                    AnalyticController.UnlockSkinFacebook();
                    Done();
                }
            },
            linkName: "Hero Tower Wars",
            linkCaption: "Join me now!"
        );
    }

    private void Done()
    {
        SetSkin();

        skinData.IsUnlocked = true;

        skinPopup.Reset();

        usedLabel.SetActive(true);
    }

    private void SetSkin()
    {
        switch (skinPopup.EUnitType)
        {
            case EUnitType.Hero:
                Data.CurrentSkinHero = skinData.SkinName;
                break;
            case EUnitType.Princess:
                Data.CurrentSkinPrincess = skinData.SkinName;
                break;
        }
    }

    public void OnClick()
    {
        if (skinData.IsUnlocked || Data.IsUnlockAllSkins)
        {
            SetSkin();
            skinPopup.ResetUsedLabel();
            SetActiveUsedLabel(true);
        }

        skinPopup.ChangeCharacterSkin(skinData.SkinName);

        skinPopup.ResetDock();
        SetActiveDock(true);

        skinPopup.ResetFx();
        SetActiveFX(true);
    }

    public void SetActiveDock(bool active)
    {
        dockActive.SetActive(active);
    }

    public void SetActiveUsedLabel(bool active)
    {
        usedLabel.SetActive(active);
    }

    public void SetActiveFX(bool active)
    {
        fx.SetActive(false);
    }
}
