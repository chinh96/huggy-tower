using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using I2.Loc;

public class SettingPopup : Popup
{
    [SerializeField] private GameObject purchaseButton;
    [SerializeField] private TextMeshProUGUI versionText;
    [SerializeField] private GameObject updateButton;

    private void Awake()
    {
        purchaseButton.SetActive(false);
    }

    protected override void BeforeShow()
    {
        base.BeforeShow();

        //versionText.text = $"Version {Application.version}";
        versionText.GetComponent<LocalizationParamsManager>().SetParameterValue("VALUE", Application.version.ToString(), true);
#if UNITY_IOS
        purchaseButton.SetActive(true);
#endif
        updateButton.SetActive(RemoteConfigController.Instance.HasNewUpdate);
    }

    public void OnClickUpdateButton()
    {
#if UNITY_ANDROID
        Application.OpenURL("market://details?id=com.gamee.herotowerwar");
#else
        Application.OpenURL("itms-apps://itunes.apple.com/app/id1570840391");
#endif
    }

    public void OnClickRateUs()
    {
        RatingController.Instance.LinkToStore();
    }

    public void OnClickShowPopupSelect() 
    {
        PopupController.Instance.Show<PopupSelectLanguage>();
    }
}
