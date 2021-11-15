using UnityEngine;

public class TGLuckySpinGiftPopup : Popup
{
    protected override void BeforeShow()
    {
        base.BeforeShow();

        var data = ResourcesController.Hero.SkinLuckySpin;
        data.IsUnlocked = true;
        Data.CurrentSkinHero = data.SkinName;
    }
}
