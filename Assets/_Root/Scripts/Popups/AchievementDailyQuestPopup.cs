using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementDailyQuestPopup : Popup
{
    [SerializeField] private AchievementPopup achievementPopup;
    [SerializeField] private DailyQuestPopup dailyQuestPopup;
    [SerializeField] private GameObject achievementActiveButton;
    [SerializeField] private GameObject dailyQuestActiveButton;

    protected override void AfterInstantiate()
    {
        base.AfterInstantiate();

        achievementPopup.Init();
        dailyQuestPopup.Init();
    }

    protected override void BeforeShow()
    {
        base.BeforeShow();

        ResourcesController.DailyQuest.IncreaseByType(DailyQuestType.BuySkin);
        ResourcesController.Achievement.IncreaseByType(AchievementType.BuySkin);

        achievementPopup.Show();
        dailyQuestPopup.Show();
        achievementPopup.UpdateProgress();

        OnClickDailyQuestButton();
    }

    public void OnClickDailyQuestButton()
    {
        achievementPopup.gameObject.SetActive(false);
        achievementActiveButton.SetActive(false);

        dailyQuestPopup.gameObject.SetActive(true);
        dailyQuestActiveButton.SetActive(true);
    }

    public void OnClickAchievementButton()
    {
        achievementPopup.gameObject.SetActive(true);
        achievementActiveButton.SetActive(true);

        dailyQuestPopup.gameObject.SetActive(false);
        dailyQuestActiveButton.SetActive(false);
    }
}
