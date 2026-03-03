using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Get;
    public List<ScreenPanel> screens;

    private void Awake()
    {
        Get = this;
    }

    private void Start()
    {
        screens = transform.GetComponentsInChildren<ScreenPanel>(true).ToList();
    }

    public T GetScreen<T>()
    {
        Type screenType = typeof(T);
        var screenObject =  transform.GetComponentInChildren<T>(true);
        return screenObject;
    }
}