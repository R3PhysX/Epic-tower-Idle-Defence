using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinAttribute
{
}

public class CoinPerWaveBonus : IAttribute
{
    internal override AttributeUIUpdateValue InitValue(AttributeData data, bool fromFactory)
    {
        base.InitValue(data, fromFactory);

        EventManager.AddListener(EventID.Event_WaveUpdate, OnWaveUpdate);
        Constants.Get.SilverCoinWaveBonus = (int)data.savedData.savedInitialValue;

        AttributeUIUpdateValue value = new AttributeUIUpdateValue();
        value.currentValue = Constants.Get.SilverCoinWaveBonus;
        value.newUpgradeValue = Constants.Get.SilverCoinWaveBonus + valuePerUpgradeNext;
        value.newUpgradeValue = Mathf.Clamp(value.newUpgradeValue, 0, attributeData.maxValue);
        value.cost = upgradeCost;
        value.isMaxed = value.currentValue >= attributeData.maxValue;
        attributeData.lastValue = value; return value; 
    }

    internal override AttributeUIUpdateValue Upgrade(bool withGoldUpgrade)
    {
        base.Upgrade(withGoldUpgrade);

        AttributeUIUpdateValue value = new AttributeUIUpdateValue();

        Constants.Get.SilverCoinWaveBonus += (int)valuePerUpgrade;
        Constants.Get.SilverCoinWaveBonus = Mathf.Clamp(Constants.Get.SilverCoinWaveBonus, 0, (int)attributeData.maxValue);
        value.currentValue = Constants.Get.SilverCoinWaveBonus;
        value.newUpgradeValue = Constants.Get.SilverCoinWaveBonus + valuePerUpgradeNext;
        value.newUpgradeValue = Mathf.Clamp(value.newUpgradeValue, 0, attributeData.maxValue);
        value.cost = upgradeCost;

        value.currentValue = Mathf.Clamp(value.currentValue, 0, attributeData.maxValue);
        value.isMaxed = value.currentValue >= attributeData.maxValue;

        attributeData.lastValue = value; return value; 
    }

    private void OnWaveUpdate(object arg)
    {
        int coinToAdd = Constants.Get.SilverCoinWaveBonus;
        EventManager.TriggerEvent(EventID.Add_SilverCoin, coinToAdd);
    }

}

public class CoinPer50Kills : IAttribute
{
    private int enemyDieCount;

    internal override AttributeUIUpdateValue InitValue(AttributeData data, bool fromFactory)
    {
        base.InitValue(data, fromFactory);

        EventManager.AddListener(EventID.Enemy_Die, OnEnemyDie);
        Constants.Get.SilverCoinEach50Kills = (int)data.savedData.savedInitialValue;

        AttributeUIUpdateValue value = new AttributeUIUpdateValue();
        value.currentValue = Constants.Get.SilverCoinEach50Kills;
        value.newUpgradeValue = Constants.Get.SilverCoinEach50Kills + valuePerUpgradeNext;
        value.newUpgradeValue = Mathf.Clamp(value.newUpgradeValue, 0, attributeData.maxValue);
        value.cost = upgradeCost;
        value.isMaxed = value.currentValue >= attributeData.maxValue;
        attributeData.lastValue = value; return value; 
    }

    internal override AttributeUIUpdateValue Upgrade(bool withGoldUpgrade)
    {
        base.Upgrade(withGoldUpgrade);

        AttributeUIUpdateValue value = new AttributeUIUpdateValue();

        Constants.Get.SilverCoinEach50Kills += (int)valuePerUpgrade;
        Constants.Get.SilverCoinEach50Kills = Mathf.Clamp(Constants.Get.SilverCoinEach50Kills, 0, (int)attributeData.maxValue);
        value.currentValue = Constants.Get.SilverCoinEach50Kills;
        value.newUpgradeValue = Constants.Get.SilverCoinEach50Kills + valuePerUpgradeNext;
        value.newUpgradeValue = Mathf.Clamp(value.newUpgradeValue, 0, attributeData.maxValue);
        value.cost = upgradeCost;

        value.currentValue = Mathf.Clamp(value.currentValue, 0, attributeData.maxValue);
        value.isMaxed = value.currentValue >= attributeData.maxValue;

        attributeData.lastValue = value; return value; 
    }

    private void OnEnemyDie(object arg)
    {
        enemyDieCount += 1;

        if (enemyDieCount >= 50)
        {
            enemyDieCount = 0;
            int coinToAdd = Constants.Get.SilverCoinEach50Kills;
            EventManager.TriggerEvent(EventID.Add_SilverCoin, coinToAdd);
        }
    }
}

public class GoldCoinPerWaveBonus : IAttribute
{
    internal override AttributeUIUpdateValue InitValue(AttributeData data, bool fromFactory)
    {
        base.InitValue(data, fromFactory);

        EventManager.AddListener(EventID.Event_WaveUpdate, OnWaveUpdate);
        Constants.Get.GoldCoinWaveBonus = (int)data.savedData.savedInitialValue;

        AttributeUIUpdateValue value = new AttributeUIUpdateValue();
        value.currentValue = Constants.Get.GoldCoinWaveBonus;
        value.newUpgradeValue = Constants.Get.GoldCoinWaveBonus + valuePerUpgradeNext;
        value.newUpgradeValue = Mathf.Clamp(value.newUpgradeValue, 0, attributeData.maxValue);
        value.cost = upgradeCost;
        value.isMaxed = value.currentValue >= attributeData.maxValue;
        attributeData.lastValue = value; return value; 
    }

    internal override AttributeUIUpdateValue Upgrade(bool withGoldUpgrade)
    {
        base.Upgrade(withGoldUpgrade);

        AttributeUIUpdateValue value = new AttributeUIUpdateValue();

        Constants.Get.GoldCoinWaveBonus += (int)valuePerUpgrade;
        Constants.Get.GoldCoinWaveBonus = Mathf.Clamp(Constants.Get.GoldCoinWaveBonus, 0, (int)attributeData.maxValue);
        value.currentValue = Constants.Get.GoldCoinWaveBonus;
        value.newUpgradeValue = Constants.Get.GoldCoinWaveBonus + valuePerUpgradeNext;
        value.newUpgradeValue = Mathf.Clamp(value.newUpgradeValue, 0, attributeData.maxValue);
        value.cost = upgradeCost;

        value.currentValue = Mathf.Clamp(value.currentValue, 0, attributeData.maxValue);
        value.isMaxed = value.currentValue >= attributeData.maxValue;

        attributeData.lastValue = value; return value; 
    }

    private void OnWaveUpdate(object arg)
    {
        int coinToAdd = Constants.Get.GoldCoinWaveBonus;
        EventManager.TriggerEvent(EventID.Add_GoldCoin, coinToAdd);
    }
}

public class GoldAfterBossKill : IAttribute
{
    internal override AttributeUIUpdateValue InitValue(AttributeData data, bool fromFactory)
    {
        base.InitValue(data, fromFactory);

        EventManager.AddListener(EventID.Boss_Die, OnBossDie);
        Constants.Get.GoldCoinAfterBossDie = (int)data.savedData.savedInitialValue;

        AttributeUIUpdateValue value = new AttributeUIUpdateValue();
        value.currentValue = Constants.Get.GoldCoinAfterBossDie;
        value.newUpgradeValue = Constants.Get.GoldCoinAfterBossDie + valuePerUpgradeNext;
        value.newUpgradeValue = Mathf.Clamp(value.newUpgradeValue, 0, attributeData.maxValue);
        value.cost = upgradeCost;
        value.isMaxed = value.currentValue >= attributeData.maxValue;
        attributeData.lastValue = value; return value; 
    }

    internal override AttributeUIUpdateValue Upgrade(bool withGoldUpgrade)
    {
        base.Upgrade(withGoldUpgrade);

        AttributeUIUpdateValue value = new AttributeUIUpdateValue();

        Constants.Get.GoldCoinAfterBossDie += (int)valuePerUpgrade;
        Constants.Get.GoldCoinAfterBossDie = Mathf.Clamp(Constants.Get.GoldCoinAfterBossDie, 0, (int)attributeData.maxValue);
        value.currentValue = Constants.Get.GoldCoinAfterBossDie;
        value.newUpgradeValue = Constants.Get.GoldCoinAfterBossDie + valuePerUpgradeNext;
        value.newUpgradeValue = Mathf.Clamp(value.newUpgradeValue, 0, attributeData.maxValue);
        value.cost = upgradeCost;

        value.currentValue = Mathf.Clamp(value.currentValue, 0, attributeData.maxValue);
        value.isMaxed = value.currentValue >= attributeData.maxValue;

        attributeData.lastValue = value; return value; 
    }

    private void OnBossDie(object arg)
    {
        int coinToAdd = Constants.Get.GoldCoinAfterBossDie;
        EventManager.TriggerEvent(EventID.Add_GoldCoin, coinToAdd);
    }

}