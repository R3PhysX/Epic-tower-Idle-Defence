//using Lofelt.NiceVibrations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class AttributeUIUpdateValue{
    public float currentValue;
    public float newUpgradeValue;
    public int cost;
    public bool isMaxed;
}

public class AttributeItem : MonoBehaviour
{
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] internal GameObject maxedPanel;

    [SerializeField] internal TMP_Text attributeName;
    [SerializeField] internal TMP_Text oldValue;
    [SerializeField] internal TMP_Text newValue;
    [SerializeField] private TMP_Text cost;
    [SerializeField] private TMP_Text maxedValue;
    [SerializeField] internal CustomButton upgradeButton;

    [SerializeField] private Sprite upgradeButtonEnable;
    [SerializeField] private Sprite upgradeButtonDisable;

    internal AttributeData data;
    internal IAttribute attribute;

    internal bool SetUI(AttributeData data, IAttribute attribute)
    {
        if (data.savedData.IsUnlocked == false)
        {
            Destroy(this.gameObject);
            return false;
        }

        this.data = data;
        this.attribute = attribute;

        attributeName.text = data.attributeDisplayName;
        SetUI(attribute.InitValue(data, false));
        upgradeButton.onClick.AddListener(OnClick);

        EventManager.AddListener(EventID.Add_SilverCoin, OnCoinChange);
        EventManager.AddListener(EventID.Update_SilverCoin, OnCoinChange);

        return true;

    }

    private void OnCoinChange(object arg)
    {
        if (Constants.Get.SilverCoin >= attribute.upgradeCost)
        {
            upgradeButton.image.sprite = upgradeButtonEnable;
        }
        else
        {
            upgradeButton.image.sprite = upgradeButtonDisable;
        }
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
        if (Constants.Get.SilverCoin >= attribute.upgradeCost)
        {
            if (ActiveGameData.Instance.saveData.VisualEffect == 1)
            {
                Player.Instance.upgradeParticle.Stop();
                Player.Instance.upgradeParticle.Play();
            }
            //if (ActiveGameData.Instance.saveData.VibrateEffect == 1)
            //    HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
            SetUI(attribute.Upgrade(false));
        }
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener(EventID.Add_SilverCoin, OnCoinChange);
    }
}
