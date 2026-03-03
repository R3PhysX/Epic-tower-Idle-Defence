using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class SwipeController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private float minSwipeDistance = 50f; // Minimum swipe distance in pixels

    [System.Serializable]
    public class SwipeEvent : UnityEvent { }

    public SwipeEvent OnLeftSwipe;
    public SwipeEvent OnRightSwipe;

    public void OnBeginDrag(PointerEventData eventData)
    {
        startTouchPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // You can use this method if you need to do something while dragging
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        endTouchPosition = eventData.position;
        HandleSwipe();
    }

    private void HandleSwipe()
    {
        if (Vector2.Distance(startTouchPosition, endTouchPosition) >= minSwipeDistance)
        {
            Vector2 swipeDirection = endTouchPosition - startTouchPosition;
            float xDifference = Mathf.Abs(swipeDirection.x);
            float yDifference = Mathf.Abs(swipeDirection.y);

            if (xDifference > yDifference)
            {
                // Horizontal Swipe
                if (swipeDirection.x > 0)
                {
                    Debug.Log("Right Swipe on Image");
                    OnRightSwipe?.Invoke();
                }
                else
                {
                    Debug.Log("Left Swipe on Image");
                    OnLeftSwipe?.Invoke();
                }
            }
        }
    }
}
