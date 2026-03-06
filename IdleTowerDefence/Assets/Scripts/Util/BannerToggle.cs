using TMPro;
using UnityEngine;

public class BannerToggle : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform screensRoot;   // Your "Screens" object
    public GameObject bannerArea;       // Your "BannerArea" object (optional enable/disable)
    public RectTransform bonusBar;       // Your "BonusBar" object (optional enable/disable)
    public TextMeshProUGUI textMeshProUGUI;

    [Header("Layout")]
    public float bannerHeight = 120f;   // Must match BannerArea height
    public float slideSpeed = 12f;      // Higher = faster animation

    bool isOn;
    Vector2 targetPos;

    void Start()
    {
        if(ActiveGameData.Instance.saveData.enabled_NoAds)
        {
            bannerArea.SetActive(false);
            bonusBar.gameObject.SetActive(false);
        }
    }

    void Update()
    {
      //  if (screensRoot == null) return;

     //   screensRoot.anchoredPosition = Vector2.Lerp(
      //      screensRoot.anchoredPosition,
     //       targetPos,
     //       Time.unscaledDeltaTime * slideSpeed
     //   );
    }

    // Hook this to your button OnClick()
    public void ToggleBanner()
    {
        isOn = !isOn;
        ActiveGameData.Instance.saveData.enabled_BannerAd = isOn;

        if (bannerArea != null) bannerArea.SetActive(isOn);

        targetPos = screensRoot.anchoredPosition;
        targetPos.y = isOn ? bannerHeight : 0f;
        //screensRoot.anchoredPosition = targetPos;
        //screensRoot.offsetMax = isOn ? new Vector2(0, -bannerHeight) : new Vector2(0,0);
        screensRoot.offsetMin = isOn ? new Vector2(0, bannerHeight) : new Vector2(0, 0);
        bonusBar.anchoredPosition = isOn ? new Vector2(0, bannerHeight+30f) : new Vector2(0, 0);

        if (isOn) { AdManager.Get.LoadAndShow(); textMeshProUGUI.text = "Hide & Stop x2 Rewards"; } else { AdManager.Get.DestroyBanner(); textMeshProUGUI.text = "Enable x2 Rewards"; }

    }

    // Optional: explicit methods if you prefer
    public void ShowBanner()
    {
        isOn = true;
        if (bannerArea != null) bannerArea.SetActive(true);
        targetPos = screensRoot.anchoredPosition;
        targetPos.y = bannerHeight;
        screensRoot.offsetMin = isOn ? new Vector2(0, bannerHeight) : new Vector2(0, 0);
        bonusBar.anchoredPosition = isOn ? new Vector2(0, bannerHeight + 30f) : new Vector2(0, 0);
        AdManager.Get.LoadAndShow();
        textMeshProUGUI.text = "Hide & Stop x2 Rewards";
    }

    public void HideBanner()
    {
        isOn = false;
        if (bannerArea != null) bannerArea.SetActive(false);
        targetPos = screensRoot.anchoredPosition;
        targetPos.y = 0f;
        screensRoot.offsetMin = isOn ? new Vector2(0, bannerHeight) : new Vector2(0, 0);
        bonusBar.anchoredPosition = isOn ? new Vector2(0, bannerHeight + 30f) : new Vector2(0, 0);
        AdManager.Get.DestroyBanner();
        textMeshProUGUI.text = "Enable x2 Rewards";
    }
}
