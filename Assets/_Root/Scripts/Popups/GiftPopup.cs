using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using DG.Tweening;

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
            Data.currentSkinHeroId = Data.SkinGift.Id;
            Data.SkinGift.IsUnlocked = true;
            GameController.Instance.Player.ChangeSword();
            Close();

            AnalyticController.AdjustLogEventClaimGiftProcessWinLevel();
        });
    }
}
