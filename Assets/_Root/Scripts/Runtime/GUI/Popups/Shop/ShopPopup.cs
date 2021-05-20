using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopPopup : Popup
{
    [SerializeField] private List<ShopItem> shopItems;

    private void Awake()
    {
        shopItems.ForEach(item => item.SetShopPopup(this));
    }

    public void CheckItems()
    {
        shopItems.ForEach(item => item.CheckNonConsume());
    }

    public void OnClickMoreCoins()
    {
        AdController.Instance.ShowRewardedAd(() =>
        {
            Data.CoinTotal += 500;
        });
    }
}
