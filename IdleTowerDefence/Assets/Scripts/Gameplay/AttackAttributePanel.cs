//using Lofelt.NiceVibrations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AttackAttributePanel : AttributePanel
{
    public AttackAttributeData attributeData;
    public Transform contentPanel;
    public GameObject unlockAttributePanel;
    public CustomButton unlockButton;
    public TMP_Text attributeNameText;
    public TMP_Text costText;

    public int cost = 0;
    private AttributeData unlockingData;

    private void OnEnable()
    {
        unlockButton?.onClick.AddListener(OnClick_Unlock);
    }

    private void OnClick_Unlock()
    {
        if (cost <= Constants.Get.GoldCoin)
        {
            GameAnalyticsManager.Instance.NewDesignEventGA("Unlock_New_Attributes");
            unlockingData.savedData.IsUnlocked = true;
            
            Constants.Get.GoldCoin -= cost;
            EventManager.TriggerEvent(EventID.Update_Currency);
            EventManager.TriggerEvent(EventID.Update_GoldCoin);

            if (string.IsNullOrEmpty(unlockingData.attachedAttributeClassName) == false)
            {
                var nextAttr = attributeData.attributes.Find(x => x.attributeClassName == unlockingData.attachedAttributeClassName);
                if (nextAttr != null)
                    nextAttr.savedData.IsUnlocked = true;

                //if (ActiveGameData.Instance.saveData.VibrateEffect == 1)
                //    HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);
            }
            InitializeAttributes();
        }
        else
        {
            ToastManager.Get.ShowMessage("Not Enough Coin");
        }
    }

    internal override void InitializeAttributes()
    {
        if (isFactory)
        {
            contentPanel?.transform.DestroyAllChildren();
            unlockAttributePanel?.SetActive(false);
        }

        factoryAttributeItems.Clear();
        attributes.Clear();
        attributeItems.Clear();

        foreach (AttributeData attributeData in attributeData.attributes)
        {
            IAttribute attribute = CreateAttributeInstance(attributeData);
            attributes.Add(attributeData.attributeClassName, attribute);

            if (isFactory)
            {
                if (attributeData.savedData.IsUnlocked == false)
                {
                    attributeNameText.text = "<size=55>" + attributeData.attributeDisplayName.ToUpper() + "</size>\n" + attributeData.attributeDesc;
                    costText.text = "<size=50>UNLOCK</size>\n" + attributeData.unlockPrice;
                    cost = attributeData.unlockPrice;
                    unlockingData = attributeData;
                    unlockAttributePanel.SetActive(true);
                    break;
                }
                var obj = Instantiate(factoryItemPrefab.gameObject, contentPanel).GetComponent<FactoryAttributeItem>();
                obj.SetUI(attributeData, attribute);
                if (attributeData.attributeClassName == "AttackDamage")
                {
                    obj.upgradeButton.gameObject.name = "AttackDamageAttribute";
                    obj.upgradeButton.onClick.AddListener(() =>
                    {
                        if (TutorialManager.Get.tutorialVariables.ContainsKey("ClickOnUpgradeAttribute") && TutorialManager.Get.tutorialVariables["ClickOnUpgradeAttribute"] == true)
                            EventManager.TriggerEvent(EventID.TutorialNextStep);
                    });
                }
                factoryAttributeItems.Add(obj);
            }
            else
            {
                var obj = Instantiate(itemPrefab.gameObject, attributeParent.content).GetComponent<AttributeItem>();
                if (obj.SetUI(attributeData, attribute))
                    attributeItems.Add(obj);

                if (attributeData.attributeClassName == "AttackDamage")
                {
                    obj.upgradeButton.gameObject.name = "AttackDamageAttribute";
                    obj.upgradeButton.onClick.AddListener(() =>
                    {
                        if (TutorialManager.Get.tutorialVariables.ContainsKey("ClickOnUpgradeAttribute") && TutorialManager.Get.tutorialVariables["ClickOnUpgradeAttribute"] == true)
                            EventManager.TriggerEvent(EventID.TutorialNextStep);
                    });
                }
            }
        }
    }

    private void OnDisable()
    {
        unlockButton?.onClick.RemoveAllListeners();
    }

}