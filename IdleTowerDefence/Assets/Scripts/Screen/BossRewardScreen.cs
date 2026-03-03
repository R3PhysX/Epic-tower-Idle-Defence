using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[System.Serializable]
public class FreeUpgradeItem
{
    public GameObject panelObject;
    public TMP_Text attributeName;
    public TMP_Text currentValue;
    public TMP_Text upgradeValue;
    public CustomButton upgradeButton;
}

public class BossRewardScreen : MonoBehaviour
{
    [SerializeField] private AttackAttributePanel attackAttributePanel;
    [SerializeField] private HealthAttributePanel healthAttributePanel;
    [SerializeField] private CoinAttributePanel coinAttributePanel;

    [SerializeField] private List<FreeUpgradeItem> freeUpgradeItem = new List<FreeUpgradeItem>();

    private float timeScale;

    internal bool ShowRewardPopup()
    {
        timeScale = Time.timeScale;

        List<AttributeItem> availableUpgrade = attackAttributePanel.attributeItems
                            .Concat(healthAttributePanel.attributeItems)
                            .Concat(coinAttributePanel.attributeItems)
                            .Where(x => x.data.savedData.IsUnlocked == true && x.maxedPanel.activeSelf == false)
                            .ToList();

        if (availableUpgrade.Count <= 0)
            return false;

        List<AttributeItem> freeUpgrade = availableUpgrade
                        .OrderBy(x => Random.value) // Shuffle the list
                        .Take(Mathf.Min(3, availableUpgrade.Count))              // Take the first 3 items
                        .ToList();

        freeUpgradeItem.ForEach(x => x.panelObject.SetActive(false));

        int i = 0;
        foreach(var item in freeUpgrade)
        {
            freeUpgradeItem[i].currentValue.text = item.oldValue.text;
            freeUpgradeItem[i].upgradeValue.text = item.newValue.text;
            freeUpgradeItem[i].attributeName.text = item.attributeName.text;
            freeUpgradeItem[i].upgradeButton.onClick.RemoveAllListeners();
            freeUpgradeItem[i].upgradeButton.onClick.AddListener(() => { Constants.Get.SilverCoin += item.attribute.upgradeCost; item.upgradeButton.onClick?.Invoke(); gameObject.SetActive(false); Time.timeScale = timeScale; });

            freeUpgradeItem[i].panelObject.SetActive(true);
            i++;
        }

        Time.timeScale = 0;
        gameObject.SetActive(true);

        return true;
    }
}