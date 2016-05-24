using UnityEngine;
using System;
using VRDataLib.Data;
using System.Collections;
using System.Collections.Generic;

// This script is a simple example of how an interactive item can
// be used to change things on gameobjects by handling events.
public class InteractiveSceneItem : MonoBehaviour
{
	
	public VRInteractiveItem m_InteractiveItem;
	public Renderer m_Renderer;

	private Dictionary<string, string> args;

	private bool 
	isLooking 	= false, 
	isPOI 		= false;

	public static float FOCUS_TIME = 0.75f;
	private float lookAtTime;

	private Vector3 scl;


	void Start()
	{
		
		m_InteractiveItem.OnOver += HandleOver;
		m_InteractiveItem.OnOut += HandleOut;
		m_InteractiveItem.OnClick += HandleClick;
		m_InteractiveItem.OnDoubleClick += HandleDoubleClick;

		scl = this.transform.localScale;

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

		if (VRData.canLook) {

			isLooking = true;

			this.transform.localScale = this.transform.localScale * 1.15f;

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
		this.transform.localScale = scl;

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