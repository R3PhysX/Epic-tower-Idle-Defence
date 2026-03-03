using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public bool tutorialShown;

    public int currentSelectedWorld = 0;
    public int GoldCoin = 0;
    public int Diamond = 0;
    public int Gems = 0;

    public string lastRewardClaimedDate;
    public int currentRewardDay = 1;
    public int isRewardClaimed = 0;

    public int[] bestWave = new int[3] { 1, 1, 1 }; //fewfwe

    public string lastIdleRewardDate;

    public List<AttributeSavedData> attackSavedData = new List<AttributeSavedData>();
    public List<AttributeSavedData> coinSavedData = new List<AttributeSavedData>();
    public List<AttributeSavedData> healthSavedData = new List<AttributeSavedData>();
    public List<SavedCardData> savedCardInfo = new List<SavedCardData>();

    public float dumpedCardValue = 0;

    public string username;
    public int avatarId = 1;

    public string gameStartedDate;

    public int[] totalPlayedTimeDay = new int[3] { 0, 0, 0 }; //fwe
    public float[] totalPlayedTime = new float[3] { 0, 0, 0 }; //fewfwe

    public int[] battleTimes = new int[3] { 0, 0, 0 }; //efwef
    public int[] bestBattleGold = new int[3] { 0, 0, 0 }; //efwefe

    public int[] coinEarned = new int[3] { 0, 0, 0 }; //fewfwef
    public int[] goldEarned = new int[3] { 0, 0, 0 };//efwef
    public int[] gemEarned = new int[3] { 0, 0, 0 };//wefwef

    public int[] enemyDestroyed = new int[3] { 0, 0, 0 }; //wefwef
    public int[] bossDestroyed = new int[3] { 0, 0, 0 };//ewfwef

    public int upgradeTimes;
    public int dailyGiftClaimed;

    public int cardCollected;

    public List<int> cardSlotUnlocked;

    public List<int> cardSlotIds;
    public List<int> collectedWaveReward_world1 = new List<int>();
    public List<int> collectedWaveReward_world2 = new List<int>();
    public List<int> collectedWaveReward_world3 = new List<int>();

    public int SoundEffect = 1;
    public int MusicEffect = 1;
    public int VibrateEffect = 1;
    public int VisualEffect = 1;

    public int introTutorial = 0;
    public int MainMenuTutorial = 0;
    public int CardTutorial = 0;

    public bool enabled_NoAds;
    public bool enabled_3;
    public bool enabled_3_5;
    public bool enabled_5;

    public int enabled_3_adCount = 5;

    public int reviewOn10thWave = 0;
    public int reviewOnWorld2 = 0;
    public int reviewGiven = 0;

    public int diceRollTicket = 3;
    public int playerIndexDiceRoll = 0;

}

[System.Serializable]
public class OldSaveData_V2
{
    public bool tutorialShown;

    public int currentSelectedWorld = 0;
    public int GoldCoin = 0;
    public int Diamond = 0;
    public int Gems = 0;

    public string lastRewardClaimedDate;
    public int currentRewardDay = 1;
    public int isRewardClaimed = 0;

    public int[] bestWave = new int[2] { 1, 1 };

    public string lastIdleRewardDate;

    public List<AttributeSavedData> attackSavedData = new List<AttributeSavedData>();
    public List<AttributeSavedData> coinSavedData = new List<AttributeSavedData>();
    public List<AttributeSavedData> healthSavedData = new List<AttributeSavedData>();
    public List<SavedCardData> savedCardInfo = new List<SavedCardData>();

    public float dumpedCardValue = 0;

    public string username;
    public int avatarId = 1;

    public string gameStartedDate;

    public int[] totalPlayedTimeDay = new int[2] { 0, 0 };
    public float[] totalPlayedTime = new float[2] { 0, 0 };

    public int[] battleTimes = new int[2] { 0, 0 };
    public int[] bestBattleGold = new int[2] { 0, 0 };

    public int[] coinEarned = new int[2] { 0, 0 };
    public int[] goldEarned = new int[2] { 0, 0 };
    public int[] gemEarned = new int[2] { 0, 0 };

    public int[] enemyDestroyed = new int[2] { 0, 0 };
    public int[] bossDestroyed = new int[2] { 0, 0 };

    public int upgradeTimes;
    public int dailyGiftClaimed;

    public int cardCollected;

    public List<int> cardSlotUnlocked;

    public List<int> cardSlotIds;
    public List<int> collectedWaveReward_world1 = new List<int>();
    public List<int> collectedWaveReward_world2 = new List<int>();

    public int SoundEffect = 1;
    public int MusicEffect = 1;
    public int VibrateEffect = 1;
    public int VisualEffect = 1;

    public int introTutorial = 0;
    public int MainMenuTutorial = 0;
    public int CardTutorial = 0;

    public bool enabled_NoAds;
    public bool enabled_3;
    public bool enabled_3_5;
    public bool enabled_5;

    public int enabled_3_adCount = 5;

    public int reviewOn10thWave = 0;
    public int reviewOnWorld2 = 0;
    public int reviewGiven = 0;

}

public class OldSaveData_V1
{
    public bool tutorialShown;

    public int GoldCoin = 0;
    public int Diamond = 0;
    public int Gems = 0;

    public string lastRewardClaimedDate;
    public int currentRewardDay = 1;
    public int isRewardClaimed = 0;

    public int bestWave = 1;

    public string lastIdleRewardDate;

    public List<AttributeSavedData> attackSavedData = new List<AttributeSavedData>();
    public List<AttributeSavedData> coinSavedData = new List<AttributeSavedData>();
    public List<AttributeSavedData> healthSavedData = new List<AttributeSavedData>();
    public List<SavedCardData> savedCardInfo = new List<SavedCardData>();

    public string username;
    public int avatarId = 1;

    public string gameStartedDate;

    public int totalPlayedTimeDay;
    public float totalPlayedTime;

    public int battleTimes;
    public int bestBattleGold;

    public int coinEarned;
    public int goldEarned;
    public int gemEarned;

    public int enemyDestroyed;
    public int bossDestroyed;

    public int upgradeTimes;
    public int dailyGiftClaimed;

    public int cardCollected;

    public List<int> cardSlotUnlocked;

    public List<int> cardSlotIds;
    public List<int> collectedWaveReward = new List<int>();

    public int SoundEffect = 1;
    public int MusicEffect = 1;
    public int VibrateEffect = 1;
    public int VisualEffect = 1;

    public int introTutorial = 0;
    public int MainMenuTutorial = 0;
    public int CardTutorial = 0;

    public bool enabled_NoAds;
    public bool enabled_3;
    public bool enabled_3_5;
    public bool enabled_5;

    public int enabled_3_adCount = 5;
}