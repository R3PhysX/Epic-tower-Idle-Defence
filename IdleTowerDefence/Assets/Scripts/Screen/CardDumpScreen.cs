using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDumpScreen : ScreenPanel
{
    [SerializeField] private CardData cardData;
    [SerializeField] private CustomButton closeButton;
    [SerializeField] private CustomButton dumpButton;
    [SerializeField] private Transform cardDumpItemContainer;
    [SerializeField] private DumpCardItem dumpCardPrefab;

    [SerializeField] private GameObject goldCardBar;

    private List<DumpCardItem> cardItems = new List<DumpCardItem>();

    [SerializeField] private Image fillImage;
    private float fillCount;
    [SerializeField] private TMP_Text percentageText;
    

    [SerializeField] private Slider estimatedFillImage;
    private float estimatedFillCount;

    private int dumpCardCount;
    [SerializeField] private TMP_Text dumpCardCountText;

    [Header("Animation")]
    public Image cardItem;
    public GameObject CardContainerParent;
    public Transform CardInPosition;
    public Transform CardDestinationPosition;

    public Transform goldCardPosition;
    public Transform goldCardDestinationPosition;

    private UnityPool cardAnimationPool;

    public GameObject blocker;

    public RectTransform liquidImage;
    public int startHeight = 67;
    public int endHeight = 403;

    public float maxHeight = 511;
    public float minHeightl = 200;

    private void OnEnable()
    {
        closeButton?.onClick.AddListener(OnCLick_Close);
        dumpButton?.onClick.AddListener(OnCLick_Dump);
        cardItems.ForEach(x => x.ResetCard());

        fillCount = ActiveGameData.Instance.saveData.dumpedCardValue;
        estimatedFillCount = ActiveGameData.Instance.saveData.dumpedCardValue;

        fillImage.fillAmount = fillCount / cardData.godModeCardRequireDumpValue;
        estimatedFillImage.value = estimatedFillCount / cardData.godModeCardRequireDumpValue;

        percentageText.text = (fillImage.fillAmount * 100).ToString("0.00") + "%";
        
        float height = startHeight + (fillImage.fillAmount * endHeight);
        liquidImage.sizeDelta = new Vector2(liquidImage.sizeDelta.x, height);

        dumpCardCount = 0;
        dumpCardCountText.text = dumpCardCount.ToString();
    }

    private void Start()
    {
        cardAnimationPool = new UnityPool(cardItem.gameObject, 10, CardContainerParent.transform);

        cardItems.Clear();
        cardDumpItemContainer.DestroyAllChildren();
        foreach (var item in cardData.cards)
        {
            var obj = Instantiate(dumpCardPrefab.gameObject, cardDumpItemContainer).GetComponent<DumpCardItem>();
            obj.Set(this, item);
            cardItems.Add(obj);
        }
    }

    internal bool UpdateUI(string key = "none")
    {
        if (key.Equals("plus") && estimatedFillImage.value >= 1f)
        {
            ToastManager.Get.ShowMessage("Card Dump Limit Exceed!");
            return false;
        }

        estimatedFillCount = ActiveGameData.Instance.saveData.dumpedCardValue;
        dumpCardCount = 0;

        foreach (var item in cardItems)
        {
            estimatedFillCount += (item.dumpCount * item.data.cardValue);
            dumpCardCount += item.dumpCount;
        }

        float value = estimatedFillCount / cardData.godModeCardRequireDumpValue;
        estimatedFillImage.value = value;
        dumpCardCountText.text = dumpCardCount.ToString();

        return true;
    }

    private void OnCLick_Dump()
    {
        StartCoroutine(StartAnimating());
        return;
    }

    IEnumerator StartAnimating()
    {
        Debug.LogError("Couritne");
        blocker.gameObject.SetActive(true);
        yield return null;

        fillCount = ActiveGameData.Instance.saveData.dumpedCardValue;

        float cardAnimTime = (5f / (float)dumpCardCount);
        if (cardAnimTime > 1f)
            cardAnimTime = 1f;

        foreach (var item in cardItems)
        {
            if ((fillImage.fillAmount * 100) >= 100)
            {
                break;
            }
            else
            {
                for (int i = 0; i < item.dumpCount; i++)
                {
                    if ((fillImage.fillAmount * 100) >= 100)
                    {
                        break;
                    }
                    else
                    {
                        var card = cardAnimationPool.Get<Image>(CardContainerParent.transform);
                        card.sprite = item.data.cardImage;

                        var rectTransform = card.GetComponent<RectTransform>();
                        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, maxHeight);

                        card.transform.position = item.transform.position;

                        card.gameObject.SetActive(true);

                        LeanTween.move(card.gameObject, CardInPosition, cardAnimTime * 0.25f).setOnComplete(() =>
                        {
                            LeanTween.move(card.gameObject, CardDestinationPosition, cardAnimTime * 0.35f).setOnComplete(() =>
                            {
                                LeanTween.value(card.gameObject, maxHeight, minHeightl, cardAnimTime*  0.35f)
                                .setOnUpdate((float value) =>
                                {
                                    rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, value);
                                }).setOnComplete(() =>
                                {

                                    rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, maxHeight);
                                    cardAnimationPool.Add(card);

                                    fillCount += item.data.cardValue;

                                    item.data.savedData.cardCount -= 1;

                                    if (item.data.savedData.cardCount <= 0)
                                    {
                                        item.data.savedData.unlocked = 0;
                                    }

                                    item.Set();

                                    float filAmount = fillCount / cardData.godModeCardRequireDumpValue;
                                    if((ActiveGameData.Instance.saveData.dumpedCardValue/ cardData.godModeCardRequireDumpValue) < 0.25f && filAmount >= 0.25f)
                                    {
                                        //  AppsFlyerEventManager.Get.SendEvent(AppsFlyerEventManager.AF_Card_Dump_25_PER, "1");
                                        GameAnalyticsManager.Instance.NewDesignEventGA("Card_Dump_25");
                                    }
                                    if ((ActiveGameData.Instance.saveData.dumpedCardValue / cardData.godModeCardRequireDumpValue) < 0.5f && filAmount >= 0.5f)
                                    {
                                        // AppsFlyerEventManager.Get.SendEvent(AppsFlyerEventManager.AF_Card_Dump_50_PER, "1");
                                        GameAnalyticsManager.Instance.NewDesignEventGA("Card_Dump_50");
                                    }
                                    ActiveGameData.Instance.saveData.dumpedCardValue = fillCount;

                                    fillImage.fillAmount = filAmount;

                                    percentageText.text = Mathf.Clamp((fillImage.fillAmount * 100), 0, 100).ToString("0.00") + "%";

                                    float height = startHeight + (fillImage.fillAmount * endHeight);
                                    liquidImage.sizeDelta = new Vector2(liquidImage.sizeDelta.x, height);

                                });
                            });
                        });

                        yield return new WaitForSeconds(cardAnimTime);
                    }
                }
            }
        }

        yield return new WaitForSeconds(0.3f);

        foreach (var item in cardItems)
        {
            item.ResetCard();
        }

        if (percentageText.text.Contains("100"))
        {
            goldCardBar.SetActive(false);

            yield return new WaitForSeconds(0.5f);

            var card = cardAnimationPool.Get<Image>(CardContainerParent.transform);
            card.sprite = cardData.cards[4].cardImage;

            card.transform.position = goldCardPosition.transform.position;
            card.gameObject.SetActive(true);

            LeanTween.move(card.gameObject, goldCardDestinationPosition, 0.75f).setOnComplete(() =>
            {
                
            });

            yield return new WaitForSeconds(1.5f);

            cardData.cards[4].savedData.unlocked = 1;
            cardData.cards[4].savedData.cardCount = 1;
            cardData.cards[4].savedData.level = 3;

            ScreenManager.Get.GetScreen<CardsScreen>().UpdateCardUI();

            //  AppsFlyerEventManager.Get.SendEvent(AppsFlyerEventManager.AF_Card_Dump_100_PER, "1");
            GameAnalyticsManager.Instance.NewDesignEventGA("Card_Dump_100");
            blocker.gameObject.SetActive(false);

            Hide();
            yield break;
        }

        dumpCardCount = 0;
        dumpCardCountText.text = dumpCardCount.ToString();

        var scr = ScreenManager.Get.GetScreen<CardsScreen>();
        scr.UpdateCardUI();

        blocker.gameObject.SetActive(false);
    }

    private void OnCLick_Close()
    {
        ScreenManager.Get.GetScreen<CardDumpScreen>().Hide();
    }
    
    private void OnDisable()
    {
        closeButton?.onClick.RemoveAllListeners();
        dumpButton?.onClick.RemoveAllListeners();
    }
}
