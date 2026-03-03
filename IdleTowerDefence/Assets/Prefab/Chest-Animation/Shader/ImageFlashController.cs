using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageFlashController : MonoBehaviour
{
    // public Material _flashLight;
    public Material mat;
    [Range(0f,1f)] public float _flashValue;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        mat.SetFloat("_FlashAmount", _flashValue);
    }
}
