using UnityEngine;

public class ButtonAd : MonoBehaviour
{
    [SerializeField] private ButtonCustom buttonCustom;
    [SerializeField] private GameObject loadingImage;

    private void Start()
    {
        if (AdController.Instance.IsRewardLoaded)
        {
            HideLoading();
        }
        else
        {
            ShowLoading();
        }

        EventController.AdsRewardLoaded += HideLoading;
        EventController.AdsRewardRequested += ShowLoading;
    }

    private void ShowLoading()
    {
        loadingImage.SetActive(true);
        buttonCustom.canClick = false;
    }

    private void HideLoading()
    {
        loadingImage.SetActive(false);
        buttonCustom.canClick = true;
    }
}
