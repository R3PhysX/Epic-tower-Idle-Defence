using GameAnalyticsSDK.Setup;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Android;

public class Init : MonoBehaviour
{
    private static Init _instance;

    public static Init Instance => _instance;

    public ActiveGameData activeGameData;

    public AttackAttributeData attackAttribute;
    public HealthAttributeData healthAttribute;
    public CoinAttributeData coinAttribute;
    public CardData cardData;

    [SerializeField] private ParticleSystem touchParticle;

    private UnityPool pool;
    private bool isInitiated;

    private string currentSavedDataString = "IdleTowerDefence_SavedDataV3";
    private string oldSavedDataStringV2 = "IdleTowerDefence_SavedDataV2";
    private string oldSavedDataStringV1 = "IdleTowerDefence_SavedData";

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {

        Application.targetFrameRate = 60;
        Application.runInBackground = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        activeGameData = ActiveGameData.Instance;

        if (PlayerPrefs.HasKey(currentSavedDataString))
        {
            string data = PlayerPrefs.GetString(currentSavedDataString);
            activeGameData.saveData = JsonConvert.DeserializeObject<SaveData>(data);

            for (int i = 0; i < activeGameData.saveData.attackSavedData.Count; i++)
            {
                attackAttribute.attributes[i].savedData = activeGameData.saveData.attackSavedData[i];
            }

            for (int i = 0; i < activeGameData.saveData.healthSavedData.Count; i++)
            {
                healthAttribute.attributes[i].savedData = activeGameData.saveData.healthSavedData[i];
            }

            for (int i = 0; i < activeGameData.saveData.coinSavedData.Count; i++)
            {
                coinAttribute.attributes[i].savedData = activeGameData.saveData.coinSavedData[i];
            }

            for (int i = 0; i < activeGameData.saveData.savedCardInfo.Count; i++)
            {
                cardData.cards[i].savedData = activeGameData.saveData.savedCardInfo[i];
            }
        }
        else
        {
            Reset();
            if (PlayerPrefs.HasKey(oldSavedDataStringV1))
            {
                LoadV1DataToCurrent();
            }
            else if (PlayerPrefs.HasKey(oldSavedDataStringV2))
            {
                LoadV2DataToCurrent();
            }
            else
            {
                AssignFreshUserData();
            }

        }

        isInitiated = true;
        pool = new UnityPool(touchParticle.gameObject, 10, transform);

        // activeGameData.saveData.introTutorial = 1;
        //  activeGameData.saveData.MainMenuTutorial= 1;

        if (activeGameData.saveData.introTutorial == 1)
            SceneLoadManager.Instance.LoadScene(Scenes.MainMenu);
        else
            SceneLoadManager.Instance.LoadScene(Scenes.Gameplay);
    }

    private void AssignFreshUserData()
    {
        activeGameData.saveData = new SaveData();

        activeGameData.saveData.username = "user" + UnityEngine.Random.Range(1111, 9999);

        ActiveGameData.Instance.saveData.gameStartedDate = System.DateTime.Now.ToString();
        ActiveGameData.Instance.saveData.totalPlayedTime[ActiveGameData.Instance.currentSelectedWorld] = 0;
        ActiveGameData.Instance.saveData.totalPlayedTimeDay[ActiveGameData.Instance.currentSelectedWorld] = 0;

        ActiveGameData.Instance.saveData.cardSlotUnlocked = new List<int>() { 1, 0, 0 };
        ActiveGameData.Instance.saveData.cardSlotIds = new List<int>() { -1, -1, -1 };

        for (int i = 0; i < attackAttribute.attributes.Count; i++)
        {
            attackAttribute.attributes[i].savedData.IsUnlocked = attackAttribute.attributes[i].Unlocked;
        }

        for (int i = 0; i < healthAttribute.attributes.Count; i++)
        {
            healthAttribute.attributes[i].savedData.IsUnlocked = healthAttribute.attributes[i].Unlocked;
        }

        for (int i = 0; i < coinAttribute.attributes.Count; i++)
        {
            coinAttribute.attributes[i].savedData.IsUnlocked = coinAttribute.attributes[i].Unlocked;
        }

        if (string.IsNullOrEmpty(ActiveGameData.Instance.saveData.lastIdleRewardDate))
        {
            ActiveGameData.Instance.saveData.lastIdleRewardDate = DateTime.Now.AddMinutes(-30).ToString();
        }

        string json = JsonConvert.SerializeObject(activeGameData.saveData);
        PlayerPrefs.SetString(currentSavedDataString, json);
    }

    private void LoadV1DataToCurrent()
    {

        string dataJson = PlayerPrefs.GetString(oldSavedDataStringV1);
        var OldSavedData = JsonConvert.DeserializeObject<OldSaveData_V1>(dataJson);

        activeGameData.saveData = new SaveData();

        activeGameData.saveData.avatarId = OldSavedData.avatarId;
        activeGameData.saveData.username = OldSavedData.username;

        activeGameData.saveData.tutorialShown = OldSavedData.tutorialShown;
        activeGameData.saveData.GoldCoin = OldSavedData.GoldCoin;
        activeGameData.saveData.Diamond = OldSavedData.Diamond;
        activeGameData.saveData.Gems = OldSavedData.Gems;
        activeGameData.saveData.lastRewardClaimedDate = OldSavedData.lastRewardClaimedDate;
        activeGameData.saveData.currentRewardDay = OldSavedData.currentRewardDay;
        activeGameData.saveData.isRewardClaimed = OldSavedData.isRewardClaimed;
        activeGameData.saveData.lastIdleRewardDate = OldSavedData.lastIdleRewardDate;

        activeGameData.saveData.bestWave[0] = OldSavedData.bestWave;

        ActiveGameData.Instance.saveData.gameStartedDate = OldSavedData.gameStartedDate;
        ActiveGameData.Instance.saveData.totalPlayedTime[0] = OldSavedData.totalPlayedTime;
        ActiveGameData.Instance.saveData.totalPlayedTimeDay[0] = OldSavedData.totalPlayedTimeDay;
        ActiveGameData.Instance.saveData.battleTimes[0] = OldSavedData.battleTimes;
        ActiveGameData.Instance.saveData.bestBattleGold[0] = OldSavedData.bestBattleGold;
        ActiveGameData.Instance.saveData.coinEarned[0] = OldSavedData.coinEarned;
        ActiveGameData.Instance.saveData.goldEarned[0] = OldSavedData.goldEarned;
        ActiveGameData.Instance.saveData.gemEarned[0] = OldSavedData.gemEarned;
        ActiveGameData.Instance.saveData.enemyDestroyed[0] = OldSavedData.enemyDestroyed;
        ActiveGameData.Instance.saveData.bossDestroyed[0] = OldSavedData.bossDestroyed;

        ActiveGameData.Instance.saveData.upgradeTimes = OldSavedData.upgradeTimes;
        ActiveGameData.Instance.saveData.dailyGiftClaimed = OldSavedData.dailyGiftClaimed;
        ActiveGameData.Instance.saveData.cardCollected = OldSavedData.cardCollected;

        ActiveGameData.Instance.saveData.cardSlotUnlocked = new List<int>() { OldSavedData.cardSlotUnlocked[0], OldSavedData.cardSlotUnlocked[1], 0 };
        ActiveGameData.Instance.saveData.cardSlotIds = new List<int>() { OldSavedData.cardSlotIds[0], OldSavedData.cardSlotIds[1], -1 };
        ActiveGameData.Instance.saveData.collectedWaveReward_world1 = OldSavedData.collectedWaveReward;

        ActiveGameData.Instance.saveData.introTutorial = OldSavedData.introTutorial;
        ActiveGameData.Instance.saveData.MainMenuTutorial = OldSavedData.MainMenuTutorial;
        ActiveGameData.Instance.saveData.CardTutorial = OldSavedData.CardTutorial;

        ActiveGameData.Instance.saveData.enabled_NoAds = OldSavedData.enabled_NoAds;
        ActiveGameData.Instance.saveData.enabled_3 = OldSavedData.enabled_3;
        ActiveGameData.Instance.saveData.enabled_3_5 = OldSavedData.enabled_3_5;
        ActiveGameData.Instance.saveData.enabled_5 = OldSavedData.enabled_5;
        ActiveGameData.Instance.saveData.enabled_3_adCount = OldSavedData.enabled_3_adCount;

        for (int i = 0; i < OldSavedData.attackSavedData.Count; i++)
        {
            attackAttribute.attributes[i].savedData = OldSavedData.attackSavedData[i];
        }

        for (int i = 0; i < OldSavedData.healthSavedData.Count; i++)
        {
            healthAttribute.attributes[i].savedData = OldSavedData.healthSavedData[i];
        }

        for (int i = 0; i < OldSavedData.coinSavedData.Count; i++)
        {
            coinAttribute.attributes[i].savedData = OldSavedData.coinSavedData[i];
        }

        for (int i = 0; i < OldSavedData.savedCardInfo.Count; i++)
        {
            cardData.cards[i].savedData = OldSavedData.savedCardInfo[i];
        }

        if (string.IsNullOrEmpty(ActiveGameData.Instance.saveData.lastIdleRewardDate))
        {
            ActiveGameData.Instance.saveData.lastIdleRewardDate = DateTime.Now.AddMinutes(-30).ToString();
        }

        string json = JsonConvert.SerializeObject(activeGameData.saveData);
        PlayerPrefs.SetString(currentSavedDataString, json);
    }

    private void LoadV2DataToCurrent()
    {
        string dataJson = PlayerPrefs.GetString(oldSavedDataStringV2);
        var OldSavedData = JsonConvert.DeserializeObject<OldSaveData_V2>(dataJson);

        activeGameData.saveData = new SaveData();

        activeGameData.saveData.avatarId = OldSavedData.avatarId;
        activeGameData.saveData.username = OldSavedData.username;

        activeGameData.saveData.tutorialShown = OldSavedData.tutorialShown;
        activeGameData.saveData.GoldCoin = OldSavedData.GoldCoin;
        activeGameData.saveData.Diamond = OldSavedData.Diamond;
        activeGameData.saveData.Gems = OldSavedData.Gems;
        activeGameData.saveData.lastRewardClaimedDate = OldSavedData.lastRewardClaimedDate;
        activeGameData.saveData.currentRewardDay = OldSavedData.currentRewardDay;
        activeGameData.saveData.isRewardClaimed = OldSavedData.isRewardClaimed;
        activeGameData.saveData.lastIdleRewardDate = OldSavedData.lastIdleRewardDate;

        activeGameData.saveData.bestWave[0] = OldSavedData.bestWave[0];
        activeGameData.saveData.bestWave[1] = OldSavedData.bestWave[1];

        ActiveGameData.Instance.saveData.gameStartedDate = OldSavedData.gameStartedDate;

        ActiveGameData.Instance.saveData.totalPlayedTime[0] = OldSavedData.totalPlayedTime[0];
        ActiveGameData.Instance.saveData.totalPlayedTime[1] = OldSavedData.totalPlayedTime[1];

        ActiveGameData.Instance.saveData.totalPlayedTimeDay[0] = OldSavedData.totalPlayedTimeDay[0];
        ActiveGameData.Instance.saveData.totalPlayedTimeDay[1] = OldSavedData.totalPlayedTimeDay[1];

        ActiveGameData.Instance.saveData.battleTimes[0] = OldSavedData.battleTimes[0];
        ActiveGameData.Instance.saveData.battleTimes[1] = OldSavedData.battleTimes[1];

        ActiveGameData.Instance.saveData.bestBattleGold[0] = OldSavedData.bestBattleGold[0];
        ActiveGameData.Instance.saveData.bestBattleGold[1] = OldSavedData.bestBattleGold[1];

        ActiveGameData.Instance.saveData.coinEarned[0] = OldSavedData.coinEarned[0];
        ActiveGameData.Instance.saveData.coinEarned[1] = OldSavedData.coinEarned[1];

        ActiveGameData.Instance.saveData.goldEarned[0] = OldSavedData.goldEarned[0];
        ActiveGameData.Instance.saveData.goldEarned[1] = OldSavedData.goldEarned[1];

        ActiveGameData.Instance.saveData.gemEarned[0] = OldSavedData.gemEarned[0];
        ActiveGameData.Instance.saveData.gemEarned[1] = OldSavedData.gemEarned[1];

        ActiveGameData.Instance.saveData.enemyDestroyed[0] = OldSavedData.enemyDestroyed[0];
        ActiveGameData.Instance.saveData.enemyDestroyed[1] = OldSavedData.enemyDestroyed[1];

        ActiveGameData.Instance.saveData.bossDestroyed[0] = OldSavedData.bossDestroyed[0];
        ActiveGameData.Instance.saveData.bossDestroyed[1] = OldSavedData.bossDestroyed[1];

        ActiveGameData.Instance.saveData.upgradeTimes = OldSavedData.upgradeTimes;
        ActiveGameData.Instance.saveData.dailyGiftClaimed = OldSavedData.dailyGiftClaimed;
        ActiveGameData.Instance.saveData.cardCollected = OldSavedData.cardCollected;

        ActiveGameData.Instance.saveData.cardSlotUnlocked = new List<int>() { OldSavedData.cardSlotUnlocked[0], OldSavedData.cardSlotUnlocked[1], 0 };
        ActiveGameData.Instance.saveData.cardSlotIds = new List<int>() { OldSavedData.cardSlotIds[0], OldSavedData.cardSlotIds[1], -1 };
        ActiveGameData.Instance.saveData.collectedWaveReward_world1 = OldSavedData.collectedWaveReward_world1;
        ActiveGameData.Instance.saveData.collectedWaveReward_world2 = OldSavedData.collectedWaveReward_world2;

        ActiveGameData.Instance.saveData.introTutorial = OldSavedData.introTutorial;
        ActiveGameData.Instance.saveData.MainMenuTutorial = OldSavedData.MainMenuTutorial;
        ActiveGameData.Instance.saveData.CardTutorial = OldSavedData.CardTutorial;

        ActiveGameData.Instance.saveData.enabled_NoAds = OldSavedData.enabled_NoAds;
        ActiveGameData.Instance.saveData.enabled_3 = OldSavedData.enabled_3;
        ActiveGameData.Instance.saveData.enabled_3_5 = OldSavedData.enabled_3_5;
        ActiveGameData.Instance.saveData.enabled_5 = OldSavedData.enabled_5;
        ActiveGameData.Instance.saveData.enabled_3_adCount = OldSavedData.enabled_3_adCount;

        for (int i = 0; i < OldSavedData.attackSavedData.Count; i++)
        {
            attackAttribute.attributes[i].savedData = OldSavedData.attackSavedData[i];
        }

        for (int i = 0; i < OldSavedData.healthSavedData.Count; i++)
        {
            healthAttribute.attributes[i].savedData = OldSavedData.healthSavedData[i];
        }

        for (int i = 0; i < OldSavedData.coinSavedData.Count; i++)
        {
            coinAttribute.attributes[i].savedData = OldSavedData.coinSavedData[i];
        }

        for (int i = 0; i < OldSavedData.savedCardInfo.Count; i++)
        {
            cardData.cards[i].savedData = OldSavedData.savedCardInfo[i];
        }

        if (string.IsNullOrEmpty(ActiveGameData.Instance.saveData.lastIdleRewardDate))
        {
            ActiveGameData.Instance.saveData.lastIdleRewardDate = DateTime.Now.AddMinutes(-30).ToString();
        }

        string json = JsonConvert.SerializeObject(activeGameData.saveData);
        PlayerPrefs.SetString(currentSavedDataString, json);
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && isInitiated)
        {
            SaveData();
        }
    }

#if UNITY_EDITOR
    private void OnDestroy()
    {
        if (isInitiated)
        {
            SaveData();
        }
    }
#endif

    private void SaveData()
    {
        activeGameData.saveData.attackSavedData.Clear();
        activeGameData.saveData.healthSavedData.Clear();
        activeGameData.saveData.coinSavedData.Clear();
        activeGameData.saveData.savedCardInfo.Clear();

        for (int i = 0; i < attackAttribute.attributes.Count; i++)
        {
            activeGameData.saveData.attackSavedData.Add(attackAttribute.attributes[i].savedData);
        }

        for (int i = 0; i < healthAttribute.attributes.Count; i++)
        {
            activeGameData.saveData.healthSavedData.Add(healthAttribute.attributes[i].savedData);
        }

        for (int i = 0; i < coinAttribute.attributes.Count; i++)
        {
            activeGameData.saveData.coinSavedData.Add(coinAttribute.attributes[i].savedData);
        }

        for (int i = 0; i < cardData.cards.Count; i++)
        {
            activeGameData.saveData.savedCardInfo.Add(cardData.cards[i].savedData);
        }

        string json = JsonConvert.SerializeObject(activeGameData.saveData);
        PlayerPrefs.SetString(currentSavedDataString, json);
        PlayerPrefs.Save();

        Debug.Log("Saved");
    }

    [ContextMenu("Reset")]
    public void Reset()
    {
        for (int i = 0; i < attackAttribute.attributes.Count; i++)
        {
            attackAttribute.attributes[i].savedData = new AttributeSavedData();
        }

        for (int i = 0; i < healthAttribute.attributes.Count; i++)
        {
            healthAttribute.attributes[i].savedData = new AttributeSavedData();
        }

        for (int i = 0; i < coinAttribute.attributes.Count; i++)
        {
            coinAttribute.attributes[i].savedData = new AttributeSavedData();
        }

        for (int i = 0; i < cardData.cards.Count; i++)
        {
            cardData.cards[i].savedData = new SavedCardData();
            cardData.cards[i].savedData.id = cardData.cards[i].cardId;
        }
    }

    private void Update()
    {
        if (ActiveGameData.Instance.saveData.VisualEffect == 1)
        {
            if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
                Vector3 clickPosition;

                if (Input.touchCount > 0)
                {
                    clickPosition = Input.GetTouch(0).position;
                }
                else
                {
                    clickPosition = Input.mousePosition;
                }

                clickPosition = Camera.main.ScreenToWorldPoint(clickPosition);

                var obj = pool.Get<ParticleSystem>(transform);
                clickPosition.z = -1f;
                obj.transform.position = clickPosition;

                LeanTween.delayedCall(1f, () =>
                {
                    pool.Add(obj);
                }).setIgnoreTimeScale(true);
            }
        }
    }
}