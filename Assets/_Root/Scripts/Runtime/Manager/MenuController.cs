using System;
using Lance.Common;
using UnityEngine.SceneManagement;

public class MenuController : Singleton<MenuController>
{
    private void Start()
    {
        AdController.Instance.ShowBanner();
        // SceneManager.LoadScene(Constants.GAMEPLAY_SCENE);
    }
}