using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttributeDatabase", menuName = "Attributes/Attribute Database")]
public class AttributeDatabase : ScriptableObject
{
    public AttackAttributeData attackAttribute;
    public HealthAttributeData healthAttribute;
    public CoinAttributeData coinAttribute;
}

[System.Serializable]
public class AttributeData
{
    public bool Unlocked;
    public string attributeClassName;
    public string attributeDisplayName;
    public string attributeDesc;
    public float initialValue;
    public float maxValue;
    public float valuePerUpgrade;
    public float increaseValuePerUpgrade;
    public int initialCost;
    public int increasedCostBy;
    public int startingIncrementCost;
    public bool isInPercentage;

    public string attachedAttributeClassName;
    public int unlockPrice;

    public AttributeSavedData savedData;
    public AttributeUIUpdateValue lastValue;
}

[System.Serializable]
public class AttributeSavedData
{
    public bool IsUnlocked;
    public float savedInitialValue;
    public int numberOfUpgradeDone;
}