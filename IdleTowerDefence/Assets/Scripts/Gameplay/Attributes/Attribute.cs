using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IAttribute
{
    internal AttributeData attributeData;
    internal int upgradeCost;
    internal int lastupgradeCost = 0;
    internal AttributeUIUpdateValue lastValue;
    internal float valuePerUpgrade;
    internal float valuePerUpgradeNext;

    private float GoldMultiplier = 2.5f;

    private bool init;

    internal virtual AttributeUIUpdateValue InitValue(AttributeData data, bool fromFactory)
    {
        if (fromFactory == false)
            GoldMultiplier = 1;

        attributeData = data;
        upgradeCost = (int)(data.initialCost * GoldMultiplier);
        lastupgradeCost = (int)(data.startingIncrementCost);
        valuePerUpgrade = data.valuePerUpgrade;
        valuePerUpgradeNext = valuePerUpgradeNext + data.valuePerUpgrade;

        if (data.savedData.savedInitialValue <= 0)
        {
            data.savedData.savedInitialValue = data.initialValue;
        }

        if (data.savedData.savedInitialValue == data.initialValue)
        {
            init = true;
        }

        if (fromFactory && data.savedData.numberOfUpgradeDone > 0)
        {
            UpgradeFromSave(data.savedData.numberOfUpgradeDone);
        }
        UpgradeValue(data.savedData.numberOfUpgradeDone);

        return default;
    }

    internal virtual AttributeUIUpdateValue Upgrade(bool withGoldUpgrade)
    {
        if (withGoldUpgrade)
            Constants.Get.GoldCoin -= upgradeCost;
        else
            Constants.Get.SilverCoin -= upgradeCost;

        if (init)
        {
            valuePerUpgrade = valuePerUpgrade - attributeData.increaseValuePerUpgrade;
            init = false;
        }

        lastupgradeCost = lastupgradeCost + attributeData.increasedCostBy;

        valuePerUpgrade = valuePerUpgrade + attributeData.increaseValuePerUpgrade;
        valuePerUpgradeNext = valuePerUpgrade + attributeData.increaseValuePerUpgrade;

        upgradeCost = (int)(upgradeCost + (lastupgradeCost * GoldMultiplier));

        EventManager.TriggerEvent(EventID.Update_SilverCoin, Constants.Get.SilverCoin);
        EventManager.TriggerEvent(EventID.Update_GoldCoin, Constants.Get.GoldCoin);

        return default;
    }

    internal virtual AttributeUIUpdateValue UpgradeFromSave(int times)
    {
        for (int i = 0; i < times; i++)
        {
            lastupgradeCost = lastupgradeCost + attributeData.increasedCostBy;

            upgradeCost = (int)(upgradeCost + (lastupgradeCost * GoldMultiplier));
        }
        return lastValue;
    }

    internal virtual AttributeUIUpdateValue UpgradeValue(int times)
    {
        for (int i = 0; i < times; i++)
        {
            if(i==0)
                valuePerUpgrade = valuePerUpgrade - attributeData.increaseValuePerUpgrade;
            valuePerUpgrade = valuePerUpgrade + attributeData.increaseValuePerUpgrade;

            valuePerUpgradeNext = valuePerUpgrade + attributeData.increaseValuePerUpgrade;
        }

        return lastValue;
    }
}