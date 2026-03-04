using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Get;

    [SerializeField] private List<GameObject> enemyManager;
    [SerializeField] private List<GameObject> world_background;
    [SerializeField] private int initialSilverCoin = 50;
    [SerializeField] private TMP_Text fpsText;
    private float updateInterval;

    [SerializeField] private TMP_Text waveDoneText;
    [SerializeField] private TMP_Text waveDoneGoldText;
    [SerializeField] private TMP_Text waveDoneGemsText;

    [SerializeField] private Image silverCoinImage;
    [SerializeField] private TMP_Text silverCoinText;

    [SerializeField] private Image goldCoinImage;
    [SerializeField] private TMP_Text goldCoinText;

    [SerializeField] private TMP_Text critText;
    [SerializeField] private TMP_Text stunText;
    [SerializeField] private TMP_Text deadHitText;
    [SerializeField] private TMP_Text dodgeText;
    [SerializeField] private TMP_Text killText;

    [SerializeField] private GameObject bossWarningObject;

    [SerializeField] internal GameObject gameplayPanel;
    [SerializeField] private GameObject attributePanelTop;
    [SerializeField] private GameObject attributePanelBottom;
    [SerializeField] internal BossRewardScreen bossRewardScreen;

    internal System.Random randomGenerator = new System.Random();

    internal int startingGoldCoin;

    internal int currentWave = Constants.Get.CurrentWaveMilestone;

    public Camera mainCam;
    public CanvasGroup canvasGroup;

    internal bool isGameplayStarted;

    private float timeGone;

    private void Awake()
    {
        Get = this;
        for (int i = 0; i < world_background.Count; i++)
        {
            enemyManager[i].gameObject.SetActive(i == ActiveGameData.Instance.currentSelectedWorld);
            world_background[i].gameObject.SetActive(i == ActiveGameData.Instance.currentSelectedWorld);
        }

        if(ActiveGameData.Instance.saveData.enabled_BannerAd)
        {
            Debug.Log("Showing Banner as it is active!");
            if (canvasGroup.TryGetComponent<BannerToggle>(out BannerToggle component))
            {
                component.ShowBanner();
            }
        }
        else
        {
            Debug.Log("Not Showing Banner as it is not active!");
            if (canvasGroup.TryGetComponent<BannerToggle>(out BannerToggle component))
            {
                component.HideBanner();
            }
        }

        if (currentWave >= 1) {

            if (ActiveGameData.Instance.currentSelectedWorld == 0)
            {
                initialSilverCoin += (currentWave * (42 + Random.Range(1,5))) + (currentWave * Constants.Get.SilverCoinWaveBonus) + currentWave;
            }
            else if(ActiveGameData.Instance.currentSelectedWorld == 1)
            {
                initialSilverCoin += (currentWave * (48 + Random.Range(1, 5)) * 2) + (currentWave * Constants.Get.SilverCoinWaveBonus) + (currentWave * 2);
            }
            else if(ActiveGameData.Instance.currentSelectedWorld == 2)
            {
                initialSilverCoin += (currentWave * (56 + Random.Range(1, 5)) * 3) + (currentWave * Constants.Get.SilverCoinWaveBonus) + (currentWave * 3);
            }
        }
    }

    internal void IncrementWave()
    {

        if (ActiveGameData.Instance.saveData.currentSelectedWorld == 2)
        {
            Constants.Get.Gems += 5;
            ActiveGameData.Instance.saveData.gemEarned[ActiveGameData.Instance.currentSelectedWorld] += 5;
        }
        else
        {
            Constants.Get.Gems += 2;
            ActiveGameData.Instance.saveData.gemEarned[ActiveGameData.Instance.currentSelectedWorld] += 2;
        }

        GameplayManager.Get.currentWave++;
        if (ActiveGameData.Instance.saveData.bestWave[ActiveGameData.Instance.currentSelectedWorld] < GameplayManager.Get.currentWave)
        {
            if (ActiveGameData.Instance.saveData.bestWave[ActiveGameData.Instance.currentSelectedWorld] < 30 && GameplayManager.Get.currentWave >= 30)
            {
                // AppsFlyerEventManager.Get.SendEvent(AppsFlyerEventManager.AF_Reach_30_Wave, "1");
                GameAnalyticsManager.Instance.NewDesignEventGA("Reach_30_wave");
            }
            if (ActiveGameData.Instance.saveData.bestWave[ActiveGameData.Instance.currentSelectedWorld] < 100 && GameplayManager.Get.currentWave >= 100)
            {
               if(ActiveGameData.Instance.currentSelectedWorld == 0)
                    GameAnalyticsManager.Instance.NewDesignEventGA("Reach_100_wave_world_1");
                //AppsFlyerEventManager.Get.SendEvent(AppsFlyerEventManager.AF_Reach_100_World_1, "1");
                else
                    GameAnalyticsManager.Instance.NewDesignEventGA("Reach_100_wave_world_2");
                //AppsFlyerEventManager.Get.SendEvent(AppsFlyerEventManager.AF_Reach_100_World_2, "1");
            }
            ActiveGameData.Instance.saveData.bestWave[ActiveGameData.Instance.currentSelectedWorld] = GameplayManager.Get.currentWave;
        }

        ScreenManager.Get.GetScreen<GameScreen>().gemText.text = Constants.Get.Gems.ToString();
        LeanTween.delayedCall(0.1f, () =>
        {
            Vector2 randomCirclePoint = (Random.insideUnitCircle.normalized * 1f);
            Vector3 spawnPosition = Player.Instance.transform.position + new Vector3(randomCirclePoint.x, randomCirclePoint.y, 0f);
            
            if (ActiveGameData.Instance.saveData.currentSelectedWorld == 2)
                ShowWaveDone(waveDoneText, spawnPosition, 5);
            else
                ShowWaveDone(waveDoneText, spawnPosition, 2);

        });
        LeanTween.delayedCall(0.3f, () =>
        {
            Vector2 randomCirclePoint = (Random.insideUnitCircle.normalized * 1f);
            Vector3 spawnPosition = Player.Instance.transform.position + new Vector3(randomCirclePoint.x, randomCirclePoint.y, 0f);

            ShowWaveDone(waveDoneGemsText, spawnPosition, 2);

        });
        if (Constants.Get.GoldCoinWaveBonus > 0)
        {
            LeanTween.delayedCall(0.5f, () =>
            {
                Vector2 randomCirclePoint = (Random.insideUnitCircle.normalized * 1f);
                Vector3 spawnPosition = Player.Instance.transform.position + new Vector3(randomCirclePoint.x, randomCirclePoint.y, 0f);
                ShowWaveDone(waveDoneGoldText, spawnPosition, Constants.Get.GoldCoinWaveBonus);
            });
        }

    }

    [ContextMenu("Test")]
    private void SetGamePanelPosition()
    {
        // Calculate the middle position between the top of the screen and the UI object
        float middlePositionY = (attributePanelTop.transform.position.y + attributePanelBottom.transform.position.y) / 2f;

        Vector3 middle = new Vector3(0, middlePositionY, 0);

        gameplayPanel.transform.position = new Vector3(0, middle.y, 0);
    }

    private void Start()
    {

        timeGone = Time.time;
        AudioManager.Instance?.PlaySFXSound(AudioClipsType.BattleStart);

        LeanTween.delayedCall(0.3f, () =>
        {
            SetGamePanelPosition();
            mainCam.orthographicSize = 6.5f;
        });

        canvasGroup.alpha = 0.01f;

        LeanTween.delayedCall(0.5f, () =>
        {
            LeanTween.value(gameObject, (val) =>
{
    mainCam.orthographicSize = val;
}, 6.5f, 5f, 1f).setEase(LeanTweenType.easeOutSine)
.setOnComplete(() => mainCam.orthographicSize = 5);
        });

        LeanTween.delayedCall(1f, () =>
        {
            LeanTween.value(gameObject, (val) =>
            {
                canvasGroup.alpha = val;
            }, 0, 1f, 0.5f).setEase(LeanTweenType.easeOutSine)
.setOnComplete(() =>
{
    canvasGroup.alpha = 1;
    isGameplayStarted = true;
});
        });

        Constants.Get.SilverCoin = initialSilverCoin;
        startingGoldCoin = Constants.Get.GoldCoin;

        ScreenManager.Get.GetScreen<GameScreen>().silverCoinText.text = Constants.Get.SilverCoin.ToString();
        ScreenManager.Get.GetScreen<GameScreen>().Show();
        ScreenManager.Get.GetScreen<AttributeScreen>().Show();

        ScreenManager.Get.GetScreen<CardsScreen>().Initialize();

        ActiveGameData.Instance.saveData.battleTimes[ActiveGameData.Instance.currentSelectedWorld] += 1;
    }

    internal void ShowSilverCoinGain(Vector3 position, int count)
    {
        var obj2 = GameObject.Instantiate(silverCoinImage.gameObject, silverCoinImage.transform.parent);
        obj2.transform.position = position;
        obj2.gameObject.SetActive(true);

        var obj = GameObject.Instantiate(silverCoinText.gameObject, silverCoinText.transform.parent).GetComponent<TMP_Text>();
        obj.transform.position = position;
        obj.text = "+" + count;
        obj.gameObject.transform.localScale = Vector3.zero;
        obj.gameObject.SetActive(true);

        LeanTween.scale(obj.gameObject, Vector3.one, 0.2f).setOnComplete(() =>
        {
            LeanTween.moveY(obj.gameObject, obj.transform.position.y + 0.25f, 1f);
        });


        LeanTween.delayedCall(1f, () =>
        {
            LeanTween.scale(obj.gameObject, Vector3.zero, 0.2f).setOnComplete(() =>
            {
                if (obj.gameObject != null) Destroy(obj.gameObject);
            });
            if (obj2.gameObject != null) Destroy(obj2.gameObject);
        });
    }

    internal void ShowGoldCoinGain(Vector3 position, int count)
    {
        var obj2 = GameObject.Instantiate(goldCoinImage.gameObject, goldCoinImage.transform.parent);
        obj2.transform.position = position;
        obj2.gameObject.SetActive(true);

        var obj = GameObject.Instantiate(goldCoinText.gameObject, goldCoinText.transform.parent).GetComponent<TMP_Text>();
        obj.transform.position = position;
        obj.text = "+" + count;
        obj.gameObject.transform.localScale = Vector3.zero;
        obj.gameObject.SetActive(true);

        LeanTween.scale(obj.gameObject, Vector3.one, 0.2f).setOnComplete(() =>
        {
            LeanTween.moveY(obj.gameObject, obj.transform.position.y + 0.25f, 1f);
        });

        LeanTween.delayedCall(1f, () =>
        {
            LeanTween.scale(obj.gameObject, Vector3.zero, 0.2f).setOnComplete(() =>
            {
                if (obj.gameObject != null) Destroy(obj.gameObject);
            });
            if (obj2.gameObject != null) Destroy(obj2.gameObject);
        });
    }

    internal void ShowCritDamage(Vector3 position)
    {
        var obj = GameObject.Instantiate(critText.gameObject, critText.transform.parent).GetComponent<TMP_Text>();
        obj.transform.position = position;
        obj.gameObject.transform.localScale = Vector3.zero;
        obj.gameObject.SetActive(true);

        LeanTween.scale(obj.gameObject, Vector3.one, 0.2f).setOnComplete(() =>
        {
            LeanTween.moveY(obj.gameObject, obj.transform.position.y + 0.25f, 1f);
        });
        LeanTween.delayedCall(1f, () =>
        {
            LeanTween.scale(obj.gameObject, Vector3.zero, 0.2f).setOnComplete(() =>
            {
                if (obj.gameObject != null) Destroy(obj.gameObject);
            });
        });
    }

    internal void ShowStunDamage(Vector3 position)
    {
        var obj = GameObject.Instantiate(stunText.gameObject, stunText.transform.parent).GetComponent<TMP_Text>();
        obj.transform.position = position;
        obj.gameObject.transform.localScale = Vector3.zero;
        obj.gameObject.SetActive(true);

        LeanTween.scale(obj.gameObject, Vector3.one, 0.2f).setOnComplete(() =>
        {
            LeanTween.moveY(obj.gameObject, obj.transform.position.y + 0.25f, 1f);
        });
        LeanTween.delayedCall(1f, () =>
        {
            LeanTween.scale(obj.gameObject, Vector3.zero, 0.2f).setOnComplete(() =>
            {
                if (obj.gameObject != null) Destroy(obj.gameObject);
            });
        });
    }

    internal void ShowDeadHitDamage(Vector3 position)
    {
        var obj = GameObject.Instantiate(deadHitText.gameObject, deadHitText.transform.parent).GetComponent<TMP_Text>();
        obj.transform.position = position;
        obj.gameObject.transform.localScale = Vector3.zero;
        obj.gameObject.SetActive(true);

        LeanTween.scale(obj.gameObject, Vector3.one, 0.2f).setOnComplete(() =>
        {
            LeanTween.moveY(obj.gameObject, obj.transform.position.y + 0.25f, 1f);
        });

        LeanTween.delayedCall(1f, () =>
        {
            LeanTween.scale(obj.gameObject, Vector3.zero, 0.2f).setOnComplete(() =>
            {
                if (obj.gameObject != null) Destroy(obj.gameObject);
            });
        });
    }

    internal void ShowDodge(Vector3 position)
    {
        var obj = GameObject.Instantiate(dodgeText.gameObject, dodgeText.transform.parent).GetComponent<TMP_Text>();
        obj.transform.position = position;
        obj.gameObject.transform.localScale = Vector3.zero;
        obj.gameObject.SetActive(true);

        LeanTween.scale(obj.gameObject, Vector3.one, 0.2f).setOnComplete(() =>
        {
            LeanTween.moveY(obj.gameObject, obj.transform.position.y + 0.25f, 1f);
        });
        LeanTween.delayedCall(1f, () =>
        {
            LeanTween.scale(obj.gameObject, Vector3.zero, 0.2f).setOnComplete(() =>
            {
                if (obj.gameObject != null) Destroy(obj.gameObject);
            });
        });
    }

    internal void ShowKill(Vector3 position)
    {
        var obj = GameObject.Instantiate(killText.gameObject, killText.transform.parent).GetComponent<TMP_Text>();
        obj.transform.position = position;
        obj.gameObject.transform.localScale = Vector3.zero;
        obj.gameObject.SetActive(true);

        LeanTween.scale(obj.gameObject, Vector3.one, 0.2f).setOnComplete(() =>
        {
            LeanTween.moveY(obj.gameObject, obj.transform.position.y + 0.25f, 1f);
        });
        LeanTween.delayedCall(1f, () =>
        {
            LeanTween.scale(obj.gameObject, Vector3.zero, 0.2f).setOnComplete(() =>
            {
                if (obj.gameObject != null) Destroy(obj.gameObject);
            });
        });
    }

    internal void ShowWaveDone(TMP_Text text, Vector3 position, int gain)
    {
        var obj = GameObject.Instantiate(text.gameObject, text.transform.parent).GetComponent<TMP_Text>();
        obj.transform.position = position;
        obj.gameObject.transform.localScale = Vector3.zero;
        obj.text = "Wave Done +" + gain;
        obj.gameObject.SetActive(true);

        LeanTween.scale(obj.gameObject, Vector3.one, 0.2f).setOnComplete(() =>
        {
            LeanTween.moveY(obj.gameObject, obj.transform.position.y + 0.25f, 2f);
        });
        LeanTween.delayedCall(2f, () =>
        {
            LeanTween.scale(obj.gameObject, Vector3.zero, 0.2f).setOnComplete(() =>
            {
                if (obj.gameObject != null) Destroy(obj.gameObject);
            });
        });
    }

    internal void ShowBoss()
    {
        var obj = GameObject.Instantiate(bossWarningObject.gameObject, bossWarningObject.transform.parent);
        obj.gameObject.transform.localScale = Vector3.zero;
        obj.gameObject.SetActive(true);

        LeanTween.scale(obj.gameObject, Vector3.one, 0.2f);

        LeanTween.delayedCall(2f, () =>
        {
            LeanTween.scale(obj.gameObject, Vector3.zero, 0.2f).setOnComplete(() =>
            {
                if (obj.gameObject != null) Destroy(obj.gameObject);
            });
        });
    }

    private void OnDisable()
    {
        ActiveGameData.Instance.saveData.totalPlayedTime[ActiveGameData.Instance.currentSelectedWorld] += timeGone;
        if (ActiveGameData.Instance.saveData.totalPlayedTime[ActiveGameData.Instance.currentSelectedWorld] >= 86400)
        {
            ActiveGameData.Instance.saveData.totalPlayedTimeDay[ActiveGameData.Instance.currentSelectedWorld] += 1;
            ActiveGameData.Instance.saveData.totalPlayedTime[ActiveGameData.Instance.currentSelectedWorld] -= 86400;
        }

        LeanTween.cancelAll();
        var goldEarned = Constants.Get.GoldCoin - GameplayManager.Get.startingGoldCoin;
        ActiveGameData.Instance.saveData.goldEarned[ActiveGameData.Instance.currentSelectedWorld] += goldEarned;

        if (goldEarned > ActiveGameData.Instance.saveData.bestBattleGold[ActiveGameData.Instance.currentSelectedWorld])
            ActiveGameData.Instance.saveData.bestBattleGold[ActiveGameData.Instance.currentSelectedWorld] = goldEarned;
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus && Player.Instance.isDead == false && isGameplayStarted == true)
        {
            if (ScreenManager.Get.GetScreen<GameOverScreen>().isActiveAndEnabled == false && ScreenManager.Get.GetScreen<CardsScreen>().isActiveAndEnabled == false && ScreenManager.Get.GetScreen<ReviveScreen>().isActiveAndEnabled == false && TutorialManager.Get != null && TutorialManager.Get.isTutorialShowing==false && bossRewardScreen.gameObject.activeSelf == false && ScreenManager.Get.GetScreen<GemRewarderPopup>().isActiveAndEnabled == false)
                ScreenManager.Get.GetScreen<SettingScreen>().Show();
        }
    }
}