﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
//using System.Net;
//using System.Text;
using VRDataLib.Data;
using VRDataLib.Connection;
//using UnityEngine.UI;

namespace VRDataLib {

	namespace Data {

		public class VRDataObject {

			//Online events
			public delegate void sendRequest ();
			public static event sendRequest OnSendRequest;

			public const int OBJS_PER_REQUEST = 7;
			public static string REQUEST_STATUS = "";

			public static List <VRDataObject> allDataObjs = new List <VRDataObject>();

			public ArrayList objData = new ArrayList();

			public VRDataObject(string _modelType, Transform _targetObj, Dictionary <string, string> _args) {

				//Keep room for different models
				switch (_modelType) {

				case ("A"):

					objData = VRDataObjectBuilder.buildModel_A(_targetObj, _args);
					break;

				default:
					Debug.LogError("VR data model type not defined, check your VRDataObject");
					break;

				}

				//HACK: Not handling async properly
				allDataObjs.Add(this);

				if(allDataObjs.Count >= OBJS_PER_REQUEST) {

					if (OnSendRequest != null) {

						Debug.Log ("Sufficient objects in data list, sending now.");
						OnSendRequest ();

					}

				}

			}

			public static void clearAllData() {
				
				allDataObjs.Clear();
				Debug.Log ("Cleared all object data from memory");
			
			}

			public void updateVRDataObject () {

				//TODO: Update values

			}

		}

		public static class VRDataObjectBuilder {

			public static ArrayList _data;
			public const int PRECISION = 2;

			public static ArrayList buildModel_A (Transform _targetObj, Dictionary <string, string> _args) {

				_data = new ArrayList();

				nTuple <string>
				_scene,
				_poi,
				_duration,
				_timestamp,
				_interaction;

				nTupleList <float>
				_pos,
				_rot,
				_scale;

				List <nTuple<float>>
				_posVal = new List <nTuple <float>> (
					new nTuple <float> [] {
						new nTuple <float>  ("x", (float) Math.Round(_targetObj.transform.position.x, PRECISION)),
						new nTuple <float>  ("y", (float) Math.Round(_targetObj.transform.position.y, PRECISION)),
						new nTuple <float>  ("z", (float) Math.Round( _targetObj.transform.position.z,PRECISION))
					}),
				
				_rotVal = new List <nTuple <float>> (
					new nTuple <float> [] {
						new nTuple <float> ("x", (float) Math.Round(_targetObj.transform.rotation.x, PRECISION)),
						new nTuple <float> ("y", (float) Math.Round(_targetObj.transform.rotation.y, PRECISION)),
						new nTuple <float> ("z", (float) Math.Round(_targetObj.transform.rotation.z, PRECISION))
					}),
				
				_scaleVal = new List <nTuple <float>> (
					new nTuple <float> [] {
						new nTuple <float> ("x", (float) Math.Round(_targetObj.transform.localScale.x, PRECISION)),
						new nTuple <float> ("y", (float) Math.Round(_targetObj.transform.localScale.y, PRECISION)),
						new nTuple <float> ("z", (float) Math.Round(_targetObj.transform.localScale.z, PRECISION))
					});

				_scene 			= new nTuple 		<string> 	("scene", SceneManager.GetActiveScene().name);
				_poi 			= new nTuple 		<string> 	("poi", _targetObj.name);
				_pos 			= new nTupleList 	<float> 	("pos", _posVal);
				_rot			= new nTupleList 	<float> 	("rot", _rotVal);
				_scale			= new nTupleList 	<float>  	("scale", _scaleVal);
				_duration	 	= new nTuple 		<string> 	("duration", _args["duration"]);
				_interaction 	= new nTuple 		<string> 	("interaction", _args["interaction"].ToString());
				_timestamp	 	= new nTuple 		<string> 	("timestamp", _args["timestamp"]);

				_data.Add(_scene);
				_data.Add(_poi);
				_data.Add(_pos);
				_data.Add(_rot);
				_data.Add(_scale);
				_data.Add(_duration);
				_data.Add(_interaction);
				_data.Add(_timestamp);

				return _data;

			}

			public static string buildVRDataString (bool createFile) {
				
				StringBuilder.stringifyData(createFile);

				return StringBuilder.getDataString();

			}

		}

		public static class OfflineBuffer {

			public static List<string> offlineBufferList = new List <string> ();

		}

		public static class StringBuilder {

			private static string data = "";

			public static string getDataString() {
				
				return data;

			}

			public static string stringifyData(bool _createFile) {

				//clear data first
				data = "[";

				int index = 0;

				foreach (VRDataObject _obj in VRDataObject.allDataObjs){

					index ++;

					data += stringifyObj(_obj);
					if(index < VRDataObject.allDataObjs.Count) {

						data += ", ";

					} else {

						data += "";

					}

				}

				data += "]";

				if (_createFile){

					//TODO: write to file
					return data;

				} else {

					return data;

				}

			}

			private static string stringifyObj(VRDataObject _obj) {

				string tupleData = "{";
				int index = 0;

				foreach (nTuple _tup in _obj.objData) {

					index++;

					tupleData += _tup.convertToString();

					if (index < _obj.objData.Count) {

						tupleData += ", ";

					} else {

						tupleData += "";

					}

				}

				tupleData += "}";
				return tupleData;

			}

		}

		#region Tuple

		public class nTuple: IEnumerable{

			public IEnumerator<nTuple> GetEnumerator()
			{
				return this.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			} 

			public virtual ArrayList getItems (){return null;}
			public virtual string convertToString (){return null;}

		}

		public class nTuple <T1> : nTuple{

			protected string key1;
			protected T1 value1;
			internal string stringTuple;
			internal int numVals;

			public nTuple (string _key1, T1 _value1) {

				this.key1 = _key1;
				this.value1 = _value1;

			}

			public override ArrayList getItems() {

				return new ArrayList (){key1, value1};

			}

			public override string convertToString() {
				
				stringTuple = "\"" + this.key1 + "\"" + ": " + "\"" + this.value1.ToString() + "\"";

				return stringTuple;

			}

		}

		public class nTupleList <T1> : nTuple {

			protected string key1;
			protected List<nTuple<T1>> value1;
			internal string stringTuple;
			internal int numVals;

			public nTupleList (string _key1, List<nTuple<T1>> _value1) {

				this.key1 = _key1;
				this.value1 = _value1;

			}

			public override ArrayList getItems() {

				return new ArrayList (){key1, value1};

			}

			public override string convertToString() {

				int itr = 0;

				stringTuple = "\"" + key1 + "\"" + ": ";

				stringTuple += "{";

				foreach (nTuple _tup in value1) {

					itr++;
					stringTuple += _tup.convertToString ();

					if (itr < value1.Count) {
						stringTuple += ", ";
					} else {
						stringTuple += "";
					}

				}

				stringTuple += "}";

				return stringTuple;

			}

		}

		#endregion Tuple

	}

	namespace Connection {

		public static class Session {

			internal static bool connectionActive = false;

			public static void writeToOfflineBuffer () {

				Debug.Log ("Unable to send request, writing to offline buffer");

			}

		}

		//TODO: Upgrade to System.net
		/*
		public static class POST {

			public static void sendPostRequest (string _url, Dictionary<string, string> _params) {

				WebRequest request 	= WebRequest.Create(_url);
				request.Method 		= "POST";
				byte[] byteData 	= new byte[_params.Count];

				int dataIndex = 0;

				foreach (KeyValuePair<string, string> param in _params) {

					string fieldData = "{" + param.Key + ", " + param.Value + "}";
					byteData [dataIndex] = Encoding.UTF8.GetBytes(fieldData);
					dataIndex++;

				}

				request.ContentType 	= "application/json";
				request.ContentLength 	= byteData.Length;

				Stream dataStream = request.GetRequestStream();
				dataStream.Write(byteData, 0, byteData.Length);
				dataStream.Close();

				WebResponse response = request.GetResponse();
				Debug.Log(((HttpWebResponse) response).StatusDescription);
				response.Close();


			}

		}*/


	}
}

public class VRData : MonoBehaviour {

	//Offline events
	public delegate void bufferOffline ();
	public static event bufferOffline OnConnectionUnavailable;

	public static bool canLook = false;

	public string
		URL_INIT,
		URL_DATA;

	private string msg;

	//public Text message;


	public IEnumerator postInit(){

		yield return null;

		WWWForm form = new WWWForm ();
		form.AddField ("appid", "123");
		form.AddField ("deviceid", "456");

		WWW www = new WWW(URL_INIT, form);
		StartCoroutine(HTTPRequest(www, "initRequest"));

		Debug.Log ("Init()");
	}

	public IEnumerator postData(string _data){

		yield return null;

		WWWForm form = new WWWForm ();
		form.AddField ("data", _data);
		form.AddField ("appid", "123");
		form.AddField ("userid", "23");
		form.AddField ("sessionid", msg);

		WWW www = new WWW(URL_DATA, form);
		StartCoroutine(HTTPRequest(www, "dataRequest"));
	}

	public IEnumerator sendDataCo(string _data) {
		
		yield return null;
		StartCoroutine(postData(_data));
		Debug.Log ("Data()");

	}

	IEnumerator HTTPRequest(WWW www, string _flag){

		yield return www;

		// check for errors
		if (www.error == null)
		{
			
			Session.connectionActive = true;

			msg = www.text;

			//Check type of request
			switch (_flag) {

			case ("initRequest"):

				Debug.Log ("API init success!");
				canLook = true;

				//TODO: hook coroutine to event
				//Start listening to sendDataRequests
				//VRDataObject.OnSendRequest += handleSendData;
				VRDataObject.OnSendRequest += handleSendData;
				break;

			case("dataRequest"):

				Debug.Log ("Data sent successfully!\n" + VRDataObjectBuilder.buildVRDataString (false));

				break;

			}

		} else {
			
			Debug.Log("Error: "+ www.error);

			Session.connectionActive = false;

			switch (_flag) {

			case ("initRequest"):
				
				OnConnectionUnavailable ();
                //HACK: Handle canLook differently in offline mode
                canLook = true;
				break;

			case("dataRequest"):
				//TODO: write to file
				break;

			}

		}

	}

	#region event_handling
	internal void handleSendData () {

		//yield return null;

		string _data = VRDataObjectBuilder.buildVRDataString(false);
		Debug.Log (_data);
		VRDataObject.clearAllData ();

		StartCoroutine (sendDataCo (_data));

		/*
		if (Session.connectionActive) {

			//TODO: Wait for vr data to build
			//string _data = yield return VRDataObjectBuilder.buildVRDataString(false);
			string _data = VRDataObjectBuilder.buildVRDataString(false);
			Debug.Log (_data);
			VRDataObject.clearAllData ();
			StartCoroutine (sendDataCo (_data));

		} else {

			string _data = VRDataObjectBuilder.buildVRDataString(true);
			Debug.Log (_data);
			VRDataObject.clearAllData ();

		}*/

	}
	#endregion event_handling

	void OnEnable () {
		
	}

	void Start () {

		URL_INIT= "YOUR URL";
		URL_DATA = "YOUR URL";

        //Listen when to send request
        StartCoroutine(postInit());

		OnConnectionUnavailable += Session.writeToOfflineBuffer;

		//Create "Resources" directory to write data file when no connection
		if (!Directory.Exists("Assets/Resources")) {
			
			Directory.CreateDirectory ("Assets/Resources");
			Debug.Log ("created dir");

		}

	}

}