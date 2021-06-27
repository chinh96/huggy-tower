using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinPopup : Popup
{
    [SerializeField] private ProgressGift progressGift;
    [SerializeField] private GameObject tapToContinueButton;
    [SerializeField] private GameObject claimX5Button;
    [SerializeField] private CoinGeneration coinGeneration;

    protected override void BeforeShow()
    {
        base.BeforeShow();

        claimX5Button.SetActive(true);
        tapToContinueButton.SetActive(false);
        progressGift.Reset();
    }

    protected override void AfterShown()
    {
        base.AfterShown();

        Data.CoinTotal += ResourcesController.Config.CoinBonusPerLevel;

        progressGift.Move(() =>
        {
            tapToContinueButton.SetActive(true);
        });
    }

    public void OnClickX5()
    {
        AdController.Instance.ShowRewardedAd(() =>
        {
            claimX5Button.SetActive(false);

            int coinTotal = Data.CoinTotal + ResourcesController.Config.CoinBonusPerLevel * 5;
            coinGeneration.GenerateCoin(() =>
            {
                Data.CoinTotal++;
            }, () =>
            {
                Data.CoinTotal = coinTotal;
            });
        });
    }

    public void OnClickContinue()
    {
        GameController.Instance.OnNextLevel();
    }

    public void OnClickHomeButton()
    {
        GameController.Instance.OnBackToHome();
    }
}
