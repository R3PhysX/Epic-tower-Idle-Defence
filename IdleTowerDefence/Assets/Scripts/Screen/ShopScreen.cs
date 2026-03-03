using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopScreen : ScreenPanel
{
    [SerializeField] private TMP_Text unlockText;
    [SerializeField] private CustomButton unlock3xButton;
    [SerializeField] private GameObject unlock3xFreeButton;
    [SerializeField] private List<GameObject> unlockItems;
    [SerializeField] private List<Image> unlockItemsBar;
    [SerializeField] private GameObject selectedItem;
    [SerializeField] private TMP_Text offerOldPriceText;

    public Color rewardedItemColor;

    public List<ShopItem> coinItems;
    public List<ShopItem> gemItems;
      
    public ShopItem misc_noAds;
    public ShopItem misc_speed3_5;
    public ShopItem misc_speed_5;

    public Action<bool, ShopItem> onPurchase;

    public Transform currencyPanel;

    [SerializeField] private TMP_Text goldCoin;
    [SerializeField] private TMP_Text gemCoin;

    [SerializeField] private CustomButton restoreButton;

    [SerializeField] private GameObject x3AnimationPanel;
    [SerializeField] private Image glowImage;
    [SerializeField] private GameObject x3Image;
    [SerializeField] private GameObject continueButton;

    private void OnEnable()
    {
        GameAnalyticsManager.Instance.NewDesignEventGA("Click_Shop");
        restoreButton.gameObject.SetActive(false);
#if UNITY_IOS
        restoreButton.gameObject.SetActive(true);
        restoreButton.onClick.AddListener(IAPManager.Get.Restore);
#endif

        unlock3xButton.onClick.AddListener(Unlock_3xSpeed);
        selectedItem.gameObject.SetActive(true);
        if (ActiveGameData.Instance.saveData.enabled_3_adCount > 0)
            selectedItem.transform.position = unlockItems[ActiveGameData.Instance.saveData.enabled_3_adCount - 1].transform.position;

        for (int i = 5; i > ActiveGameData.Instance.saveData.enabled_3_adCount; i--)
            unlockItemsBar[i - 1].color = rewardedItemColor;

        if (ActiveGameData.Instance.saveData.enabled_3_adCount <= 0)
        {
            selectedItem.gameObject.SetActive(false);
            unlock3xButton.gameObject.SetActive(false);
            unlockText.text = "All rewards are collected!";
        }

        EventManager.AddListener(EventID.Update_Currency, Update_Currency);
        Update_Currency(default);
        coinItems.ForEach(x => { x.Initialize(); x.button.onClick.AddListener(() => { OnClick_BuyCoin(x); }); });
        gemItems.ForEach(x => { x.Initialize(); x.button.onClick.AddListener(() => { OnClick_BuyGem(x); }); });

        misc_noAds.Initialize();
        misc_speed3_5.Initialize();
        misc_speed_5.Initialize();

        var findedproduct = IAPManager.Get.iapData.productIds.Find(x => x.product == ProductName.Speed3_5);
        offerOldPriceText.text = findedproduct.currencyCode + " " + findedproduct.price.ToString("0.00");

        ResetMisc();
    }

    private void Unlock_3xSpeed()
    {
        float timeScale = Time.timeScale;
        AdManager.Get.ShowRewardedAd((status) =>
        {
            Time.timeScale = timeScale;
            if (status)
            {
                Debug.Log("Reward Garnted");

                if (ActiveGameData.Instance.saveData.enabled_3_adCount == 5)
                {
                    unlockItemsBar[ActiveGameData.Instance.saveData.enabled_3_adCount - 1].color = rewardedItemColor;
                    CoinAnimation.Get.ShowCoin(currencyPanel.transform.position, 100, () =>
                    {
                        Constants.Get.GoldCoin += 100;
                        EventManager.TriggerEvent(EventID.Update_Currency);
                    });
                }
                else if (ActiveGameData.Instance.saveData.enabled_3_adCount == 4)
                {
                    unlockItemsBar[ActiveGameData.Instance.saveData.enabled_3_adCount - 1].color = rewardedItemColor;
                    CoinAnimation.Get.ShowGem(currencyPanel.transform.position, 25, () =>
                    {
                        Constants.Get.Gems += 25;
                        EventManager.TriggerEvent(EventID.Update_Currency);
                    });
                }
                else if (ActiveGameData.Instance.saveData.enabled_3_adCount == 3)
                {
                    unlockItemsBar[ActiveGameData.Instance.saveData.enabled_3_adCount - 1].color = rewardedItemColor;
                    CoinAnimation.Get.ShowCoin(currencyPanel.transform.position, 250, () =>
                    {
                        Constants.Get.GoldCoin += 250;
                        EventManager.TriggerEvent(EventID.Update_Currency);
                    });
                }
                else if (ActiveGameData.Instance.saveData.enabled_3_adCount == 2)
                {
                    unlockItemsBar[ActiveGameData.Instance.saveData.enabled_3_adCount - 1].color = rewardedItemColor;
                    CoinAnimation.Get.ShowGem(currencyPanel.transform.position, 50, () =>
                    {
                        Constants.Get.Gems += 50;
                        EventManager.TriggerEvent(EventID.Update_Currency);
                    });
                }

                unlockItemsBar[ActiveGameData.Instance.saveData.enabled_3_adCount - 1].color = rewardedItemColor;

                ActiveGameData.Instance.saveData.enabled_3_adCount -= 1;
                selectedItem.gameObject.SetActive(true);
                if(ActiveGameData.Instance.saveData.enabled_3_adCount > 0)
                    selectedItem.transform.position = unlockItems[ActiveGameData.Instance.saveData.enabled_3_adCount - 1].transform.position;

                if (ActiveGameData.Instance.saveData.enabled_3_adCount <= 0)
                {
                    // AppsFlyerEventManager.Get.SendEvent(AppsFlyerEventManager.AF_Ad_Pass_Completed, "1");
                    GameAnalyticsManager.Instance.NewDesignEventGA("Ad_Pass_Completed");
                    selectedItem.gameObject.SetActive(false);
                    unlock3xButton.gameObject.SetActive(false);
                    Show3xAnimation();
                    ActiveGameData.Instance.saveData.enabled_3 = true;
                    unlockText.text = "All rewards are collected!";
                }
            }
            else
            {
                Debug.Log("Reward Not Granted");
                ToastManager.Get.ShowMessage("Ads Not Available");
            }
        });
    }

    private void ResetMisc()
    {
        misc_noAds.button.onClick.RemoveAllListeners();
        misc_speed3_5.button.onClick.RemoveAllListeners();
        misc_speed_5.button.onClick.RemoveAllListeners();

        if (ActiveGameData.Instance.saveData.enabled_NoAds)
        {
            misc_noAds.button.GetComponentInChildren<TMP_Text>().text = "Purchased";
            unlock3xFreeButton.gameObject.SetActive(true);
        }
        else
            misc_noAds.button.onClick.AddListener(() => { OnClick_BuyMisc(misc_noAds); });

        if (ActiveGameData.Instance.saveData.enabled_3_5)
            misc_speed3_5.button.GetComponentInChildren<TMP_Text>().text = "Purchased";
        else
            misc_speed3_5.button.onClick.AddListener(() => { OnClick_BuyMisc(misc_speed3_5); });

        if (ActiveGameData.Instance.saveData.enabled_5)
            misc_speed_5.button.GetComponentInChildren<TMP_Text>().text = "Purchased";
        else
            misc_speed_5.button.onClick.AddListener(() => { OnClick_BuyMisc(misc_speed_5); });
    }

    private void OnClick_BuyMisc(ShopItem misc_Product)
    {
        IAPManager.Get.BuyProduct(misc_Product, OnPurchase);
    }

    private void Update_Currency(object arg)
    {
        GoldCoinTextAnim(int.Parse(goldCoin.text), Constants.Get.GoldCoin);
        GemTextAnim(int.Parse(gemCoin.text), Constants.Get.Gems);
    }

    private void GoldCoinTextAnim(int from, int to)
    {
        LeanTween.cancel(goldCoin.gameObject);
        LeanTween.value(goldCoin.gameObject, (value) => {
            goldCoin.text = value.ToString("0");
        }, from, to, 0.5f).setOnComplete(() => {
            goldCoin.text = Constants.Get.GoldCoin.ToString();
        });
    }

    private void GemTextAnim(int from, int to)
    {
        LeanTween.cancel(gemCoin.gameObject);
        LeanTween.value(gemCoin.gameObject, (value) => {
            gemCoin.text = value.ToString("0");
        }, from, to, 0.5f).setOnComplete(() => {
            gemCoin.text = Constants.Get.Gems.ToString();
        });
    }

    private void OnClick_BuyCoin(ShopItem item)
    {
        IAPManager.Get.BuyProduct(item, OnPurchase);
    }

    private void OnClick_BuyGem(ShopItem item)
    {
        IAPManager.Get.BuyProduct(item, OnPurchase);
    }

    private void OnPurchase(bool status, ShopItem purchaseItem)
    {
       /* if(status)
            AppsFlyerEventManager.Get.SendEvent(AppsFlyerEventManager.AF_IAP_PURCHASED, purchaseItem.product.ToString());
*/
        if (status && purchaseItem.resourcesType == ShopResourcesType.Coin)
        {
            CoinAnimation.Get.ShowCoin(currencyPanel.transform.position, purchaseItem.quantity, () =>
            {
                Constants.Get.GoldCoin += purchaseItem.quantity;
                EventManager.TriggerEvent(EventID.Update_Currency);
            });
        }
        else if (status && purchaseItem.resourcesType == ShopResourcesType.Gem)
        {
            CoinAnimation.Get.ShowGem(currencyPanel.transform.position, purchaseItem.quantity, () =>
            {
                Constants.Get.Gems += purchaseItem.quantity;
                EventManager.TriggerEvent(EventID.Update_Currency);
            });
        }
        else if (status && purchaseItem.resourcesType == ShopResourcesType.NoAds)
        {
            ActiveGameData.Instance.saveData.enabled_NoAds = true;
            ResetMisc();
        }
        else if (status && purchaseItem.resourcesType == ShopResourcesType.Speed3_5)
        {
            ActiveGameData.Instance.saveData.enabled_3_5 = true;
            ResetMisc();
        }
        else if (status && purchaseItem.resourcesType == ShopResourcesType.Speed5)
        {
            ActiveGameData.Instance.saveData.enabled_5 = true;
            ResetMisc();
        }
        else
        {
            ToastManager.Get.ShowMessage("Purchase Failed !");
        }
    }

    [ContextMenu("Show3xAnimation")]
    private void Show3xAnimation()
    {
        LeanTween.cancel(glowImage.gameObject);
        LeanTween.cancel(x3Image);

        x3Image.transform.localScale = Vector3.zero;
        x3AnimationPanel.gameObject.SetActive(true);
        continueButton.gameObject.SetActive(false);
        glowImage.color = new Color(1, 1, 1, 0);

        x3AnimationPanel.gameObject.SetActive(true);

        LeanTween.delayedCall(1f, ()=> {

            LeanTween.value(glowImage.gameObject, (updateValue) => {
                glowImage.color = new Color(1, 1, 1, updateValue);
            }, 0f, 1f, 1f);

            LeanTween.rotateAroundLocal(glowImage.gameObject, Vector3.forward, 360f, 1f)
                     .setEase(LeanTweenType.linear) // Linear rotation
                     .setRepeat(-1);
        });
        
        LeanTween.scale(x3Image, Vector3.one, 1f).setEase(LeanTweenType.easeOutBack);

        LeanTween.delayedCall(2f, () =>
        {
            continueButton.gameObject.SetActive(true);
        });

    }

    public void Hide3xPanel()
    {
        x3AnimationPanel.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        unlock3xButton.onClick.RemoveAllListeners();
        EventManager.RemoveListener(EventID.Update_Currency, Update_Currency);
        restoreButton.onClick.RemoveAllListeners();
        coinItems.ForEach(x => { x.button.onClick.RemoveAllListeners(); });
        gemItems.ForEach(x => { x.button.onClick.RemoveAllListeners(); });
    }
}