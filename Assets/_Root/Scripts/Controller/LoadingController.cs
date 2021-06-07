using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class LoadingController : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private Ease ease;
    [SerializeField] private Image progress;

    private void Start()
    {
        if (Data.DateTimeStart == "")
        {
            Data.DateTimeStart = DateTime.Now.ToString();
        }

        Addressables.InitializeAsync();

        progress.fillAmount = 0;
        progress.DOFillAmount(1, duration).SetEase(ease).OnComplete(() =>
        {
            SceneManager.LoadScene(Constants.HOME_SCENE);
        });

        DOTween.Sequence().AppendInterval(.5f).AppendCallback(() =>
        {
            SoundController.Instance.PlayBackground(SoundType.BackgroundHome);
        });
    }
}