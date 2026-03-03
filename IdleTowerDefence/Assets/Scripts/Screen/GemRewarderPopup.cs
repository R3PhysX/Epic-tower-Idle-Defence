using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GemRewarderPopup : ScreenPanel
{
    [SerializeField] private TMP_Text gemText;
    [SerializeField] private CustomButton loseButton, yesButton;
    [SerializeField] private GameObject adIcon;
    private float timeScale;
    private int gemRewardCount;

    private void OnEnable()
    {
        timeScale = Time.timeScale;
        Time.timeScale = 0;

        gemRewardCount = UnityEngine.Random.Range(15, 25);
        gemText.text = gemRewardCount + " GEMS";

        loseButton.onClick.AddListener(OnClick_Close);
        yesButton.onClick.AddListener(OnClick_Yes);

        adIcon.gameObject.SetActive(ActiveGameData.Instance.saveData.enabled_NoAds == false);

    }

    private void OnClick_Close()
    {
        Hide();   
    }

    private void OnClick_Yes()
    {
        AdManager.Get.ShowRewardedAd((status) =>
        {
            if (status)
            {
                Debug.Log("Reward Garnted");
                GameScreen gameScreen = ScreenManager.Get.GetScreen<GameScreen>();
                CoinAnimation.Get.ShowGem(gameScreen.gemText.transform.position, gemRewardCount, () =>
                {
                    Constants.Get.Gems += gemRewardCount;
                    EventManager.TriggerEvent(EventID.Update_Currency);
                });
                OnClick_Close();
            }
            else
            {
                Debug.Log("Reward Not Granted");
                ToastManager.Get.ShowMessage("Ads Not Available");
            }
        });
    }

    private void OnDisable()
    {
        loseButton.onClick.RemoveAllListeners();
        yesButton.onClick.RemoveAllListeners();
        Time.timeScale = timeScale;
    }
}
