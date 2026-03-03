using GoogleMobileAds.Api;
using UnityEngine;
using System;

public class AdManager : MonoBehaviour
{
    public static AdManager Get;

    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;

    // Ad IDs
    [Header("AdMob IDs")]
    [SerializeField] private string androidInterstitialAdId = "ca-app-pub-8631672248244079/1315719195";
    [SerializeField] private string androidRewardedAdId = "ca-app-pub-8631672248244079/3526949566";
    [SerializeField] private string iOSInterstitialAdId = "ca-app-pub-8631672248244079/1798763480";
    [SerializeField] private string iOSRewardedAdId = "ca-app-pub-8631672248244079/3502727391";

    private string interstitialAdId;
    private string rewardedAdId;

    private float lastAdTime;
    [SerializeField] private int interstitialInterval = 60;
    [SerializeField] private bool canRunOnEditorInterstitial = false;
    [SerializeField] private bool canRunOnEditorReward = false;

    private bool rewardGranted;
    private Action<bool> rewardCallback;

    private void Awake()
    {
        Get = this;

#if UNITY_ANDROID
        interstitialAdId = androidInterstitialAdId;
        rewardedAdId = androidRewardedAdId;
#elif UNITY_IOS
        interstitialAdId = iOSInterstitialAdId;
        rewardedAdId = iOSRewardedAdId;
#endif
    }

    private void Start()
    {
#if UNITY_EDITOR
        if (!canRunOnEditorInterstitial && !canRunOnEditorReward)
            return;
#endif

        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("Google Mobile Ads Initialized (10.6.0)");
            LoadInterstitialAd();
            LoadRewardedAd();
        });

        lastAdTime = Time.realtimeSinceStartup;
    }

    // ---------------- INTERSTITIAL ----------------
    public void LoadInterstitialAd()
    {
#if UNITY_EDITOR
        if (!canRunOnEditorInterstitial) return;
#endif

        interstitialAd?.Destroy();
        interstitialAd = null;

        Debug.Log("Loading Interstitial Ad...");
        var adRequest = new AdRequest();

        InterstitialAd.Load(interstitialAdId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null)
                {
                    Debug.LogError("Interstitial Load Failed: " + error);
                    return;
                }
                interstitialAd = ad;
                RegisterInterstitialEvents();
                Debug.Log("Interstitial Loaded");
            });
    }

    public void ShowInterstitialAd()
    {
#if UNITY_EDITOR
        if (!canRunOnEditorInterstitial) return;
#endif

        if (Time.realtimeSinceStartup - lastAdTime < interstitialInterval)
            return;

        lastAdTime = Time.realtimeSinceStartup;

        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            interstitialAd.Show();
        }
        else
        {
            Debug.Log("Interstitial Not Ready");
            LoadInterstitialAd();
        }
    }

    private void RegisterInterstitialEvents()
    {
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial Closed → Reloading");
            LoadInterstitialAd();
        };

        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial Failed to show: " + error);
            LoadInterstitialAd();
        };
    }

    // ---------------- REWARDED ----------------
    public void LoadRewardedAd(bool forceLoad = false)
    {
#if UNITY_EDITOR
        if (!canRunOnEditorReward) return;
#endif

        rewardedAd?.Destroy();
        rewardedAd = null;

        Debug.Log("Loading Rewarded Ad...");
        var adRequest = new AdRequest();

        RewardedAd.Load(rewardedAdId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null)
                {
                    Debug.LogError("Rewarded Load Failed: " + error);
                    return;
                }
                rewardedAd = ad;
                RegisterRewardedEvents();
                Debug.Log("Rewarded Loaded");
            });
    }

    public bool IsRewardAdAvailable()
    {
        return rewardedAd != null && rewardedAd.CanShowAd();
    }

    public void ShowRewardedAd(Action<bool> callback, bool forceAd = false)
    {
#if UNITY_EDITOR
        if (!canRunOnEditorReward)
        {
            callback?.Invoke(true);
            return;
        }
#endif

        rewardCallback = callback;
        rewardGranted = false;

        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                rewardGranted = true;
                Debug.Log("Reward Earned: " + reward.Type);
            });
        }
        else
        {
            Debug.Log("Rewarded Ad Not Ready");
            callback?.Invoke(false);
            LoadRewardedAd();
        }
    }

    private void RegisterRewardedEvents()
    {
        rewardedAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded Closed");
            LeanTween.delayedCall(0.5f, () =>
            {
                rewardCallback?.Invoke(rewardGranted);
                rewardCallback = null;
                rewardGranted = false;
            }).setIgnoreTimeScale(true);

            LoadRewardedAd();
        };

        rewardedAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded Failed to Show: " + error);
            LoadRewardedAd();
        };
    }

    private void OnDestroy()
    {
        interstitialAd?.Destroy();
        rewardedAd?.Destroy();
    }
}
