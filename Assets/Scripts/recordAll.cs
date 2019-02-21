using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using GetSocialSdk;
using System;
#endif

public class recordAll : MonoBehaviour {
	public bool recording;
	#if UNITY_EDITOR
	public GetSocialSdk.Capture.Scripts.GetSocialCapture capture;
	#endif
	// Use this for initialization
	void Start () {
		#if UNITY_EDITOR
		capture.captureMode = GetSocialSdk.Capture.Scripts.GetSocialCapture.GetSocialCaptureMode.Manual;
		#endif
		recording = false;
	}

	public void startStopRecord(){
		#if UNITY_EDITOR
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
		#endif
	}

	// Update is called once per frame
	void Update () {
		#if UNITY_EDITOR
		if(Input.GetKeyDown(KeyCode.R))
			startStopRecord();
		#endif
	}
}
