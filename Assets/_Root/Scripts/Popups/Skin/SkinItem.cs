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
    [SerializeField] private TextMeshProUGUI cost;
    [SerializeField] private TextMeshProUGUI costDisable;

    private SkinData skinData;
    private SkinPopup skinPopup;

    public void Init(SkinData skinData, SkinPopup skinPopup)
    {
        this.skinData = skinData;
        this.skinPopup = skinPopup;

        Reset();
    }

    public void Reset()
    {
        skeletonGraphic.ChangeSkin(skinData.SkinName);

        cost.text = skinData.Coin.ToString();
        costDisable.text = skinData.Coin.ToString();

        HideAllButton();

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
            }
        }
    }

    private void HideAllButton()
    {
        buttonBuy.SetActive(false);
        buttonAds.SetActive(false);
        buttonDailyReward.SetActive(false);
        buttonDisableBuy.SetActive(false);
        buttonFacebook.SetActive(false);
    }

    public void OnClickButtonBuy()
    {
        AnalyticController.UnlockSkinCoins();

        Data.CoinTotal -= skinData.Coin;
        Done();

        ResourcesController.DailyQuest.IncreaseByType(DailyQuestType.BuySkin);
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
        }

        skinPopup.ChangeCharacterSkin(skinData.SkinName);
    }
}
