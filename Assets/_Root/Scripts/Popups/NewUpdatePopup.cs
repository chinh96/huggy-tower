using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using I2.Loc;
public class NewUpdatePopup : Popup
{
    [SerializeField] private GameObject checkIcon;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private GameObject versionText;
    protected override void BeforeShow()
    {
        base.BeforeShow();
        versionText.GetComponent<LocalizationParamsManager>().SetParameterValue("VALUE", Application.version.ToString(), true);
        CheckIcon();
        description.text = RemoteConfigController.Instance.UpdateDescription.Replace("<br>", "\n");
    }

    public void OnClickCheckButton()
    {
        Data.DontShowUpdateAgain = !Data.DontShowUpdateAgain;
        CheckIcon();
    }

    public void CheckIcon()
    {
        checkIcon.SetActive(!Data.DontShowUpdateAgain);
    }

    public void OnClickUpdateButton()
    {
#if UNITY_ANDROID
        Application.OpenURL("market://details?id=com.gamee.wuggykissytower");
#else
        Application.OpenURL("itms-apps://itunes.apple.com/app/id1570840391");
#endif
    }
}
