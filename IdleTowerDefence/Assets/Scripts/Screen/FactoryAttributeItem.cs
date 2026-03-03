//using Lofelt.NiceVibrations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FactoryAttributeItem : MonoBehaviour
{
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private GameObject maxedPanel;

    [SerializeField] private TMP_Text attributeName;
    [SerializeField] private TMP_Text oldValue;
    [SerializeField] private TMP_Text newValue;
    [SerializeField] private TMP_Text cost;
    [SerializeField] private TMP_Text maxedValue;
    [SerializeField] internal CustomButton upgradeButton;

    [SerializeField] private Sprite upgradeButtonEnable;
    [SerializeField] private Sprite upgradeButtonDisable;

    private AttributeData data;
    private IAttribute attribute;

    internal void SetUI(AttributeData data, IAttribute attribute)
    {
        this.data = data;
        this.attribute = attribute;

        attributeName.text = data.attributeDisplayName;
        SetUI(attribute.InitValue(data, true));
        upgradeButton.onClick.AddListener(OnClick);

        EventManager.AddListener(EventID.Update_GoldCoin, OnCoinChange);
        EventManager.AddListener(EventID.Update_Currency, OnCoinChange);
        
    }

    private void OnCoinChange(object arg)
    {
        if (Constants.Get.GoldCoin >= attribute.upgradeCost)
        {
            upgradeButton.image.sprite = upgradeButtonEnable;
        }
        else
        {
            upgradeButton.image.sprite = upgradeButtonDisable;
        }
    }

    internal void RefreshLastUI()
    {
        OnCoinChange(null);
    }

    internal void SetUI(AttributeUIUpdateValue value)
    {
        if (data.isInPercentage)
        {
            oldValue.text = value.currentValue.ToString("0") + "%";
            newValue.text = "<sprite=0> " + value.newUpgradeValue.ToString("0") + "%";
            maxedValue.text = value.currentValue.ToString("0") + "%";
        }
        else
        {
            oldValue.text = value.currentValue.ToString("0.00");
            newValue.text = "<sprite=0> " + value.newUpgradeValue.ToString("0.00");
            maxedValue.text = value.currentValue.ToString("0.00");
        }
        cost.text = value.cost.ToString("0");

        if (value.isMaxed)
        {
            upgradePanel.gameObject.SetActive(false);
            maxedPanel.gameObject.SetActive(true);
        }

        OnCoinChange(null);
    }

    private void OnClick()
    {
        if (Constants.Get.GoldCoin >= attribute.upgradeCost)
        {
            //if (ActiveGameData.Instance.saveData.VibrateEffect == 1)
            //    HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
            var upgradeData = attribute.Upgrade(true);
            SetUI(upgradeData);
            data.savedData.numberOfUpgradeDone += 1;
            data.savedData.savedInitialValue = upgradeData.currentValue;
            Debug.Log("Upgrade_Value" + attributeName.text);
            GameAnalyticsManager.Instance.NewDesignEventGA("click_upgrade_stat :" + attributeName.text);
        }
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener(EventID.Update_GoldCoin, OnCoinChange);
    }
}
