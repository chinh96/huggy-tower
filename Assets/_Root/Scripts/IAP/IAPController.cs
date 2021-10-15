using UnityEngine;
using UnityEngine.Purchasing;

public class IAPController : Singleton<IAPController>, IStoreListener
{
    public string RemoveAdsId = "com.herotowerwar.removeads";
    public string UnlockSkinsId = "com.herotowerwar.unlockhero";
    public string VipId = "com.herotowerwar.vip";

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        InitializePurchasing();
    }

    public void InitializePurchasing()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        builder.AddProduct(RemoveAdsId, ProductType.NonConsumable);
        builder.AddProduct(UnlockSkinsId, ProductType.NonConsumable);
        builder.AddProduct(VipId, ProductType.NonConsumable);

        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        CheckRemoveAds(controller);
        CheckUnlockSkins(controller);
        CheckVip(controller);
    }

    private void CheckRemoveAds(IStoreController controller)
    {
        if (controller.products.WithID(RemoveAdsId).hasReceipt && !Data.IsRemovedAds)
        {
            Data.IsRemovedAds = true;
        }
    }

    private void CheckUnlockSkins(IStoreController controller)
    {
        if (controller.products.WithID(UnlockSkinsId).hasReceipt && !Data.IsUnlockAllSkins)
        {
            Data.IsUnlockAllSkins = true;
        }
    }

    private void CheckVip(IStoreController controller)
    {
        if (controller.products.WithID(VipId).hasReceipt && !Data.IsVip)
        {
            Data.IsRemovedAds = true;
            Data.IsUnlockAllSkins = true;
            Data.IsVip = true;
            Data.CoinTotal += 500000;
        }
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        return PurchaseProcessingResult.Complete;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
    }
}
