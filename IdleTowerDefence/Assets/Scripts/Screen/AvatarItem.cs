using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AvatarItem : MonoBehaviour
{
    [SerializeField] private CustomButton button;
    [SerializeField] private Image icon;

    internal void Set(AvatarInfo info, Action<AvatarInfo> callback)
    {
        icon.sprite = info.avatar;

        button.onClick.AddListener(() => { callback?.Invoke(info); });
    }
}
