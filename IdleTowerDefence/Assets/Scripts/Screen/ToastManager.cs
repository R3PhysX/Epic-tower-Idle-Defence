using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToastManager : MonoBehaviour
{
    public static ToastManager Get;
    public TMP_Text textMessage;
    public GameObject messageObject;
    public CanvasGroup canvasGroup;

    private void Awake()
    {
        Get = this;
    }

    public void ShowMessage(string text)
    {
        LeanTween.cancel(messageObject.gameObject);
        canvasGroup.alpha = 1f;
        textMessage.text = text;
        messageObject.gameObject.SetActive(true);

        LeanTween.delayedCall(messageObject.gameObject, 0.75f, () =>
        {
            LeanTween.value(messageObject.gameObject, (val) =>
            {
                canvasGroup.alpha = val;
            }, 1f, 0f, 0.75f).setEase(LeanTweenType.easeOutSine).setIgnoreTimeScale(true)
.setOnComplete(() => messageObject.gameObject.SetActive(false));
        }).setIgnoreTimeScale(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ShowMessage("Test");
        }
    }
}
