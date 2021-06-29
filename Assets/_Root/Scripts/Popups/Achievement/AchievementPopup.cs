using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementPopup : Popup
{
    [SerializeField] private Transform content;
    [SerializeField] private AchievementItem achievementItem;
    [SerializeField] private GameObject decor;

    private List<AchievementItem> achievementItems = new List<AchievementItem>();

    protected override void AfterInstantiate()
    {
        base.AfterInstantiate();

        int index = 0;
        ResourcesController.Achievement.AchievementDatas.ForEach(data =>
        {
            achievementItems.Add(Instantiate(achievementItem, content));
            if (index < ResourcesController.Achievement.AchievementDatas.Count - 1)
            {
                Instantiate(decor, content);
            }
            index++;
        });
    }

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
