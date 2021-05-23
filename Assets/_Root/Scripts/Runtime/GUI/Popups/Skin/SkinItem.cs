using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using TMPro;

public class SkinItem : MonoBehaviour
{
    [SerializeField] private SkeletonGraphic skeletonGraphic;
    [SerializeField] private GameObject buttonBuy;
    [SerializeField] private GameObject buttonDisableBuy;
    [SerializeField] private GameObject buttonAds;
    [SerializeField] private GameObject buttonDailyReward;
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

        if (!skinData.IsUnlocked)
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
            }
        }
    }

    private void HideAllButton()
    {
        buttonBuy.SetActive(false);
        buttonAds.SetActive(false);
        buttonDailyReward.SetActive(false);
        buttonDisableBuy.SetActive(false);
    }

    public void OnClickButtonBuy()
    {
        Data.CoinTotal -= skinData.Coin;
        SetSkin();
    }

    public void OnClickButtonAds()
    {
        AdController.Instance.ShowRewardedAd(() =>
        {
            SetSkin();
        });
    }

    public void OnClickButtonDailyReward()
    {
        PopupController.Instance.Show<DailyRewardPopup>();
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

        skinData.IsUnlocked = true;

        skinPopup.Reset();
    }
}
