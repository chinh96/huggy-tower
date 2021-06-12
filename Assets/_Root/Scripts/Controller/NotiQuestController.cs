using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotiQuestController : Singleton<NotiQuestController>
{
    [SerializeField] private NotiQuest notiQuest;

    public void Show(DailyQuestData data)
    {
        NotiQuest notiQuest = Instantiate(this.notiQuest, this.notiQuest.transform.parent);
        notiQuest.transform.position = this.notiQuest.transform.position;
        notiQuest.Init(data);
        notiQuest.MoveIn();
    }
}
