using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class TutorialData
{
    public string textMessage;
    public bool isTutorialStep;
    public string variableString;
    public string pointerLocationObject;
}

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Get;
    public GameObject panel;
    public GameObject handPointer;

    public List<TutorialData> introTutorialSteps;

    public List<TutorialData> mainMenuSteps;
    public List<TutorialData> cardSteps;

    private List<TutorialData> tutorialStps;

    public TMP_Text tutorialText;
    
    private int currentIndex = -1;
    private bool isTyping = false;

    private float typeSpeed = 0.075f;

    internal bool isTutorialShowing;
    private bool waitingForCallback;

    public Dictionary<string, bool> tutorialVariables = new Dictionary<string, bool>();

    public GameObject rayCastPanel;
    public GameObject blocker;

    private void Awake()
    {
        Get = this;
        tutorialVariables.Add("ClickOnIdle", false);
        tutorialVariables.Add("ClickOnFactory", false);
        tutorialVariables.Add("ClickOnUpgradeAttribute", false);
        tutorialVariables.Add("ClickOnHome", false);
        tutorialVariables.Add("ClickOnBattle", false);

        LeanTween.delayedCall(1.1f, () =>
        {
            if (isTutorialShowing == false)
                blocker.gameObject.SetActive(false);
        });
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventID.TutorialNextStep, OnTutorialNextStep);    
    }

    private void OnTutorialNextStep(object arg)
    {
        if (isTutorialShowing)
        {
            waitingForCallback = false;
            tutorialVariables[tutorialStps[currentIndex].variableString] = false;
            ShowNextMessage(true);
        }
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(EventID.TutorialNextStep, OnTutorialNextStep);
    }

    internal void ShowIntroductionStep()
    {
        isTutorialShowing = true;
        blocker.gameObject.SetActive(true);
        Time.timeScale = 0;
        currentIndex = -1;
        tutorialStps = introTutorialSteps;
        ShowNextMessage();
        ActiveGameData.Instance.saveData.introTutorial = 1;
    }

    internal void ShowMainMenuStep()
    {
        isTutorialShowing = true;
        blocker.gameObject.SetActive(true);
        currentIndex = -1;
        tutorialStps = mainMenuSteps;
        ShowNextMessage();
        ActiveGameData.Instance.saveData.MainMenuTutorial = 1;
    }

    internal void ShowCardStep()
    {
        isTutorialShowing = true;
        blocker.gameObject.SetActive(true);
        currentIndex = -1;
        tutorialStps = cardSteps;
        ShowNextMessage();
        ActiveGameData.Instance.saveData.CardTutorial = 1;
    }

    private void Update()
    {
        // Check for user interaction (e.g., tap)
        if (isTutorialShowing && Input.GetMouseButtonDown(0))
        {
            // If typing, skip to the end of the text
            if (isTyping)
            {
                StopAllCoroutines();
                if(tutorialStps[currentIndex].isTutorialStep==false)
                    tutorialText.text = tutorialStps[currentIndex].textMessage;
                isTyping = false;
            }
            else
            {
                // Show the next message
                ShowNextMessage();
            }
        }
    }

    private void ShowNextMessage(bool force = false)
    {
        
        if (waitingForCallback && force==false)
            return;

        blocker.gameObject.SetActive(true);
        rayCastPanel.gameObject.SetActive(false);
        panel.gameObject.SetActive(true);
        handPointer.gameObject.SetActive(false);

        if (currentIndex >= tutorialStps.Count-1)
        {
            // All messages shown, you can handle completion or loop back to the beginning
            Debug.Log("Tutorial completed!");
            panel.gameObject.SetActive(false);
            blocker.gameObject.SetActive(false);
            isTutorialShowing = false;
            Time.timeScale = 1f;
            return;
        }

        if (tutorialStps[currentIndex + 1].isTutorialStep == false)
        {
            currentIndex++;
            if (currentIndex < tutorialStps.Count)
            {
                // Start typing animation
                StartCoroutine(TypeText(tutorialStps[currentIndex].textMessage));
            }
        }
        else if (tutorialStps[currentIndex + 1].isTutorialStep == true)
        {
            panel.gameObject.SetActive(false);
            currentIndex++;
            waitingForCallback = true;
            tutorialVariables[tutorialStps[currentIndex].variableString] = true;

            LeanTween.delayedCall(0.6f, () =>
            {
                var obj = GameObject.Find(tutorialStps[currentIndex].pointerLocationObject);
                if (obj != null)
                {
                    handPointer.transform.position = obj.transform.position;
                    handPointer.gameObject.SetActive(true);
                    rayCastPanel.gameObject.SetActive(true);
                    blocker.gameObject.SetActive(false);
                    rayCastPanel.transform.position = obj.transform.position;
                }
            }).setIgnoreTimeScale(true);
        }
        else
        {
            panel.gameObject.SetActive(false);
            blocker.gameObject.SetActive(false);
        }
    }
    
    private IEnumerator TypeText(string message)
    {
        isTyping = true;
        tutorialText.text = "";

        foreach (char letter in message.ToCharArray())
        {
            tutorialText.text += letter;
            yield return new WaitForSecondsRealtime(typeSpeed);
        }

        isTyping = false;
    }
}
