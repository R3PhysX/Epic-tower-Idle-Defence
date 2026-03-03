using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DumpCardItem : MonoBehaviour
{
    [SerializeField] private TMP_Text cardCount;
    [SerializeField] private TMP_Text cardDumpingCount;
    [SerializeField] private Image cardImage;
    [SerializeField] private CustomButton plusButton;
    [SerializeField] private CustomButton minusButton;

    internal CardInfo data;

    internal int dumpCount;

    private CardDumpScreen screen;

    private void OnEnable()
    {
        plusButton.onClick.AddListener(OnClick_Plus);
        minusButton.onClick.AddListener(OnClick_Minus);
    }

    private void OnClick_Plus()
    {
        if (dumpCount >= (data.savedData.cardCount -1))
            return;
        dumpCount += 1;
        
        if (screen.UpdateUI("plus") == false)
        {
            dumpCount -= 1;
        }
        cardDumpingCount.text = dumpCount.ToString();
    }

    private void OnClick_Minus()
    {
        if (dumpCount <= 0)
            return;
        dumpCount -= 1;
        cardDumpingCount.text = dumpCount.ToString();

        screen.UpdateUI();
    }

    internal void Set(CardDumpScreen screen, CardInfo info)
    {
        this.screen = screen;
        this.data = info;
        dumpCount = 0;
        cardDumpingCount.text = dumpCount.ToString();
        cardCount.text = (data.savedData.cardCount - 1).ToString();
        cardImage.sprite = info.cardImage;

        gameObject.SetActive(info.savedData.cardCount > 1 && info.savedData.unlocked == 1);

    }

    internal void ResetCard()
    {
        if (data == null)
            return;

        dumpCount = 0;
        cardDumpingCount.text = dumpCount.ToString();
        cardCount.text = (data.savedData.cardCount - 1).ToString();
        cardImage.sprite = data.cardImage;

        gameObject.SetActive(data.savedData.cardCount > 1 && data.savedData.unlocked == 1);
    }

    internal void Set()
    {
        if (data == null)
            return;

        cardCount.text = (data.savedData.cardCount - 1).ToString();
        gameObject.SetActive(data.savedData.cardCount > 1 && data.savedData.unlocked == 1);
    }

    private void OnDisable()
    {
        plusButton.onClick.RemoveAllListeners();
        minusButton.onClick.RemoveAllListeners();
    }
}