using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GiftcodePopup : Popup
{
    [SerializeField] private TMP_InputField input;
    [SerializeField] private GameObject textWarning;
    [SerializeField] private GameObject useButtonDeactive;

    private SkinData skinGiftcode;

    protected override void AfterInstantiate()
    {
        base.AfterInstantiate();

        skinGiftcode = ResourcesController.Hero.SkinGiftcode;
    }

    public void OnClickUseButton()
    {
        if (input.text == skinGiftcode.Giftcode)
        {
            PopupController.Instance.Show<GiftcodeGiftPopup>(null, ShowAction.DoNothing);
            Close();
        }
        else
        {
            textWarning.SetActive(true);
        }
    }

    public void OnClickFacebookGroupButton()
    {
        PopupController.Instance.Show<FacebookPopup>(null, ShowAction.DoNothing);
        Close();
    }

    public void OnChangeInput()
    {
        input.text = input.text.ToUpper();
        useButtonDeactive.SetActive(input.text.Length != 12);
    }
}
