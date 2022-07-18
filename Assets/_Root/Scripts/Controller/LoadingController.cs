using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
// using Facebook.Unity;

public class LoadingController : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private Ease ease;
    [SerializeField] private Image progressNormal;
    [SerializeField] private CanvasScaler canvasScaler;

    private Image progress;

    private void Awake()
    {
        progress = progressNormal;
        canvasScaler.matchWidthOrHeight = Camera.main.aspect > .7f ? 1 : 0;
    }



    private async void Start()
    {
        //if (Data.CurrentSkinHero == "Angel")
        //{
        //    Data.CurrentSkinHero = "Skin1";
        //}

        Addressables.InitializeAsync();

        Vibration.Init();

        LoadLevel();
        progress.fillAmount = 0;
        progress.DOFillAmount(5, duration).SetEase(ease).OnComplete(WaitLoadingScene);
    }

    private bool _flag;
    private async void WaitLoadingScene()
    {
        await UniTask.WaitUntil(() => _flag);
        SceneManager.LoadScene(Constants.HOME_SCENE);
    }

    public async void LoadLevel()
    {
        var obj = await DataBridge.Instance.GetLevel(Data.CurrentLevel);
        Debug.Log("Loaded...");
        _flag = true;
    }
}