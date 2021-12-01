using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupEventNoel : MonoBehaviour
{
    [SerializeField]
    private List<ItemEventNoel> ListItem;
    private void Start()
    {
        InitData();
    }
    void InitData()
    {
        var data = ResourcesController.DailyEventReward.EventCollectRewards;
        for (int i = 0; i < data.Count; i++)
        {
            var itemData = data[i];
            var itemEvent = ListItem[i];
            itemEvent.InitItemEventNoel(StateClaimDailyEvent.WAITING_CLAIM, itemData, (e, gameObject) =>
              {

              });
        }
    }
}
