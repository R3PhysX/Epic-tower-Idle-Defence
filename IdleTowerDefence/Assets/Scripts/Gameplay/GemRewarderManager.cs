using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemRewarderManager : MonoBehaviour
{

    public static GemRewarderManager Get;

    public GameObject rewarder;
    public GameObject gemIcon;
    public CustomButton button;

    public float minScale = 0.9f;
    public float maxScale = 1f;
    public float breathDuration = 0.92f;

    public Transform[] waypoints; // Array of waypoints (set these in the Unity Inspector)
    public float moveSpeed = 1f;

    private void Awake()
    {
        Get = this;
    }

    void StartBreathing()
    {
        // Scale down
        LeanTween.scale(rewarder.gameObject, new Vector3(minScale, minScale, minScale), breathDuration / 2)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnComplete(() =>
            {
                // Scale up
                LeanTween.scale(rewarder.gameObject, new Vector3(maxScale, maxScale, maxScale), breathDuration / 2)
                    .setEase(LeanTweenType.easeInQuad)
                    .setOnComplete(StartBreathing); // Repeat the breathing effect
            }).setIgnoreTimeScale(true);
    }

    [ContextMenu("ShowRewarder")]
    internal void ShowRewarder()
    {
        LeanTween.cancel(rewarder.gameObject);
        StartBreathing();
        Debug.LogError("Listener Added");
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick_RewardGem);
        Vector3[] pathPoints = new Vector3[waypoints.Length];
        for (int i = 0; i < waypoints.Length; i++)
        {
            pathPoints[i] = waypoints[i].position;
        }

        // Calculate the total distance of the path to determine the animation duration
        float totalDistance = 0f;
        for (int i = 0; i < pathPoints.Length - 1; i++)
        {
            totalDistance += Vector3.Distance(pathPoints[i], pathPoints[i + 1]);
        }
        float pathDuration = totalDistance / moveSpeed;

        // Use LeanTween to move the object along the path
        rewarder.gameObject.SetActive(true);
        LeanTween.moveSpline(rewarder, pathPoints, pathDuration).setEase(LeanTweenType.linear).setIgnoreTimeScale(true).setOnComplete(()=> {
            rewarder.gameObject.SetActive(false);
        });

        return;
    }

    public void OnClick_RewardGem()
    {
        Debug.LogError("Clicked");
        button.onClick.RemoveAllListeners();
        LeanTween.cancel(rewarder.gameObject);
        ScreenManager.Get.GetScreen<GemRewarderPopup>().Show();
        rewarder.gameObject.SetActive(false);
    }
}