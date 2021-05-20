using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItem : MonoBehaviour
{
    [SerializeField] private ShopItemType shopItemType;
    [SerializeField] private GameObject buyButton;

    private ShopPopup shopPopup;

    public void SetShopPopup(ShopPopup shopPopup)
    {
        this.shopPopup = shopPopup;
    }

    private void Start()
    {
        CheckBuyButton();
    }

    public void CheckBuyButton()
    {
        switch (shopItemType)
        {
            case ShopItemType.UnlockAllSkins:
                buyButton.SetActive(!Data.IsUnlockAllSkins);
                break;
            case ShopItemType.RemoveAds:
                buyButton.SetActive(!Data.IsRemovedAds);
                break;
            case ShopItemType.Vip:
                buyButton.SetActive(!Data.IsVip);
                break;
        }
    }

    public void OnPurchaseSuccess()
    {
        switch (shopItemType)
        {
            case ShopItemType.Gold1:
                Data.CoinTotal += 10000;
                break;
            case ShopItemType.Gold2:
                Data.CoinTotal += 10000;
                break;
            case ShopItemType.Gold3:
                Data.CoinTotal += 10000;
                break;
            case ShopItemType.UnlockAllSkins:
                Data.IsUnlockAllSkins = true;
                break;
            case ShopItemType.RemoveAds:
                Data.IsRemovedAds = true;
                break;
            case ShopItemType.Vip:
                Data.IsUnlockAllSkins = true;
                Data.IsRemovedAds = true;
                Data.IsVip = true;
                break;
        }

        shopPopup.CheckItems();
    }
}

enum ShopItemType
{
    Gold1,
    Gold2,
    Gold3,
    UnlockAllSkins,
    RemoveAds,
    Vip
}
