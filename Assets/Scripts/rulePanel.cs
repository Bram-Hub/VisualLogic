using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rulePanel : MonoBehaviour {
    public GameObject rules;
    bool isActive = false;
	// Use this for initialization
	void Start () {
        rules.SetActive(false);
	}
    public void show() {
        isActive = (isActive) ? false : true;
        rules.SetActive(isActive);
    }
}
