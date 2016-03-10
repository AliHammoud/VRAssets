using UnityEngine;
using System;
using VRDataLib.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

// This script is a simple example of how an interactive item can
// be used to change things on gameobjects by handling events.
public class InteractiveSceneItem : MonoBehaviour
{
	#region InitAttributes

	/// A reference to an instance of VRInteractive Item required by the InteractiveSceneItem object
	private VRInteractiveItem m_InteractiveItem;

	/// The referenced renderer of the interactive scene item.
	private Renderer m_Renderer;

	/// The data arguments for the transfer of VR user data.
	private Dictionary<string, string> args;

	///Object material in the default state.
	public Material m_defaultMat;

	///Material for when object is looked at.
	public Material m_overMat;

	#endregion InitAttributes

	#region LookAtParams

	private bool
	enableLookatScale 	= false,
	isLooking 			= false, 
	isPOI 				= false;

	public static float FOCUS_TIME = 0.75f;
	private float lookAtTime;

	private Vector3 initialScale;

	#endregion LookAtParams

	void Start()
	{
		
		m_InteractiveItem = new VRInteractiveItem();
		m_Renderer = this.GetComponent<MeshRenderer> ();
		Debug.Log (m_Renderer);

		m_InteractiveItem.OnOver += HandleOver;
		m_InteractiveItem.OnOut += HandleOut;
		m_InteractiveItem.OnClick += HandleClick;
		m_InteractiveItem.OnDoubleClick += HandleDoubleClick;

		initialScale = this.transform.localScale;

		m_Renderer.material = m_defaultMat;

	}


	public void OnDisable()
	{
		
		m_InteractiveItem.OnOver -= HandleOver;
		m_InteractiveItem.OnOut -= HandleOut;
		m_InteractiveItem.OnClick -= HandleClick;
		m_InteractiveItem.OnDoubleClick -= HandleDoubleClick;

	}


	//Handle the Over event
	public void HandleOver()
	{
		m_Renderer.material = m_overMat;

		if (enableLookatScale) {

			this.transform.localScale = this.transform.localScale * 1.15f;

		}

		Debug.Log ("Over");

		if (VRData.canLook) {

			isLooking = true;

			StartCoroutine (letFocus());

		}

	}

	private IEnumerator letFocus() {

		args = new Dictionary<string, string>();

		lookAtTime = Time.timeSinceLevelLoad;

		yield return new WaitForSeconds (FOCUS_TIME);

		if (isLooking) {

			isPOI = true;

		} else {

			isPOI = false;

		}

	}


	//Handle the Out event
	public void HandleOut()
	{
		this.transform.localScale = initialScale;

		m_Renderer.material = m_defaultMat;

		if (isPOI) {

			//Get look duration
			double lookDuration = Time.timeSinceLevelLoad - lookAtTime;
			lookDuration = Math.Round (lookDuration, VRDataObjectBuilder.PRECISION);
			args.Add ("duration", lookDuration.ToString());

			//Get timestamp (seconds since 01/01/1970)
			long ticks = DateTime.UtcNow.Ticks - DateTime.Parse("01/01/1970 00:00:00").Ticks;
			ticks /= 10000000; //Convert windows ticks to seconds
			args.Add ("timestamp", ticks.ToString());

			//Set interaction type
			args.Add("interaction", "focus");

			Debug.Log ("Focused on: " + this.GetComponent<Transform> ().name + " for " + lookDuration + " secs");

			//Create a new data object
			//working
			//VRDataObject obj = new VRDataObject ("A", this.GetComponent<Transform>(), args);
			//experiment
			new VRDataObject ("A", this.GetComponent<Transform>(), args);

		}

		isLooking = false;
		isPOI = false;

	}


	//Handle the Click event
	public void HandleClick()
	{

		Debug.Log("Show click state");

	}


	//Handle the DoubleClick event
	public void HandleDoubleClick()
	{
		
		Debug.Log("Show double click");

	}
}