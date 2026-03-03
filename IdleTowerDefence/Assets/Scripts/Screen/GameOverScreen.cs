using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScreen : ScreenPanel
{
    [SerializeField] private Button closeButton;
    [SerializeField] private Button claimButton;

    [SerializeField] private RectTransform bonusSlider;
    [SerializeField] private TMP_Text goldCoinText;
    [SerializeField] private TMP_Text gemText;
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text highestText;

    [SerializeField] private List<GameObject> adIcons;

    private int goldCoinCollected;

    public float startAngle = -180f;
    public float endAngle = 180f;

    public float bonus1MinRange;
    public float bonus1MaxRange;

    public float bonus2MinRange;
    public float bonus2MaxRange;

    public float bonus3MinRange;
    public float bonus3MaxRange;

    public float bonus4MinRange;
    public float bonus4MaxRange;

    public float bonus5MinRange;
    public float bonus5MaxRange;

    private (int, float) multiplier = (5, 1f);

    private float timeScale;

    private void OnEnable()
    {
        timeScale = Time.timeScale;
        Time.timeScale = 1f;
        closeButton.onClick.AddListener(OnClick_Close);
        claimButton.onClick.AddListener(OnClick_ClaimButton);

        goldCoinCollected = Constants.Get.GoldCoin - GameplayManager.Get.startingGoldCoin;
        goldCoinText.text = goldCoinCollected.ToString();

        gemText.text = (GameplayManager.Get.currentWave - 1).ToString("0");

        waveText.text = "WAVE " + GameplayManager.Get.currentWave;

        if (ActiveGameData.Instance.saveData.bestWave[ActiveGameData.Instance.currentSelectedWorld] < GameplayManager.Get.currentWave)
            ActiveGameData.Instance.saveData.bestWave[ActiveGameData.Instance.currentSelectedWorld] = GameplayManager.Get.currentWave;

        highestText.text = "BEST WAVE " + ActiveGameData.Instance.saveData.bestWave[ActiveGameData.Instance.currentSelectedWorld];

        // Set up the initial rotation of the image.
        bonusSlider.localEulerAngles = new Vector3(0, 0, startAngle);

        // Create a rotation animation using LeanTween.
        LeanTween.value(bonusSlider.gameObject, startAngle, endAngle, 1.0f)
            .setOnUpdate((float value) =>
            {
                // Update the rotation of the image during the animation.
                bonusSlider.localEulerAngles = new Vector3(0, 0, value);
                goldCoinText.text = (goldCoinCollected * getBonus().Item2).ToString("0");
            })
            .setEase(LeanTweenType.easeInOutSine) // Use a smooth easing function.
            .setLoopPingPong(-1).setIgnoreTimeScale(true);


        adIcons[0].SetActive(ActiveGameData.Instance.adFor4x);
        adIcons[1].SetActive(ActiveGameData.Instance.adFor2_4x);
       // adIcons[2].SetActive(ActiveGameData.Instance.adFor1_4x);

        if (ActiveGameData.Instance.saveData.enabled_NoAds)
        {
            adIcons[0].SetActive(false);
            adIcons[1].SetActive(false);
           // adIcons[2].SetActive(false);
        }

    }

    private void OnClick_Close()
    {
        if(SceneLoadManager.Instance != null)
            SceneLoadManager.Instance.LoadScene(Scenes.MainMenu);
        else
            SceneManager.LoadScene(0);
    }

    private void OnClick_ClaimButton()
    {
        claimButton.onClick.RemoveAllListeners();

        LeanTween.cancel(bonusSlider.gameObject);
        var bonus = getBonus();
        Debug.Log(multiplier.Item1 + " | " + multiplier.Item2);
        var gain = goldCoinCollected * multiplier.Item2;
        goldCoinText.text = gain.ToString("0");
        if ((ActiveGameData.Instance.adFor4x && multiplier.Item1 == 1) || (ActiveGameData.Instance.adFor2_4x && multiplier.Item1 == 2) || (ActiveGameData.Instance.adFor1_4x && multiplier.Item1 == 3))
        {
            float timeScale = Time.timeScale;
            AdManager.Get.ShowRewardedAd((status) =>
            {
                Time.timeScale = timeScale;
                if (status)
                {
                    ProcessReward();
                    Debug.Log("Watched Ad after death");
                    GameAnalyticsManager.Instance.NewDesignEventGA("view_ad_after_death");
                }
                else
                {
                    if (Application.internetReachability == NetworkReachability.NotReachable)
                    {
                        ToastManager.Get.ShowMessage("Internet Not Available");
                        multiplier = (5, 1f);
                    }
                    ProcessReward();
                }
            });
        }
        else
        {
            if (multiplier.Item1 == 2)
                ActiveGameData.Instance.adFor2_4x = true;
            if (multiplier.Item1 == 3)
                ActiveGameData.Instance.adFor1_4x = true;
            ProcessReward();
        }
    }

    private void ProcessReward()
    {
        GameScreen gameScreen = ScreenManager.Get.GetScreen<GameScreen>();

        var gain = goldCoinCollected * multiplier.Item2;
        goldCoinText.text = gain.ToString("0");

        EventManager.TriggerEvent(EventID.Add_GoldCoin, (int)gain - goldCoinCollected);

        int gainCoin = (int)gain - goldCoinCollected;
        int gainGem = GameplayManager.Get.currentWave - 1;

        if (gainCoin > 0)
            CoinAnimation.Get.ShowCoin(gameScreen.goldCoinText.transform.position, gainCoin, () => {
                Constants.Get.GoldCoin += gainCoin;
                EventManager.TriggerEvent(EventID.Update_Currency);
            });

        CoinAnimation.Get.ShowGem(gameScreen.gemText.transform.position, gainGem);

        Hide();
        if(gainCoin > 0 || gainGem > 0)
            LeanTween.delayedCall(3f, () => { SceneLoadManager.Instance.LoadScene(Scenes.MainMenu); });
        else
            LeanTween.delayedCall(1f, () => { SceneLoadManager.Instance.LoadScene(Scenes.MainMenu); });
    }

    private (int, float) getBonus()
    {
        (int, float) value = (5, 1f);
        float normalizedZAngle = Mathf.Repeat(bonusSlider.localEulerAngles.z, 360.0f);
        if (normalizedZAngle > 180.0f)
        {
            normalizedZAngle -= 360.0f;
        }

        if (normalizedZAngle > bonus1MinRange && normalizedZAngle < bonus1MaxRange) { value = (3, 1.4f); }
        else if (normalizedZAngle > bonus2MinRange && normalizedZAngle < bonus2MaxRange) { value = (4, 1.2f); }
        else if (normalizedZAngle > bonus4MinRange && normalizedZAngle < bonus4MaxRange) { value = (2, 2.4f); }
        else if (normalizedZAngle > bonus5MinRange && normalizedZAngle < bonus5MaxRange) { value = (1, 4); }
        else value = (5, 1f);

        if (value != multiplier)
            AudioManager.Instance?.PlaySFXSound(AudioClipsType.RewardMultiplierTick);
        multiplier = value;
        return value;
    }

    private void OnDisable()
    {
        Time.timeScale = timeScale;
        closeButton.onClick.RemoveAllListeners();
        claimButton.onClick.RemoveAllListeners();

        LeanTween.cancel(bonusSlider.gameObject);
    }
}
