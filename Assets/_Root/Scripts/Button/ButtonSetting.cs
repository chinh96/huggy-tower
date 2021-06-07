using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSetting : MonoBehaviour
{
    public void OnClick()
    {
        PopupController.Instance.Show<SettingPopup>();
    }
}
