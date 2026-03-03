using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MilestonePopup : ScreenPanel
{
    [SerializeField] private CustomButton milestoneButton;
    [SerializeField] private CustomButton startOverButton;
    [SerializeField] private TMP_Text milestoneButtonText;
    [SerializeField] private TMP_Text startOverButtonText;
    [SerializeField] private TMP_Text milestoneDescriptionText;

    private void OnEnable()
    {
        milestoneButton.onClick.AddListener(OnClick_Yes);
        startOverButton.onClick.AddListener(OnClick_No);
    }

    private void Start()
    {
        int bucketedWave = (ActiveGameData.Instance.saveData.bestWave[ActiveGameData.Instance.currentSelectedWorld] / 10) * 10 + 1;
        if(milestoneButtonText != null)
        {
            milestoneButtonText.text = "Start From Wave " + bucketedWave;
        }
        if(startOverButtonText != null)
        {
            startOverButtonText.text = "Reset From Wave 1";
        }
        if (milestoneDescriptionText != null)
        {
            
        }


    }

    private void OnClick_Yes()
    {
        Hide();
        Constants.Get.CurrentWaveMilestone = (ActiveGameData.Instance.saveData.bestWave[ActiveGameData.Instance.currentSelectedWorld] / 10) * 10  + 1;
        SceneLoadManager.Instance.LoadScene(Scenes.Gameplay);
    }

    private void OnClick_No()
    {
        Constants.Get.CurrentWaveMilestone = 1;
        Hide();
        SceneLoadManager.Instance.LoadScene(Scenes.Gameplay);
    }

    private void OnDisable()
    {
        milestoneButton.onClick.RemoveAllListeners();
        startOverButton.onClick.RemoveAllListeners();

    }
}
