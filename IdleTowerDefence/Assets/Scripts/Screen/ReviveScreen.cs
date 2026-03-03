using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReviveScreen : ScreenPanel
{
    public TMP_Text counterText, titleText;
    public CustomButton continueButton, closeButton;
    public GameObject adIcon;

    private float timeScale;
    private void OnEnable()
    {
        timeScale = Time.timeScale;
        LeanTween.delayedCall(0.3f, () => { Time.timeScale = 0; }).setIgnoreTimeScale(true);

        if (ActiveGameData.Instance.saveData.enabled_NoAds)
        {
            adIcon.gameObject.SetActive(false);
            titleText.text = "Click Continue To Revive";
        }

        continueButton.onClick.AddListener(OnClick_Continue);
        closeButton.onClick.AddListener(OnClick_Close);
        counterText.text = "10";

        LeanTween.value(this.gameObject, (val) =>
        {
            counterText.text = val.ToString("0");
        }, 10, 0, 10).setOnComplete(() =>
        {
            Hide();
            ScreenManager.Get.GetScreen<GameOverScreen>().Show();
        }).setIgnoreTimeScale(true);

    }

    private void OnClick_Continue()
    {
        LeanTween.cancel(this.gameObject);
        float timeScale = Time.timeScale;
        AdManager.Get.ShowRewardedAd((status) =>
        {
            Time.timeScale = timeScale;
            if (status)
            {
                Debug.Log("Reward Garnted");
                ScreenManager.Get.GetScreen<GameScreen>().ResetTo1x();
                Player.Instance.currentHealth = Constants.Get.PlayerHealth;

                Constants.Get.SpawningRate += ((Constants.Get.SpawningRate * 9) / 100f);
                Constants.Get.SpawningRate += ((Constants.Get.SpawningRate * 9) / 100f);
                Constants.Get.SpawningRate += ((Constants.Get.SpawningRate * 9) / 100f);
                Constants.Get.SpawningRate = Mathf.Clamp(Constants.Get.SpawningRate, 0.5f, 2f);

                EventManager.TriggerEvent(EventID.Player_HealthUpdate, null);
                Hide();
                Player.Instance.ShockWaveEffect();
                EnemyGenerator.Get.RemoveAllEnemy();
                EnemyGenerator.Get.LevelDownBosses();
                LeanTween.delayedCall(0.3f, () => { 
                    Player.Instance.isDead = false;
                    GameplayManager.Get.isGameplayStarted = true;
                }).setIgnoreTimeScale(true);
                Debug.Log("Revive Done");
                GameAnalyticsManager.Instance.NewDesignEventGA("Revive_ad_watched");
            }
            else
            {
                Debug.Log("Reward Not Granted");
                ToastManager.Get.ShowMessage("Ads Not Available");
                Hide();
                ScreenManager.Get.GetScreen<GameOverScreen>().Show();
            }
        });
    }

    private void OnClick_Close()
    {
        Hide();
        LeanTween.delayedCall(0.3f, () => { ScreenManager.Get.GetScreen<GameOverScreen>().Show(); }).setIgnoreTimeScale(true);
    }

    private void OnDisable()
    {
        LeanTween.cancel(this.gameObject);
        continueButton.onClick.RemoveAllListeners();
        closeButton.onClick.RemoveAllListeners();
        Time.timeScale = timeScale;
    }
}