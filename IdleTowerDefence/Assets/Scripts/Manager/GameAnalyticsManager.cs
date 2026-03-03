using GameAnalyticsSDK;
using UnityEngine;

public class GameAnalyticsManager : MonoBehaviour
{
    public static GameAnalyticsManager Instance;

    public void BusineesEventTracker(string currency, int amount, string itemType, string itemId, string cartType, string receipt, string signature)
    {
        // GameAnalytics.NewBusinessEventGooglePlay(currency, amount, itemType, itemId, cartType, receipt, signature);
    }

    public void NewDesignEventGA(string eventName)
    {
        GameAnalytics.NewDesignEvent(eventName);
    }

    public void NewProgressonEventGA(GAProgressionStatus gAProgressionStatus,string world, string wave)
    {
        GameAnalytics.NewProgressionEvent(gAProgressionStatus, world, wave);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        GameAnalytics.Initialize();
    }

}
