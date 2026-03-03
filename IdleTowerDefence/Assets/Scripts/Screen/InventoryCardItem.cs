using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCardItem : MonoBehaviour
{
    [SerializeField] private TMP_Text cardName;
    [SerializeField] private TMP_Text cardCount;
    [SerializeField] private Image cardImage;
    [SerializeField] private CustomButton button;
    [SerializeField] internal CustomButton tickToggle;
    [SerializeField] private GameObject tick;
    [SerializeField] private bool isToggleOn;

    [SerializeField] private List<Image> stars;
    [SerializeField] private Sprite activeStar;
    [SerializeField] private Sprite disableStar;

    internal CardInfo data;

    private Action<CardInfo, int> onAddCard;
    private Action<CardInfo> onRemoveCard;

    private Card card;

    private void OnEnable()
    {
        button.onClick.AddListener(OnClick_Card);
        tickToggle.onClick.AddListener(OnClick_Tick);
    }

    private void OnClick_Tick()
    {
        if (isToggleOn)
        {
            card.Deactivate();
            onRemoveCard?.Invoke(data);
            isToggleOn = false;
            SetToggle();
        }
        else
        {
            int emptySlotIndex = -1;
            for (int i = 0; i < ActiveGameData.Instance.saveData.cardSlotIds.Count; i++)
            {
                if (ActiveGameData.Instance.saveData.cardSlotUnlocked[i] == 1 && (ActiveGameData.Instance.saveData.cardSlotIds[i] == -1 || ActiveGameData.Instance.saveData.cardSlotIds[i] == 0))
                {
                    emptySlotIndex = i;
                    break;
                }
            }

            if (emptySlotIndex != -1)
            {
                card.Activate();
                onAddCard?.Invoke(data, emptySlotIndex);
                isToggleOn = true;
                SetToggle();
            }
        }
    }

    private void SetToggle()
    {
        tick.gameObject.SetActive(isToggleOn);
    }

    internal void SetToggleOption(bool status)
    {
        tickToggle.gameObject.SetActive(status);
    }

    internal void Set(CardInfo info, Card card, Action<CardInfo, int> onAddCard = default, Action<CardInfo> onRemoveCard = default)
    {
        this.card = card;
        data = info;
        cardName.text = data.cardName;
        cardImage.sprite = info.cardImage;
        var obj = ActiveGameData.Instance.saveData.cardSlotIds.Find(x => x == data.cardId);
        isToggleOn = obj != -1 && obj != 0;

        UpdateUI();
        SetToggle();
        this.onAddCard = onAddCard;
        this.onRemoveCard = onRemoveCard;
    }

    internal void Set()
    {
        cardName.text = data.cardName;
        cardImage.sprite = data.cardImage;
        var obj = ActiveGameData.Instance.saveData.cardSlotIds.Find(x => x == data.cardId);
        isToggleOn = obj != -1 && obj != 0;

        UpdateUI();
        SetToggle();
    }

    internal void UpdateUI()
    {
        cardCount.text = "" + data.savedData.cardCount;
        for (int i = 0; i < 3; i++)
        {
            stars[i].sprite = ((i + 1) <= data.savedData.level) ? activeStar : disableStar;
        }

        gameObject.SetActive(data.savedData.cardCount > 0 && data.savedData.unlocked == 1);

    }

    private void OnClick_Card()
    {
        var scr = ScreenManager.Get.GetScreen<CardInfoScreen>();
        scr.Set(data, card);
        scr.Show();
    }

    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();
        tickToggle.onClick.RemoveAllListeners();
    }
}