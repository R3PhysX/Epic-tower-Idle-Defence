using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitPopup : ScreenPanel
{
    [SerializeField] private CustomButton yesButton;
    [SerializeField] private CustomButton noButton;

    private void OnEnable()
    {
        yesButton.onClick.AddListener(OnClick_Yes);
        noButton.onClick.AddListener(OnClick_No);
    }

    private void OnClick_Yes()
    {
        Hide();
        Application.Quit();
    }

    private void OnClick_No()
    {
        Hide();
    }

    private void OnDisable()
    {
        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();

    }
}
