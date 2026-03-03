using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasScalerAsPerDevice : MonoBehaviour
{
    private CanvasScaler scaler;
    private float threashold = 0.645f;

    private void Awake()
    {
        scaler = GetComponent<CanvasScaler>();

        float aspectRatio = (float)Screen.width / Screen.height;
        Debug.Log(aspectRatio);

        if (aspectRatio <= threashold)
        {
            scaler.matchWidthOrHeight = 0;
        }
        else if (aspectRatio > threashold)
        {
            scaler.matchWidthOrHeight = 1;
        }
        else
        {
            scaler.matchWidthOrHeight = 0.5f;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
            Awake();
    }
}
