using TMPro;
using UnityEngine;

public class NotificationPopup : Popup
{
    [SerializeField] private TextMeshProUGUI txtMessage;
    protected override void BeforeShow()
    {
        base.BeforeShow();

        txtMessage.text = (string)data;
    }
}
