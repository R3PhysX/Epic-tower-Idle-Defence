using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardInfoScreen : ScreenPanel
{
    [SerializeField] private InventoryCardItem cardItem;
    [SerializeField] private GameObject particleObject;
    [SerializeField] private TMP_Text cardTitle;
    [SerializeField] private TMP_Text cardDesc;

    [SerializeField] private TMP_Text level1_val1_title;
    [SerializeField] private TMP_Text level1_val1_value;
    [SerializeField] private TMP_Text level1_val2_title;
    [SerializeField] private TMP_Text level1_val2_value;

    [SerializeField] private TMP_Text level2_val1_title;
    [SerializeField] private TMP_Text level2_val1_value;
    [SerializeField] private TMP_Text level2_val2_title;
    [SerializeField] private TMP_Text level2_val2_value;

    [SerializeField] private TMP_Text level3_val1_title;
    [SerializeField] private TMP_Text level3_val1_value;
    [SerializeField] private TMP_Text level3_val2_title;
    [SerializeField] private TMP_Text level3_val2_value;

    [SerializeField] private CustomButton closeButton;
    [SerializeField] private CustomButton upgradeButton;
    [SerializeField] private TMP_Text upgradeButtonText;
    [SerializeField] private TMP_Text collectCardText;

    private void OnEnable()
    {
        closeButton.onClick.AddListener(OnClick_Close);
    }

    internal void Set(CardInfo info, Card card)
    {
        cardItem.Set(info, card);
        cardDesc.text = info.carddesc;
        cardTitle.text = info.cardName;

        level1_val1_title.text = info.val1Desc;
        level1_val2_title.text = info.val2Desc;
        level2_val1_title.text = info.val1Desc;
        level2_val2_title.text = info.val2Desc;
        level3_val1_title.text = info.val1Desc;
        level3_val2_title.text = info.val2Desc;

        level1_val1_value.text = info.level1.value1 + " " + info.val1PostFix;
        level1_val2_value.text = info.level1.value2 + " " + info.val2PostFix;

        level2_val1_value.text = info.level2.value1 + " " + info.val1PostFix;
        level2_val2_value.text = info.level2.value2 + " " + info.val2PostFix;

        level3_val1_value.text = info.level3.value1 + " " + info.val1PostFix;
        level3_val2_value.text = info.level3.value2 + " " + info.val2PostFix;
        CheckUpgradeBtn();
    }

    private void CheckUpgradeBtn()
    {
        upgradeButton.onClick.RemoveAllListeners();
        collectCardText.gameObject.SetActive(false);
        if (cardItem.data.savedData.level >= 3)
        {
            upgradeButtonText.text = "MAX";
        }
        else
        {
            collectCardText.text = (cardItem.data.savedData.cardCount >= 5) ? "Ready for Upgrade" : "Collect " + (5 - cardItem.data.savedData.cardCount) + " more to upgrade";
            collectCardText.gameObject.SetActive(true);
            upgradeButton.onClick.AddListener(OnClick_Upgrade);
            upgradeButtonText.text = "Upgrade";
        }
    }

    private void OnClick_Upgrade()
    {
        if (cardItem.data.savedData.cardCount >= 5 && cardItem.data.savedData.level < 3)
        {
            upgradeButton.onClick.RemoveAllListeners();
            cardItem.data.savedData.cardCount = cardItem.data.savedData.cardCount - 5;
            cardItem.data.savedData.cardCount = Mathf.Clamp(cardItem.data.savedData.cardCount, 1, cardItem.data.savedData.cardCount);
            cardItem.data.savedData.level += 1;
            particleObject.gameObject.SetActive(true);
            AudioManager.Instance?.PlaySFXSound(AudioClipsType.CardUpgrade);
            LeanTween.scale(cardItem.gameObject, Vector3.one * 1.2f, 0.25f).setIgnoreTimeScale(true)
                    .setEase(LeanTweenType.easeOutQuad).setOnComplete(() =>
                    {
                        LeanTween.scale(cardItem.gameObject, Vector3.one, 0.12f).setIgnoreTimeScale(true)
                            .setEase(LeanTweenType.easeInQuad).setOnComplete(() =>
                            {
                                cardItem.UpdateUI();
                                CheckUpgradeBtn();
                                ScreenManager.Get.GetScreen<CardsScreen>().UpdateCardUI();
                                ActiveGameData.Instance.saveData.upgradeTimes += 1;
                            });
                    });
            LeanTween.delayedCall(1f, () => { particleObject.gameObject.SetActive(false); }).setIgnoreTimeScale(true);

        }
        else if (cardItem.data.savedData.cardCount < 5)
        {
            ToastManager.Get.ShowMessage("Collect 5 Cards to Upgrade");
        }
        else if (cardItem.data.savedData.level >= 3)
        {
            ToastManager.Get.ShowMessage("Max Upgraded");
        }
    }

    private void OnClick_Close()
    {
        Hide();
    }

    private void OnDisable()
    {
        upgradeButton.onClick.RemoveAllListeners();
        closeButton.onClick.RemoveAllListeners();
    }
}
