using System;
using Lance.Common.Loading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class LoadingController : MonoBehaviour
{
    public LoadingScreenManager LoadingScreenManager;

    private void Start()
    {
        if (Data.DateTimeStart == "")
        {
            Data.DateTimeStart = DateTime.Now.ToString();
        }

        Addressables.InitializeAsync();
        LoadingScreenManager.LoadScene(Constants.HOME_SCENE);
    }
}