using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
using Facebook.Unity;

public class LoadingController : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private Ease ease;
    [SerializeField] private Image progress;

    private void Start()
    {
        Addressables.InitializeAsync();

        Vibration.Init();

        progress.fillAmount = 0;
        progress.DOFillAmount(5, duration).SetEase(ease).OnComplete(() =>
        {
            if (Data.IsIntro && RemoteConfigController.Instance.HasIntro)
            {
                SceneManager.LoadScene(Constants.INTRO_SCENE);
            }
            else
            {
                SceneManager.LoadScene(Constants.HOME_SCENE);
            }
        });
    }
}