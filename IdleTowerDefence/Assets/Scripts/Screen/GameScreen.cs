using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameScreen : ScreenPanel
{
    public CardData cardData;
    [SerializeField] private Image waveProgressBar;
    [SerializeField] private TMP_Text waveNumberText;
    [SerializeField] private CustomButton speedButton;
    [SerializeField] private CustomButton cardButton;
    [SerializeField] private TMP_Text speedText;
    [SerializeField] private TMP_Text premiumSpeedText;

    [SerializeField] private CustomButton settingButton;

    [SerializeField] internal TMP_Text silverCoinText;
    [SerializeField] internal TMP_Text goldCoinText;
    [SerializeField] internal TMP_Text diamondText;
    [SerializeField] internal TMP_Text gemText;
    [SerializeField] internal TMP_Text ticketText;

    [SerializeField] internal GameObject ticketGameObject;
    [SerializeField] private GameObject gameplayObject;

    [SerializeField] private List<Image> cards;
    [SerializeField] private Sprite emptyCard;

    [SerializeField] private GameObject speedThunderIcon;
    [SerializeField] private Image speedImage;
    [SerializeField] private Sprite simpleBG;
    [SerializeField] private Sprite premiumBG;

    private void OnEnable()
    {
        gameplayObject.gameObject.SetActive(true);
        waveNumberText.text = "Wave 1";
        waveProgressBar.fillAmount = 1f;

        ticketGameObject.SetActive(ActiveGameData.Instance.diceRollActive);

        EventManager.AddListener(EventID.Event_WaveUpdate, OnWaveUpdate);
        EventManager.AddListener(EventID.Event_WaveEnemyProgressUpdate, OnWaveProgressUpdate);
        EventManager.AddListener(EventID.Add_SilverCoin, OnAdd_Silver);
        EventManager.AddListener(EventID.Update_Currency, OnUpdate_Currency);
        EventManager.AddListener(EventID.Add_GoldCoin, OnAdd_Gold);
        EventManager.AddListener(EventID.Update_SilverCoin, OnUpdate_Silver);

        Time.timeScale = 1f;
        speedText.text = "Speed\nx1";
        speedButton.onClick.AddListener(OnClick_Speed);
        cardButton.onClick.AddListener(OnClick_Card);
        settingButton.onClick.AddListener(OnClick_Setting);

        UpdateCardIcon();
    }

    private void OnUpdate_Currency(object arg)
    {
        goldCoinText.text = Constants.Get.GoldCoin.ToString();
        diamondText.text = Constants.Get.Diamond.ToString();
        gemText.text = Constants.Get.Gems.ToString();
        ticketText.text = ActiveGameData.Instance.saveData.diceRollTicket.ToString();
    }

    private void Start()
    {
        if (Constants.Get.cardUnlockWave >= ActiveGameData.Instance.saveData.bestWave[0])
            cards.ForEach(x => x.transform.parent.gameObject.SetActive(false));
    }

    internal void UpdateCardIcon()
    {
        bool isCardSelected = false;
        for (int i = 0; i < ActiveGameData.Instance.saveData.cardSlotIds.Count; i++)
        {
            if (ActiveGameData.Instance.saveData.cardSlotIds[i] == 0 || ActiveGameData.Instance.saveData.cardSlotIds[i] == -1)
            {
                cards[i].transform.parent.gameObject.SetActive(false);
                continue;
            }

            cards[i].sprite = cardData.cards.Find(x => x.cardId == ActiveGameData.Instance.saveData.cardSlotIds[i]).cardImage;
            cards[i].transform.parent.gameObject.SetActive(true);
            isCardSelected = true;
        }

        if (isCardSelected == false)
        {
            cards[0].sprite = emptyCard;
            cards[0].transform.parent.gameObject.SetActive(true);
        }
    }

    public override void Show()
    {
        base.Show();
        goldCoinText.text = Constants.Get.GoldCoin.ToString();
        diamondText.text = Constants.Get.Diamond.ToString();
        gemText.text = Constants.Get.Gems.ToString();
        ticketText.text = ActiveGameData.Instance.saveData.diceRollTicket.ToString();
    }

    private void OnClick_Setting()
    {
        ScreenManager.Get.GetScreen<SettingScreen>().Show();
    }
    public void OnClick_Card()
    {
        ScreenManager.Get.GetScreen<CardsScreen>().Show();
    }

    private void OnClick_Speed()
    {
        if (Player.Instance.isDead)
            return;

        if (Time.timeScale == 1f)
        {
            GameAnalyticsManager.Instance.NewDesignEventGA("click_speed_increase_button : " + "x1.5");
            speedImage.sprite = simpleBG;
            Time.timeScale = 1.5f;
            speedText.text = "Speed\nx1.5";
            premiumSpeedText.text = "";
            speedThunderIcon.SetActive(false);
        }
        else if (Time.timeScale == 1.5f)
        {
            GameAnalyticsManager.Instance.NewDesignEventGA("click_speed_increase_button : " + "x2");
            speedImage.sprite = simpleBG;
            Time.timeScale = 2f;
            speedText.text = "Speed\nx2";
            premiumSpeedText.text = "";
            speedThunderIcon.SetActive(false);
        }else if (ActiveGameData.Instance.saveData.enabled_3 && (Time.timeScale == 2f))
        {
            GameAnalyticsManager.Instance.NewDesignEventGA("click_speed_increase_button : " + "x3");
            speedImage.sprite = simpleBG;
            Time.timeScale = 3f;
            speedText.text = "Speed\nx3";
            premiumSpeedText.text = "";
            speedThunderIcon.SetActive(false);
        }
        else if (ActiveGameData.Instance.saveData.enabled_3_5 && (Time.timeScale == 2f || Time.timeScale == 3f))
        {
            GameAnalyticsManager.Instance.NewDesignEventGA("click_speed_increase_button : " + "x3.5");
            speedImage.sprite = premiumBG;
            Time.timeScale = 3.5f;
            speedText.text = "";
            premiumSpeedText.text = "3.5x";
            speedThunderIcon.SetActive(true);
        }
        else if (ActiveGameData.Instance.saveData.enabled_5 && (Time.timeScale == 3f || Time.timeScale == 3.5f || Time.timeScale == 2f))
        {
            GameAnalyticsManager.Instance.NewDesignEventGA("click_speed_increase_button : " + "x5");
            speedImage.sprite = premiumBG;
            Time.timeScale = 5;
            speedText.text = "";
            premiumSpeedText.text = "5x";
            speedThunderIcon.SetActive(true);
        }
#if UNITY_EDITOR
        else if (Time.timeScale != 20)
        {
            speedImage.sprite = simpleBG;
            Time.timeScale = 20f;
            speedText.text = "Speed\nx20";
            premiumSpeedText.text = "";
            speedThunderIcon.SetActive(false);
        }
#endif
        else
        {
            ResetTo1x();
        }
    }

    public void ResetTo1x()
    {
        speedImage.sprite = simpleBG;
        Time.timeScale = 1f;
        speedText.text = "Speed\nx1";
        premiumSpeedText.text = "";
        speedThunderIcon.SetActive(false);
    }

    private void OnWaveUpdate(object arg)
    {
        int waveNum = (int)arg;
        waveNumberText.text = "Wave " + waveNum;
    }

    private void OnWaveProgressUpdate(object arg)
    {
        float progress = (float)arg;
        waveProgressBar.fillAmount = progress;
    }

    private void OnAdd_Silver(object arg)
    {
        int coinToAdd = (int)arg;
        Constants.Get.SilverCoin += coinToAdd;

        silverCoinText.text = Constants.Get.SilverCoin.ToString();
    }


    private void OnUpdate_Silver(object arg)
    {
        int coinToUpdate = (int)arg;
        Constants.Get.SilverCoin = coinToUpdate;

        silverCoinText.text = Constants.Get.SilverCoin.ToString();
    }

    private void OnAdd_Gold(object arg)
    {
        int coinToAdd = (int)arg;
        Constants.Get.GoldCoin += coinToAdd;

        goldCoinText.text = Constants.Get.GoldCoin.ToString();
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;

        EventManager.RemoveListener(EventID.Event_WaveUpdate, OnWaveUpdate);
        EventManager.RemoveListener(EventID.Event_WaveEnemyProgressUpdate, OnWaveProgressUpdate);
        EventManager.RemoveListener(EventID.Add_SilverCoin, OnAdd_Silver);
        EventManager.RemoveListener(EventID.Update_Currency, OnUpdate_Currency);
        EventManager.RemoveListener(EventID.Add_GoldCoin, OnAdd_Gold);
        EventManager.RemoveListener(EventID.Update_SilverCoin, OnUpdate_Silver);

        //gameplayObject?.gameObject?.SetActive(false);
        speedButton?.onClick.RemoveAllListeners();
        settingButton?.onClick.RemoveAllListeners();
        cardButton?.onClick.RemoveAllListeners();   
    }
}
