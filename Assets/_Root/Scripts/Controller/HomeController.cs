using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System;
using UnityEngine.Purchasing;

public class HomeController : Singleton<HomeController>
{
    [SerializeField] private GameObject rescuePartyButton;
    [SerializeField] private GameObject fbLoginButton;
    [SerializeField] private GameObject removeAdsButton;
    [SerializeField] private Image overlay;
    [SerializeField] private CanvasScaler canvasScaler;

    protected override void Awake()
    {
        base.Awake();

        CheckButton();
        overlay.DOFade(1, 0);
    }

    public void OnPurchaseSuccessRemoveAds(Product product)
    {
        Data.IsRemovedAds = true;
        CheckButton();

        AnalyticController.AdjustLogEventPurchaseItem("o6ssbb", 2.99f, "USD", product.transactionID);
    }

    public void CheckButton()
    {
#if UNITY_IOS
        fbLoginButton.SetActive(RemoteConfigController.Instance.EnableFbLogin);
#endif
        removeAdsButton.SetActive(!Data.IsRemovedAds);
        rescuePartyButton.SetActive(Data.TimeToRescueParty.TotalMilliseconds > 0);
    }

    private void Start()
    {
        canvasScaler.matchWidthOrHeight = Camera.main.aspect > .7f ? 1 : 0;
        AdController.Instance.ShowBanner();
        SoundController.Instance.PlayBackground(SoundType.BackgroundHome);
        FadeOutOverlay();
        CheckNewUpdatePopup();
        CheckAchievementDailyQuest();
        // CheckRescueParty();
        // CheckLanguage();
    }

    private void CheckAchievementDailyQuest()
    {
        NotificationController.Instance.CheckDailyQuestRepeat();
        NotificationController.Instance.CheckDailyRewardRepeat();

        ResourcesController.Achievement.IncreaseByType(AchievementType.PlayToLevel);

        ResourcesController.Achievement.CheckCompleteCastle();
        ResourcesController.DailyQuest.CheckCompleteCastle();
    }

    private void CheckRescueParty()
    {
        ResourcesController.ReceiveSkinRescueParty(() =>
        {
            PopupController.Instance.Show<RescuePartyReceiveSkinPopup>();
        });
    }

    private void CheckLanguage()
    {
        if (Data.FirstOpenLanguage)
        {
            Data.FirstOpenLanguage = false;
            PopupController.Instance.Show<PopupSelectLanguage>();
        }
    }

    public void TapToStart()
    {
        SoundController.Instance.PlayOnce(SoundType.ButtonStart);
        overlay.gameObject.SetActive(true);
        FadeInOverlay(() =>
        {
            SceneManager.LoadSceneAsync(Constants.GAME_SCENE);
        });
    }

    private void FadeInOverlay(Action action = null)
    {
        overlay.gameObject.SetActive(true);
        overlay.DOFade(1, .3f).OnComplete(() =>
        {
            action?.Invoke();
        });
    }

    private void FadeOutOverlay()
    {
        overlay.DOFade(0, .3f).OnComplete(() =>
        {
            overlay.gameObject.SetActive(false);
        });
    }

    public void OnClickDailyButton()
    {
        AnalyticController.ClickDailyReward();
        PopupController.Instance.Show<DailyRewardPopup>();
    }

    public void OnClickCastleButton()
    {
        PopupController.Instance.Show<WorldPopup>();
    }

    public void OnClickSkinButton()
    {
        PopupController.Instance.Show<SkinPopup>();
    }

    public void OnClickAchievementButton()
    {
        PopupController.Instance.Show<AchievementPopup>();
    }

    public void OnClickSettingButton()
    {
        PopupController.Instance.Show<SettingPopup>();
    }

    public void OnClickFacebookButton()
    {
        PopupController.Instance.Show<FacebookPopup>();
    }

    public void OnClickDailyQuestButton()
    {
        PopupController.Instance.Show<DailyQuestPopup>();
    }

    public void OnClickAchievementDailyQuestButton()
    {
        PopupController.Instance.Show<AchievementDailyQuestPopup>();
    }

    public void OnClickLibraryButton()
    {
        PopupController.Instance.Show<LibraryPopup>();
    }

    public void OnClickLeaderboardButton()
    {
        AnalyticController.ClickRankButton();
        LeaderboardController.Instance.Show();
    }

    public void OnClickLeaderboardRescuePartyButton()
    {
        LeaderboardRescuePartyController.Instance.Show();
    }

    public void OnClickRescuePartyButton()
    {
        Data.FirstOpenRescuePartyInHome = false;
        PopupController.Instance.Show<RescuePartyPopup>();
    }

    public void CheckNewUpdatePopup()
    {
        if (RemoteConfigController.Instance.HasNewUpdate && !Data.DontShowUpdateAgain)
        {
            DOTween.Sequence().AppendInterval(.5f).AppendCallback(() =>
            {
                PopupController.Instance.Show<NewUpdatePopup>();
            });
        }
    }

    public void OnClickHalloweenButton()
    {
        PopupController.Instance.Show<CoomingSoonPopup>();
    }
}