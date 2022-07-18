using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using I2.Loc;

public class AchievementPopup : Popup
{
    [SerializeField] private Transform content;
    [SerializeField] private AchievementItem achievementItem;
    [SerializeField] private CoinGeneration coinGeneration;
    [SerializeField] private Image progress;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private List<GameObject> heroes;
    [SerializeField] private List<GameObject> doneIcons;

    private List<AchievementItem> achievementItems = new List<AchievementItem>();
    private AchievementDailyQuestPopup achievementDailyQuestPopup;

    public void Init(AchievementDailyQuestPopup achievementDailyQuestPopup)
    {
        achievementItems.Clear();
        content.Clear();
        this.achievementDailyQuestPopup = achievementDailyQuestPopup;
        AfterInstantiate();
    }

    public void Show()
    {
        BeforeShow();
    }

    protected override void AfterInstantiate()
    {
        base.AfterInstantiate();

        int index = 0;
        //ResourcesController.Achievement.AchievementDatas.ForEach(data =>
        //{
        //    achievementItems.Add(Instantiate(achievementItem, content));
        //    index++;
        //});
    }

    protected override void BeforeShow()
    {
        base.BeforeShow();

        int index = 0;
        achievementItems.ForEach(item =>
        {
            //item.Init(ResourcesController.Achievement.AchievementDatas[index], this);
            index++;
        });
    }

    public void GenerateCoin(GameObject from, int bonus)
    {
        int coinTotal = Data.CoinTotal + bonus;

        coinGeneration.GenerateCoin(() =>
        {
            Data.CoinTotal++;
        }, () =>
        {
            Data.CoinTotal = coinTotal;
            UpdateProgress(true);
        }, from);
    }

    public void UpdateProgress(bool hasAnimation = false)
    {
        //int total = ResourcesController.Achievement.AchievementDatas.Count;
        //int number = ResourcesController.Achievement.GetDatasIsClaimed().Count;
        ////text.text = $"Complete {number}/{total}";
        //text.GetComponent<LocalizationParamsManager>().SetParameterValue("VALUE", number + "/" + total, true);

        //float endValue = (float)number / total;
        //if (hasAnimation)
        //{
        //    progress.DOFillAmount(endValue, .3f).OnComplete(() =>
        //    {
        //        var index = ResourcesController.Achievement.AchievementTargetDatas.FindIndex(item => item.Number == number);
        //        if (index >= 0 && !ResourcesController.Achievement.AchievementTargetDatas[index].IsClaimed)
        //        {
        //            PopupController.Instance.Show<AchievementGiftPopup>(index, ShowAction.DoNothing);
        //            achievementDailyQuestPopup.Close();
        //        }
        //    });
        //}
        //else
        //{
        //    progress.fillAmount = endValue;
        //}

        //int index = 0;
        //ResourcesController.Achievement.AchievementTargetDatas.ForEach(item =>
        //{
        //    heroes[index].SetActive(!item.IsClaimed);
        //    doneIcons[index].SetActive(item.IsClaimed);
        //    index++;
        //});
    }
}
