using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {
    // Use this for initialization
    public timelineController tController;
    bool visible;
    void Start () {
        visible = true;
	}
	// Update is called once per frame
	void Update () {
        Color selected;
        selected = (tController.isCurrent(gameObject)) ? Color.green : Color.red;
        gameObject.GetComponent<CanvasRenderer>().SetColor(selected);
	}
    private void OnMouseDown() {
        tController.setCurrentNode(gameObject);
    }
}