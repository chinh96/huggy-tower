using System;
using System.Collections.Generic;
//using SimpleJSON;
using UnityEngine;
using UnityEngine.Purchasing;

// ReSharper disable once InconsistentNaming
public class IAPManager : Singleton<IAPManager>, IStoreListener
{
    #region properties

    public string googleStoreKey;
    public IAPListener listener;
    public event Action<PurchaseEventArgs> PurchaseSucceededEvent;
    public event Action<string> PurchaseFailedEvent;
    private IStoreController _controller;
    private IExtensionProvider _extensions;
    public List<IAPData> SkuItem { get; set; } = new List<IAPData>();
    public InformationPurchaseResult ReceiptInfo { get; set; }

    public bool IsInitialize { get; set; }

    #endregion

    #region method

    public void OnInitializeFailed(
        InitializationFailureReason error)
    {
        switch (error)
        {
            case InitializationFailureReason.AppNotKnown:
                Debug.LogError("Is your App correctly uploaded on the relevant publisher console?");
                break;
            case InitializationFailureReason.PurchasingUnavailable:
                Debug.LogWarning("Billing disabled!");
                break;
            case InitializationFailureReason.NoProductsAvailable:
                Debug.LogWarning("No products available for purchase!");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(error), error, null);
        }
    }

    public PurchaseProcessingResult ProcessPurchase(
        PurchaseEventArgs e)
    {
#if GET_RECEIPTINFO && !UNITY_EDITOR
#if UNITY_ANDROID
            ReceiptInfo = GetIapAndroidInformationPurchase(e.purchasedProduct.receipt);
#elif UNITY_IOS
            ReceiptInfo = GetIapIosInformationPurchase(e.purchasedProduct.receipt);
#endif
#endif
        PurchaseVerified(e);
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(
        Product i,
        PurchaseFailureReason p)
    {
        //TODO Log Firebase
        PurchaseFailedEvent?.Invoke(p.ToString());
    }

    public void OnInitialized(
        IStoreController controller,
        IExtensionProvider extensions)
    {
        _controller = controller;
        _extensions = extensions;
        //validate
    }

    public void Initialized(
        IEnumerable<IAPData> skuItems)
    {
        if (this != Instance)
        {
            return;
        }
        
        if (IsInitialize)  return;
        SkuItem.Clear();
        SkuItem.AddRange(skuItems);
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        RequestProductData(builder);
        builder.Configure<IGooglePlayConfiguration>();

        UnityPurchasing.Initialize(this, builder);
        listener.Initialized(this);
        IsInitialize = true;
    }

    private void RequestProductData(
        ConfigurationBuilder builder)
    {
        // ReSharper disable once ForCanBeConvertedToForeach
        for (var i = 0; i < SkuItem.Count; i++)
        {
            var sku = SkuItem[i].sku;
            Enum.TryParse(SkuItem[i].productType,out ProductType type);
            builder.AddProduct(sku, type);
        }
    }

#if GET_RECEIPTINFO && !UNITY_EDITOR
    private InformationPurchaseResult GetIapAndroidInformationPurchase(
        string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        var jsonNode = JSON.Parse(json)["Payload"]
            .Value;
        var jsonData = JSON.Parse(jsonNode)["json"]
            .Value;
        var purchaseTime = JSON.Parse(jsonData)["purchaseTime"]
            .AsLong;
        const string device = "ANDROID";
        var productId = JSON.Parse(jsonData)["productId"]
            .Value;
        var iapType = GetIapType(productId);
        var transactionId = JSON.Parse(json)["TransactionID"]
            .Value;
        var purchaseState = JSON.Parse(jsonData)["purchaseState"]
            .AsInt;
        var purchaseToken = JSON.Parse(jsonData)["purchaseToken"]
            .Value;
        var signature = JSON.Parse(jsonNode)["signature"]
            .Value;
        return new InformationPurchaseResult(device,
            iapType,
            transactionId,
            productId,
            purchaseState,
            purchaseTime,
            purchaseToken,
            signature,
            SystemInfo.deviceUniqueIdentifier);
    }

    private InformationPurchaseResult GetIapIosInformationPurchase(
        string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        var jsonNode = JSON.Parse(json)["receipt"]
            .Value;
        var purchaseTime = JSON.Parse(jsonNode)["receipt_creation_date_ms"]
            .AsLong;
        const string device = "IOS";
        var productId = JSON.Parse(jsonNode)["in_app"][0]["product_id"]
            .Value;
        var iapType = GetIapType(productId);
        var transactionId = JSON.Parse(jsonNode)["in_app"][0]["transaction_id"]
            .Value;
        var purchaseState = JSON.Parse(json)["status"]
            .AsInt;
        const string purchaseToken = "";
        const string signature = "";
        return new InformationPurchaseResult(device,
            iapType,
            transactionId,
            productId,
            purchaseState,
            purchaseTime,
            purchaseToken,
            signature,
            SystemInfo.deviceUniqueIdentifier);
    }
#endif

    public void RestorePurchase()
    {
#if UNITY_IOS
        if (!IsInitialize)
        {
            Debug.Log("Restore purchases fail. not initialized!");
            return;
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
        {
            Debug.Log("Restore purchase started ...");

            var storeProvider = _extensions.GetExtension<IAppleExtensions>();
            storeProvider.RestoreTransactions(_ =>
            {
                // no purchase are avaiable to restore
                Debug.Log("Restore purchase continuting: " + _ + ". If no further messages, no purchase available to restore.");
            });
        }
        else
        {
            Debug.Log("Restore purchase fail. not supported on this platform. current = " + Application.platform);
        }
#endif
    }

    private void PurchaseVerified(
        PurchaseEventArgs e)
    {
        //TODO Log Firebase
        PurchaseSucceededEvent?.Invoke(e);
    }

    public void PurchaseProduct(
        string productId)
    {
        //TODO Log Firebase
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        _controller?.InitiatePurchase(productId);
#endif
    }

    private string GetIapType(
        string sku)
    {
        // ReSharper disable once ForCanBeConvertedToForeach
        for (var i = 0; i < SkuItem.Count; i++)
        {
            if (SkuItem[i]
                .sku.Equals(sku))
            {
                return SkuItem[i]
                    .productType;
            }
        }

        return "";
    }

    private Product GetProduct(
        string productId)
    {
        if (_controller?.products != null && !string.IsNullOrEmpty(productId))
        {
            return _controller.products.WithID(productId);
        }

        return null;
    }

    #endregion
}