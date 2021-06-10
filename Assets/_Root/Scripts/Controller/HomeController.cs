using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class HomeController : Singleton<HomeController>
{
    [SerializeField] private GameObject removeAdsButton;
    [SerializeField] private Image overlay;

    protected override void Awake()
    {
        base.Awake();

        CheckRemoveAds();
        overlay.DOFade(1, 0);
    }

    public void OnPurchaseSuccessRemoveAds()
    {
        Data.IsRemovedAds = true;
        CheckRemoveAds();
    }

    public void CheckRemoveAds()
    {
        removeAdsButton.SetActive(!Data.IsRemovedAds);
    }

    private void Start()
    {
        AdController.Instance.ShowBanner();
        SoundController.Instance.PlayBackground(SoundType.BackgroundHome);
        FadeOutOverlay();
    }

    public void TapToStart()
    {
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
}