using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class PanalItemWeek : MonoBehaviour
{
    [SerializeField] private GameObject itemDay;
    [SerializeField] private GameObject containerGrid;
    [SerializeField] private GameObject itemDay7;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void InitDataWeek(int week, List<ItemConfigEvent> weekData, DailyRewardEventItem.onClickHandle clickcb)
    {
        int i = 0;
        for (i = 0; i < weekData.Count && i < 6; i++)
        {

            Transform item;
            var itemCfg = weekData[i];
            int day = i + 7 * week;
            if (i < containerGrid.transform.childCount)
                item = containerGrid.transform.GetChild(i);
            else
            {
                item = Instantiate(itemDay, containerGrid.transform).transform;
                item.SetParent(containerGrid.transform);
            }
            var _comp = item.GetComponent<DailyRewardEventItem>();
            StateClaimDailyEvent state = GetStateItem(day, Data.DailyRewardEventCurrent);
            _comp.InitDailyItem(day, state, itemCfg, clickcb);
        }
        if (weekData.Count >= 7)
        {
            var itemCfg = weekData[weekData.Count - 1];
            int day7 = 6 + 7 * week;
            itemDay7.SetActive(true);
            var _comp = itemDay7.GetComponent<DailyRewardEventItem>();
            StateClaimDailyEvent state = GetStateItem(day7, Data.DailyRewardEventCurrent);
            _comp.InitDailyItem(day7, state, itemCfg, clickcb);
        }
        else
        {
            itemDay7.SetActive(false);
        }
        for (; i < containerGrid.transform.childCount; i++)
        {
            var item = containerGrid.transform.GetChild(i);
            item.gameObject.SetActive(false);
        }

    }
    private StateClaimDailyEvent GetStateItem(int day, int currentDay)
    {
        if (day > currentDay) return StateClaimDailyEvent.WAITING_CLAIM;
        if (day < currentDay) return StateClaimDailyEvent.CLAIMED;
        if (day == currentDay)
        {
            var time = DateTime.Now - DateTime.Parse(Data.lastTimeClaimDailyEvent);
            if (time.Days > 0)
                return StateClaimDailyEvent.CAN_CLAIM;
        }
        return StateClaimDailyEvent.WAITING_CLAIM;
    }


}
