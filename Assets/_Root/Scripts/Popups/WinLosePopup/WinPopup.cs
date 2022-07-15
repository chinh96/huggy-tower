using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
public class WinPopup : Popup
{
    [SerializeField] private ProgressGift progressGift;
    [SerializeField] private GameObject tapToContinueButton;
    [SerializeField] private GameObject claimX5Button;
    [SerializeField] private CoinGeneration coinGeneration;
    [SerializeField] private Image iconCrossAds;
    [SerializeField] private Sprite[] imgIconCrossAds;

    [SerializeField] private GameObject huggy_2;
    [SerializeField] private GameObject huggy_1;
    [SerializeField] private GameObject princess;

    [SerializeField] private LevelText levelText;

    private string[] UrlData = new string[2] { "market://details?id=com.gamee.huggytimepin", "market://details?id=com.GameeStudio.PoppyPin3D" };
    private int idCrossCurrent = 0;

    private bool havePrincess = false;

    protected override void BeforeShow()
    {
        if (GameController.Instance.Princess) havePrincess = true;
        else havePrincess = false;
        huggy_1.SetActive(!havePrincess);
        huggy_2.SetActive(havePrincess);
        princess.SetActive(havePrincess);

        AdController.Instance.HideBanner();
        base.BeforeShow();

        levelText.ChangeLevelMinusOne();

        claimX5Button.SetActive(true);
        tapToContinueButton.SetActive(false);
        progressGift.Reset();

        //idCrossCurrent = idCrossCurrent == 0 ? 1 : 0;
        //iconCrossAds.sprite = imgIconCrossAds[idCrossCurrent];
        //checkCrossAds();

        GameObject huggy = havePrincess ? huggy_2 : huggy_1;
        huggy.GetComponent<HeroWinLoseController>().PlayWin();

        if (havePrincess)
        {
            string[] princessWinList = { "Win", "Win2", "Win3" };
            string princessWin = princessWinList[UnityEngine.Random.Range(0, princessWinList.Length)];
            if (princessWin == "Win3") huggy_2.GetComponent<SkeletonGraphic>().Play("Win3", true);
            princess.GetComponent<SkeletonGraphic>().Play(princessWin, true);
        }
    }

    void checkCrossAds()
    {
        Debug.Log(RemoteConfigController.Instance.HasCrossAds);
        iconCrossAds.gameObject.SetActive(RemoteConfigController.Instance.HasCrossAds);
    }

    protected override void AfterShown()
    {
        SoundType[] soundWins = { SoundType.HuggyWin, SoundType.HuggyWin2 };
        SoundController.Instance.PlayOnce(soundWins[UnityEngine.Random.Range(0, soundWins.Length)]);
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
            Debug.Log("X5 Coin....");
            claimX5Button.SetActive(false);

            int coinTotal = Data.CoinTotal + ResourcesController.Config.CoinBonusPerLevel * 5;
            coinGeneration.GenerateCoin(() =>
            {
                Data.CoinTotal++;
            }, () =>
            {
                Data.CoinTotal = coinTotal;
            });

            AnalyticController.AdjustLogEventClaimX5CoinWinLevel();
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

    public void OnClickCrossAdsFishPin()
    {
#if UNITY_ANDROID
        // Application.OpenURL("market://details?id=com.gamee.savethehero");
        Application.OpenURL(UrlData[idCrossCurrent]);
#else
        // Application.OpenURL("itms-apps://itunes.apple.com/app/id1562329957");
#endif
    }
}
