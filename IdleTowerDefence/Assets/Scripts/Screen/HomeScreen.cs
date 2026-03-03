//using Lofelt.NiceVibrations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HomeScreen : ScreenPanel
{
    [SerializeField] private int world2Unlock_Wave = 40;
    [SerializeField] private int world3Unlock_Wave = 40;

    [SerializeField] private CustomButton battleButton;
    [SerializeField] private CustomButton dailyRewardButton;
    [SerializeField] private CustomButton adPassButton;
    [SerializeField] private CustomButton shopButton;
    [SerializeField] private Image iButton;
    [SerializeField] private CustomButton settingButton;
    [SerializeField] private CustomButton idleRewardCollectButton;
    [SerializeField] private CustomButton profileButton;
    [SerializeField] private CustomButton goldRushButton;
    [SerializeField] private CustomButton diceRollButton;

    [SerializeField] public TMP_Text goldRushTimerText;
    [SerializeField] public TMP_Text diceRollTimerText;

    [SerializeField] internal TMP_Text goldCoinText;
    [SerializeField] internal TMP_Text diamondCoinText;
    [SerializeField] internal TMP_Text gemCoinText;
    [SerializeField] internal TMP_Text ticketText;

    [SerializeField] private TMP_Text idlegoldCoinText;
    [SerializeField] private TMP_Text idlediamondText;
    [SerializeField] private TMP_Text idlegemText;

    [SerializeField] private Transform waveParent;
    [SerializeField] private WaveItem waveObject;

    [SerializeField] private TMP_Text timerText;

    [SerializeField] private Image profileIcon;
    [SerializeField] private AvatarData avatarData;

    public int coinsPerInterval = 100;
    public int diamondsPerInterval = 50;
    public int gemsPerInterval = 30;
    public float rewardInterval = 3 * 3600; // 3 hours in seconds

    private DateTime lastRewardTime;
    private int accumulatedCoins = 0;
    private int accumulatedDiamonds = 0;
    private int accumulatedGems = 0;

    private Coroutine timerCoroutine;

    [SerializeField] private Image idleRewardSprite;
    [SerializeField] private Color color1;
    [SerializeField] private Color color2;


    [SerializeField] private RectTransform worldPanel;
    [SerializeField] private List<RectTransform> worldPanels;
    [SerializeField] private GameObject world2lockIcon;
    [SerializeField] private GameObject world3lockIcon;
    
    public CustomButton leftArrow;
    public CustomButton rightArrow;

    public DayOfWeek offer1Day;
    public DayOfWeek offer2Day;

    public DayOfWeek diceRoll1Day;
    public DayOfWeek diceRoll2Day;
    public DayOfWeek diceRoll3Day;

    private string DiceRollStartTimeKey = "ITD_DiceRollEventTime";

    private string GoldRushStartTimeKey = "ITD_GoldRushOfferTime";
    private string GoldRushPurchasedKey = "ITD_GoldRush_Purchased";
    private Coroutine UpdateGoldRushTimerRoutine;
    private Coroutine UpdateDiceRollTimerRoutine;

    private void OnEnable()
    {
        GameAnalyticsManager.Instance.NewDesignEventGA("Click_Home");
        ticketText.text = ActiveGameData.Instance.saveData.diceRollTicket.ToString();
        SetWorldPanel();

        leftArrow.onClick.AddListener(OnClick_LeftArrow);
        rightArrow.onClick.AddListener(OnClick_RightArrow);

        battleButton.onClick.AddListener(OnClick_Battle);
        dailyRewardButton.onClick.AddListener(OnClick_DailyReward);
        diceRollButton.onClick.AddListener(OnClick_DiceRoll);
        adPassButton.onClick.AddListener(OnClick_AdPass);
        settingButton.onClick.AddListener(OnClick_Setting);
        idleRewardCollectButton.onClick.AddListener(OnClick_CollectIdleReward);
        profileButton.onClick.AddListener(OnClick_Profile);

        EventManager.AddListener(EventID.Update_Currency, Update_Currency);

        if (ActiveGameData.Instance.saveData.enabled_NoAds)
        {
            adPassButton.gameObject.SetActive(false);
        }

        if (UpdateGoldRushTimerRoutine != null)
            StopCoroutine(UpdateGoldRushTimerRoutine);

        CheckForReward();
        CheckForRewardIButton();
        leftArrow.gameObject.SetActive(ActiveGameData.Instance.currentSelectedWorld > 0);
        rightArrow.gameObject.SetActive(ActiveGameData.Instance.currentSelectedWorld < worldPanels.Count - 1);

        world2lockIcon.SetActive(ActiveGameData.Instance.saveData.bestWave[0] <= world2Unlock_Wave);
        world3lockIcon.SetActive(ActiveGameData.Instance.saveData.bestWave[0] <= world3Unlock_Wave);
    }

    private void SetWorldPanel()
    {
        worldPanel.anchoredPosition = new Vector3(0, worldPanel.anchoredPosition.y);

        for (int i = 0; i < worldPanels.Count; i++)
        {
            worldPanels[i].anchoredPosition = new Vector3(i * Screen.width, worldPanels[i].anchoredPosition.y);
            worldPanels[i].gameObject.SetActive(true);
        }

        worldPanel.anchoredPosition = new Vector3(ActiveGameData.Instance.currentSelectedWorld * Screen.width * -1, worldPanel.anchoredPosition.y);
    }

    public void OnClick_LeftArrow()
    {
        if (ActiveGameData.Instance.currentSelectedWorld <= 0)
            return;

        Debug.Log("Left");
        ActiveGameData.Instance.currentSelectedWorld -= 1;
        leftArrow.gameObject.SetActive(ActiveGameData.Instance.currentSelectedWorld > 0);
        rightArrow.gameObject.SetActive(ActiveGameData.Instance.currentSelectedWorld < worldPanels.Count - 1);

        LeanTween.value(worldPanel.gameObject, (val) =>
        {
            worldPanel.anchoredPosition = new Vector3(val, worldPanel.anchoredPosition.y);
        }, worldPanel.anchoredPosition.x, ActiveGameData.Instance.currentSelectedWorld * Screen.width * -1, 0.2f).setEase(LeanTweenType.easeOutSine)
.setOnComplete(() =>
{
});

        RefreshWavePanel();
    }

    public void OnClick_RightArrow()
    {
        if (ActiveGameData.Instance.currentSelectedWorld >= worldPanels.Count-1)
            return;
        Debug.Log("Right");
        ActiveGameData.Instance.currentSelectedWorld += 1;
        leftArrow.gameObject.SetActive(ActiveGameData.Instance.currentSelectedWorld > 0);
        rightArrow.gameObject.SetActive(ActiveGameData.Instance.currentSelectedWorld < worldPanels.Count - 1);

        LeanTween.value(worldPanel.gameObject, (val) =>
        {
            worldPanel.anchoredPosition = new Vector3(val, worldPanel.anchoredPosition.y);
        }, worldPanel.anchoredPosition.x, ActiveGameData.Instance.currentSelectedWorld * Screen.width * -1, 0.2f).setEase(LeanTweenType.easeOutSine)
.setOnComplete(() =>
{
});

        RefreshWavePanel();
    }
    
    private void OnClick_AdPass()
    {
        shopButton?.onClick.Invoke();
    }

    public async void CheckForReward()
    {
        if (TutorialManager.Get != null && TutorialManager.Get.isTutorialShowing == false)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                return;
            }

            SceneLoadManager.Instance.SetLoading(true);
            var data = await FetchTimeFromInternet();
            SceneLoadManager.Instance.SetLoading(false);
            TimeSpan timeDifference = data - DateTime.UtcNow;

            double absoluteTimeDifference = Math.Abs(timeDifference.TotalMinutes);

            if (absoluteTimeDifference > 5)
            {
                ToastManager.Get.ShowMessage("System time may have been modified! Please check");
                return;
            }
        }

        // Load the last reward time from player preferences.
        if (string.IsNullOrEmpty(ActiveGameData.Instance.saveData.lastIdleRewardDate))
            ActiveGameData.Instance.saveData.lastIdleRewardDate = DateTime.Now.ToString();

        string lastRewardTimeString = ActiveGameData.Instance.saveData.lastIdleRewardDate;
        lastRewardTime = DateTime.Parse(lastRewardTimeString);

        // Start the timer coroutine to update the UI.
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
        timerCoroutine = StartCoroutine(UpdateTimerUI());
        NotificationManager.Get.ScheduleNotification_RewardComplete((int)(lastRewardTime.AddSeconds(rewardInterval) - DateTime.Now).TotalSeconds);
    }

    internal void UpdateProfile()
    {
        var profile = avatarData.avatarInfo.Find(x => x.id == ActiveGameData.Instance.saveData.avatarId);
        profileIcon.sprite = profile.avatar;
    }

    private void Start()
    {
        RefreshWavePanel();

        UpdateProfile();
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            ToastManager.Get.ShowMessage("An internet Connection is required to collect reward !");
            return;
        }

        SceneLoadManager.Instance.SetLoading(true);
        goldRushButton.gameObject.SetActive(false);
        diceRollButton.gameObject.SetActive(false);
        LeanTween.delayedCall(2f, () =>
        {
            SceneLoadManager.Instance.SetLoading(false);
            if (ActiveGameData.Instance.dailyRewardPopupShown == false && TutorialManager.Get != null && TutorialManager.Get.isTutorialShowing == false && ActiveGameData.Instance.saveData.isRewardClaimed == 0)
            {
                ActiveGameData.Instance.dailyRewardPopupShown = true;
                ScreenManager.Get.GetScreen<DailyRewardScreen>().Show();
            }

            CheckForGoldRushOffer();
            CheckForDiceRollEvent();
        });

        CheckForRewardIButton();

    }

    public void RefreshWavePanel()
    {
        waveParent.transform.DestroyAllChildren();

        int start = 1;
        if (ActiveGameData.Instance.saveData.bestWave[ActiveGameData.Instance.currentSelectedWorld] >= 7)
            start = ActiveGameData.Instance.saveData.bestWave[ActiveGameData.Instance.currentSelectedWorld] - 6;

        int end = start + 7;
        bool firstItem = false;
        for (int i = start; i <= end; i++)
        {
            var obj = GameObject.Instantiate(waveObject.gameObject, waveParent).GetComponent<WaveItem>();
            if (ActiveGameData.Instance.currentSelectedWorld == 2)
            {
                bool val = ActiveGameData.Instance.saveData.collectedWaveReward_world3.Contains(i);
                if (val == false && firstItem == false)
                {
                    obj.Set(i, !val, true);
                    firstItem = true;
                }
                else
                    obj.Set(i, !val, false);
            }
            else if (i % 3 == 0)
            {
                bool val = false;
                if (ActiveGameData.Instance.currentSelectedWorld == 0)
                    val = ActiveGameData.Instance.saveData.collectedWaveReward_world1.Contains(i);
                else
                    val = ActiveGameData.Instance.saveData.collectedWaveReward_world2.Contains(i);

                if (val == false && firstItem == false)
                {
                    obj.Set(i, !val, true);
                    firstItem = true;
                }
                else
                    obj.Set(i, !val);
            }
            else
                obj.Set(i, false);
        }
    }

    internal void CheckForRewardIButton()
    {
        iButton.gameObject.SetActive(false);
        if (ActiveGameData.Instance.dailyRewardPopupShown == false && TutorialManager.Get != null && TutorialManager.Get.isTutorialShowing == false && ActiveGameData.Instance.saveData.isRewardClaimed == 0)
        {
            iButton.gameObject.SetActive(true);
        }
    }

    public void OnGoldRushPopup()
    {
        ScreenManager.Get.GetScreen<GoldRushScreen>().Show();
    }

    internal async void CheckForGoldRushOffer()
    {
        if (ActiveGameData.Instance.saveData.bestWave[0] < 5)
            return;

        if (TutorialManager.Get != null && TutorialManager.Get.isTutorialShowing == true)
        {
            return;
        }
            
        var data = await FetchTimeFromInternet();

        TimeSpan timeDifference = data - DateTime.UtcNow;

        double absoluteTimeDifference = Math.Abs(timeDifference.TotalMinutes);

        if (absoluteTimeDifference > 5)
        {
            ToastManager.Get.ShowMessage("System time may have been modified! Please check");
            return;
        }

        DateTime currentTime = DateTime.Now;
        DayOfWeek today = currentTime.DayOfWeek;

        // Check if today is Tuesday or Friday
        if (today == offer1Day || today == offer2Day)
        {
            bool firstTime = false;
            if (!PlayerPrefs.HasKey(GoldRushStartTimeKey))
            {
                firstTime = true;
                // First access on Gold Rush day; store the start time
                PlayerPrefs.SetString(GoldRushStartTimeKey, currentTime.ToString());
            }

            // Parse the start time from PlayerPrefs
            DateTime goldRushStartTime = DateTime.Parse(PlayerPrefs.GetString(GoldRushStartTimeKey));
            DateTime goldRushEndTime = goldRushStartTime.AddHours(24);

            // Check if current time is within 24 hours from the start time
            if (currentTime <= goldRushEndTime)
            {
                goldRushButton.gameObject.SetActive(true);
                UpdateGoldRushTimerRoutine = StartCoroutine(UpdateGoldRushTimer(goldRushEndTime));
                if (firstTime)
                {
                    if (TutorialManager.Get != null && TutorialManager.Get.isTutorialShowing == false)
                    {
                        ScreenManager.Get.GetScreen<GoldRushScreen>().Show();
                    }
                }
            }
            else
            {
                // Clear start time as the offer period has expired
                PlayerPrefs.DeleteKey(GoldRushStartTimeKey);
                PlayerPrefs.DeleteKey(GoldRushPurchasedKey);

            }
        }
        else
        {
            // Not Tuesday or Friday, clear any previous start time
            PlayerPrefs.DeleteKey(GoldRushStartTimeKey);
            PlayerPrefs.DeleteKey(GoldRushPurchasedKey);
        }
    }

    internal async void CheckForDiceRollEvent()
    {
        if (ActiveGameData.Instance.saveData.bestWave[0] < 10)
            return;

        if (TutorialManager.Get != null && TutorialManager.Get.isTutorialShowing == true)
        {
            return;
        }

        var data = await FetchTimeFromInternet();

        TimeSpan timeDifference = data - DateTime.UtcNow;

        double absoluteTimeDifference = Math.Abs(timeDifference.TotalMinutes);

        if (absoluteTimeDifference > 5)
        {
            ToastManager.Get.ShowMessage("System time may have been modified! Please check");
            return;
        }
                           
        DateTime currentTime = DateTime.Now;
        DayOfWeek today = currentTime.DayOfWeek;
        bool isEventDay = (today == diceRoll1Day || today == diceRoll2Day || today == diceRoll3Day);

        // Check if today is Tuesday or Friday
        if (isEventDay || PlayerPrefs.HasKey(DiceRollStartTimeKey))
        {
            bool firstTime = false;
            if (!PlayerPrefs.HasKey(DiceRollStartTimeKey))
            {
                firstTime = true;
                // First access on Gold Rush day; store the start time
                PlayerPrefs.SetString(DiceRollStartTimeKey, currentTime.ToString());
                ActiveGameData.Instance.saveData.diceRollTicket = 3;
                ActiveGameData.Instance.saveData.playerIndexDiceRoll = 0;
            }

            // Parse the start time from PlayerPrefs
            DateTime diceRollStartTime = DateTime.Parse(PlayerPrefs.GetString(DiceRollStartTimeKey));
            DateTime diceRollEndTime = diceRollStartTime.AddHours(72);

            // Check if current time is within 72 hours from the start time
            if (currentTime <= diceRollEndTime)
            {
                ActiveGameData.Instance.diceRollActive = true;
                diceRollButton.gameObject.SetActive(true);
                UpdateDiceRollTimerRoutine = StartCoroutine(UpdateDiceRollTimer(diceRollEndTime));
                if (firstTime)
                {
                    if (TutorialManager.Get != null && TutorialManager.Get.isTutorialShowing == false)
                    {
                        ScreenManager.Get.GetScreen<DiceRollEventPopup>().Show();
                    }
                }
            }
            else
            {
                ActiveGameData.Instance.diceRollActive = false;
                // Clear start time as the offer period has expired
                PlayerPrefs.DeleteKey(DiceRollStartTimeKey);

            }
        }
        else
        {
            ActiveGameData.Instance.diceRollActive = false;
            // Not Tuesday or Friday, clear any previous start time
            PlayerPrefs.DeleteKey(DiceRollStartTimeKey);
        }
    }

    private IEnumerator UpdateDiceRollTimer(DateTime diceRollEndTime)
    {
        while (true)
        {
            TimeSpan timeRemaining = diceRollEndTime - DateTime.Now;

            if (timeRemaining.TotalSeconds > 0)
            {
                int totalHours = (int)Math.Floor(timeRemaining.TotalHours);
                // Format remaining time as HH:MMh
                diceRollTimerText.text = $"{totalHours:D2}H {timeRemaining.Minutes:D2}M {timeRemaining.Seconds:D2}S";
            }
            else
            {
                ActiveGameData.Instance.diceRollActive = false;
                // Time is up; hide the Gold Rush screen or reset
                diceRollTimerText.text = "00:00h";
                PlayerPrefs.DeleteKey(DiceRollStartTimeKey);
                diceRollButton.gameObject.SetActive(false);
                Debug.Log("Dice Roll Event has ended.");
                yield break; // Exit coroutine
            }

            yield return new WaitForSeconds(1f); // Wait for 1 second before updating again
        }
    }

    private IEnumerator UpdateGoldRushTimer(DateTime goldRushEndTime)
    {
        while (true)
        {
            TimeSpan timeRemaining = goldRushEndTime - DateTime.Now;

            if (timeRemaining.TotalSeconds > 0)
            {
                // Format remaining time as HH:MMh
                goldRushTimerText.text = $"{timeRemaining.Hours:D2}H {timeRemaining.Minutes:D2}M {timeRemaining.Seconds:D2}S";
            }
            else
            {
                // Time is up; hide the Gold Rush screen or reset
                goldRushTimerText.text = "00:00h";
                PlayerPrefs.DeleteKey(GoldRushStartTimeKey);
                PlayerPrefs.DeleteKey(GoldRushPurchasedKey);
                goldRushButton.gameObject.SetActive(false);
                Debug.Log("Gold Rush offer has ended.");
                yield break; // Exit coroutine
            }

            yield return new WaitForSeconds(1f); // Wait for 1 second before updating again
        }
    }

    private void OnClick_Profile()
    {
        ScreenManager.Get.GetScreen<ProfileScreen>().Show();
    }

    private async void OnClick_CollectIdleReward()
    {
        if (TutorialManager.Get.tutorialVariables.ContainsKey("ClickOnIdle") && TutorialManager.Get.tutorialVariables["ClickOnIdle"] == true)
            EventManager.TriggerEvent(EventID.TutorialNextStep);

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            ToastManager.Get.ShowMessage("An internet Connection is required to collect reward !");
            return;
        }

        SceneLoadManager.Instance.SetLoading(true);
        var data = await FetchTimeFromInternet();
        SceneLoadManager.Instance.SetLoading(false);

        TimeSpan timeDifference = data - DateTime.UtcNow;

        double absoluteTimeDifference = Math.Abs(timeDifference.TotalMinutes);

        if (absoluteTimeDifference > 5)
        {
            ToastManager.Get.ShowMessage("System time may have been modified! Please check");
            return;
        }

        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);

        ActiveGameData.Instance.saveData.lastIdleRewardDate = DateTime.Now.ToString();

        string lastRewardTimeString = ActiveGameData.Instance.saveData.lastIdleRewardDate;

        lastRewardTime = DateTime.Parse(lastRewardTimeString);

        //Constants.Get.GoldCoin += accumulatedCoins;
        //Constants.Get.Diamond += accumulatedDiamonds;
        //Constants.Get.Gems += accumulatedGems;

        int gainCoin = accumulatedCoins;
        int gainDiamond = accumulatedDiamonds;
        int gainGem = accumulatedGems;

        if (accumulatedCoins > 0)
            CoinAnimation.Get.ShowCoin(goldCoinText.transform.position, accumulatedCoins, () => {
                Constants.Get.GoldCoin += gainCoin;
                EventManager.TriggerEvent(EventID.Update_Currency);
            });
        if (accumulatedDiamonds > 0)
            CoinAnimation.Get.ShowDiamond(diamondCoinText.transform.position, accumulatedDiamonds, () => {
                Constants.Get.Diamond += gainDiamond;
                EventManager.TriggerEvent(EventID.Update_Currency);
            });
        if (accumulatedGems > 0)
            CoinAnimation.Get.ShowGem(gemCoinText.transform.position, accumulatedGems, () => { 
                Constants.Get.Gems += gainGem;
                EventManager.TriggerEvent(EventID.Update_Currency);
            });

        ActiveGameData.Instance.saveData.goldEarned[ActiveGameData.Instance.currentSelectedWorld] += accumulatedCoins;
        ActiveGameData.Instance.saveData.gemEarned[ActiveGameData.Instance.currentSelectedWorld] += accumulatedGems;

        //AppsFlyerEventManager.Get.SendEvent(AppsFlyerEventManager.AF_Idle_Reward_Claim, "1");
        GameAnalyticsManager.Instance.NewDesignEventGA("Idle_Reward_Claim");

        accumulatedCoins = 0;
        accumulatedDiamonds = 0;
        accumulatedGems = 0;

        NotificationManager.Get.ScheduleNotification_RewardComplete((int)(lastRewardTime.AddSeconds(rewardInterval) - DateTime.Now).TotalSeconds);
        timerCoroutine = StartCoroutine(UpdateTimerUI());
    }

    IEnumerator UpdateTimerUI()
    {
        while (true)
        {
            TimeSpan timeRemaining = lastRewardTime.AddSeconds(rewardInterval) - DateTime.Now;
            if (timeRemaining.TotalSeconds <= 0)
            {
                // The reward is ready to be claimed.
                timerText.text = "Ready to Collect!";


                idlegoldCoinText.text = coinsPerInterval.ToString();
                idlediamondText.text = diamondsPerInterval.ToString();
                idlegemText.text = gemsPerInterval.ToString();

                accumulatedCoins = coinsPerInterval;
                accumulatedDiamonds = diamondsPerInterval;
                accumulatedGems = gemsPerInterval;
                idleRewardSprite.gameObject.SetActive(true);
                ColorChange();
                break;
            }

            else
            {
                LeanTween.cancel(idleRewardSprite.gameObject);
                idleRewardSprite.gameObject.SetActive(false);
                // Update the timer text with the time remaining.
                timerText.text = "Full in " + timeRemaining.ToString(@"hh\:mm\:ss");

                double goneSecond = rewardInterval - timeRemaining.TotalSeconds;

                accumulatedCoins = (int)((goneSecond / rewardInterval) * coinsPerInterval);
                accumulatedDiamonds = (int)((goneSecond / rewardInterval) * diamondsPerInterval);
                accumulatedGems = (int)((goneSecond / rewardInterval) * gemsPerInterval);

                idlegoldCoinText.text = accumulatedCoins.ToString();
                idlediamondText.text = accumulatedDiamonds.ToString();
                idlegemText.text = accumulatedGems.ToString();

                yield return new WaitForSeconds(1f);
            }
        }
    }

    private void ColorChange()
    {
        // LeanTween.value allows you to interpolate between two colors over a specified duration
        LeanTween.value(idleRewardSprite.gameObject, color1, color2, 0.6f)
            .setOnUpdate((Color val) =>
            {
                // OnUpdate is called each frame with the current interpolated color value
                idleRewardSprite.color = val;
            })
            .setOnComplete(() =>
            {
                // OnComplete is called when the first color-to-color transition is complete
                // Reverse the colors and call ColorChange again for a loop
                Color temp = color1;
                color1 = color2;
                color2 = temp;
                ColorChange();
            });
    }

    public string SecondsToHHMMSS(int seconds)
    {
        int hours = seconds / 3600;
        int minutes = (seconds % 3600) / 60;
        int remainingSeconds = seconds % 60;

        return string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, remainingSeconds);
    }

    public override void Show()
    {
        base.Show();
        goldCoinText.text = Constants.Get.GoldCoin.ToString();
        diamondCoinText.text = Constants.Get.Diamond.ToString();
        gemCoinText.text = Constants.Get.Gems.ToString();
        ticketText.text = ActiveGameData.Instance.saveData.diceRollTicket.ToString();
    }

    private void Update_Currency(object arg)
    {
        ticketText.text = ActiveGameData.Instance.saveData.diceRollTicket.ToString();
        GoldCoinTextAnim(int.Parse(goldCoinText.text), Constants.Get.GoldCoin);
        GemTextAnim(int.Parse(gemCoinText.text), Constants.Get.Gems);
        //goldCoinText.text = Constants.Get.GoldCoin.ToString();
        //diamondCoinText.text = Constants.Get.Diamond.ToString();
        //gemCoinText.text = Constants.Get.Gems.ToString();
    }
    
    private void GoldCoinTextAnim(int from, int to)
    {
        LeanTween.cancel(goldCoinText.gameObject);
        LeanTween.value(goldCoinText.gameObject, (value)=> {
            goldCoinText.text = value.ToString("0");
        }, from, to, 0.5f).setOnComplete(()=> {
            goldCoinText.text = Constants.Get.GoldCoin.ToString();
        });
    }

    private void GemTextAnim(int from, int to)
    {
        LeanTween.cancel(gemCoinText.gameObject);
        LeanTween.value(gemCoinText.gameObject, (value) => {
            gemCoinText.text = value.ToString("0");
        }, from, to, 0.5f).setOnComplete(() => {
            gemCoinText.text = Constants.Get.Gems.ToString();
        });
    }

    private void OnClick_Setting()
    {
        ScreenManager.Get.GetScreen<SettingScreen>().Show();
    }

    private void OnClick_Battle()
    {
        if (TutorialManager.Get.tutorialVariables.ContainsKey("ClickOnBattle") && TutorialManager.Get.tutorialVariables["ClickOnBattle"] == true)
            EventManager.TriggerEvent(EventID.TutorialNextStep);

        int bucketedWave = (ActiveGameData.Instance.saveData.bestWave[ActiveGameData.Instance.currentSelectedWorld] / 11) * 10;

        if (ActiveGameData.Instance.currentSelectedWorld == 1 && ActiveGameData.Instance.saveData.bestWave[0] <= world2Unlock_Wave)
        {
            ToastManager.Get.ShowMessage("Complete Wave "+ world2Unlock_Wave + " to Unlock !");
            return;
        }else if (ActiveGameData.Instance.currentSelectedWorld == 2 && ActiveGameData.Instance.saveData.bestWave[0] <= world3Unlock_Wave)
        {
            ToastManager.Get.ShowMessage("Complete Wave " + world3Unlock_Wave + " to Unlock !");
            return;
        }

        if (ActiveGameData.Instance.saveData.VibrateEffect == 1)
        {
            //HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
        }
            

        if (ActiveGameData.Instance.currentSelectedWorld == 0)
            GameAnalyticsManager.Instance.NewDesignEventGA("enter_game_world_1");
        else
            GameAnalyticsManager.Instance.NewDesignEventGA("enter_game_world_2");

        if (bucketedWave > 0)
        {
            ScreenManager.Get.GetScreen<MilestonePopup>().Show();
        }
        else
        {
            Constants.Get.CurrentWaveMilestone = 1;
            SceneLoadManager.Instance.LoadScene(Scenes.Gameplay);
        }

    }

    private void OnClick_DailyReward()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            ToastManager.Get.ShowMessage("An internet Connection is required to collect reward !");
            return;
        }
        
        ScreenManager.Get.GetScreen<DailyRewardScreen>().Show();
    }

    private void OnClick_DiceRoll()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            ToastManager.Get.ShowMessage("An internet Connection is required to collect reward !");
            return;
        }

        ScreenManager.Get.GetScreen<DiceRollEventPopup>().Show();
    }

    private void OnDisable()
    {
        leftArrow.onClick.RemoveAllListeners();
        rightArrow.onClick.RemoveAllListeners();

        battleButton.onClick.RemoveAllListeners();
        dailyRewardButton.onClick.RemoveAllListeners();
        settingButton.onClick.RemoveAllListeners();
        idleRewardCollectButton.onClick.RemoveAllListeners();
        profileButton.onClick.RemoveAllListeners();
        adPassButton.onClick.RemoveAllListeners();

        EventManager.RemoveListener(EventID.Update_Currency, Update_Currency);

        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
    }

    public async Task<DateTime> FetchTimeFromInternet()
    {
        string url = "https://www.timeapi.io/api/Time/current/zone?timeZone=UTC";

        UnityWebRequest www = UnityWebRequest.Get(url);

        // Send the request asynchronously
        AsyncOperation asyncOperation = www.SendWebRequest();

        // Wait for the operation to complete
        while (!asyncOperation.isDone)
        {
            await Task.Delay(100); // You can adjust the delay as needed
        }

        if (!(www.result== UnityWebRequest.Result.ConnectionError) && !(www.result == UnityWebRequest.Result.ProtocolError))
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

[System.Serializable]
public class WorldTimeInfo
{
    public string dateTime;
    // Add other properties if needed
}