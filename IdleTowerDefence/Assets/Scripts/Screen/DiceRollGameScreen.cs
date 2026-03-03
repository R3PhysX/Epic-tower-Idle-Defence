using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[Serializable]
public enum BoardItemType
{
    None,
    Coin,
    Gem,
    Forward,
    Backward
}

[Serializable]
public class BoardPositionData
{
    public BoardItemType boardItemType;
    public int quantity;
    public Transform transform;
    public GameObject itemObject;
}

public class DiceRollGameScreen : ScreenPanel
{

    public GameObject blocker;

    public TMP_Text coinText;
    public TMP_Text gemText;

    public TMP_Text diceRollTimerText;
    public TMP_Text ticketText;
    public CustomButton rollButton;

    private string DiceRollStartTimeKey = "ITD_DiceRollEventTime";

    private Coroutine UpdateDiceRollTimerRoutine;

    public GameObject player;
    public Sprite unlockSprite;
    public Sprite lockSprite;

    public List<BoardPositionData> boardPositionDatas;

    [Header("Dice")]
    public Transform diceTransform;
    public Vector3[] diceFaceRotations = new Vector3[]
    {
        new Vector3(0, 0, 0),      // Face 1
        new Vector3(0, 0, 90),     // Face 2
        new Vector3(90, 0, 0),     // Face 3
        new Vector3(-90, 0, 0),    // Face 4
        new Vector3(0, 0, -90),    // Face 5
        new Vector3(180, 0, 0)     // Face 6
    };

    [Header("Double It")]
    public Transform doubleItPopup;
    public Image rewardTypeIcon1;
    public Image rewardTypeIcon2;
    public TMP_Text reward1;
    public TMP_Text reward2;
    public Sprite goldIcon;
    public Sprite gemIcon;

    public CustomButton yesButton;
    public CustomButton noButton;

    private void OnEnable()
    {
        this.GetComponent<AudioSource>().enabled = (ActiveGameData.Instance.saveData.MusicEffect == 1);

        AudioManager.Instance?.StopBackGroundMusic();
        player.gameObject.transform.position = boardPositionDatas[ActiveGameData.Instance.saveData.playerIndexDiceRoll].transform.position;
        ticketText.text = ActiveGameData.Instance.saveData.diceRollTicket.ToString();
        coinText.text = ActiveGameData.Instance.saveData.GoldCoin.ToString();
        gemText.text = ActiveGameData.Instance.saveData.Gems.ToString();
        StopAllCoroutines();
        CheckForDiceRollEvent();
        CheckIfTicketAvailable();
        blocker.gameObject.SetActive(false);
        AdManager.Get.LoadRewardedAd(true);
    }

    private void OnDisable()
    {
        AudioManager.Instance?.PlayBackGroundMusic(AudioClipsType.BGM);
    }

    private void CheckIfTicketAvailable()
    {
        rollButton.onClick.RemoveAllListeners();
        if (ActiveGameData.Instance.saveData.diceRollTicket > 0)
        {
            rollButton.onClick.AddListener(OnClick_Roll);
            rollButton.image.sprite = unlockSprite;
        }
        else
        {
            rollButton.image.sprite = lockSprite;
        }
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
                // Format remaining time as HH:MMh
                int totalHours = (int)Math.Floor(timeRemaining.TotalHours);
                diceRollTimerText.text = $"{totalHours:D2}H {timeRemaining.Minutes:D2}M {timeRemaining.Seconds:D2}S Left";
            }
            else
            {
                ActiveGameData.Instance.diceRollActive = false;
                // Time is up; hide the Dice Roll screen or reset
                diceRollTimerText.text = "Over";
                PlayerPrefs.DeleteKey(DiceRollStartTimeKey);
                Hide();
                Debug.Log("Dice Roll Event has ended.");
                yield break; // Exit coroutine
            }

            yield return new WaitForSeconds(1f); // Wait for 1 second before updating agains
        }
    }

    public void OnClick_Roll()
    {
        blocker.gameObject.SetActive(true);
        AudioManager.Instance.PlaySFXSound(AudioClipsType.DiceButtonPressed);
        RollDice();
    }

    public void RollDice()
    {
        ActiveGameData.Instance.saveData.diceRollTicket -= 1;
        ticketText.text = ActiveGameData.Instance.saveData.diceRollTicket.ToString();
        CheckIfTicketAvailable();

        // Pick a random face (1 to 6)
        int randomFace = Random.Range(0, 6);
        Debug.Log($"Rolling: {randomFace + 1}");

        // Get the target rotation for the final face
        Vector3 targetRotation = diceFaceRotations[randomFace];

        List<int> randomFaces = GetThreeNumbersExcept(randomFace);

        LeanTween.rotate(diceTransform.gameObject, diceFaceRotations[randomFaces[0]], 0.2f)
           .setEase(LeanTweenType.easeOutQuad)
           .setOnComplete(() =>
           {
               LeanTween.rotate(diceTransform.gameObject, diceFaceRotations[randomFaces[1]], 0.2f)
               .setEase(LeanTweenType.easeOutQuad)
               .setOnComplete(() =>
               {
                   LeanTween.rotate(diceTransform.gameObject, diceFaceRotations[randomFaces[2]], 0.2f)
                   .setEase(LeanTweenType.easeOutQuad)
                   .setOnComplete(() =>
                   {
                       LeanTween.rotate(diceTransform.gameObject, targetRotation, 0.2f)
                        .setEase(LeanTweenType.easeOutQuad)
                        .setOnComplete(() =>
                        {
                            // Log the rolled number (add 1 because index starts at 0)
                            int rolledNumber = randomFace + 1;
                            StartCoroutine(MovePlayer(rolledNumber));
                            Debug.Log($"Rolled: {rolledNumber}");
                        });
                     });
                });
           });
    }

    IEnumerator MovePlayer(int number, bool reverse = false)
    {
        for (int i = 0; i < number; i++)
        {
            if (reverse == false)
            {
                if (ActiveGameData.Instance.saveData.playerIndexDiceRoll >= boardPositionDatas.Count - 1)
                    ActiveGameData.Instance.saveData.playerIndexDiceRoll = -1;

                AudioManager.Instance.PlaySFXSound(AudioClipsType.PlayerMoveDice);
                LeanTween.move(player.gameObject, boardPositionDatas[ActiveGameData.Instance.saveData.playerIndexDiceRoll + 1].transform, 0.5f).setOnComplete(() =>
                {
                    ActiveGameData.Instance.saveData.playerIndexDiceRoll += 1;
                }).setEase(LeanTweenType.easeOutCubic);
            }
            else
            {
                AudioManager.Instance.PlaySFXSound(AudioClipsType.PlayerMoveDice);
                LeanTween.move(player.gameObject, boardPositionDatas[ActiveGameData.Instance.saveData.playerIndexDiceRoll - 1].transform, 0.5f).setOnComplete(() =>
                {
                    ActiveGameData.Instance.saveData.playerIndexDiceRoll -= 1;
                }).setEase(LeanTweenType.easeOutCubic);
            }

            yield return new WaitForSeconds(0.52f);
        }

        if (boardPositionDatas[ActiveGameData.Instance.saveData.playerIndexDiceRoll].boardItemType == BoardItemType.Coin)
        {
            if (ActiveGameData.Instance.saveData.playerIndexDiceRoll == 13)
            {
                rewardTypeIcon1.sprite = goldIcon;
                rewardTypeIcon2.sprite = goldIcon;

                reward1.text = boardPositionDatas[ActiveGameData.Instance.saveData.playerIndexDiceRoll].quantity.ToString();
                reward2.text = (boardPositionDatas[ActiveGameData.Instance.saveData.playerIndexDiceRoll].quantity * 2).ToString();

                yesButton.onClick.RemoveAllListeners();
                noButton.onClick.RemoveAllListeners();

                yesButton.onClick.AddListener(OnClick_Yes_DoubleIt_Coin);
                noButton.onClick.AddListener(OnClick_No_DoubleIt_Coin);

                doubleItPopup.gameObject.SetActive(true);
            }
            else
            {
                CoinAnimation.Get.ShowCoin(coinText.transform.position, boardPositionDatas[ActiveGameData.Instance.saveData.playerIndexDiceRoll].quantity, () =>
                {
                    Constants.Get.GoldCoin += boardPositionDatas[ActiveGameData.Instance.saveData.playerIndexDiceRoll].quantity;
                    coinText.text = ActiveGameData.Instance.saveData.GoldCoin.ToString();
                    EventManager.TriggerEvent(EventID.Update_Currency);
                });
            }
        }
        else if (boardPositionDatas[ActiveGameData.Instance.saveData.playerIndexDiceRoll].boardItemType == BoardItemType.Gem)
        {
            if (ActiveGameData.Instance.saveData.playerIndexDiceRoll == 15)
            {
                rewardTypeIcon1.sprite = gemIcon;
                rewardTypeIcon2.sprite = gemIcon;

                reward1.text = boardPositionDatas[ActiveGameData.Instance.saveData.playerIndexDiceRoll].quantity.ToString();
                reward2.text = (boardPositionDatas[ActiveGameData.Instance.saveData.playerIndexDiceRoll].quantity * 2).ToString();

                yesButton.onClick.RemoveAllListeners();
                noButton.onClick.RemoveAllListeners();

                yesButton.onClick.AddListener(OnClick_Yes_DoubleIt_Gem);
                noButton.onClick.AddListener(OnClick_No_DoubleIt_Gem);

                doubleItPopup.gameObject.SetActive(true);
            }
            else
            {
                CoinAnimation.Get.ShowGem(gemText.transform.position, boardPositionDatas[ActiveGameData.Instance.saveData.playerIndexDiceRoll].quantity, () =>
                {
                    Constants.Get.Gems += boardPositionDatas[ActiveGameData.Instance.saveData.playerIndexDiceRoll].quantity;
                    gemText.text = ActiveGameData.Instance.saveData.Gems.ToString();
                    EventManager.TriggerEvent(EventID.Update_Currency);
                });
            }
        }
        else if (boardPositionDatas[ActiveGameData.Instance.saveData.playerIndexDiceRoll].boardItemType == BoardItemType.Forward)
        {
            StartCoroutine(MovePlayer(boardPositionDatas[ActiveGameData.Instance.saveData.playerIndexDiceRoll].quantity));
        }
        else if (boardPositionDatas[ActiveGameData.Instance.saveData.playerIndexDiceRoll].boardItemType == BoardItemType.Backward)
        {
            StartCoroutine(MovePlayer(boardPositionDatas[ActiveGameData.Instance.saveData.playerIndexDiceRoll].quantity, true));
        }

        yield return new WaitForSeconds(1f);

        blocker.gameObject.SetActive(false);
    }

    public List<int> GetThreeNumbersExcept(int exclude)
    {
        
        // Create a list of numbers from 0 to 5, excluding the given number
        List<int> numbers = new List<int> { 0, 1, 2, 3, 4, 5 };
        numbers.Remove(exclude);

        // Shuffle the list randomly
        for (int i = numbers.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            int temp = numbers[i];
            numbers[i] = numbers[randomIndex];
            numbers[randomIndex] = temp;
        }

        // Take the first three numbers from the shuffled list
        return numbers.GetRange(0, 3);
    }

    public void CloseButton()
    {
        Hide();
    }

    public void OnClick_Yes_DoubleIt_Coin()
    {
        AdManager.Get.ShowRewardedAd((status) =>
        {
            if (status)
            {
                CoinAnimation.Get.ShowCoin(coinText.transform.position, boardPositionDatas[ActiveGameData.Instance.saveData.playerIndexDiceRoll].quantity * 2, () =>
                {
                    Constants.Get.GoldCoin += boardPositionDatas[ActiveGameData.Instance.saveData.playerIndexDiceRoll].quantity * 2;
                    coinText.text = ActiveGameData.Instance.saveData.GoldCoin.ToString();
                    EventManager.TriggerEvent(EventID.Update_Currency);
                });
            }
            else
            {
                Debug.Log("Reward Not Granted");
                ToastManager.Get.ShowMessage("Ads Not Available");
                OnClick_No_DoubleIt_Coin();
            }
        }, true);

        doubleItPopup.gameObject.SetActive(false);
    }

    public void OnClick_No_DoubleIt_Coin()
    {
        CoinAnimation.Get.ShowCoin(coinText.transform.position, boardPositionDatas[ActiveGameData.Instance.saveData.playerIndexDiceRoll].quantity, () =>
        {
            Constants.Get.GoldCoin += boardPositionDatas[ActiveGameData.Instance.saveData.playerIndexDiceRoll].quantity;
            coinText.text = ActiveGameData.Instance.saveData.GoldCoin.ToString();
            EventManager.TriggerEvent(EventID.Update_Currency);
        });
        doubleItPopup.gameObject.SetActive(false);
    }

    public void OnClick_Yes_DoubleIt_Gem()
    {
        AdManager.Get.ShowRewardedAd((status) =>
        {
            if (status)
            {
                CoinAnimation.Get.ShowGem(gemText.transform.position, boardPositionDatas[ActiveGameData.Instance.saveData.playerIndexDiceRoll].quantity * 2, () =>
                {
                    Constants.Get.Gems += boardPositionDatas[ActiveGameData.Instance.saveData.playerIndexDiceRoll].quantity * 2;
                    gemText.text = ActiveGameData.Instance.saveData.Gems.ToString();
                    EventManager.TriggerEvent(EventID.Update_Currency);
                });
            }
            else
            {
                Debug.Log("Reward Not Granted");
                ToastManager.Get.ShowMessage("Ads Not Available");
                OnClick_No_DoubleIt_Gem();
            }
        }, true);

        
        doubleItPopup.gameObject.SetActive(false);
    }

    public void OnClick_No_DoubleIt_Gem()
    {
        CoinAnimation.Get.ShowGem(gemText.transform.position, boardPositionDatas[ActiveGameData.Instance.saveData.playerIndexDiceRoll].quantity, () =>
        {
            Constants.Get.Gems += boardPositionDatas[ActiveGameData.Instance.saveData.playerIndexDiceRoll].quantity;
            gemText.text = ActiveGameData.Instance.saveData.Gems.ToString();
            EventManager.TriggerEvent(EventID.Update_Currency);
        });
        doubleItPopup.gameObject.SetActive(false);
    }
}