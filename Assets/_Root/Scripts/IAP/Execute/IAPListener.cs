#pragma warning disable 0649
using System;
using UnityEngine;
using UnityEngine.Purchasing;


// ReSharper disable once InconsistentNaming
public class IAPListener : MonoBehaviour
{
    private IAPManager _iapManager;
    public Action actionSuccess;
    public Action actionFail;

    public void Initialized(IAPManager iapManager)
    {
        _iapManager = iapManager;
        iapManager.PurchaseSucceededEvent += HandlePurchaseSuccess;
        iapManager.PurchaseFailedEvent += HandlePurchaseFaild;
    }

    private void HandlePurchaseFaild(string obj)
    {
        //SoundController.Current.PLayPurchaseFaild();
        // var popupController = ServiceLocator.Instance.Resolve<PopupController>();
        // popupController.ShowNotification(Constant.TEXT_NOTIFICATION_TITLE, Constant.TEXT_PURCHASE_FAILD, () =>
        // {
        //     popupController.ReleaseStack();
        //     actionFail?.Invoke();
        //     actionFail = null;
        // });
    }

    private void HandlePurchaseSuccess(PurchaseEventArgs e)
    {
        //SoundController.Current.PLayPurchaseSuccess();
#if !UNITY_EDITOR
        //if (_iapManager.receiptInfo != null)
        //ClientRequest.UpdateIAP (JsonUtility.ToJson (_iapManager), obj);
#endif

        var shop = PopupController.Instance.GetTopPopup() as ShopPopup;

        //TODO show purchase success.
        // ReSharper disable once SwitchStatementMissingSomeCases
        switch (e.purchasedProduct.definition.id)
        {
            case "com.herotowerwar.gold1":
                Data.CoinTotal += 50000;
                AnalyticController.AdjustLogEventPurchaseItem("kqr0x8", 0.99f, "USD", e.purchasedProduct.transactionID);
                break;
            case "com.herotowerwar.gold2":
                Data.CoinTotal += 150000;
                AnalyticController.AdjustLogEventPurchaseItem("ygs9gy", 1.99f, "USD", e.purchasedProduct.transactionID);
                break;
            case "com.herotowerwar.gold3":
                Data.CoinTotal += 500000;
                AnalyticController.AdjustLogEventPurchaseItem("kk1yzu", 4.99f, "USD", e.purchasedProduct.transactionID);
                break;
            case "com.herotowerwar.removeads":

                Data.IsRemovedAds = true;
                shop?.CheckItems();
                AnalyticController.AdjustLogEventPurchaseItem("o6ssbb", 2.99f, "USD", e.purchasedProduct.transactionID);
                if (HomeController.Instance != null) HomeController.Instance.CheckButton();
                break;
            case "com.herotowerwar.unlockhero":
                Data.IsUnlockAllSkins = true;
                AnalyticController.AdjustLogEventPurchaseItem("5dxgq2", 4.99f, "USD", e.purchasedProduct.transactionID);
                
                shop?.CheckItems();
                EventController.SkinPopupReseted();
                break;
            case "com.herotowerwar.vip":
                Data.IsUnlockAllSkins = true;
                Data.IsRemovedAds = true;
                Data.IsVip = true;
                Data.CoinTotal += 500000;
                AnalyticController.AdjustLogEventPurchaseItem("usm42a", 9.99f, "USD", e.purchasedProduct.transactionID);
                shop?.CheckItems();

                break;
        }
    }
}