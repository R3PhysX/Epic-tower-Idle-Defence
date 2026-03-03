using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

[CreateAssetMenu(fileName = "IAPData", menuName = "IAP/IAPData")]
public class IAPData : ScriptableObject
{
    [NonReorderable] public List<ProductIdData> productIds;
}

[Serializable]
public class ProductIdData
{
    public ProductName product;
    public string androidProductId;
    public string iosProductId;
    public decimal price;
    public ProductType productType;
    public ShopResourcesType shopResourcesType;
    internal string currencyCode;
}

[System.Serializable]
public enum ShopResourcesType
{
    Coin = 1,
    Gem = 2,
    NoAds = 3,
    Speed3_5 = 4,
    Speed5 = 5
}

[System.Serializable]
public enum ProductName
{
    CoinPack1 = 1,
    CoinPack2 = 2,
    CoinPack3 = 3,
    CoinPack4 = 4,
    CoinPack5 = 5,
    GemPack1 = 6,
    GemPack2 = 7,
    GemPack3 = 8,
    GemPack4 = 9,
    GemPack5 = 10,
    NoAds = 11,
    Speed3_5 = 12,
    Speed5 = 13,
    GoldRush = 14
}