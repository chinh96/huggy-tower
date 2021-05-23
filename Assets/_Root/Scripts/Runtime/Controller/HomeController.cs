using UnityEngine.SceneManagement;

public class HomeController : Singleton<HomeController>
{
    private void Start()
    {
        AdController.Instance.ShowBanner();
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

    public void ShowCastlePopup()
    {

    }

    public void ShowSkinPopup()
    {
        PopupController.Instance.Show<SkinPopup>();
    }
}