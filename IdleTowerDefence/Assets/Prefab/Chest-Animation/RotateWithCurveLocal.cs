using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class RotateWithCurveLocal : MonoBehaviour
{
	#region PUBLIC_VARS
	public Transform me;
	public float speed;
	public AnimationCurve curve;
	public Vector3 startRotation;
	public Vector3 endRotation;
	public float onDisablePoint = 0f;
	public bool useLoop = true;
	
	[HideInInspector]
	public bool mI_startRotation = false;

	public UnityEvent afterAnimation;
	#endregion

	#region PRIVATE_VARS
	private float i = 0;
	#endregion

	#region UNITY_CALLBACKS
	private void Start()
	{
		if (me == null)
		{
			me = this.transform;
		}
	}

	private void OnDisable()
	{
		me.localEulerAngles = Vector3.Lerp(startRotation, endRotation, curve.Evaluate(onDisablePoint));
	}

	private void Update()
	{
		if(mI_startRotation)
        {
			i += Time.unscaledDeltaTime * speed;
			if (i <= 1)
				me.localEulerAngles = Vector3.Lerp(startRotation, endRotation, curve.Evaluate(i));
			else
			{
				if (afterAnimation != null)
				{
					afterAnimation.Invoke();
				}
				if (!useLoop)
					this.enabled = false;
				me.localEulerAngles = Vector3.Lerp(startRotation, endRotation, curve.Evaluate(1));
				i = 0;
			}
		}
		
	}
	#endregion

	#region PUBLIC_FUNCTIONS
	#endregion

	#region PRIVATE_FUNCTIONS
	#endregion

	#region CO-ROUTINES
	#endregion

	#region EVENT_HANDLERS
	#endregion

	#region UI_CALLBACKS
	#endregion
}