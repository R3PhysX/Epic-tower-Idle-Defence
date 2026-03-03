using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardItem : MonoBehaviour
{
    [SerializeField] private Image mainImage;
    [SerializeField] private Image highlight;
    [SerializeField] private Sprite enabledSprite;
    [SerializeField] private Sprite disabledSprite;
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private TMP_Text claimedText;

    internal int currentDay;
    internal bool status;
    internal DailyRewardData rewardData;

    private int goldCoinReward;
    private int diamondReward;
    private int gemReward;

    internal void SetUI(int day, bool status, bool highLight)
    {
        currentDay = day;
        this.status = status;
        mainImage.sprite = enabledSprite;
        dayText.text = "DAY " + day.ToString();
        claimedText.text = status ? "Claimed" : "Claim";
        highlight.gameObject.SetActive(highLight);
    }

    internal void SetReward(int gold, int diamond, int gem)
    {
        goldCoinReward = gold;
        diamondReward = diamond;
        gemReward = gem;
    }

    internal void SetUI(int day)
    {
        mainImage.sprite = disabledSprite;
        dayText.text = "DAY " + day.ToString();
        claimedText.text = "?";
    }
}
