using Facebook.Unity;
using UnityEngine;

public class FacebookInitializer : MonoBehaviour
{
    void Awake()
    {
        if (!FB.IsInitialized)
        {
            FB.Init(OnInitComplete, OnHideUnity);
        }
        else
        {
            FB.ActivateApp();
        }
    }

    private void OnInitComplete()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
            Debug.Log("Facebook SDK Initialized.");
        }
        else
        {
            Debug.LogError("Failed to initialize Facebook SDK.");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        Time.timeScale = isGameShown ? 1 : 0;
    }
}