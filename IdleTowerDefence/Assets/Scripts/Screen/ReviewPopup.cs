#if UNITY_ANDROID
using Google.Play.Review;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_IOS
using UnityEngine.iOS;
#endif
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class ReviewPopup : ScreenPanel
{
    [SerializeField] private List<Button> starButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button laterButton;
    [SerializeField] private Button rateButton;

    private int SelectedStar;

#if UNITY_ANDROID
    private ReviewManager _reviewManager;
    private PlayReviewInfo _playReviewInfo;
#endif
#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void RequestInAppReview();
#endif

    private void Start()
    {
        
    }

    private void OnEnable()
    {
        StartCoroutine(ReviewRequestCoroutine());

        foreach (var item in starButton)
        {
            item.transform.GetChild(0).gameObject.SetActive(false);
        }
        SelectedStar = 0;
        starButton[0].onClick.AddListener(() => { OnStartClick(1); });
        starButton[1].onClick.AddListener(() => { OnStartClick(2); });
        starButton[2].onClick.AddListener(() => { OnStartClick(3); });
        starButton[3].onClick.AddListener(() => { OnStartClick(4); });
        starButton[4].onClick.AddListener(() => { OnStartClick(5); });

        closeButton.onClick.AddListener(OnClick_Close);
        laterButton.onClick.AddListener(OnClick_Close);
        rateButton.onClick.AddListener(OnClick_Rate);
    }

    private void OnClick_Close()
    {
        Hide();
    }

    private void OnClick_Rate()
    {
        if (SelectedStar <= 0)
            return;

        if (SelectedStar == 1)
            GameAnalyticsManager.Instance.NewDesignEventGA("Rate_Game_Review :" + "1_star");
        //AppsFlyerEventManager.Get.SendEvent(AppsFlyerEventManager.AF_REVIEW_1_STAR, "1");
        else if (SelectedStar == 2)
            GameAnalyticsManager.Instance.NewDesignEventGA("Rate_Game_Review :" + "2_star");
        //AppsFlyerEventManager.Get.SendEvent(AppsFlyerEventManager.AF_REVIEW_2_STAR, "1");
        else if (SelectedStar == 3)
            GameAnalyticsManager.Instance.NewDesignEventGA("Rate_Game_Review :" + "3_star");
        //AppsFlyerEventManager.Get.SendEvent(AppsFlyerEventManager.AF_REVIEW_3_STAR, "1");
        else if (SelectedStar == 4)
            GameAnalyticsManager.Instance.NewDesignEventGA("Rate_Game_Review :" + "4_star");
        //AppsFlyerEventManager.Get.SendEvent(AppsFlyerEventManager.AF_REVIEW_4_STAR, "1");
        else
            GameAnalyticsManager.Instance.NewDesignEventGA("Rate_Game_Review :" + "5_star");
        //AppsFlyerEventManager.Get.SendEvent(AppsFlyerEventManager.AF_REVIEW_5_STAR, "1");

        if (SelectedStar <= 3)
        {
            OnClick_Close();
            ToastManager.Get.ShowMessage("Appreciate your rating, Thanks for your support!!");
            return;
        }

        rateButton.onClick.RemoveAllListeners();
        ActiveGameData.Instance.saveData.reviewGiven = 1;
        StartCoroutine(ReviewCoroutine());
    }

    private void OnStartClick(int v)
    {
        SelectedStar = v;
        for (int i = 0; i < starButton.Count; i++)
        {
            if (i < SelectedStar)
                starButton[i].transform.GetChild(0).gameObject.SetActive(true);
            else
                starButton[i].transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    IEnumerator ReviewRequestCoroutine()
    {
        yield return null;

#if UNITY_ANDROID
        if (_reviewManager == null)
            _reviewManager = new ReviewManager();

        Debug.Log("Review Requesting");
        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        Debug.Log("Review Request Done");
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            Debug.LogError(requestFlowOperation.Error.ToString());
            yield break;
        }
        _playReviewInfo = requestFlowOperation.GetResult();
#endif
    }

    IEnumerator ReviewCoroutine()
    {
        yield return null;

#if UNITY_ANDROID
        Debug.Log("Review Flow Request");
        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
        yield return launchFlowOperation;
        Debug.Log("Review Flow Done");
        _playReviewInfo = null; // Reset the object
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            Debug.LogError(launchFlowOperation.Error.ToString());
            OnClick_Close();
            ToastManager.Get.ShowMessage("Appreciate your rating, Thanks for your support!!");
            yield break;
        }
#endif
#if UNITY_IOS
        // Device.RequestStoreReview();
        RequestInAppReview();
#endif
        OnClick_Close();
        ToastManager.Get.ShowMessage("Appreciate your rating, Thanks for your support!!");
    }

    private void OnDisable()
    {
        starButton[0].onClick.RemoveAllListeners();
        starButton[1].onClick.RemoveAllListeners();
        starButton[2].onClick.RemoveAllListeners();
        starButton[3].onClick.RemoveAllListeners();
        starButton[4].onClick.RemoveAllListeners();

        closeButton.onClick.RemoveAllListeners();
        laterButton.onClick.RemoveAllListeners();
        rateButton.onClick.RemoveAllListeners();
    }
}
