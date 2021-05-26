using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeController : Singleton<HomeController>
{
    private void Start()
    {
        AdController.Instance.ShowBanner();
        SoundController.Instance.PlayBackground(SoundType.BackgroundHome);
    }

    public void TapToStart()
    {
        SceneManager.LoadSceneAsync(Constants.GAME_SCENE);
    }

    public void ShowSettingPopup()
    {
        PopupController.Instance.Show<SettingPopup>();
    }

    public void ShowDailyRewardPopup()
    {
        PopupController.Instance.Show<DailyRewardPopup>();
    }

    public void ShowWorldPopup()
    {
        PopupController.Instance.Show<WorldPopup>();
    }

    public void ShowSkinPopup()
    {
        PopupController.Instance.Show<SkinPopup>();
    }

    public void OnClickFacebookButton()
    {
        Application.OpenURL("https://www.facebook.com/groups/hero.tower.wars");
    }
}