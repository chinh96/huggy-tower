using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class AchievementGiftPopup : Popup
{
    [SerializeField] private SkeletonGraphic hero;

    private SkinData skinData;

    protected override void BeforeShow()
    {
        base.BeforeShow();

        skinData = ResourcesController.Hero.SkinAchievement;
        hero.ChangeSkin(skinData.SkinName);
    }

    public void OnClickClaimButton()
    {
        Data.CurrentSkinHero = skinData.SkinName;
        skinData.IsUnlocked = true;
        Close();
    }
}
