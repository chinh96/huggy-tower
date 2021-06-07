using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FacebookPopup : Popup
{
    public void OnClickFacebookButton()
    {
        Application.OpenURL("https://www.facebook.com/groups/hero.tower.wars");
    }
}
