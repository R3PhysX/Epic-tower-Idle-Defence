using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthAttribute
{
}

public class TowerHealth : IAttribute
{
    internal override AttributeUIUpdateValue InitValue(AttributeData data, bool fromFactory)
    {
        base.InitValue(data, fromFactory);

        Constants.Get.PlayerHealth = attributeData.savedData.savedInitialValue;

        AttributeUIUpdateValue value = new AttributeUIUpdateValue();
        value.currentValue = Constants.Get.PlayerHealth;
        value.newUpgradeValue = Constants.Get.PlayerHealth + valuePerUpgradeNext;
        value.newUpgradeValue = Mathf.Clamp(value.newUpgradeValue, 0, attributeData.maxValue);
        value.cost = upgradeCost;
        value.isMaxed = value.currentValue >= attributeData.maxValue;
        EventManager.TriggerEvent(EventID.Attribute_Health, value.currentValue);
        attributeData.lastValue = value; return value; 
    }

    internal override AttributeUIUpdateValue Upgrade(bool withGoldUpgrade)
    {
        base.Upgrade(withGoldUpgrade);

        AttributeUIUpdateValue value = new AttributeUIUpdateValue();

        Constants.Get.PlayerHealth += valuePerUpgrade;
        Constants.Get.PlayerHealth = Mathf.Clamp(Constants.Get.PlayerHealth, 0, attributeData.maxValue);
        value.currentValue = Constants.Get.PlayerHealth;
        value.newUpgradeValue = Constants.Get.PlayerHealth + valuePerUpgradeNext;
        value.newUpgradeValue = Mathf.Clamp(value.newUpgradeValue, 0, attributeData.maxValue);
        value.cost = upgradeCost;

        value.isMaxed = value.currentValue >= attributeData.maxValue;

        EventManager.TriggerEvent(EventID.Player_HealthAdd, valuePerUpgrade);
        EventManager.TriggerEvent(EventID.Attribute_Health, value.currentValue);
        attributeData.lastValue = value; return value; 
    }
}

public class TowerHealthRegenration : IAttribute
{
    internal override AttributeUIUpdateValue InitValue(AttributeData data, bool fromFactory)
    {
        base.InitValue(data, fromFactory);

        Constants.Get.HealthRegenPerSecond = attributeData.savedData.savedInitialValue;

        AttributeUIUpdateValue value = new AttributeUIUpdateValue();
        value.currentValue = Constants.Get.HealthRegenPerSecond;
        value.newUpgradeValue = Constants.Get.HealthRegenPerSecond + valuePerUpgradeNext;
        value.newUpgradeValue = Mathf.Clamp(value.newUpgradeValue, 0, attributeData.maxValue);
        value.cost = upgradeCost;
        value.isMaxed = value.currentValue >= attributeData.maxValue;
        attributeData.lastValue = value; return value; 
    }

    internal override AttributeUIUpdateValue Upgrade(bool withGoldUpgrade)
    {
        base.Upgrade(withGoldUpgrade);

        AttributeUIUpdateValue value = new AttributeUIUpdateValue();

        Constants.Get.HealthRegenPerSecond += valuePerUpgrade;
        Constants.Get.HealthRegenPerSecond = Mathf.Clamp(Constants.Get.HealthRegenPerSecond, 0, attributeData.maxValue);
        value.currentValue = Constants.Get.HealthRegenPerSecond;
        value.newUpgradeValue = Constants.Get.HealthRegenPerSecond + valuePerUpgradeNext;
        value.newUpgradeValue = Mathf.Clamp(value.newUpgradeValue, 0, attributeData.maxValue);
        value.cost = upgradeCost;


        value.currentValue = Mathf.Clamp(value.currentValue, 0, attributeData.maxValue);
        value.currentValue = (float)Math.Round(value.currentValue, 2);
        value.isMaxed = value.currentValue >= attributeData.maxValue;

        attributeData.lastValue = value; return value; 
    }
}

public class DamageResistance : IAttribute
{
    internal override AttributeUIUpdateValue InitValue(AttributeData data, bool fromFactory)
    {
        base.InitValue(data, fromFactory);

        Constants.Get.DamageResistancePercentage = attributeData.savedData.savedInitialValue;

        AttributeUIUpdateValue value = new AttributeUIUpdateValue();
        value.currentValue = Constants.Get.DamageResistancePercentage;
        value.newUpgradeValue = Constants.Get.DamageResistancePercentage + valuePerUpgradeNext;
        value.newUpgradeValue = Mathf.Clamp(value.newUpgradeValue, 0, attributeData.maxValue);
        value.cost = upgradeCost;
        value.isMaxed = value.currentValue >= attributeData.maxValue;
        attributeData.lastValue = value; return value; 
    }

    internal override AttributeUIUpdateValue Upgrade(bool withGoldUpgrade)
    {
        base.Upgrade(withGoldUpgrade);

        AttributeUIUpdateValue value = new AttributeUIUpdateValue();

        Constants.Get.DamageResistancePercentage += valuePerUpgrade;
        Constants.Get.DamageResistancePercentage = Mathf.Clamp(Constants.Get.DamageResistancePercentage, 0, attributeData.maxValue);
        value.currentValue = Constants.Get.DamageResistancePercentage;
        value.newUpgradeValue = Constants.Get.DamageResistancePercentage + valuePerUpgradeNext;
        value.newUpgradeValue = Mathf.Clamp(value.newUpgradeValue, 0, attributeData.maxValue);
        value.cost = upgradeCost;

        
        value.isMaxed = value.currentValue >= attributeData.maxValue;

        attributeData.lastValue = value; return value; 
    }
}

public class DodgeChance : IAttribute
{
    internal override AttributeUIUpdateValue InitValue(AttributeData data, bool fromFactory)
    {
        base.InitValue(data, fromFactory);

        Constants.Get.DodgeChance = attributeData.savedData.savedInitialValue;

        AttributeUIUpdateValue value = new AttributeUIUpdateValue();
        value.currentValue = Constants.Get.DodgeChance;
        value.newUpgradeValue = Constants.Get.DodgeChance + valuePerUpgradeNext;
        value.newUpgradeValue = Mathf.Clamp(value.newUpgradeValue, 0, attributeData.maxValue);
        value.cost = upgradeCost;
        value.isMaxed = value.currentValue >= attributeData.maxValue;
        attributeData.lastValue = value; return value; 
    }

    internal override AttributeUIUpdateValue Upgrade(bool withGoldUpgrade)
    {
        base.Upgrade(withGoldUpgrade);

        AttributeUIUpdateValue value = new AttributeUIUpdateValue();

        Constants.Get.DodgeChance += valuePerUpgrade;
        Constants.Get.DodgeChance = Mathf.Clamp(Constants.Get.DodgeChance, 0, attributeData.maxValue);
        value.currentValue = Constants.Get.DodgeChance;
        value.newUpgradeValue = Constants.Get.DodgeChance + valuePerUpgradeNext;
        value.newUpgradeValue = Mathf.Clamp(value.newUpgradeValue, 0, attributeData.maxValue);
        value.cost = upgradeCost;

        value.isMaxed = value.currentValue >= attributeData.maxValue;
        attributeData.lastValue = value; return value; 
    }
}