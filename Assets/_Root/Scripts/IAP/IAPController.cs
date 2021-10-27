using UnityEngine;
using UnityEngine.Purchasing;

public class IAPController : MonoBehaviour
{
    public string Gold1 = "com.herotowerwar.gold1";
    public string Gold2 = "com.herotowerwar.gold2";
    public string Gold3 = "com.herotowerwar.gold3";
    public string RemoveAdsId = "com.herotowerwar.removeads";
    public string UnlockSkinsId = "com.herotowerwar.unlockhero";
    public string VipId = "com.herotowerwar.vip";

    private void Start()
    {
        CheckGold1(CodelessIAPStoreListener.Instance.StoreController);
        CheckGold2(CodelessIAPStoreListener.Instance.StoreController);
        CheckGold3(CodelessIAPStoreListener.Instance.StoreController);
        CheckRemoveAds(CodelessIAPStoreListener.Instance.StoreController);
        CheckUnlockSkins(CodelessIAPStoreListener.Instance.StoreController);
        CheckVip(CodelessIAPStoreListener.Instance.StoreController);
    }

    private void CheckGold1(IStoreController controller)
    {
        if (controller.products.WithID(Gold1).hasReceipt && !Data.IsGold1)
        {
            Data.IsGold1 = true;
            Data.CoinTotal += 50000;
        }
    }

    private void CheckGold2(IStoreController controller)
    {
        if (controller.products.WithID(Gold2).hasReceipt && !Data.IsGold2)
        {
            Data.IsGold2 = true;
            Data.CoinTotal += 150000;
        }
    }

    private void CheckGold3(IStoreController controller)
    {
        if (controller.products.WithID(Gold3).hasReceipt && !Data.IsGold3)
        {
            Data.IsGold3 = true;
            Data.CoinTotal += 500000;
        }
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
}
