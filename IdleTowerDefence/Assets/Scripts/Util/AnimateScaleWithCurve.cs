//Batboy's Animation Script Collection

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimateScaleWithCurve : MonoBehaviour
{
    #region PUBLIC_VARIABLES
    public Transform targetObject;
    public Image grayedBG;

    [Space(10)]
    public float animationSpeed;
    public bool loop = false;
    public bool animateOnEnable = false;

    [Space(10)]
    public Vector3 startScale;
    public Vector3 endScale;

    [Space(10)]
    public AnimationCurve scaleCurve;
    #endregion

    #region PRIVATE_VARIABLES
    private Coroutine scaleCoroutine;
    #endregion

    #region UNITY_CALLBACKS
    private void Awake()
    {
        if (targetObject == null)
            targetObject = this.transform;
    }

    private void OnEnable()
    {
        if(animateOnEnable)
        {
            PlayAnimation();
        }
    }
    #endregion

    #region PUBLIC_METHODS
    public void PlayAnimation()
    {
        if (scaleCoroutine != null)
            StopCoroutine(scaleCoroutine);
        scaleCoroutine = StartCoroutine(DoScaleAnimation());
    }

    public void StopAnimation()
    {
        if (scaleCoroutine != null)
            StopCoroutine(scaleCoroutine);
    }
    #endregion

    #region PRIVATE_METHODS
    #endregion

    #region CO-ROUTINES
    private IEnumerator DoScaleAnimation()
    {
        grayedBG.enabled = false;
        bool isLoop = true;
        float i = 0;
        float rate = 1 / animationSpeed;
        while (isLoop)
        {
            while (i < 1)
            {
                i += rate * Time.fixedUnscaledDeltaTime;
                targetObject.localScale = Vector3.LerpUnclamped(startScale, endScale, scaleCurve.Evaluate(i));
                yield return null;
            }
            i = 1;
            targetObject.localScale = Vector3.LerpUnclamped(startScale, endScale, scaleCurve.Evaluate(i));
            isLoop = loop;
            i = 0;
        }
        grayedBG.enabled = true;
    }
    #endregion
}
