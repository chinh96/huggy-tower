using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class HomeController : Singleton<HomeController>
{
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

    public void OnPurchaseSuccessRemoveAds()
    {
        Data.IsRemovedAds = true;
        CheckButton();
    }

    public void CheckButton()
    {
        fbLoginButton.SetActive(RemoteConfigController.Instance.EnableFbLogin);
        removeAdsButton.SetActive(!Data.IsRemovedAds);
    }

    private void Start()
    {
        canvasScaler.matchWidthOrHeight = Camera.main.aspect > .7f ? 1 : 0;
        AdController.Instance.ShowBanner();
        SoundController.Instance.PlayBackground(SoundType.BackgroundHome);
        FadeOutOverlay();
        CheckNewUpdatePopup();
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
        AnalyticController.ClickDailyQuestButton();
        PopupController.Instance.Show<DailyQuestPopup>();
    }

    public void OnClickLibraryButton()
    {
        PopupController.Instance.Show<LibraryPopup>();
    }

    public void OnClickLeaderboardButton()
    {
        LeaderboardController.Instance.Show();
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
}