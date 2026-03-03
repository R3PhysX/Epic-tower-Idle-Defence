using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DiceRollEventPopup : ScreenPanel
{
    public TMP_Text diceRollTimerText;
    public TMP_Text ticketText;

    private string DiceRollStartTimeKey = "ITD_DiceRollEventTime";

    private Coroutine UpdateDiceRollTimerRoutine;

    private void OnEnable()
    {
        ticketText.text = ActiveGameData.Instance.saveData.diceRollTicket.ToString();
        StopAllCoroutines();
        CheckForDiceRollEvent();
    }

    internal void CheckForDiceRollEvent()
    {
        DateTime currentTime = DateTime.Now;

        // Parse the start time from PlayerPrefs
        DateTime diceRollStartTime = DateTime.Parse(PlayerPrefs.GetString(DiceRollStartTimeKey));
        DateTime diceRollEndTime = diceRollStartTime.AddHours(72);

        // Check if current time is within 72 hours from the start time
        if (currentTime <= diceRollEndTime)
        {
            UpdateDiceRollTimerRoutine = StartCoroutine(UpdateDiceRollTimer(diceRollEndTime));
        }
        else
        {
            ActiveGameData.Instance.diceRollActive = false;
            // Clear start time as the offer period has expired
            PlayerPrefs.DeleteKey(DiceRollStartTimeKey);
            Hide();
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
                // Time is up; hide the Dice Roll screen or reset
                diceRollTimerText.text = "00:00h";
                PlayerPrefs.DeleteKey(DiceRollStartTimeKey);
                Hide();
                Debug.Log("Dice Roll Event has ended.");
                yield break; // Exit coroutine
            }

            yield return new WaitForSeconds(1f); // Wait for 1 second before updating again
        }
    }

    public void OnClick_Play()
    {
        Hide();
        ScreenManager.Get.GetScreen<DiceRollGameScreen>().Show();
    }

    public void CloseButton()
    {
        Hide();
    }
}
