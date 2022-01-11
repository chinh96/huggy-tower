using System;

[Serializable]
// ReSharper disable once InconsistentNaming
public class IAPData
{
    public string sku;
    public string productType;

    public IAPData()
    {
    }

    public IAPData(string sku, string productType)
    {
        this.sku = sku;
        this.productType = productType;
    }
}