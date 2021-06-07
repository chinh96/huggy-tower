using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonFacebook : MonoBehaviour
{
    public void OnClick()
    {
        PopupController.Instance.Show<FacebookPopup>();
    }
}
