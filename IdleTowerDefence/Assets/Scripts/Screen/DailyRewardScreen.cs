using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DailyRewardScreen : ScreenPanel
{
    [SerializeField] private CustomButton closeButton;
    [SerializeField] private CustomButton claimButton;
    [SerializeField] private CustomButton claim2xButton;
    [SerializeField] private TMP_Text claim2xButtonText;
    [SerializeField] private TMP_Text currentDayText;
    [SerializeField] private TMP_Text goldCoin;
    [SerializeField] private TMP_Text diamond;
    [SerializeField] private TMP_Text gem;
    [SerializeField] private ScrollRect rewardListScroll;
    [SerializeField] private RewardItem rewardItem;
    private int currentDay;
    private bool currentDayClaimed;
    private DateTime lastClaimTime;

    private List<RewardItem> itemList = new List<RewardItem>();
    private RewardItem currentDayItem;

    public int dayToAdd;
    private int goldCoinReward;
    private int diamondReward;
    private int gemReward;

    [SerializeField] private DailyRewardData rewardData;

    private void OnEnable()
    {
        closeButton.onClick.AddListener(OnClick_Close);
        claimButton.onClick.AddListener(OnClick_Claim);
        claim2xButton.onClick.AddListener(OnClick_Claim2x);

        claimButton.gameObject.SetActive(false);
        claim2xButton.gameObject.SetActive(false);

        Refresh();
    }

    private async void Refresh()
    {
        SceneLoadManager.Instance.SetLoading(true);

        rewardListScroll.content.DestroyAllChildren();

        var data = await FetchTimeFromInternet();
        SceneLoadManager.Instance.SetLoading(false);

        TimeSpan timeDifference = data - DateTime.UtcNow;

        double absoluteTimeDifference = Math.Abs(timeDifference.TotalMinutes);

        if (absoluteTimeDifference > 5)
        {
            ToastManager.Get.ShowMessage("System time may have been modified! Please check");
            return;
        }

        currentDay = ActiveGameData.Instance.saveData.currentRewardDay;

        string lastClaimTimeString = ActiveGameData.Instance.saveData.lastRewardClaimedDate;

        if (!string.IsNullOrEmpty(lastClaimTimeString))
        {
            lastClaimTime = DateTime.Parse(lastClaimTimeString);
            if (HasDayPassed(lastClaimTime, DateTime.Now))
            {
                if ((DateTime.Now - lastClaimTime).TotalDays > 1)
                {
                    // Reset to Day 1 if more than 1 day has passed
                    currentDay = 1;
                }
                else
                {
                    currentDay += 1;
                }

                if (currentDay > 16)
                    currentDay = 1;
                ActiveGameData.Instance.saveData.isRewardClaimed = 0;
            }
        }

        currentDayText.text = "DAY " + currentDay;
        currentDayClaimed = ActiveGameData.Instance.saveData.isRewardClaimed == 1;

        claimButton.gameObject.SetActive(!(ActiveGameData.Instance.saveData.isRewardClaimed == 1));
        claim2xButton.gameObject.SetActive(!(ActiveGameData.Instance.saveData.isRewardClaimed == 1));

        if (ActiveGameData.Instance.saveData.enabled_NoAds)
        {
            claimButton.gameObject.SetActive(false);
            claim2xButtonText.text = "CLAIM X2";
        }

        goldCoinReward = (rewardData.goldPerDay * currentDay) + rewardData.startingGold;
        diamondReward = (rewardData.diamondPerDay * currentDay) + rewardData.startingDiamond;
        gemReward = (rewardData.crystalPerDay * currentDay) + rewardData.startingCrystal;

        goldCoin.text = goldCoinReward.ToString();
        diamond.text = diamondReward.ToString();
        gem.text = gemReward.ToString();

        for (int i = 1; i <= 16; i++)
        {
            var obj = GameObject.Instantiate(rewardItem, rewardListScroll.content).GetComponent<RewardItem>();
            if (i < currentDay)
            {
                //unclaimed
                obj.SetUI(i, true, false);
            }
            else if (i == currentDay)
            {
                //can claim or not
                obj.SetUI(i, currentDayClaimed, true);
                currentDayItem = obj;
            }
            else
            {
                obj.SetUI(i);
            }

            obj.rewardData = rewardData;
        }
    }

    private bool HasDayPassed(DateTime lastClaimTime, DateTime currentTime)
    {
        return currentTime.Date > lastClaimTime.Date;
    }

    private void OnClick_Claim()
    {
        if (currentDayItem.status == false)
        {
            currentDayItem.SetUI(currentDay, true, true);
            ActiveGameData.Instance.saveData.currentRewardDay = currentDay;
            ActiveGameData.Instance.saveData.lastRewardClaimedDate = DateTime.Now.AddDays(dayToAdd).ToString();
            ActiveGameData.Instance.saveData.isRewardClaimed = 1;

            //ActiveGameData.Instance.saveData.GoldCoin += goldCoinReward;
            //ActiveGameData.Instance.saveData.Diamond += diamondReward;
            //ActiveGameData.Instance.saveData.Gems += gemReward;

            int gainCoin = goldCoinReward;
            int gainDiamond = diamondReward;
            int gainGem = gemReward;

            ActiveGameData.Instance.saveData.dailyGiftClaimed += 1;
            ActiveGameData.Instance.saveData.goldEarned[ActiveGameData.Instance.currentSelectedWorld] += goldCoinReward;
            ActiveGameData.Instance.saveData.gemEarned[ActiveGameData.Instance.currentSelectedWorld] += gemReward;

            HomeScreen homeScreen = ScreenManager.Get.GetScreen<HomeScreen>();
            if (goldCoinReward > 0)
                CoinAnimation.Get.ShowCoin(homeScreen.goldCoinText.transform.position, goldCoinReward, () => {
                    Constants.Get.GoldCoin += gainCoin;
                    EventManager.TriggerEvent(EventID.Update_Currency);
                });
            if (diamondReward > 0)
                CoinAnimation.Get.ShowDiamond(homeScreen.diamondCoinText.transform.position, diamondReward, () => {
                    Constants.Get.Diamond += gainDiamond;
                    EventManager.TriggerEvent(EventID.Update_Currency);
                });
            if (gemReward > 0)
                CoinAnimation.Get.ShowGem(homeScreen.gemCoinText.transform.position, gemReward, () => {
                    Constants.Get.Gems += gainGem;
                    EventManager.TriggerEvent(EventID.Update_Currency);
                });

           // AppsFlyerEventManager.Get.SendEvent(AppsFlyerEventManager.AF_Daily_Reward, "1");
            homeScreen.CheckForRewardIButton();
            Hide();
        }
    }

    private void OnClick_Claim2x()
    {
        if (currentDayItem.status == false)
        {
            float timeScale = Time.timeScale;
            AdManager.Get.ShowRewardedAd((status) =>
            {
                Time.timeScale = timeScale;
                if (status)
                {
                    Debug.Log("Reward Garnted");
                    currentDayItem.SetUI(currentDay, true, true);
                    ActiveGameData.Instance.saveData.currentRewardDay = currentDay;
                    ActiveGameData.Instance.saveData.lastRewardClaimedDate = DateTime.Now.AddDays(dayToAdd).ToString();
                    ActiveGameData.Instance.saveData.isRewardClaimed = 1;

                    //ActiveGameData.Instance.saveData.GoldCoin += goldCoinReward * 2;
                    //ActiveGameData.Instance.saveData.Diamond += diamondReward * 2;
                    //ActiveGameData.Instance.saveData.Gems += gemReward * 2;

                    //EventManager.TriggerEvent(EventID.Update_Currency);

                    int gainCoin = goldCoinReward * 2;
                    int gainDiamond = diamondReward * 2;
                    int gainGem = gemReward * 2;

                    ActiveGameData.Instance.saveData.dailyGiftClaimed += 1;
                    ActiveGameData.Instance.saveData.goldEarned[ActiveGameData.Instance.currentSelectedWorld] += goldCoinReward * 2;
                    ActiveGameData.Instance.saveData.gemEarned[ActiveGameData.Instance.currentSelectedWorld] += gemReward * 2;

                    HomeScreen homeScreen = ScreenManager.Get.GetScreen<HomeScreen>();
                    if (goldCoinReward > 0)
                        CoinAnimation.Get.ShowCoin(homeScreen.goldCoinText.transform.position, gainCoin, () =>
                        {
                            Constants.Get.GoldCoin += gainCoin;
                            EventManager.TriggerEvent(EventID.Update_Currency);
                        });
                    if (diamondReward > 0)
                        CoinAnimation.Get.ShowDiamond(homeScreen.diamondCoinText.transform.position, gainDiamond, () =>
                        {
                            Constants.Get.Diamond += gainDiamond;
                            EventManager.TriggerEvent(EventID.Update_Currency);
                        });
                    if (gemReward > 0)
                        CoinAnimation.Get.ShowGem(homeScreen.gemCoinText.transform.position, gainGem, () =>
                        {
                            Constants.Get.Gems += gainGem;
                            EventManager.TriggerEvent(EventID.Update_Currency);
                        });

                   // AppsFlyerEventManager.Get.SendEvent(AppsFlyerEventManager.AF_Daily_Reward_2X, "1");
                    homeScreen.CheckForRewardIButton();
                    Hide();
                }
                else
                {
                    Debug.Log("Reward Not Granted");
                    ToastManager.Get.ShowMessage("Ads Not Available");
                }
            });
        }
    }

    private void OnClick_Close()
    {
        Hide();
    }

    private void OnDisable()
    {
        closeButton.onClick.RemoveAllListeners();
        claimButton.onClick.RemoveAllListeners();
        claim2xButton.onClick.RemoveAllListeners();
    }

    async Task<DateTime> FetchTimeFromInternet()
    {
        Debug.Log("FetchTimeFromInternet");
        string url = "https://www.timeapi.io/api/Time/current/zone?timeZone=UTC";

        UnityWebRequest www = UnityWebRequest.Get(url);

        // Send the request asynchronously
        AsyncOperation asyncOperation = www.SendWebRequest();

        // Wait for the operation to complete
        while (!asyncOperation.isDone)
        {
            await Task.Delay(100); // You can adjust the delay as needed
        }

        if (!(www.result == UnityWebRequest.Result.ConnectionError) && !(www.result == UnityWebRequest.Result.ProtocolError))
        {
            string jsonResponse = www.downloadHandler.text;

            // Parse the JSON response to get the time information
            WorldTimeInfo worldTimeInfo = JsonUtility.FromJson<WorldTimeInfo>(jsonResponse);

            // Access the time data and parse it as DateTime
            DateTime currentTime = DateTime.Parse(worldTimeInfo.dateTime);

            string formattedTime = currentTime.ToString("yyyy-MM-dd HH:mm:ss");
            Debug.Log("Current Time: " + formattedTime);

            return currentTime;

            // Format the time in a similar way to DateTime.Now

        }
        else
        {
            Debug.LogError("Failed to fetch time: " + www.error);
            return DateTime.Now;
        }
    }
}
