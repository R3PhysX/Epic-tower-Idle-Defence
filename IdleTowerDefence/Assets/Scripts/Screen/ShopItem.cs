using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class ShopItem : MonoBehaviour
{
    public ProductName product;
    public ShopResourcesType resourcesType;
    public int quantity;
    public TMP_Text priceText;
    public TMP_Text quantityText;
    public CustomButton button;

    internal void Initialize()
    {
        if (quantityText != null)
            quantityText.text = quantity.ToString();

        if (priceText != null)
        {
            var findedproduct = IAPManager.Get.iapData.productIds.Find(x => x.product == product);
            priceText.text = findedproduct.currencyCode + " " + findedproduct.price.ToString("0.00");
        }
    }
}
