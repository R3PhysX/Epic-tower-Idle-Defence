using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewCardUnlockScreen : ScreenPanel
{
    [SerializeField] private CardData cardData;
    [SerializeField] private List<Image> cardImage;
    [SerializeField] private Sprite blankCard;
    [SerializeField] private CustomButton unlockButton;
    [SerializeField] private CustomButton unlock4xButton;
    [SerializeField] private CustomButton closeButton;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private int cost = 25;

    [SerializeField] private Animator animator;
    [SerializeField] private GameObject blocker;

    private void OnEnable()
    {
        closeButton?.onClick.AddListener(OnCLick_Close);
        unlockButton?.onClick.AddListener(OnCLick_Unlock);
        unlock4xButton?.onClick.AddListener(OnCLick_Unlock4x);
        if (TutorialManager.Get.isTutorialShowing == false)
            costText.text = cost.ToString();
        else
            costText.text = "0";

        blocker.gameObject.SetActive(false);
    }

    private void OnCLick_Close()
    {
        ScreenManager.Get.GetScreen<NewCardUnlockScreen>().Hide();
    }

    private void OnCLick_Unlock()
    {
        blocker.gameObject.SetActive(true);
        var cardList = cardData.cards;
        if (cardList.Count <= 0)
        {
            ToastManager.Get.ShowMessage("No Cards Available");
            blocker.gameObject.SetActive(false);
            return;
        }

        if (TutorialManager.Get.isTutorialShowing)
            Constants.Get.Gems += 25;

        if (Constants.Get.Gems < 25)
        {
            ToastManager.Get.ShowMessage("Not Enough Gems");
            blocker.gameObject.SetActive(false);
            return;
        }

        Constants.Get.Gems -= 25;
        ActiveGameData.Instance.saveData.cardCollected += 1;
        unlockButton?.onClick.RemoveAllListeners();
        unlock4xButton?.onClick.RemoveAllListeners();
        closeButton?.onClick.RemoveAllListeners();
        EventManager.TriggerEvent(EventID.Update_Currency, null);
        cardImage[0].sprite = blankCard;

        animator.Play("Chest-Open");
        AudioManager.Instance?.PlaySFXSound(AudioClipsType.ChestUnlock);

        LeanTween.delayedCall(1.2f, ()=> {

            var RandCardNumber = 0;
            float randomValue = Random.Range(1, 101);

            for(int i=0;i< cardData.cards.Count;i++)
            {
                if (randomValue <= cardData.cards[i].probability)
                {
                    RandCardNumber = i;
                    Debug.Log("Picked " + cardData.cards[i].cardName + " with probability of " + cardData.cards[i].probability);
                    break;
                }

                randomValue -= cardData.cards[i].probability;
            }

            cardList[RandCardNumber].savedData.cardCount += 1;
            cardImage[0].sprite = cardList[RandCardNumber].cardImage;

            var scr = ScreenManager.Get.GetScreen<CardsScreen>();

            if (cardList[RandCardNumber].savedData.unlocked == 0)
            {
                cardList[RandCardNumber].savedData.unlocked = 1;
                scr.AddCardInInventory(cardList[RandCardNumber]);
            }

            scr.UpdateCardUI();
            
            
            if (TutorialManager.Get != null && TutorialManager.Get.tutorialVariables.ContainsKey("ClickOnUnlock") && TutorialManager.Get.tutorialVariables["ClickOnUnlock"] == true)
            {
                unlockButton?.onClick.RemoveAllListeners();
                unlock4xButton?.onClick.RemoveAllListeners();
                closeButton?.onClick.RemoveAllListeners();
                LeanTween.delayedCall(1.25f, () =>
                {
                    blocker.gameObject.SetActive(false);
                    Hide();
                    EventManager.TriggerEvent(EventID.TutorialNextStep);
                });
            }
            else {
                LeanTween.delayedCall(0.4f, () => {
                    blocker.gameObject.SetActive(false);
                    closeButton?.onClick.AddListener(OnCLick_Close);
                    unlockButton?.onClick.AddListener(OnCLick_Unlock);
                    unlock4xButton?.onClick.AddListener(OnCLick_Unlock4x);
                });
            }
        }).setIgnoreTimeScale(true);
    }

    private void OnDisable()
    {
        closeButton?.onClick.RemoveAllListeners();
        unlockButton?.onClick.RemoveAllListeners();
        unlock4xButton?.onClick.RemoveAllListeners();
    }

    private void OnCLick_Unlock4x()
    {
        blocker.gameObject.SetActive(true);
        var cardList = cardData.cards;
        if (cardList.Count <= 0)
        {
            ToastManager.Get.ShowMessage("No Cards Available");
            blocker.gameObject.SetActive(false);
            return;
        }

        if (Constants.Get.Gems < 100)
        {
            ToastManager.Get.ShowMessage("Not Enough Gems");
            blocker.gameObject.SetActive(false);
            return;
        }

        Constants.Get.Gems -= 100;
        ActiveGameData.Instance.saveData.cardCollected += 4;
        unlockButton?.onClick.RemoveAllListeners();
        unlock4xButton?.onClick.RemoveAllListeners();
        closeButton?.onClick.RemoveAllListeners();
        EventManager.TriggerEvent(EventID.Update_Currency, null);
        cardImage.ForEach(x => x.sprite = blankCard);
        
        animator.Play("Chest-Open-4Card");
        AudioManager.Instance?.PlaySFXSound(AudioClipsType.ChestUnlock);

        LeanTween.delayedCall(1.2f, () => {

            for (int cardCount = 0; cardCount < 4; cardCount++)
            {
                var RandCardNumber = 0;
                float randomValue = Random.Range(1, 101);

                for (int i = 0; i < cardData.cards.Count; i++)
                {
                    if (randomValue <= cardData.cards[i].probability)
                    {
                        RandCardNumber = i;
                        Debug.Log("Picked " + cardData.cards[i].cardName + " with probability of " + cardData.cards[i].probability);
                        break;
                    }

                    randomValue -= cardData.cards[i].probability;
                }

                cardList[RandCardNumber].savedData.cardCount += 1;
                cardImage[cardCount].sprite = cardList[RandCardNumber].cardImage;

                var scr = ScreenManager.Get.GetScreen<CardsScreen>();

                if (cardList[RandCardNumber].savedData.unlocked == 0)
                {
                    cardList[RandCardNumber].savedData.unlocked = 1;
                    scr.AddCardInInventory(cardList[RandCardNumber]);
                }

                scr.UpdateCardUI();
            }


            if (TutorialManager.Get != null && TutorialManager.Get.tutorialVariables.ContainsKey("ClickOnUnlock") && TutorialManager.Get.tutorialVariables["ClickOnUnlock"] == true)
            {
                unlockButton?.onClick.RemoveAllListeners();
                unlock4xButton?.onClick.RemoveAllListeners();
                closeButton?.onClick.RemoveAllListeners();
                LeanTween.delayedCall(1.25f, () =>
                {
                    blocker.gameObject.SetActive(false);
                    Hide();
                    EventManager.TriggerEvent(EventID.TutorialNextStep);
                });
            }
            else
            {
                LeanTween.delayedCall(0.4f, () => {
                    blocker.gameObject.SetActive(false);
                    closeButton?.onClick.AddListener(OnCLick_Close);
                    unlockButton?.onClick.AddListener(OnCLick_Unlock);
                    unlock4xButton?.onClick.AddListener(OnCLick_Unlock4x);
                });
            }
        }).setIgnoreTimeScale(true);
    }
}
