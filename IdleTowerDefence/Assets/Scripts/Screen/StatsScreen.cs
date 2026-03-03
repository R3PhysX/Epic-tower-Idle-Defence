using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsScreen : ScreenPanel
{
    public TMP_Text gameStartedText;
    public TMP_Text battleTimeText;
    public TMP_Text bestBattleGoldText;
    public TMP_Text coinEarnedText;
    public TMP_Text goldEarnedText;
    public TMP_Text gemEarnedText;
    public TMP_Text enemyDestroyedText;
    public TMP_Text bossDestoryedText;
    public TMP_Text upgradeTimesText;
    public TMP_Text dailyGiftClaimedText;
    public TMP_Text cardCollectedText;
    public TMP_Text bestWaveSurvived;
    public TMP_Text totalTimePlayed;

    public CustomButton world1Button;
    public CustomButton world2Button;
    public CustomButton world3Button;

    public Sprite selectedSprite;
    public Sprite disableSprite;

    private void OnEnable()
    {
        GameAnalyticsManager.Instance.NewDesignEventGA("Click_Stats");
        world1Button.onClick.AddListener(() => { OnSelect_World(0, world1Button, world2Button, world3Button); });
        world2Button.onClick.AddListener(() => { OnSelect_World(1, world2Button, world1Button, world3Button); });
        world3Button.onClick.AddListener(() => { OnSelect_World(2, world3Button, world2Button, world1Button); });

        world1Button.onClick?.Invoke();
    }

    private void OnSelect_World(int world, CustomButton selected, CustomButton disable1, CustomButton disable2)
    {
        SaveData data = ActiveGameData.Instance.saveData;
        var dateTime = DateTime.Parse(data.gameStartedDate);

        selected.image.sprite = selectedSprite;
        disable1.image.sprite = disableSprite;
        disable2.image.sprite = disableSprite;

        gameStartedText.text = dateTime.ToString("dd-MM-yyyy");
        battleTimeText.text = data.battleTimes[world].ToString("00");
        bestBattleGoldText.text = data.bestBattleGold[world].ToString("00");
        coinEarnedText.text = data.coinEarned[world].ToString("00");
        goldEarnedText.text = data.goldEarned[world].ToString("00");
        gemEarnedText.text = data.gemEarned[world].ToString("00");
        enemyDestroyedText.text = data.enemyDestroyed[world].ToString("00");
        bossDestoryedText.text = data.bossDestroyed[world].ToString("00");
        upgradeTimesText.text = data.upgradeTimes.ToString("00");
        dailyGiftClaimedText.text = data.dailyGiftClaimed.ToString("00");
        cardCollectedText.text = data.cardCollected.ToString("00");
        bestWaveSurvived.text = data.bestWave[world].ToString("00");

        TimeSpan timeSpan = TimeSpan.FromSeconds(data.totalPlayedTime[world]);
        string formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

        totalTimePlayed.text = data.totalPlayedTimeDay[world] + " Day, " + formattedTime;
    }

    private void OnDisable()
    {
        world1Button.onClick.RemoveAllListeners();
        world2Button.onClick.RemoveAllListeners();
    }
}