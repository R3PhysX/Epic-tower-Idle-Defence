using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveItem : MonoBehaviour
{
    [SerializeField] public Image icon;
    [SerializeField] public Sprite fillImage;
    [SerializeField] public Sprite blankImage;
    [SerializeField] public Sprite bossSprite;

    [SerializeField] public TMP_Text number;
    [SerializeField] public GameObject gold;
    [SerializeField] public CustomButton button;

    private int wave;
    private bool isGold;
    private bool collectPrevious;

    private void OnEnable()
    {
        button.onClick.AddListener(OnClick_WaveItem);
    }

    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }

    internal void Set(int num, bool isGold = false, bool collectPrevious = false)
    {
        wave = num;
        this.isGold = isGold;
        this.collectPrevious = collectPrevious;
        number.text = num.ToString();
        gold.gameObject.SetActive(isGold);
        icon.sprite = num < ActiveGameData.Instance.saveData.bestWave[ActiveGameData.Instance.currentSelectedWorld] ? fillImage : blankImage;

        if (num >= 10 && num % 5 == 0)
            icon.sprite = bossSprite;
    }

    private void OnClick_WaveItem()
    {
        if (isGold && wave < ActiveGameData.Instance.saveData.bestWave[ActiveGameData.Instance.currentSelectedWorld])
        {
            CollectWaveReward(wave);
        }
        else if (isGold)
        {
            ToastManager.Get.ShowMessage("Pass Wave " + wave + " to Get Reward");
        }
    }

    private void CollectWaveReward(int wave)
    {
        int gain = 0;

        // Calculate reward based on the current world
        if (ActiveGameData.Instance.currentSelectedWorld == 2)
            gain = (wave * 100) + ((wave) * UnityEngine.Random.Range(1, 150));
        else
            gain = (wave * 50) + ((wave - 3) * UnityEngine.Random.Range(1, 50));

        if (collectPrevious)
            gain += CollectAllPreviousWaves();

        gain = Math.Min(gain, 3500);

        // Add the gold coin reward
        ActiveGameData.Instance.saveData.GoldCoin += gain;
        HomeScreen homeScreen = ScreenManager.Get.GetScreen<HomeScreen>();

        // Show coin animation
        CoinAnimation.Get.ShowCoin(homeScreen.goldCoinText.transform.position, gain, () =>
        {
            Constants.Get.GoldCoin += gain;
            EventManager.TriggerEvent(EventID.Update_Currency);
        });

        EventManager.TriggerEvent(EventID.Update_Currency);

        // Mark reward as collected
        if (ActiveGameData.Instance.currentSelectedWorld == 0)
            ActiveGameData.Instance.saveData.collectedWaveReward_world1.Add(wave);
        else if (ActiveGameData.Instance.currentSelectedWorld == 1)
            ActiveGameData.Instance.saveData.collectedWaveReward_world2.Add(wave);
        else
            ActiveGameData.Instance.saveData.collectedWaveReward_world3.Add(wave);

        Set(wave, false, false);
    }

    private int CollectAllPreviousWaves()
    {
        int totalGain = 0;
        List<int> collectedRewards;

        Debug.Log("Collecting previous");
        // Get the list of already collected waves based on the current world
        if (ActiveGameData.Instance.currentSelectedWorld == 0)
            collectedRewards = ActiveGameData.Instance.saveData.collectedWaveReward_world1;
        else if (ActiveGameData.Instance.currentSelectedWorld == 1)
            collectedRewards = ActiveGameData.Instance.saveData.collectedWaveReward_world2;
        else
            collectedRewards = ActiveGameData.Instance.saveData.collectedWaveReward_world3;

        // Loop through all waves up to the current wave
        for (int i = 1; i < wave; i++)
        {
            if (ActiveGameData.Instance.currentSelectedWorld != 2 && i % 3 != 0)
                continue;

            if (collectedRewards.Contains(i))
                continue; // Skip already collected waves

            Debug.Log("Collecting previous " + i);

            // Calculate reward for the current wave
            int gain = 0;
            if (ActiveGameData.Instance.currentSelectedWorld == 2)
                gain = (i * 100) + (i * UnityEngine.Random.Range(1, 150));
            else
                gain = (i * 50) + ((i - 3) * UnityEngine.Random.Range(1, 50));

            totalGain += gain; // Accumulate the reward
            collectedRewards.Add(i); // Mark the wave as collected
        }

        return totalGain;
    }

}