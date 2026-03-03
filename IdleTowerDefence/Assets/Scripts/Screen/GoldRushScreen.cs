using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoldRushScreen : ScreenPanel
{
    public ShopItem item;
    public TMP_Text goldRushTimerText;

    public GameObject purchase_Button;
    public GameObject purchased_Button;

    private string GoldRushStartTimeKey = "ITD_GoldRushOfferTime";
    private string GoldRushPurchasedKey = "ITD_GoldRush_Purchased";
    private Coroutine UpdateGoldRushTimerRoutine;

    private void OnEnable()
    {
        item.Initialize();
        StopAllCoroutines();
        CheckForGoldRushOffer();
    }

    internal void CheckForGoldRushOffer()
    {
        DateTime currentTime = DateTime.Now;

        // Parse the start time from PlayerPrefs
        DateTime goldRushStartTime = DateTime.Parse(PlayerPrefs.GetString(GoldRushStartTimeKey));
        DateTime goldRushEndTime = goldRushStartTime.AddHours(24);

        // Check if current time is within 24 hours from the start time
        if (currentTime <= goldRushEndTime)
        {
            if(PlayerPrefs.GetInt(GoldRushPurchasedKey, 0) == 1)
            {
                purchase_Button.gameObject.SetActive(false);
                purchased_Button.gameObject.SetActive(true);
            }
            else
            {
                purchase_Button.gameObject.SetActive(true);
                purchased_Button.gameObject.SetActive(false);
            }

            UpdateGoldRushTimerRoutine = StartCoroutine(UpdateGoldRushTimer(goldRushEndTime));
        }
        else
        {
            // Clear start time as the offer period has expired
            PlayerPrefs.DeleteKey(GoldRushStartTimeKey);
            PlayerPrefs.DeleteKey(GoldRushPurchasedKey);
            Hide();
        }
    }

    private IEnumerator UpdateGoldRushTimer(DateTime goldRushEndTime)
    {
        while (true)
        {
            TimeSpan timeRemaining = goldRushEndTime - DateTime.Now;

            if (timeRemaining.TotalSeconds > 0)
            {
                // Format remaining time as HH:MMh
                goldRushTimerText.text = $"{timeRemaining.Hours:D2}H {timeRemaining.Minutes:D2}M {timeRemaining.Seconds:D2}S";
            }
            else
            {
                // Time is up; hide the Gold Rush screen or reset
                goldRushTimerText.text = "00:00h";
                PlayerPrefs.DeleteKey(GoldRushStartTimeKey);
                PlayerPrefs.DeleteKey(GoldRushPurchasedKey);
                Hide();
                Debug.Log("Gold Rush offer has ended.");
                yield break; // Exit coroutine
            }

            yield return new WaitForSeconds(1f); // Wait for 1 second before updating again
        }
    }

    public void OnClick_Buy()
    {
        IAPManager.Get.BuyProduct(item, OnPurchase);
    }

    private void OnPurchase(bool status, ShopItem purchaseItem)
    {
        if (status)
        {
            //AppsFlyerEventManager.Get.SendEvent(AppsFlyerEventManager.AF_IAP_PURCHASED, purchaseItem.product.ToString());
            GameAnalyticsManager.Instance.NewDesignEventGA("IAP_Purchase :" + purchaseItem.product.ToString());
            HomeScreen homeScreen = ScreenManager.Get.GetScreen<HomeScreen>();
            CoinAnimation.Get.ShowCoin(homeScreen.goldCoinText.transform.position, purchaseItem.quantity, () =>
            {
                Constants.Get.GoldCoin += purchaseItem.quantity;
                EventManager.TriggerEvent(EventID.Update_Currency);

                purchase_Button.gameObject.SetActive(false);
                purchased_Button.gameObject.SetActive(true);

                PlayerPrefs.SetInt(GoldRushPurchasedKey, 1);

            });
        }
    }

    public void CloseButton()
    {
        Hide();
    }
}
