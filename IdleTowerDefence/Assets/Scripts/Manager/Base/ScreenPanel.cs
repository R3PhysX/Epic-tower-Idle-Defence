using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScreenPanel : MonoBehaviour
{

    private void Awake()
    {
       // gameObject.SetActive(false);
    }

    public virtual void Show()
    {
        gameObject?.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject?.SetActive(false);
    }
}