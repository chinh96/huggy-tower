using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class GiftcodeGiftPopup : Popup
{
    [SerializeField] private SkeletonGraphic hero;

    private SkinData skinData;

    protected override void BeforeShow()
    {
        base.BeforeShow();

        skinData = ResourcesController.Hero.SkinGiftcode;
        hero.ChangeSkin(skinData.SkinName);
    }

    public void OnClickClaimButton()
    {
        Data.CurrentSkinHero = skinData.SkinName;
        Data.currentSkinHeroId = skinData.Id;
        skinData.IsUnlocked = true;
        EventController.SkinPopupReseted?.Invoke();
        Close();
    }
}
