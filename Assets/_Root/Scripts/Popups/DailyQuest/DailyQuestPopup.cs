using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DailyQuestPopup : Popup
{
    [SerializeField] private Transform content;
    [SerializeField] private DailyQuestItem dailyQuestItem;
    [SerializeField] private GameObject dailyQuestDecor;
    [SerializeField] private CoinGeneration coinGeneration;

    private List<DailyQuestItem> dailyQuestItems = new List<DailyQuestItem>();

    protected override void AfterInstantiate()
    {
        base.AfterInstantiate();
        int index = 0;

        List<DailyQuestData> dailyQuestDatas = ResourcesController.DailyQuest.DailyQuestDatasCurrent;
        dailyQuestDatas.ForEach(dailyQuestData =>
        {
            DailyQuestItem item = Instantiate(dailyQuestItem, content);
            item.Init(dailyQuestData, this);
            dailyQuestItems.Add(item);

            if (index < dailyQuestDatas.Count - 1)
            {
                Instantiate(dailyQuestDecor, content);
            }
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
