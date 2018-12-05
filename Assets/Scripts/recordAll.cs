using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Linq;
using UnityEditor.SceneManagement;
public class recordAll : MonoBehaviour {
	public bool recording;
	public GetSocialSdk.Capture.Scripts.GetSocialCapture capture;
	// Use this for initialization
	void Start () {
		recording = false;
	}

	public void startStopRecord(){
		if (recording) {
			Debug.Log ("exporting to gif");
			capture.StopCapture ();
			Action<byte[]> result = bytes => {

			};

			capture.GenerateCapture (result);
			//preview.Play ();
		} else {
			capture.StartCapture ();
		}
		recording = !recording;
	}

	// Update is called once per frame
	void Update () {
		
	}
}
