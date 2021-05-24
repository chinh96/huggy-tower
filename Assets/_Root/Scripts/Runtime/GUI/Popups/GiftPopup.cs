using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class GiftPopup : Popup
{
    [SerializeField] private SkeletonGraphic hero;

    protected override void BeforeShow()
    {
        base.BeforeShow();

        hero.ChangeSkin(Data.SkinGift.SkinName);
    }

    public void OnClickClaim()
    {
        AdController.Instance.ShowRewardedAd(() =>
        {
            Data.CurrentSkinHero = Data.SkinGift.SkinName;
            Data.SkinGift.IsUnlocked = true;
            Close();
        });
    }
}
