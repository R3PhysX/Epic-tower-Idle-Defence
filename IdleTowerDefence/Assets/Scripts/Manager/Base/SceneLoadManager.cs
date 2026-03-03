using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoadManager : MonoBehaviour
{
    private static SceneLoadManager _instance = null;
    [SerializeField] private TMP_Text versionText;
    public static SceneLoadManager Instance => _instance;

    //[SerializeField] private LoadingScreen m_LoadingScreen;

    private AsyncOperation m_SceneLoading;

    private float m_TotalSceneProgress = 0f;
    private string previousSceneName = "";
    internal int previousSceneIndex = 0;

    public GameObject loadingScreenPanel;
    public Image loadingBar;

    public Animator anim;
    private bool isLoading;

    [SerializeField] private GameObject loadingBlocker;

    private void Awake()
    {
        _instance = this;
        versionText.text = "Build v" + Application.version;
    }

    public void SetLoading(bool status)
    {
        loadingBlocker.SetActive(status);
    }

    private void LoadScene(int index)
    {
        if (isLoading)
            return;
        isLoading = true;
        Time.timeScale = 1f;

        if (previousSceneIndex != 0)
        {
            AudioManager.Instance?.PlaySFXSound(AudioClipsType.CloudIn);
            anim.SetTrigger("Show");

            LeanTween.delayedCall(0.95f, () =>
            {
                if (previousSceneIndex != -1)
                {
                    Debug.Log(previousSceneIndex + " Unloading");
                    SceneManager.UnloadSceneAsync(previousSceneIndex);
                }

                previousSceneIndex = index;
                m_SceneLoading = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
                StartCoroutine(LoadSceneAsync());
            });
        }
        else
        {
            previousSceneIndex = index;
            m_SceneLoading = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
            loadingScreenPanel.SetActive(true);
            StartCoroutine(LoadSceneAsyncFake());
        }
    }

    public void LoadScene(Scenes mscene)
    {
        LoadScene((int)mscene);
    }

    IEnumerator LoadSceneAsync()
    {
        EventManager.CleanUpTable();
        if (previousSceneIndex == (int)Scenes.MainMenu)
            AdManager.Get.ShowInterstitialAd();
        m_SceneLoading.allowSceneActivation = false;
        while (!m_SceneLoading.isDone)
        {
            m_TotalSceneProgress = m_SceneLoading.progress;
            loadingBar.fillAmount = m_TotalSceneProgress;
            if (m_TotalSceneProgress >= 0.9f)
            {
                loadingBar.fillAmount = 1f;
                yield return new WaitForSeconds(1f);
                m_SceneLoading.allowSceneActivation = true;

                AudioManager.Instance?.PlaySFXSound(AudioClipsType.CloudOut);
                anim.SetTrigger("Hide");
            }
            yield return null;
        }

        m_SceneLoading = null;
        isLoading = false;
    }

    IEnumerator LoadSceneAsyncFake()
    {
        EventManager.CleanUpTable();
        m_SceneLoading.allowSceneActivation = false;
        float TimePassed = 0;
        bool sceneActivated = false;
        while (TimePassed < 3f)
        {
            loadingBar.fillAmount = TimePassed / 3f;
            TimePassed += Time.deltaTime;
            if (sceneActivated == false && TimePassed > 2.15f)
            {

                sceneActivated = true;
                m_SceneLoading.allowSceneActivation = true;
            }
            yield return null;
        }

        loadingBar.fillAmount = 1f;
        loadingScreenPanel.SetActive(false);

        m_SceneLoading = null;
        isLoading = false;
    }
}
