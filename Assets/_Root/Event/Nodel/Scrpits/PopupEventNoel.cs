using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupEventNoel : MonoBehaviour
{
    [SerializeField]
    private List<ItemEventNoel> ListItem;
    [SerializeField] private GameObject noti;
    private bool isInitData = false;
    private void Start()
    {
        InitData();
    }
    private void OnEnable()
    {
        // if (isInitData)
        // {
        updateState();
        // }
    }
    void InitData()
    {
        var data = ResourcesController.DailyEventReward.EventCollectRewards;
        for (int i = 0; i < data.Count; i++)
        {
            var itemData = data[i];
            var itemEvent = ListItem[i];
            var state = GetStateItem(itemData.NumCandyXmas, itemData.Id);
            itemEvent.InitItemEventNoel(state, itemData, (e, gameObject) =>
              {
                  TGDatas.ClaimedItems = Util.Add<string>(TGDatas.ClaimedItems, e.Id);

                  if (!e.isSkinPrincess)
                  {
                      var dataSkin = ResourcesController.Hero.SkinDatas[e.SkinId];
                      dataSkin.IsUnlocked = true;
                      Data.currentSkinHeroId = dataSkin.Id;
                      Data.CurrentSkinHero = dataSkin.SkinName;

                  }
                  else
                  {
                      var dataSkin = ResourcesController.Princess.SkinDatas[e.SkinId];
                      dataSkin.IsUnlocked = true;
                      Data.CurrentSkinPrincess = dataSkin.SkinName;
                  }

              });
        }
        isInitData = true;
    }
    void updateState()
    {
        var data = ResourcesController.DailyEventReward.EventCollectRewards;
        for (int i = 0; i < data.Count; i++)
        {
            var itemData = data[i];
            var itemEvent = ListItem[i];
            if (itemData.NumCandyXmas == -1) continue;
            var state = GetStateItem(itemData.NumCandyXmas, itemData.Id);
            itemEvent.SetStateItem(state);
        }
    }

    private StateClaimDailyEvent GetStateItem(int sockXMmas, string idItem)
    {
        if (sockXMmas == -1)
        {
            return StateClaimDailyEvent.WAITING_CLAIM;
        }
        if (sockXMmas > TGDatas.TotalTurkeyText)
            return StateClaimDailyEvent.WAITING_CLAIM;
        if (sockXMmas <= TGDatas.TotalTurkeyText)
        {
            for (int i = 0; i < TGDatas.ClaimedItems.Length; i++)
            {
                var id = TGDatas.ClaimedItems[i];
                if (idItem == id) return StateClaimDailyEvent.CLAIMED;
            }
            return StateClaimDailyEvent.CAN_CLAIM;
        }

        return StateClaimDailyEvent.WAITING_CLAIM;
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
    void onClickClaimCallBack()
    {

    }
}
