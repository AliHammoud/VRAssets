using System;
using UnityEngine;
using UnityEngine.UI;

// This class ensures that the UI (such as the reticle and selection bar)
// are set up correctly.
public class VRCameraUI : MonoBehaviour
{
	[SerializeField] private Canvas m_Canvas;       // Reference to the canvas containing the UI.

	public RectTransform tooltipUI;
	public Transform tooltipWorld;

	private void Awake()
	{
		// Make sure the canvas is on.
		m_Canvas.enabled = true;

		// Set its sorting order to the front.
		m_Canvas.sortingOrder = Int16.MaxValue;

		// Force the canvas to redraw so that it is correct before the first render.
		Canvas.ForceUpdateCanvases();

	}

	#region tooltips

	public void ShowTooltipUI(GameObject target) {

		Vector3 onScreenPos = this.GetComponent<Camera> ().WorldToViewportPoint (target.transform.position);
		Debug.Log (onScreenPos);

		//TODO: Offset
		tooltipUI.anchoredPosition = onScreenPos;

		//Rotate the tooltip to match the angle of the object you're looking at 
		float dot = Vector3.Dot (target.transform.forward, this.transform.forward);
		float alpha = dot / Mathf.Deg2Rad * ((Vector3.Magnitude (target.transform.forward) * Vector3.Magnitude (this.transform.forward)));
		Debug.Log (alpha);

		if (dot < 0.5)
			alpha *= -1;

		tooltipUI.localEulerAngles = new Vector3 (0, alpha, 0);

	}

	public void ShowTooltipWorld(Vector3 target) {

		//TODO: Animate
		tooltipWorld.position = target;

	}

	public void HideTooltipUI() {
		
		tooltipUI.localEulerAngles = Vector3.zero;
		tooltipUI.anchoredPosition = new Vector3 (0f, 950f, 0f);

	}

	public void HideTooltipWorld() {
		
		tooltipWorld.position = this.transform.position;

	}

	#endregion tooltips

}