using UnityEngine;

public class ButtonAd : MonoBehaviour
{
    [SerializeField] private GameObject loadingImage;

    private void Start()
    {
        EventController.AdsRewardLoaded += HideLoading;
        EventController.AdsRewardRequested += ShowLoading;

        HideLoading();
    }

    private void ShowLoading()
    {
        loadingImage.SetActive(true);
    }

    private void HideLoading()
    {
        loadingImage.SetActive(false);
    }
}
