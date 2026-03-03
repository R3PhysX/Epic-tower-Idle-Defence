using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardSlotItem : MonoBehaviour
{
    [SerializeField] private GameObject isLockedPanel;
    [SerializeField] private TMP_Text cost;
    internal InventoryCardItem attachedItem;

    [SerializeField] private int gemsRequired;

    [SerializeField] private CustomButton buySlot;
    private Action<int> OnBought;

    private int index;

    private void OnEnable()
    {
        buySlot.onClick.AddListener(OnClick_Buy);
        cost.text = gemsRequired.ToString();
    }

    private void OnClick_Buy()
    {
        if(Constants.Get.Gems >= gemsRequired)
        {
            Constants.Get.Gems -= gemsRequired;
            isLockedPanel.gameObject.SetActive(false);
            OnBought?.Invoke(index);
            EventManager.TriggerEvent(EventID.Update_Currency, null);
        }
        else
        {
            ToastManager.Get.ShowMessage("Not Enough Gems");
        }
    }

    internal void Set(int index, Action<int> onBought = default)
    {
        this.index = index;

        bool unlocked = ActiveGameData.Instance.saveData.cardSlotUnlocked[index] == 1;
        isLockedPanel.gameObject.SetActive(!unlocked);
        OnBought = onBought;
    }

    private void OnDisable()
    {
        buySlot.onClick.RemoveAllListeners();
    }
}
