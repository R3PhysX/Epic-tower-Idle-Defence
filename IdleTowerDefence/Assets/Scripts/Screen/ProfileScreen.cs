using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileScreen : ScreenPanel
{
    [SerializeField] private AvatarData avatarData;
    [SerializeField] private AvatarItem avatarPrefab;
    [SerializeField] private ScrollRect avatarContainer;

    [SerializeField] private CustomButton saveButton;
    [SerializeField] private CustomButton closeButton;

    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private Image profileIcon;

    private AvatarInfo currentAvatar;

    private void OnEnable()
    {
        saveButton.onClick.AddListener(OnClick_Save);
        closeButton.onClick.AddListener(OnClick_Close);

        var profile = avatarData.avatarInfo.Find(x => x.id == ActiveGameData.Instance.saveData.avatarId);
        OnClick_Avatar(profile);

        usernameField.text = ActiveGameData.Instance.saveData.username;
    }

    private void OnClick_Save()
    {
        ActiveGameData.Instance.saveData.avatarId = currentAvatar.id;
        ActiveGameData.Instance.saveData.username = usernameField.text;

        ScreenManager.Get.GetScreen<HomeScreen>().UpdateProfile();
        Hide();
    }

    private void OnClick_Close()
    {
        Hide();
    }

    private void Start()
    {
        foreach(var item in avatarData.avatarInfo)
        {
            var obj = Instantiate(avatarPrefab.gameObject, avatarContainer.content).GetComponent<AvatarItem>();
            obj.Set(item, OnClick_Avatar);
        }
    }

    private void OnClick_Avatar(AvatarInfo info)
    {
        profileIcon.sprite = info.avatar;
        currentAvatar = info;
    }

    private void OnDisable()
    {
        saveButton.onClick.RemoveAllListeners();
        closeButton.onClick.RemoveAllListeners();
    }
}