using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "CardData")]
public class CardData : ScriptableObject
{
    [NonReorderable] public float godModeCardRequireDumpValue;
    [NonReorderable] public List<CardInfo> cards;
}

[System.Serializable]
public class CardInfo
{
    public int cardId;
    public string cardName;
    public string cardClassname;
    public string carddesc;
    public string prefabName;
    public Sprite cardImage;
    public int probability;
    public float cardValue;
    public string val1Desc;
    public string val2Desc;
    public string val1PostFix;
    public string val2PostFix;
    public int minimumCardRequirement;
    public LevelInfo level1;
    public LevelInfo level2;
    public LevelInfo level3;
    public SavedCardData savedData;
}

[System.Serializable]
public class SavedCardData
{
    public int id;
    public int level = 1;
    public int cardCount = 0;
    public int unlocked = 0;
}

[System.Serializable]
public class LevelInfo
{
    public int value1;
    public int value2;
}