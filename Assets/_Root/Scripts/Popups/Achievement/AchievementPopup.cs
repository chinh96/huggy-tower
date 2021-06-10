using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementPopup : Popup
{
    [SerializeField] List<AchievementItem> achievementItems;

    protected override void BeforeShow()
    {
        base.BeforeShow();

        int index = 0;
        achievementItems.ForEach(item =>
        {
            item.Init(ResourcesController.Achievement.AchievementDatas[index]);
            index++;
        });
    }
}
