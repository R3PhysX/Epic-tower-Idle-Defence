using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DebugScreen : MonoBehaviour
{
    [SerializeField] private CustomButton debugButton;
    [SerializeField] private GameObject debugPanel;

    [SerializeField] private CustomButton updateCoinButton;
    [SerializeField] private CustomButton updateGoldCoinButton;
    [SerializeField] private CustomButton updateGemButton;
    [SerializeField] private CustomButton updateTicketButton;
    [SerializeField] private CustomButton updateDiamondButton;
    [SerializeField] private CustomButton updateSpawnRate;

    [SerializeField] private Slider spawnRateSlider;
    [SerializeField] private TMP_InputField updateCoinField;
    [SerializeField] private TMP_InputField updateGemField;
    [SerializeField] private TMP_InputField updateTicketField;
    [SerializeField] private TMP_InputField updateDiamondField;
    [SerializeField] private TMP_InputField updateSpawnRateField;

    [SerializeField] private TMP_InputField w1_wave;
    [SerializeField] private TMP_InputField w2_wave;

    private void OnEnable()
    {
        debugButton.onClick.AddListener(OnClick_Debug);
        
        if (SceneLoadManager.Instance.previousSceneIndex == 1)
        {
            updateGoldCoinButton.onClick.RemoveAllListeners();
            updateGoldCoinButton.onClick.AddListener(OnClick_UpdateGoldCoin);

            updateGemButton.onClick.RemoveAllListeners();
            updateGemButton.onClick.AddListener(OnClick_UpdateGem);

            updateDiamondButton.onClick.RemoveAllListeners();
            updateDiamondButton.onClick.AddListener(OnClick_UpdateDiamond);

            updateTicketButton.onClick.RemoveAllListeners();
            updateTicketButton.onClick.AddListener(OnClick_UpdateTicket);
            return;
        }
        else
        {
            updateCoinButton.onClick.AddListener(OnClick_UpdateCoin);

            updateSpawnRate.onClick.AddListener(OnClick_SpawnRate);
            spawnRateSlider.onValueChanged.AddListener(OnSlider);
        }
    }

    private void OnClick_UpdateGoldCoin()
    {
        int coin = int.Parse(updateCoinField.text);
        ActiveGameData.Instance.saveData.GoldCoin += coin;
        EventManager.TriggerEvent(EventID.Update_Currency);
        debugPanel.gameObject.SetActive(false);
    }

    private void OnClick_UpdateGem()
    {
        int coin = int.Parse(updateGemField.text);
        ActiveGameData.Instance.saveData.Gems += coin;
        EventManager.TriggerEvent(EventID.Update_Currency);
        debugPanel.gameObject.SetActive(false);
    }

    private void OnClick_UpdateDiamond()
    {
        int coin = int.Parse(updateDiamondField.text);
        ActiveGameData.Instance.saveData.Diamond += coin;
        EventManager.TriggerEvent(EventID.Update_Currency);
        debugPanel.gameObject.SetActive(false);
    }

    private void OnClick_UpdateTicket()
    {
        int ticket = int.Parse(updateTicketField.text);
        ActiveGameData.Instance.saveData.diceRollTicket += ticket;
        EventManager.TriggerEvent(EventID.Update_Currency);
        debugPanel.gameObject.SetActive(false);
    }

    private void OnSlider(float arg0)
    {
        updateSpawnRateField.text = spawnRateSlider.value + "";
    }

    private void OnClick_UpdateCoin()
    {
        int coin = int.Parse(updateCoinField.text);
        EventManager.TriggerEvent(EventID.Add_SilverCoin, coin);
        debugPanel.gameObject.SetActive(false);
    }

    private void OnClick_SpawnRate()
    {
        Constants.Get.SpawningRate = spawnRateSlider.value;
        debugPanel.gameObject.SetActive(false);
    }

    private void OnClick_Debug()
    {
        debugPanel.gameObject.SetActive(!debugPanel.gameObject.activeSelf);
        if (debugPanel.gameObject.activeSelf)
        {
            updateSpawnRateField.text = Constants.Get.SpawningRate + "";
            spawnRateSlider.value = Constants.Get.SpawningRate;
            updateCoinField.text = 0 + "";
        }
    }

    private void OnDisable()
    {
        debugButton.onClick.RemoveAllListeners();

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            updateGoldCoinButton.onClick.RemoveAllListeners();
            updateGemButton.onClick.RemoveAllListeners();
            updateDiamondButton.onClick.RemoveAllListeners();
            updateTicketButton.onClick.RemoveAllListeners();
            return;
        }
        else
        {
            updateCoinButton.onClick.RemoveAllListeners();
            updateSpawnRate.onClick.RemoveAllListeners();
        }
        
    }

    public void Change_World1Wave()
    {
        ActiveGameData.Instance.saveData.bestWave[0] = int.Parse(w1_wave.text);
    }

    public void Change_World2Wave()
    {
        ActiveGameData.Instance.saveData.bestWave[1] = int.Parse(w2_wave.text);
    }
}
