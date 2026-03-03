using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

public class IAPManager : MonoBehaviour, IDetailedStoreListener
{
    public static IAPManager Get;

    private IStoreController controller;
    private IExtensionProvider extensions;

    private ShopItem currentItem;
    private Action<bool, ShopItem> callback;

    [SerializeField] internal IAPData iapData;

    public void InitiaizeIAPManager()
    {
        if (IsInitialized()) { return; }

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        foreach (var item in iapData.productIds)
        {
#if UNITY_ANDROID
            builder.AddProduct(item.androidProductId, item.productType);
#else
            builder.AddProduct(item.iosProductId, item.productType);
#endif
        }

        UnityPurchasing.Initialize(this, builder);
    }

    public void Restore()
    {
#if UNITY_IOS
        extensions.GetExtension<IAppleExtensions>().RestoreTransactions((result, message) => {
            if (result)
            {
                
            }
            else
            {
                
            }
        });
#endif
    }

    private void Awake()
    {
        Get = this;
    }

    private void Start()
    {
        InitiaizeIAPManager();
    }

    private bool IsInitialized()
    {
        return controller != null && extensions != null;
    }

    internal void BuyProduct(ShopItem item, Action<bool, ShopItem> callback)
    {

        if (IsInitialized())
        {
            currentItem = item;
            this.callback = callback;

#if UNITY_ANDROID
            string id = IAPManager.Get.iapData.productIds.Find(x => x.product == item.product).androidProductId;
#else
            string id = IAPManager.Get.iapData.productIds.Find(x => x.product == item.product).iosProductId;
#endif

            Product product = controller.products.WithID(id);

            if (product != null && product.availableToPurchase)
            {
                controller.InitiatePurchase(product);
            }
        }
        else
        {
#if UNITY_EDITOR
            callback?.Invoke(true, item);
#endif
        }
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("IAP Initialized");
        this.controller = controller;
        this.extensions = extensions;

        foreach (var item in iapData.productIds)
        {
#if UNITY_ANDROID
            item.price = controller.products.WithID(item.androidProductId).metadata.localizedPrice;
            item.currencyCode = controller.products.WithID(item.androidProductId).metadata.isoCurrencyCode;
#else
            item.price = controller.products.WithID(item.iosProductId).metadata.localizedPrice;
            item.currencyCode = controller.products.WithID(item.iosProductId).metadata.isoCurrencyCode;
#endif

        }

    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError(error.ToString());
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        Debug.Log("Purchased : " + e.purchasedProduct.definition.id);
        GameAnalyticsManager.Instance.NewDesignEventGA("buy_complete :" + e.purchasedProduct.definition.id);

#if UNITY_ANDROID
        if (e.purchasedProduct.definition.id == iapData.productIds.Find(x => x.shopResourcesType == ShopResourcesType.NoAds).androidProductId)
        {
            ActiveGameData.Instance.saveData.enabled_NoAds = true;
        }
        else if (e.purchasedProduct.definition.id == iapData.productIds.Find(x => x.shopResourcesType == ShopResourcesType.Speed3_5).androidProductId)
        {
            ActiveGameData.Instance.saveData.enabled_3_5 = true;
        }
        else if (e.purchasedProduct.definition.id == iapData.productIds.Find(x => x.shopResourcesType == ShopResourcesType.Speed5).androidProductId)
        {
            ActiveGameData.Instance.saveData.enabled_5 = true;
        }
#else
        if (e.purchasedProduct.definition.id == iapData.productIds.Find(x => x.shopResourcesType == ShopResourcesType.NoAds).iosProductId)
        {
            ActiveGameData.Instance.saveData.enabled_NoAds = true;
        }
        else if (e.purchasedProduct.definition.id == iapData.productIds.Find(x => x.shopResourcesType == ShopResourcesType.Speed3_5).iosProductId)
        {
            ActiveGameData.Instance.saveData.enabled_3_5 = true;
        }
        else if (e.purchasedProduct.definition.id == iapData.productIds.Find(x => x.shopResourcesType == ShopResourcesType.Speed5).iosProductId)
        {
            ActiveGameData.Instance.saveData.enabled_5 = true;
        }
#endif

        if (callback != null || currentItem != null)
        {
            callback?.Invoke(true, currentItem);
        }

        callback = null;
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
        Debug.LogError(p.ToString());
        if (callback != null || currentItem != null)
        {
            callback?.Invoke(false, currentItem);
        }
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError("Initalization Fail : " + message);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.LogError("Purchase Fail : " + failureDescription.message);
    }
}