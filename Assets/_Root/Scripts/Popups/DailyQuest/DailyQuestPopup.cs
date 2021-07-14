using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DailyQuestPopup : Popup
{
    [SerializeField] private Transform content;
    [SerializeField] private DailyQuestItem dailyQuestItem;
    [SerializeField] private CoinGeneration coinGeneration;

    private List<DailyQuestItem> dailyQuestItems = new List<DailyQuestItem>();

    public void Init()
    {
        dailyQuestItems.Clear();
        content.Clear();
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

        List<DailyQuestDayItem> dailyQuestDayItems = ResourcesController.DailyQuest.DailyQuestDayCurrent.DailyQuestDayItems;
        dailyQuestDayItems.ForEach(dailyQuestDayItem =>
        {
            DailyQuestItem item = Instantiate(dailyQuestItem, content);
            item.Init(dailyQuestDayItem, this);
            dailyQuestItems.Add(item);
            index++;
        });
    }

    protected override void BeforeShow()
    {
        base.BeforeShow();

        dailyQuestItems.ForEach(item => item.Reset());
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
        }, from);
    }
}
