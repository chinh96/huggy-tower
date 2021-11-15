using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System;

public class TGPopup : Popup
{
    [SerializeField] private List<TGItem> tgItems;
    [SerializeField] private GameObject noti;

    protected override void AfterInstantiate()
    {
        base.AfterInstantiate();

        int index = 0;
        tgItems.ForEach(item =>
        {
            SkinData data = ResourcesController.SkinsTG.Find(data => data.TGType == item.Type);
            item.Init(data, this);
            index++;
        });
    }

    protected override void BeforeShow()
    {
        base.BeforeShow();

        tgItems.ForEach(item => item.Reset());

        CheckNoti();
    }

    public void ShowTGRankPopup()
    {
        Data.ClickedTop100Button = true;
        CheckNoti();
        TGRankController.Instance.Show();
    }

    private void CheckNoti()
    {
        noti.SetActive(!Data.ClickedTop100Button);
    }

    public void Reset()
    {
        tgItems.ForEach(item => item.Reset());
    }
}
