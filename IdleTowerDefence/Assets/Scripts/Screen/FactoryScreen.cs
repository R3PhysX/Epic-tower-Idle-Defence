using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FactoryScreen : ScreenPanel
{
    public List<AttributePanel> tabPanels; // Assign your tab content panels in the Inspector.
    private int currentTabIndex = -1;

    public List<TabData> tabsButton;

    [SerializeField] private Sprite selectedPanel;
    [SerializeField] private Sprite nonselectedPanel;

    [SerializeField] private TMP_Text goldCoin;
    [SerializeField] private TMP_Text diamondCoin;
    [SerializeField] private TMP_Text gemCoin;

    [SerializeField] private Transform activePos;
    [SerializeField] private Transform disablePos;

    [SerializeField] private TMP_Text attackText;
    [SerializeField] private TMP_Text attackSpeedText;
    [SerializeField] private TMP_Text healthText;

    private void OnEnable()
    {
        GameAnalyticsManager.Instance.NewDesignEventGA("Click_Factory");
        EventManager.AddListener(EventID.Update_Currency, Update_Currency);
        EventManager.AddListener(EventID.Update_GoldCoin, Update_Gold);
        EventManager.AddListener(EventID.Attribute_AttackDamage, Attribute_AttackDamage);
        EventManager.AddListener(EventID.Attribute_AttackSpeed, Attribute_AttackSpeed);
        EventManager.AddListener(EventID.Attribute_Health, Attribute_Health);
        tabPanels.ForEach(x => { x.factoryAttributeItems.ForEach(x => x.RefreshLastUI()); });
        Update_Currency(null);
    }

    private void Update_Currency(object arg)
    {
        goldCoin.text = Constants.Get.GoldCoin.ToString();
        diamondCoin.text = Constants.Get.Diamond.ToString();
        gemCoin.text = Constants.Get.Gems.ToString();
    }

    private void Update_Gold(object arg)
    {
        goldCoin.text = Constants.Get.GoldCoin.ToString();
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(EventID.Update_Currency, Update_Currency);
        EventManager.RemoveListener(EventID.Update_GoldCoin, Update_Gold);
        EventManager.RemoveListener(EventID.Attribute_AttackDamage, Attribute_AttackDamage);
        EventManager.RemoveListener(EventID.Attribute_AttackSpeed, Attribute_AttackSpeed);
        EventManager.RemoveListener(EventID.Attribute_Health, Attribute_Health);
    }

    private void Start()
    {
        goldCoin.text = Constants.Get.GoldCoin.ToString();
        diamondCoin.text = Constants.Get.Diamond.ToString();
        gemCoin.text = Constants.Get.Gems.ToString();

        tabPanels.ForEach(x => { x.gameObject.SetActive(false); x.InitializeAttributes(); });
        ResetAllTab();
        ShowTab(0);
    }

    public void ShowTab(int tabIndex)
    {

        // If the same tab is clicked, do nothing.
        if (tabIndex == currentTabIndex)
            return;

        // Hide the current tab's content.
        if (currentTabIndex != -1)
        {
            AudioManager.Instance?.PlaySFXSound(AudioClipsType.UITap);
            tabPanels[currentTabIndex].gameObject.SetActive(false);
            tabsButton[currentTabIndex].panelImage.sprite = nonselectedPanel;
            tabsButton[currentTabIndex].iconImage.sprite = tabsButton[currentTabIndex].disableSprite;

            LeanTween.moveY(tabsButton[currentTabIndex].panelImage.gameObject, disablePos.transform.position.y, 0.1f);
        }

        // Show the new tab's content.
        tabPanels[tabIndex].gameObject.SetActive(true);

        tabsButton[tabIndex].panelImage.sprite = selectedPanel;
        tabsButton[tabIndex].iconImage.sprite = tabsButton[tabIndex].enabledSprite;

        LeanTween.moveY(tabsButton[tabIndex].panelImage.gameObject, activePos.transform.position.y, 0.1f);

        // Update the current tab index.
        currentTabIndex = tabIndex;
    }

    private void ResetAllTab()
    {
        tabsButton.ForEach(x => {
            x.panelImage.sprite = nonselectedPanel;
            x.iconImage.sprite = x.disableSprite;

            LeanTween.moveY(x.panelImage.gameObject, disablePos.transform.position.y, 0f);
        });
    }

    private void Attribute_AttackSpeed(object arg)
    {
        float val = (float)arg;
        attackSpeedText.text = val.ToString("0.00");
    }

    private void Attribute_AttackDamage(object arg)
    {
        float val = (float)arg;
        attackText.text = val.ToString("0.00");
    }

    private void Attribute_Health(object arg)
    {
        float val = (float)arg;
        healthText.text = val.ToString("0");
    }
}
