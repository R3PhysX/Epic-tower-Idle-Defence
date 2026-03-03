using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TabData
{
    public Transform transform;
    public Image panelImage;
    public Image iconImage;
    public Sprite enabledSprite;
    public Sprite disableSprite;
}

public class AttributeScreen : ScreenPanel
{
    public List<AttributePanel> tabPanels; // Assign your tab content panels in the Inspector.
    private int currentTabIndex = -1;

    public List<TabData> tabsButton;

    [SerializeField] private Sprite selectedPanel;
    [SerializeField] private Sprite nonselectedPanel;

    [SerializeField] private TMP_Text attackText;
    [SerializeField] private TMP_Text attackSpeedText;
    [SerializeField] private TMP_Text healthText;

    [SerializeField] private Transform activePos;
    [SerializeField] private Transform disablePos;

    private void OnEnable()
    {
        EventManager.AddListener(EventID.Attribute_AttackDamage, Attribute_AttackDamage);
        EventManager.AddListener(EventID.Attribute_AttackSpeed, Attribute_AttackSpeed);
        EventManager.AddListener(EventID.Attribute_Health, Attribute_Health);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(EventID.Attribute_AttackDamage, Attribute_AttackDamage);
        EventManager.RemoveListener(EventID.Attribute_AttackSpeed, Attribute_AttackSpeed);
        EventManager.RemoveListener(EventID.Attribute_Health, Attribute_Health);
    }

    private void Start()
    {
        tabPanels.ForEach(x => { x.gameObject.SetActive(false); x.InitializeAttributes(); });
        LeanTween.delayedCall(0.5f, () =>
        {
            ResetAllTab();
            ShowTab(0);
        });
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
        tabsButton.ForEach(x =>
        {
            x.panelImage.sprite = nonselectedPanel;
            x.iconImage.sprite = x.disableSprite;

            x.panelImage.gameObject.transform.position = new Vector3(x.panelImage.gameObject.transform.position.x, disablePos.transform.position.y, x.panelImage.gameObject.transform.position.z);
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