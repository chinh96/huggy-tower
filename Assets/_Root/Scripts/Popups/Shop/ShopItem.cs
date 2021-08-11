using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class ShopItem : MonoBehaviour
{
    [SerializeField] private ShopItemType shopItemType;
    [SerializeField] private GameObject buyButton;
    [SerializeField] private GameObject doneIcon;

    private ShopPopup shopPopup;

    public void SetShopPopup(ShopPopup shopPopup)
    {
        this.shopPopup = shopPopup;
    }

    private void Start()
    {
        CheckNonConsume();
    }

    public void CheckNonConsume()
    {
        switch (shopItemType)
        {
            case ShopItemType.UnlockAllSkins:
                SetStateItems(Data.IsUnlockAllSkins);
                break;
            case ShopItemType.RemoveAds:
                SetStateItems(Data.IsRemovedAds);
                break;
            case ShopItemType.Vip:
                SetStateItems(Data.IsVip);
                break;
            default:
                SetStateItems(false);
                break;
        }
    }

    public void SetStateItems(bool done)
    {
        buyButton.SetActive(!done);
        doneIcon.SetActive(done);
    }

    public void OnPurchaseSuccess()
    {
        switch (shopItemType)
        {
            case ShopItemType.Gold1:
                Data.CoinTotal += 50000;
                break;
            case ShopItemType.Gold2:
                Data.CoinTotal += 150000;
                break;
            case ShopItemType.Gold3:
                Data.CoinTotal += 500000;
                break;
            case ShopItemType.UnlockAllSkins:
                Data.IsUnlockAllSkins = true;
                break;
            case ShopItemType.RemoveAds:
                Data.IsRemovedAds = true;
                HomeController.Instance.CheckButton();
                break;
            case ShopItemType.Vip:
                Data.CoinTotal += 500000;
                Data.IsUnlockAllSkins = true;
                Data.IsRemovedAds = true;
                Data.IsVip = true;
                break;
        }

        shopPopup.CheckItems();
        EventController.SkinPopupReseted();
    }

    public void OnPurchaseSuccessTracking(Product product)
    {
        switch (shopItemType)
        {
            case ShopItemType.Gold1:
                AnalyticController.AdjustLogEventPurchaseItem("kqr0x8", 0.99f, "USD", product.transactionID);
                break;
            case ShopItemType.Gold2:
                AnalyticController.AdjustLogEventPurchaseItem("ygs9gy", 1.99f, "USD", product.transactionID);
                break;
            case ShopItemType.Gold3:
                AnalyticController.AdjustLogEventPurchaseItem("kk1yzu", 4.99f, "USD", product.transactionID);
                break;
            case ShopItemType.UnlockAllSkins:
                AnalyticController.AdjustLogEventPurchaseItem("5dxgq2", 4.99f, "USD", product.transactionID);
                break;
            case ShopItemType.RemoveAds:
                AnalyticController.AdjustLogEventPurchaseItem("o6ssbb", 2.99f, "USD", product.transactionID);
                break;
            case ShopItemType.Vip:
                AnalyticController.AdjustLogEventPurchaseItem("usm42a", 9.99f, "USD", product.transactionID);
                break;
        }
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
