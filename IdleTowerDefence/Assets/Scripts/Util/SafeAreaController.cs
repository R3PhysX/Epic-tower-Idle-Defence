using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeAreaController : MonoBehaviour
{
    private Rect _safeArea;
    [SerializeField] private RectTransform _rectTransform;

    // Start is called before the first frame update
    void Awake()
    {
        _safeArea = Screen.safeArea;
        if (_rectTransform == null)
            _rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        Adjust(_safeArea);
    }

    void Adjust(Rect safeArea)
    {
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        _rectTransform.anchorMin = anchorMin;
        _rectTransform.anchorMax = anchorMax;
    }
}


