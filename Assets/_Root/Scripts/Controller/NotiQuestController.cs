using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotiQuestController : Singleton<NotiQuestController>
{
    [SerializeField] private NotiQuest notiQuest;

    private DailyQuestDayItem item;

    public void Save(DailyQuestDayItem item)
    {
        this.item = item;
    }

    public void Show()
    {
        // if (item != null && Data.CurrentLevel > 2)
        // {
        //     item.IsShownNoti = true;

        //     NotiQuest notiQuest = Instantiate(this.notiQuest, this.notiQuest.transform.parent);
        //     notiQuest.transform.position = this.notiQuest.transform.position;
        //     notiQuest.Init(item);
        //     notiQuest.MoveIn();

        //     item = null;
        // }
    }
}
