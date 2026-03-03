using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private void Awake()
    {
        EventManager.CleanUpTable();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (ScreenManager.Get.GetScreen<DailyRewardScreen>().isActiveAndEnabled == false && ScreenManager.Get.GetScreen<SettingScreen>().isActiveAndEnabled == false)
            {
                ScreenManager.Get.GetScreen<QuitPopup>().Show();
            }
        }    
    }

    private void OnEnable()
    {
        Time.timeScale = 1f;
        AudioManager.Instance?.PlayBackGroundMusic(AudioClipsType.BGM);
    }

    private void OnDisable()
    {
        AudioManager.Instance?.StopBackGroundMusic();
    }

    private void Start()
    {
        SceneLoadManager.Instance.SetLoading(true);

        LeanTween.delayedCall(1f, () =>
        {
            if (ActiveGameData.Instance.saveData.MainMenuTutorial == 0)
                TutorialManager.Get.ShowMainMenuStep();

            if (ActiveGameData.Instance.saveData.bestWave[ActiveGameData.Instance.currentSelectedWorld] >= Constants.Get.cardUnlockWave && ActiveGameData.Instance.saveData.CardTutorial == 0)
                TutorialManager.Get.ShowCardStep();
            else if (ActiveGameData.Instance.saveData.bestWave[0] >= 10 && ActiveGameData.Instance.saveData.reviewOn10thWave == 0 && ActiveGameData.Instance.saveData.reviewGiven == 0)
            {
                ActiveGameData.Instance.saveData.reviewOn10thWave = 1;
                ScreenManager.Get.GetScreen<ReviewPopup>().Show();
            }
            else if (ActiveGameData.Instance.saveData.bestWave[1] >= 2 && ActiveGameData.Instance.saveData.reviewOnWorld2 == 0 && ActiveGameData.Instance.saveData.reviewGiven == 0)
            {
                ActiveGameData.Instance.saveData.reviewOnWorld2 = 1;
                ScreenManager.Get.GetScreen<ReviewPopup>().Show();
            }
        });

        //   AppsFlyerEventManager.Get.SendEvent(AppsFlyerEventManager.AF_App_Open, "1");
        GameAnalyticsManager.Instance.NewDesignEventGA("click_start_game");

    }
}