using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CardsScreen : ScreenPanel
{

    [SerializeField] private List<RectTransform> layouts;

    [SerializeField] private Image progressBar;
    [SerializeField] private TMP_Text cardCollectedText;
    [SerializeField] private TMP_Text cardToCollectText;

    [SerializeField] private CardData cardObject;
    [SerializeField] private InventoryCardItem inventoryCardItem;
    [SerializeField] private Transform inventoryScrollContent;

    [SerializeField] private List<CardSlotItem> slots;
    [SerializeField] private CustomButton closeButton;
    [SerializeField] private CustomButton unlockButton;
    [SerializeField] private CustomButton dumpCardButton;

    protected Dictionary<string, Card> attributes = new Dictionary<string, Card>();
    private List<InventoryCardItem> cardItemSpawned = new List<InventoryCardItem>();

    private float timeScale;

    [SerializeField] private TMP_Text goldCoin;
    [SerializeField] private TMP_Text diamondCoin;
    [SerializeField] private TMP_Text gemCoin;

    private void OnEnable()
    {
        GameAnalyticsManager.Instance.NewDesignEventGA("Click_CardScreen");
        closeButton?.onClick.AddListener(OnCLick_Close);
        unlockButton?.onClick.AddListener(OnCLick_Unlock);
        dumpCardButton?.onClick.AddListener(OnCLick_DumpCard);
        EventManager.AddListener(EventID.Update_Currency, Update_Currency);
        if (GameplayManager.Get != null)
        {
            timeScale = Time.timeScale;
            Time.timeScale = 0;
        }
        Update_Currency(null);

        progressBar.fillAmount = ActiveGameData.Instance.saveData.dumpedCardValue / cardObject.godModeCardRequireDumpValue;

        if (cardObject.cards[4].savedData.unlocked == 1)
        {
            dumpCardButton.gameObject.SetActive(false);
        }

        if (SceneManager.GetActiveScene().buildIndex == 2)
            dumpCardButton.gameObject.SetActive(false);

    }

    private void OnCLick_DumpCard()
    {
        ScreenManager.Get.GetScreen<CardDumpScreen>().Show();
    }

    private void Update_Currency(object arg)
    {
        goldCoin.text = Constants.Get.GoldCoin.ToString();
        diamondCoin.text = Constants.Get.Diamond.ToString();
        gemCoin.text = Constants.Get.Gems.ToString();
    }

    internal void Initialize()
    {
        foreach (CardInfo cardInfo in cardObject.cards)
        {
            Card card = CreateCardInstance(cardInfo);
            attributes.Add(cardInfo.cardClassname, card);
            card.Init(cardInfo);
        }
    }

    internal void UpdateCardUI()
    {
        if (cardObject.cards[4].savedData.unlocked == 1)
        {
            dumpCardButton.gameObject.SetActive(false);
        }

        progressBar.fillAmount = ActiveGameData.Instance.saveData.dumpedCardValue / cardObject.godModeCardRequireDumpValue;
        cardItemSpawned.ForEach(x => x.UpdateUI());
        //////

        for (int i = 0; i < ActiveGameData.Instance.saveData.cardSlotIds.Count; i++)
        {
            foreach (var item in cardObject.cards)
            {
                if (ActiveGameData.Instance.saveData.cardSlotIds[i] == item.cardId && item.savedData.unlocked == 0)
                {
                    ActiveGameData.Instance.saveData.cardSlotIds[i] = -1;
                    Destroy(slots[i].attachedItem.gameObject);
                    cardItemSpawned.Remove(slots[i].attachedItem);
                    break;
                }
            }
        }

        layouts.ForEach(x => LayoutRebuilder.ForceRebuildLayoutImmediate(x));
    }

    internal void AddCardInInventory(CardInfo info)
    {
        var obj = cardItemSpawned.Find(x => x.data.cardId == info.cardId);
        obj.Set();
        Debug.Log("1");
        if (TutorialManager.Get.isTutorialShowing)
        {
            obj.tickToggle.gameObject.name = "ActivateCard";
            obj.tickToggle.onClick.AddListener(() =>
            {
                if (TutorialManager.Get != null && TutorialManager.Get.tutorialVariables.ContainsKey("ActivateCard") && TutorialManager.Get.tutorialVariables["ActivateCard"] == true)
                    EventManager.TriggerEvent(EventID.TutorialNextStep);
            });
        }
        cardItemSpawned.Add(obj);
    }

    private void Start()
    {
        if (attributes.Count <= 0)
            Initialize();

        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].Set(i, (index) =>
            {
                ActiveGameData.Instance.saveData.cardSlotUnlocked[index] = 1;

                int lockedCount = 0;
                foreach(var item in ActiveGameData.Instance.saveData.cardSlotUnlocked)
                {
                    if (item == 0)
                        lockedCount += 1;
                }

                if(lockedCount==0)
                    GameAnalyticsManager.Instance.NewDesignEventGA("Card_Slot_Unlock");
                //  AppsFlyerEventManager.Get.SendEvent(AppsFlyerEventManager.AF_Card_SLOT_UNLOCKED, "1");
            });

            if (ActiveGameData.Instance.saveData.cardSlotIds[i] != -1)
            {
                var info = cardObject.cards.Find(x => x.cardId == ActiveGameData.Instance.saveData.cardSlotIds[i]);
                var card = attributes[info.cardClassname];
                var obj = Instantiate(inventoryCardItem.gameObject, slots[i].transform).GetComponent<InventoryCardItem>();
                obj.SetToggleOption(false);
                obj.Set(info, card);
                slots[i].attachedItem = obj;
                cardItemSpawned.Add(obj);
            }
        }

        inventoryScrollContent.transform.DestroyAllChildren();
        foreach (var item in cardObject.cards)
        {
            var obj = Instantiate(inventoryCardItem.gameObject, inventoryScrollContent).GetComponent<InventoryCardItem>();
            obj.Set(item, attributes[item.cardClassname], OnAddCard, OnRemoveCard);
            cardItemSpawned.Add(obj);
        }
    }

    private void OnAddCard(CardInfo info, int index)
    {
        ActiveGameData.Instance.saveData.cardSlotIds[index] = info.cardId;

        var obj = Instantiate(inventoryCardItem.gameObject, slots[index].transform).GetComponent<InventoryCardItem>();
        obj.SetToggleOption(false);
        obj.Set(info, attributes[info.cardClassname]);
        slots[index].attachedItem = obj;
        cardItemSpawned.Add(obj);
    }

    private void OnRemoveCard(CardInfo info)
    {
        for (int i = 0; i < ActiveGameData.Instance.saveData.cardSlotIds.Count; i++)
        {
            if (ActiveGameData.Instance.saveData.cardSlotIds[i] == info.cardId)
            {
                ActiveGameData.Instance.saveData.cardSlotIds[i] = -1;
                Destroy(slots[i].attachedItem.gameObject);
                cardItemSpawned.Remove(slots[i].attachedItem);
                break;
            }
        }
    }

    protected virtual Card CreateCardInstance(CardInfo cardInfo)
    {
        string cardName = cardInfo.cardClassname;

        string fullClassName = typeof(Player).Namespace + "." + cardName.Replace(" ", "");

        Type cardType = Type.GetType(fullClassName);
        if (cardType != null)
        {
            if (typeof(Card).IsAssignableFrom(cardType))
            {
                return (Card)Activator.CreateInstance(cardType);
            }
            else
            {
                Debug.LogError(cardName + " does not implement Card.");
            }
        }
        else
        {
            Debug.LogError("Card class not found: " + fullClassName);
        }

        return null;
    }

    private void OnCLick_Close()
    {
        ScreenManager.Get.GetScreen<CardsScreen>().Hide();
    }

    private void OnCLick_Unlock()
    {
        if (TutorialManager.Get != null && TutorialManager.Get.tutorialVariables.ContainsKey("ClickOnNewCard") && TutorialManager.Get.tutorialVariables["ClickOnNewCard"] == true)
            EventManager.TriggerEvent(EventID.TutorialNextStep);
        
        ScreenManager.Get.GetScreen<NewCardUnlockScreen>().Show();

    }

    private void OnDisable()
    {
        closeButton?.onClick.RemoveAllListeners();
        unlockButton?.onClick.RemoveAllListeners();
        dumpCardButton?.onClick.RemoveAllListeners();
        EventManager.RemoveListener(EventID.Update_Currency, Update_Currency);
        if (GameplayManager.Get != null)
        {
            Time.timeScale = timeScale;
        }
        var scr = ScreenManager.Get.GetScreen<GameScreen>();
        if (scr != null) scr.UpdateCardIcon();
    }
}
