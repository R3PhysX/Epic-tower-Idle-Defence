using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MenuTabData
{
    public int index;
    public CustomButton tabButton;
    public ScreenPanel screem;
    public GameObject lockObject;
    public int lockTillWave;
}

public class TabScreen : MonoBehaviour
{
    [SerializeField] private List<MenuTabData> tabData;

    private int currentTabIndex = -1;
    private float screenHeight;
    private float screenWidth;

    private float SwipeSpeed = 0.35f;

    private bool isMoving = false;

    private void OnEnable()
    {
        
    }

    private void Start()
    {
        screenHeight = Camera.main.orthographicSize * 2.0f;
        screenWidth = screenHeight * Camera.main.aspect;
        foreach (var item in tabData)
        {
            item.screem.Hide();
            item.tabButton.onClick.AddListener(() => { OnClickTab(item); });
            if (item.lockTillWave > ActiveGameData.Instance.saveData.bestWave[0])
            {
                for (int i = 0; i < item.tabButton.transform.childCount; i++)
                    item.tabButton.transform.GetChild(i).gameObject.SetActive(false);
                item.lockObject.gameObject.SetActive(true);
            }
        }
        OnClickTab(tabData[2]);
    }

    private void OnClickTab(MenuTabData menuTabData)
    {
        if (isMoving == true)
            return;

        isMoving = true;

        if (menuTabData.lockTillWave > ActiveGameData.Instance.saveData.bestWave[0])
        {
            ToastManager.Get.ShowMessage("Reach Wave " + menuTabData.lockTillWave + " To Unlock");
            isMoving = false;
            return;
        }

        if (menuTabData.index == 1)
        {
            if (TutorialManager.Get != null && TutorialManager.Get.tutorialVariables.ContainsKey("ClickOnFactory") && TutorialManager.Get.tutorialVariables["ClickOnFactory"] == true)
                EventManager.TriggerEvent(EventID.TutorialNextStep);
        }
        else if (menuTabData.index == 2)
        {
            if (TutorialManager.Get != null && TutorialManager.Get.tutorialVariables.ContainsKey("ClickOnHome") && TutorialManager.Get.tutorialVariables["ClickOnHome"] == true)
                EventManager.TriggerEvent(EventID.TutorialNextStep);
        }
        else if (menuTabData.index == 3)
        {
            if (TutorialManager.Get != null && TutorialManager.Get.tutorialVariables.ContainsKey("ClickOnCard") && TutorialManager.Get.tutorialVariables["ClickOnCard"] == true)
                EventManager.TriggerEvent(EventID.TutorialNextStep);
        }

        if (currentTabIndex != -1)
        {
            if (currentTabIndex < menuTabData.index)
            {
                ScreenPanel scr = tabData[currentTabIndex].screem;
                LeanTween.moveX(tabData[currentTabIndex].screem.gameObject, tabData[currentTabIndex].screem.transform.position.x - screenWidth, SwipeSpeed).setOnComplete(() =>
                {
                    scr.Hide();
                });
            }
            else if (currentTabIndex > menuTabData.index)
            {
                ScreenPanel scr = tabData[currentTabIndex].screem;
                LeanTween.moveX(tabData[currentTabIndex].screem.gameObject, tabData[currentTabIndex].screem.transform.position.x + screenWidth, SwipeSpeed).setOnComplete(() =>
                {
                    scr.Hide();
                });
            }
        }

        if (currentTabIndex == -1)
        {
            currentTabIndex = menuTabData.index;
            if (tabData[currentTabIndex].screem.gameObject.activeSelf == false)
                tabData[currentTabIndex].screem.Show();
            isMoving = false;
        }
        else
        {
            if (currentTabIndex > menuTabData.index)
            {
                currentTabIndex = menuTabData.index;
                tabData[currentTabIndex].screem.transform.position = Vector3.right * screenWidth * -1;
                if (tabData[currentTabIndex].screem.gameObject.activeSelf == false)
                    tabData[currentTabIndex].screem.Show();
                LeanTween.moveX(tabData[currentTabIndex].screem.gameObject, tabData[currentTabIndex].screem.transform.position.x + screenWidth, SwipeSpeed).setOnComplete(()=> { isMoving = false; });
            }
            else if (currentTabIndex < menuTabData.index)
            {
                currentTabIndex = menuTabData.index;
                tabData[currentTabIndex].screem.transform.position = Vector3.right * screenWidth;
                if (tabData[currentTabIndex].screem.gameObject.activeSelf == false)
                    tabData[currentTabIndex].screem.Show();
                LeanTween.moveX(tabData[currentTabIndex].screem.gameObject, tabData[currentTabIndex].screem.transform.position.x - screenWidth, SwipeSpeed).setOnComplete(() => { isMoving = false; });
            }
            else
            {
                isMoving = false;
            }
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < tabData.Count; i++)
        {
            tabData[i].tabButton.onClick.RemoveAllListeners();
        }
    }
}
