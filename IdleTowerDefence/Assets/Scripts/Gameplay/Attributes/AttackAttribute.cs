using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAttribute
{
}

public class AttackDamage : IAttribute
{
    internal override AttributeUIUpdateValue InitValue(AttributeData data, bool fromFactory)
    {
        base.InitValue(data, fromFactory);

        Constants.Get.BulletDamage = attributeData.savedData.savedInitialValue;
        
        AttributeUIUpdateValue value = new AttributeUIUpdateValue();
        value.currentValue = Constants.Get.BulletDamage;
        value.newUpgradeValue = Constants.Get.BulletDamage + valuePerUpgradeNext;
        value.newUpgradeValue = Mathf.Clamp(value.newUpgradeValue, 0, attributeData.maxValue);
        value.cost = upgradeCost;
        value.isMaxed = value.currentValue >= attributeData.maxValue;

        EventManager.TriggerEvent(EventID.Attribute_AttackDamage, value.currentValue);
        attributeData.lastValue = value; return value; 
    }

    internal override AttributeUIUpdateValue Upgrade(bool withGoldUpgrade)
    {
        base.Upgrade(withGoldUpgrade);

        Constants.Get.BulletDamage += valuePerUpgrade;
        Constants.Get.BulletDamage = Mathf.Clamp(Constants.Get.BulletDamage, 0, attributeData.maxValue);

        AttributeUIUpdateValue value = new AttributeUIUpdateValue();
        value.currentValue = Constants.Get.BulletDamage;
        value.newUpgradeValue = Constants.Get.BulletDamage + valuePerUpgradeNext;
        value.newUpgradeValue = Mathf.Clamp(value.newUpgradeValue, 0, attributeData.maxValue);
        value.cost = upgradeCost;

        value.isMaxed = value.currentValue >= attributeData.maxValue;

        EventManager.TriggerEvent(EventID.Attribute_AttackDamage, value.currentValue);
        attributeData.lastValue = value; return value; 
    }

}

public class AttackRange : IAttribute
{
    internal override AttributeUIUpdateValue InitValue(AttributeData data, bool fromFactory)
    {
        base.InitValue(data, fromFactory);

        Constants.Get.BoundaryRadius = attributeData.savedData.savedInitialValue;

        AttributeUIUpdateValue value = new AttributeUIUpdateValue();
        value.currentValue = Constants.Get.BoundaryRadius;
        value.newUpgradeValue = Constants.Get.BoundaryRadius + valuePerUpgradeNext;
        value.newUpgradeValue = Mathf.Clamp(value.newUpgradeValue, 0, attributeData.maxValue);
        value.cost = upgradeCost;
        value.isMaxed = value.currentValue >= attributeData.maxValue;
        attributeData.lastValue = value; return value; 
    }

    internal override AttributeUIUpdateValue Upgrade(bool withGoldUpgrade)
    {
        base.Upgrade(withGoldUpgrade);
        AttributeUIUpdateValue value = new AttributeUIUpdateValue();
        
        Constants.Get.BoundaryRadius += valuePerUpgrade;
        Constants.Get.BoundaryRadius = Mathf.Clamp(Constants.Get.BoundaryRadius, 0, attributeData.maxValue);
        value.currentValue = Constants.Get.BoundaryRadius;
        value.newUpgradeValue = Constants.Get.BoundaryRadius + valuePerUpgradeNext;
        value.newUpgradeValue = Mathf.Clamp(value.newUpgradeValue, 0, attributeData.maxValue);
        value.cost = upgradeCost;
        Constants.Get.UpdateBoundary();

        value.currentValue = Mathf.Clamp(value.currentValue, 0, attributeData.maxValue);
        value.currentValue = (float)Math.Round(value.currentValue, 2);
        value.isMaxed = value.currentValue >= attributeData.maxValue;

        attributeData.lastValue = value; return value; 
    }
}

public class AttackSpeed : IAttribute
{
    float xMin = 1f;
    float xMax = 3f;
    float yMin = 1f;
    float yMax = 0.35f;

    internal override AttributeUIUpdateValue InitValue(AttributeData data, bool fromFactory)
    {
        base.InitValue(data, fromFactory);

        AttributeUIUpdateValue value = new AttributeUIUpdateValue();

        Constants.Get.ShootingSpeed = attributeData.savedData.savedInitialValue;

        value.currentValue = Constants.Get.ShootingSpeed;
        value.newUpgradeValue = Constants.Get.ShootingSpeed + valuePerUpgradeNext;
        value.newUpgradeValue = Mathf.Clamp(value.newUpgradeValue, 0, attributeData.maxValue);
        value.cost = upgradeCost;
        value.isMaxed = value.currentValue >= attributeData.maxValue;
        Constants.Get.ShootInterval = (Constants.Get.ShootingSpeed - xMin) * (yMax - yMin) / (xMax - xMin) + yMin;
        EventManager.TriggerEvent(EventID.Attribute_AttackSpeed, value.currentValue);
        attributeData.lastValue = value; return value; 
    }

    internal override AttributeUIUpdateValue Upgrade(bool withGoldUpgrade)
    {
        base.Upgrade(withGoldUpgrade);
        AttributeUIUpdateValue value = new AttributeUIUpdateValue();

        Constants.Get.ShootingSpeed += valuePerUpgrade;
        Constants.Get.ShootingSpeed = Mathf.Clamp(Constants.Get.ShootingSpeed, 0, attributeData.maxValue);
        value.currentValue = Constants.Get.ShootingSpeed;
        value.newUpgradeValue = Constants.Get.ShootingSpeed + valuePerUpgradeNext;
        value.newUpgradeValue = Mathf.Clamp(value.newUpgradeValue, 0, attributeData.maxValue);
        value.cost = upgradeCost;

        value.currentValue = Mathf.Clamp(value.currentValue, 0, attributeData.maxValue);
        value.currentValue = (float)Math.Round(value.currentValue, 2);

        value.isMaxed = value.currentValue >= attributeData.maxValue;
        
        Constants.Get.ShootInterval = (Constants.Get.ShootingSpeed - xMin) * (yMax - yMin) / (xMax - xMin) + yMin;

        EventManager.TriggerEvent(EventID.Attribute_AttackSpeed, value.currentValue);

        attributeData.lastValue = value; return value; 
    }
}

public class CriticalShotChance : IAttribute
{
    internal override AttributeUIUpdateValue InitValue(AttributeData data, bool fromFactory)
    {
        base.InitValue(data, fromFactory);
        Constants.Get.CriticalShotChance = attributeData.savedData.savedInitialValue;

        AttributeUIUpdateValue value = new AttributeUIUpdateValue();
        value.currentValue = Constants.Get.CriticalShotChance;
        value.newUpgradeValue = Constants.Get.CriticalShotChance + valuePerUpgradeNext;
        value.newUpgradeValue = Mathf.Clamp(value.newUpgradeValue, 0, attributeData.maxValue);
        value.cost = upgradeCost;
        value.isMaxed = value.currentValue >= attributeData.maxValue;
        attributeData.lastValue = value; return value; 
    }

    internal override AttributeUIUpdateValue Upgrade(bool withGoldUpgrade)
    {
        base.Upgrade(withGoldUpgrade);
        AttributeUIUpdateValue value = new AttributeUIUpdateValue();

        Constants.Get.CriticalShotChance += valuePerUpgrade;
        Constants.Get.CriticalShotChance = Mathf.Clamp(Constants.Get.CriticalShotChance, 0, attributeData.maxValue);
        value.currentValue = Constants.Get.CriticalShotChance;
        value.newUpgradeValue = Constants.Get.CriticalShotChance + valuePerUpgradeNext;
        value.newUpgradeValue = Mathf.Clamp(value.newUpgradeValue, 0, attributeData.maxValue);
        value.cost = upgradeCost;

        value.currentValue = Mathf.Clamp(value.currentValue, 0, attributeData.maxValue);
        value.currentValue = (float)Math.Round(value.currentValue, 2);
        value.isMaxed = value.currentValue >= attributeData.maxValue;

        attributeData.lastValue = value; return value; 
    }
}

public class CriticalShotDamage : IAttribute
{
    internal override AttributeUIUpdateValue InitValue(AttributeData data, bool fromFactory)
    {
        base.InitValue(data, fromFactory);

        Constants.Get.CriticalShotDamage = attributeData.savedData.savedInitialValue;
        AttributeUIUpdateValue value = new AttributeUIUpdateValue();
        value.currentValue = Constants.Get.CriticalShotDamage;
        value.newUpgradeValue = Constants.Get.CriticalShotDamage + valuePerUpgradeNext;
        value.newUpgradeValue = Mathf.Clamp(value.newUpgradeValue, 0, attributeData.maxValue);
        value.cost = upgradeCost;
        value.isMaxed = value.currentValue >= attributeData.maxValue;
        attributeData.lastValue = value; return value; 
    }

    internal override AttributeUIUpdateValue Upgrade(bool withGoldUpgrade)
    {
        base.Upgrade(withGoldUpgrade);
        AttributeUIUpdateValue value = new AttributeUIUpdateValue();

        Constants.Get.CriticalShotDamage += valuePerUpgrade;
        Constants.Get.CriticalShotDamage = Mathf.Clamp(Constants.Get.CriticalShotDamage, 0, attributeData.maxValue);
        value.currentValue = Constants.Get.CriticalShotDamage;
        value.newUpgradeValue = Constants.Get.CriticalShotDamage + valuePerUpgradeNext;
        value.newUpgradeValue = Mathf.Clamp(value.newUpgradeValue, 0, attributeData.maxValue);
        value.cost = upgradeCost;

        value.currentValue = Mathf.Clamp(value.currentValue, 0, attributeData.maxValue);
        value.currentValue = (float)Math.Round(value.currentValue, 2);
        value.isMaxed = value.currentValue >= attributeData.maxValue;

        attributeData.lastValue = value; return value; 
    }
}

public class RangeDamageBonus : IAttribute
{
    internal override AttributeUIUpdateValue InitValue(AttributeData data, bool fromFactory)
    {
        base.InitValue(data, fromFactory);

        Constants.Get.RangeDamageBonus = attributeData.savedData.savedInitialValue;
        AttributeUIUpdateValue value = new AttributeUIUpdateValue();
        value.currentValue = Constants.Get.RangeDamageBonus;
        value.newUpgradeValue = Constants.Get.RangeDamageBonus + valuePerUpgradeNext;
        value.newUpgradeValue = Mathf.Clamp(value.newUpgradeValue, 0, attributeData.maxValue);
        value.cost = upgradeCost;
        value.isMaxed = value.currentValue >= attributeData.maxValue;
        attributeData.lastValue = value; return value; 
    }

    internal override AttributeUIUpdateValue Upgrade(bool withGoldUpgrade)
    {
        base.Upgrade(withGoldUpgrade);
        AttributeUIUpdateValue value = new AttributeUIUpdateValue();

        Constants.Get.RangeDamageBonus += valuePerUpgrade;
        Constants.Get.RangeDamageBonus = Mathf.Clamp(Constants.Get.RangeDamageBonus, 0, attributeData.maxValue);
        value.currentValue = Constants.Get.RangeDamageBonus;
        value.newUpgradeValue = Constants.Get.RangeDamageBonus + valuePerUpgradeNext;
        value.newUpgradeValue = Mathf.Clamp(value.newUpgradeValue, 0, attributeData.maxValue);
        value.cost = upgradeCost;

        value.currentValue = Mathf.Clamp(value.currentValue, 0, attributeData.maxValue);
        value.currentValue = (float)Math.Round(value.currentValue, 2);
        value.isMaxed = value.currentValue >= attributeData.maxValue;

        attributeData.lastValue = value; return value; 
    }
}

public class DoubleShotChance : IAttribute
{
    internal override AttributeUIUpdateValue InitValue(AttributeData data, bool fromFactory)
    {
        base.InitValue(data, fromFactory);

        Constants.Get.DoubleShotChance = attributeData.savedData.savedInitialValue;
        AttributeUIUpdateValue value = new AttributeUIUpdateValue();
        value.currentValue = Constants.Get.DoubleShotChance;
        value.newUpgradeValue = Constants.Get.DoubleShotChance + valuePerUpgradeNext;
        value.newUpgradeValue = Mathf.Clamp(value.newUpgradeValue, 0, attributeData.maxValue);
        value.cost = upgradeCost;
        value.isMaxed = value.currentValue >= attributeData.maxValue;
        attributeData.lastValue = value; return value; 
    }

    internal override AttributeUIUpdateValue Upgrade(bool withGoldUpgrade)
    {
        base.Upgrade(withGoldUpgrade);
        AttributeUIUpdateValue value = new AttributeUIUpdateValue();

        Constants.Get.DoubleShotChance += valuePerUpgrade;
        Constants.Get.DoubleShotChance = Mathf.Clamp(Constants.Get.DoubleShotChance, 0, attributeData.maxValue);
        value.currentValue = Constants.Get.DoubleShotChance;
        value.newUpgradeValue = Constants.Get.DoubleShotChance + valuePerUpgradeNext;
        value.newUpgradeValue = Mathf.Clamp(value.newUpgradeValue, 0, attributeData.maxValue);
        value.cost = upgradeCost;

        value.currentValue = Mathf.Clamp(value.currentValue, 0, attributeData.maxValue);
        value.currentValue = (float)Math.Round(value.currentValue, 2);
        value.isMaxed = value.currentValue >= attributeData.maxValue;

        attributeData.lastValue = value; return value; 
    }
}

public class DoubleShotPercentage : IAttribute
{
    internal override AttributeUIUpdateValue InitValue(AttributeData data, bool fromFactory)
    {
        base.InitValue(data, fromFactory);

        Constants.Get.DoubleShotDamagePercentage = attributeData.savedData.savedInitialValue;
        AttributeUIUpdateValue value = new AttributeUIUpdateValue();
        value.currentValue = Constants.Get.DoubleShotDamagePercentage;
        value.newUpgradeValue = Constants.Get.DoubleShotDamagePercentage + valuePerUpgradeNext;
        value.newUpgradeValue = Mathf.Clamp(value.newUpgradeValue, 0, attributeData.maxValue);
        value.cost = upgradeCost;
        value.isMaxed = value.currentValue >= attributeData.maxValue;
        attributeData.lastValue = value; return value; 
    }

    internal override AttributeUIUpdateValue Upgrade(bool withGoldUpgrade)
    {
        base.Upgrade(withGoldUpgrade);
        AttributeUIUpdateValue value = new AttributeUIUpdateValue();

        Constants.Get.DoubleShotDamagePercentage += valuePerUpgrade;
        Constants.Get.DoubleShotDamagePercentage = Mathf.Clamp(Constants.Get.DoubleShotDamagePercentage, 0, attributeData.maxValue);
        value.currentValue = Constants.Get.DoubleShotDamagePercentage;
        value.newUpgradeValue = Constants.Get.DoubleShotDamagePercentage + valuePerUpgradeNext;
        value.newUpgradeValue = Mathf.Clamp(value.newUpgradeValue, 0, attributeData.maxValue);
        value.cost = upgradeCost;

        value.currentValue = Mathf.Clamp(value.currentValue, 0, attributeData.maxValue);
        value.currentValue = (float)Math.Round(value.currentValue, 2);
        value.isMaxed = value.currentValue >= attributeData.maxValue;

        attributeData.lastValue = value; return value; 
    }
}

public class StunChance : IAttribute
{
    internal override AttributeUIUpdateValue InitValue(AttributeData data, bool fromFactory)
    {
        base.InitValue(data, fromFactory);

        Constants.Get.StunChance = attributeData.savedData.savedInitialValue;
        AttributeUIUpdateValue value = new AttributeUIUpdateValue();
        value.currentValue = Constants.Get.StunChance;
        value.newUpgradeValue = Constants.Get.StunChance + valuePerUpgradeNext;
        value.newUpgradeValue = Mathf.Clamp(value.newUpgradeValue, 0, attributeData.maxValue);
        value.cost = upgradeCost;
        value.isMaxed = value.currentValue >= attributeData.maxValue;
        attributeData.lastValue = value; return value; 
    }

    internal override AttributeUIUpdateValue Upgrade(bool withGoldUpgrade)
    {
        base.Upgrade(withGoldUpgrade);
        AttributeUIUpdateValue value = new AttributeUIUpdateValue();

        Constants.Get.StunChance += valuePerUpgrade;
        Constants.Get.StunChance = Mathf.Clamp(Constants.Get.StunChance, 0, attributeData.maxValue);
        value.currentValue = Constants.Get.StunChance;
        value.newUpgradeValue = Constants.Get.StunChance + valuePerUpgradeNext;
        value.newUpgradeValue = Mathf.Clamp(value.newUpgradeValue, 0, attributeData.maxValue);
        value.cost = upgradeCost;

        value.currentValue = Mathf.Clamp(value.currentValue, 0, attributeData.maxValue);
        value.currentValue = (float)Math.Round(value.currentValue, 2);
        value.isMaxed = value.currentValue >= attributeData.maxValue;

        attributeData.lastValue = value; return value; 
    }
}

public class StunTime : IAttribute
{
    internal override AttributeUIUpdateValue InitValue(AttributeData data, bool fromFactory)
    {
        base.InitValue(data, fromFactory);

        Constants.Get.StunTime = attributeData.savedData.savedInitialValue;
        AttributeUIUpdateValue value = new AttributeUIUpdateValue();
        value.currentValue = Constants.Get.StunTime;
        value.newUpgradeValue = Constants.Get.StunTime + valuePerUpgradeNext;
        value.newUpgradeValue = Mathf.Clamp(value.newUpgradeValue, 0, attributeData.maxValue);
        value.cost = upgradeCost;
        value.isMaxed = value.currentValue >= attributeData.maxValue;
        attributeData.lastValue = value; return value; 
    }

    internal override AttributeUIUpdateValue Upgrade(bool withGoldUpgrade)
    {
        base.Upgrade(withGoldUpgrade);
        AttributeUIUpdateValue value = new AttributeUIUpdateValue();

        Constants.Get.StunTime += valuePerUpgrade;
        Constants.Get.StunTime = Mathf.Clamp(Constants.Get.StunTime, 0, attributeData.maxValue);
        value.currentValue = Constants.Get.StunTime;
        value.newUpgradeValue = Constants.Get.StunTime + valuePerUpgradeNext;
        value.newUpgradeValue = Mathf.Clamp(value.newUpgradeValue, 0, attributeData.maxValue);
        value.cost = upgradeCost;

        value.currentValue = Mathf.Clamp(value.currentValue, 0, attributeData.maxValue);
        value.currentValue = (float)Math.Round(value.currentValue, 2);

        value.isMaxed = value.currentValue >= attributeData.maxValue;

        attributeData.lastValue = value; return value; 
    }
}

public class DeadHitChance : IAttribute
{
    internal override AttributeUIUpdateValue InitValue(AttributeData data, bool fromFactory)
    {
        base.InitValue(data, fromFactory);

        Constants.Get.DeadHitChance = attributeData.savedData.savedInitialValue;
        AttributeUIUpdateValue value = new AttributeUIUpdateValue();
        value.currentValue = Constants.Get.DeadHitChance;
        value.newUpgradeValue = Constants.Get.DeadHitChance + valuePerUpgradeNext;
        value.newUpgradeValue = Mathf.Clamp(value.newUpgradeValue, 0, attributeData.maxValue);
        value.cost = upgradeCost;
        value.isMaxed = value.currentValue >= attributeData.maxValue;
        attributeData.lastValue = value; return value; 
    }

    internal override AttributeUIUpdateValue Upgrade(bool withGoldUpgrade)
    {
        base.Upgrade(withGoldUpgrade);
        AttributeUIUpdateValue value = new AttributeUIUpdateValue();

        Constants.Get.DeadHitChance += valuePerUpgrade;
        Constants.Get.DeadHitChance = Mathf.Clamp(Constants.Get.DeadHitChance, 0, attributeData.maxValue);
        value.currentValue = Constants.Get.DeadHitChance;
        value.newUpgradeValue = Constants.Get.DeadHitChance + valuePerUpgradeNext;
        value.newUpgradeValue = Mathf.Clamp(value.newUpgradeValue, 0, attributeData.maxValue);
        value.cost = upgradeCost;

        value.currentValue = Mathf.Clamp(value.currentValue, 0, attributeData.maxValue);
        value.currentValue = (float)Math.Round(value.currentValue, 2);
        value.isMaxed = value.currentValue >= attributeData.maxValue;

        attributeData.lastValue = value; return value; 
    }
}