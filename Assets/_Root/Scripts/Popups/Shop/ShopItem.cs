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

    public void PurchaseItem1()
    {
        IAPManager.Instance.PurchaseProduct(Constants.IAP_PACK1);
    }
    public void PurchaseItem2()
    {
        IAPManager.Instance.PurchaseProduct(Constants.IAP_PACK2);
    }
    public void PurchaseItem3()
    {
        IAPManager.Instance.PurchaseProduct(Constants.IAP_PACK3);
    }
    public void PurchaseItemUnlockHero()
    {
        IAPManager.Instance.PurchaseProduct(Constants.IAP_UNLOCK_HERO);
    }
    public void PurchaseItemRemoveAds()
    {
        IAPManager.Instance.PurchaseProduct(Constants.IAP_REMOVE_ADS);
    }
    public void PurchaseItemVip()
    {
        IAPManager.Instance.PurchaseProduct(Constants.IAP_VIP);
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
