using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using TMPro;
using Facebook.Unity;
using System;

public class SkinItem : MonoBehaviour
{
    // [SerializeField] private EUnitType eUnitType;
    [SerializeField] private SkeletonGraphic skeletonGraphic;
    [SerializeField] private GameObject buttonBuy;
    [SerializeField] private GameObject buttonDisableBuy;
    [SerializeField] private GameObject buttonAds;
    [SerializeField] private GameObject buttonDailyReward;
    [SerializeField] private GameObject buttonFacebook;
    [SerializeField] private GameObject buttonAchievement;
    [SerializeField] private GameObject buttonGiftcode;
    [SerializeField] private GameObject buttonRescueParty;
    [SerializeField] private GameObject buttonTG;
    [SerializeField] private GameObject buttonTGLuckySpin;
    [SerializeField] private TextMeshProUGUI cost;
    [SerializeField] private TextMeshProUGUI costDisable;
    [SerializeField] private GameObject dockActive;
    [SerializeField] private GameObject usedLabel;
    [SerializeField] private GameObject fx;

    private SkinData skinData;
    private SkinPopup skinPopup;

    private SubjectSkinChange m_subjectSkinChange;

    public void Init(SkinData skinData, SkinPopup skinPopup)
    {
        this.skinData = skinData;
        this.skinPopup = skinPopup;


    }
    private void Start()
    {
        m_subjectSkinChange = GetComponentInChildren<SubjectSkinChange>();
        if (m_subjectSkinChange != null)
        {
            m_subjectSkinChange.Next(skinData);
        }

    }

    public void Reset()
    {
        skeletonGraphic.ChangeSkin(skinData.SkinName, skinPopup.EUnitType);


        cost.text = skinData.Coin.ToString();
        costDisable.text = skinData.Coin.ToString();

        HideAll();

        if (!skinData.IsUnlocked && !Data.IsUnlockAllSkins)
        {
            switch (skinData.SkinType)
            {
                case SkinType.Coin:
                    buttonBuy.SetActive(Data.CoinTotal >= skinData.Coin);
                    buttonDisableBuy.SetActive(Data.CoinTotal < skinData.Coin);
                    break;
                case SkinType.Ads:
                    buttonAds.SetActive(true);
                    break;
                case SkinType.Daily:
                    if (Data.TotalDays > skinData.DayDaily)
                    {
                        buttonBuy.SetActive(Data.CoinTotal >= skinData.Coin);
                        buttonDisableBuy.SetActive(Data.CoinTotal < skinData.Coin);
                    }
                    else
                    {
                        buttonDailyReward.SetActive(true);
                    }
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
                case SkinType.RescueParty:
                    if (Data.TimeToRescueParty.TotalMilliseconds > 0)
                    {
                        buttonRescueParty.SetActive(true);
                    }
                    else
                    {
                        buttonBuy.SetActive(Data.CoinTotal >= skinData.Coin);
                        buttonDisableBuy.SetActive(Data.CoinTotal < skinData.Coin);
                    }
                    break;
                case SkinType.TGLuckySpin:
                    if (TGDatas.IsInTG)
                    {
                        buttonTGLuckySpin.SetActive(true);
                    }
                    else
                    {
                        buttonBuy.SetActive(Data.CoinTotal >= skinData.Coin);
                        buttonDisableBuy.SetActive(Data.CoinTotal < skinData.Coin);
                    }
                    break;
                case SkinType.ThanksGiving:
                    if (TGDatas.IsInTG)
                    {
                        buttonTG.SetActive(true);
                    }
                    else
                    {
                        buttonBuy.SetActive(Data.CoinTotal >= skinData.Coin);
                        buttonDisableBuy.SetActive(Data.CoinTotal < skinData.Coin);
                    }
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
                if (skinData.SkinName == currentSkinName)
                {
                    if (Data.currentSkinHeroId == "")
                    {
                        SetActiveDock(true);
                        SetActiveUsedLabel(true);
                        SetActiveFX(true);
                    }
                    else
                    {
                        if (Data.currentSkinHeroId == skinData.Id)
                        {
                            SetActiveDock(true);
                            SetActiveUsedLabel(true);
                            SetActiveFX(true);
                        }
                    }
                }
                break;
            case EUnitType.Princess:
                currentSkinName = Data.CurrentSkinPrincess;
                if (skinData.SkinName == currentSkinName)
                {
                    SetActiveDock(true);
                    SetActiveUsedLabel(true);
                    SetActiveFX(true);
                }
                break;
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
        buttonRescueParty.SetActive(false);
        buttonTG.SetActive(false);
        buttonTGLuckySpin.SetActive(false);
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
        PopupController.Instance.Show<AchievementDailyQuestPopup>(true, ShowAction.DoNothing);
    }


    public void OnClickButtonGiftcode()
    {
        PopupController.Instance.Show<GiftcodePopup>(null, ShowAction.DoNothing);
    }

    public void OnClickButtonRescurParty()
    {
        PopupController.Instance.Show<RescuePartyPopup>(null, ShowAction.DoNothing);
    }

    public void OnClickButtonTG()
    {
        PopupController.Instance.Show<DailyRewardPopupEvent>();
    }

    public void OnClickButtonTGLuckySpin()
    {
        PopupController.Instance.Show<TGLuckySpinPopup>(null, ShowAction.DoNothing);
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
                    Done();
                    AnalyticController.UnlockSkinFacebook();
                    AnalyticController.AdjustLogEventShareFb();
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
                Data.currentSkinHeroId = skinData.Id;
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

        skinPopup.ChangeCharacterSkin(skinData);

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
