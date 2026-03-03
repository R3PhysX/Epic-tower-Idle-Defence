using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingScreen : ScreenPanel
{
    public bool isGameplay;
    [SerializeField] private TMP_Text versionText;
    [SerializeField] private CustomButton closeButton;

    [SerializeField] private CustomButton button_EndRound;
    [SerializeField] private CustomButton button_Resume;
    [SerializeField] private CustomButton button_privacy;
    [SerializeField] private CustomButton button_tnc;

    [SerializeField] private Toggle musicToggle;
    [SerializeField] private GameObject musicOnObject;
    [SerializeField] private GameObject musicOffObject;

    [SerializeField] private Toggle soundToggle;
    [SerializeField] private GameObject soundOnObject;
    [SerializeField] private GameObject soundOffObject;

    [SerializeField] private Toggle vibrateToggle;
    [SerializeField] private GameObject vibrateOnObject;
    [SerializeField] private GameObject vibrateOffObject;

    [SerializeField] private Toggle visualEffectToggle;
    [SerializeField] private GameObject visualEffectOnObject;
    [SerializeField] private GameObject visualEffectOffObject;

    private float timeScale = 1f;

    private void OnEnable()
    {
        if (versionText != null)
            versionText.text = "Build v" + Application.version;

        timeScale = Time.timeScale;
        if (isGameplay)
        {
            LeanTween.delayedCall(0.3f, () => { Time.timeScale = 0; });
        }

        closeButton.onClick.AddListener(OnClick_Close);

        musicToggle.onValueChanged.AddListener(OnValueChange_Music);
        soundToggle.onValueChanged.AddListener(OnValueChange_Sound);
        vibrateToggle.onValueChanged.AddListener(OnValueChange_Vibrate);
        visualEffectToggle.onValueChanged.AddListener(OnValueChange_VisualEffect);

        button_EndRound?.onClick.AddListener(OnClick_EndRound);
        button_Resume?.onClick.AddListener(OnClick_Resume);

        button_privacy?.onClick.AddListener(OnClick_Privacy);
        button_tnc?.onClick.AddListener(OnClick_Tnc);

        musicToggle.isOn = ActiveGameData.Instance.saveData.MusicEffect == 1;
        soundToggle.isOn = ActiveGameData.Instance.saveData.SoundEffect == 1;
        vibrateToggle.isOn = ActiveGameData.Instance.saveData.VibrateEffect == 1;
        visualEffectToggle.isOn = ActiveGameData.Instance.saveData.VisualEffect == 1;

    }

    private void OnClick_EndRound()
    {
        Hide();
        Player.Instance.isDead = true;
        ScreenManager.Get.GetScreen<GameOverScreen>().Show();
    }

    private void OnClick_Resume()
    {
        OnClick_Close();
    }

    private void OnClick_Privacy()
    {

    }

    private void OnClick_Tnc()
    {

    }

    private void OnClick_Close()
    {
        Hide();
    }

    private void OnValueChange_Music(bool arg0)
    {
        musicOnObject.gameObject.SetActive(musicToggle.isOn);
        musicOffObject.gameObject.SetActive(!musicToggle.isOn);
        ActiveGameData.Instance.saveData.MusicEffect = musicToggle.isOn ? 1 : 0;

        if (musicToggle.isOn && SceneLoadManager.Instance.previousSceneIndex == 1)
            AudioManager.Instance?.PlayBackGroundMusic(AudioClipsType.BGM);
        else
            AudioManager.Instance?.StopBackGroundMusic();
    }

    private void OnValueChange_Sound(bool arg0)
    {
        soundOnObject.gameObject.SetActive(soundToggle.isOn);
        soundOffObject.gameObject.SetActive(!soundToggle.isOn);
        ActiveGameData.Instance.saveData.SoundEffect = soundToggle.isOn ? 1 : 0;
    }

    private void OnValueChange_Vibrate(bool arg0)
    {
        vibrateOnObject.gameObject.SetActive(vibrateToggle.isOn);
        vibrateOffObject.gameObject.SetActive(!vibrateToggle.isOn);
        ActiveGameData.Instance.saveData.VibrateEffect = vibrateToggle.isOn ? 1 : 0;
    }

    private void OnValueChange_VisualEffect(bool arg0)
    {
        visualEffectOnObject.gameObject.SetActive(visualEffectToggle.isOn);
        visualEffectOffObject.gameObject.SetActive(!visualEffectToggle.isOn);
        ActiveGameData.Instance.saveData.VisualEffect = visualEffectToggle.isOn ? 1 : 0;
    }

    private void OnDisable()
    {
        if (isGameplay)
        {
            Time.timeScale = timeScale;
        }

        closeButton.onClick.RemoveAllListeners();

        musicToggle.onValueChanged.RemoveAllListeners();
        soundToggle.onValueChanged.RemoveAllListeners();
        vibrateToggle.onValueChanged.RemoveAllListeners();
        visualEffectToggle.onValueChanged.RemoveAllListeners();

        button_EndRound?.onClick.RemoveAllListeners();
        button_Resume?.onClick.RemoveAllListeners();
        button_privacy?.onClick.RemoveAllListeners();
        button_tnc?.onClick.RemoveAllListeners();
    }

    public void OpenDiscord()
    {
        Application.OpenURL("https://discord.gg/jBc7sZMKNW");
    }
    public void OpenFirepathWebsite()
    {
        Application.OpenURL("https://www.firepathgames.com/");
    }
    public void OpenTermAndConditions()
    {
        Application.OpenURL("https://firepathgames.com/terms-of-service");
    }
    public void OpenPrivacyAndPolicy()
    {
        Application.OpenURL("https://firepathgames.com/privacy-policy");
    }
}
