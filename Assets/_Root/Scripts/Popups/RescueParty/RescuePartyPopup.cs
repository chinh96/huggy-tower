using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System;

public class RescuePartyPopup : Popup
{
    [SerializeField] private List<RescuePartyItem> rescuePartyItems;
    [SerializeField] private GameObject noti;

    protected override void AfterInstantiate()
    {
        base.AfterInstantiate();

        int index = 0;
        rescuePartyItems.ForEach(item =>
        {
            SkinData data = ResourcesController.SkinRescuePartys.Find(data => data.RescuePartyType == item.Type);
            item.Init(data);
            index++;
        });
    }

    protected override void BeforeShow()
    {
        base.BeforeShow();

        rescuePartyItems.ForEach(item => item.Reset());

        CheckNoti();
    }

    public void ShowLeaderboardRescueParty()
    {
        AnalyticController.ClickTop100HalloweenButton();
        Data.ClickedTop100Button = true;
        CheckNoti();
        LeaderboardRescuePartyController.Instance.Show();
    }

    private void CheckNoti()
    {
        noti.SetActive(!Data.ClickedTop100Button);
    }
}
