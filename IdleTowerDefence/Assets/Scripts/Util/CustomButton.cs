using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CustomButton : Button, IPointerDownHandler, IPointerUpHandler
{
    public bool CanAnimate = true;

    [HideInInspector]
    public bool isPressed;

    private RectTransform rectTransform;

    public override void OnPointerClick(PointerEventData eventData)
    {
        if(IsInteractable())
            AudioManager.Instance?.PlayUITapSound();
        base.OnPointerClick(eventData);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (CanAnimate && IsInteractable())
        {
            if (rectTransform == null)
            {
                rectTransform = transform.GetComponent<RectTransform>();

                // If Pivot is not center, then make it center
                if (rectTransform.pivot.x != 0.5f || rectTransform.pivot.y != 0.5f)
                {
                    SetPivot(rectTransform, new Vector2(0.5f, 0.5f));
                }
            }

            LeanTween.scale(this.gameObject, new Vector3(0.92f, 0.92f, 1f), 0.1f).setEaseOutBack().setIgnoreTimeScale(true);
        }
        base.OnPointerDown(eventData);
        isPressed = true;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (CanAnimate)
            LeanTween.scale(this.gameObject, new Vector3(1, 1, 1f), 0.15f).setEaseOutBack().setIgnoreTimeScale(true);
        base.OnPointerUp(eventData);
        isPressed = false;
    }

    private void SetPivot(RectTransform rectTransform, Vector2 pivot)
    {
        Vector3 deltaPosition = rectTransform.pivot - pivot;    // get change in pivot
        deltaPosition.Scale(rectTransform.rect.size);           // apply sizing
        deltaPosition.Scale(rectTransform.localScale);          // apply scaling
        deltaPosition = rectTransform.rotation * deltaPosition; // apply rotation

        rectTransform.pivot = pivot;                            // change the pivot
        rectTransform.localPosition -= deltaPosition;           // reverse the position change
    }
}
