using Lance.Common.Loading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class LoadingController : MonoBehaviour
{
    public LoadingScreenManager LoadingScreenManager;

    private void Start()
    {
        Addressables.InitializeAsync();
        LoadingScreenManager.LoadScene(Constants.MENU_SCENE);
    }
}