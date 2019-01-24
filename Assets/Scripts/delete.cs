using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class delete : MonoBehaviour {
    bool deleteMode;
    Image deleteBtn;
    Color highlightedCol;
	// Use this for initialization
	void Start () {
        deleteMode = false;
        highlightedCol = new Color32(255,80,80,255);
        deleteBtn = gameObject.GetComponent<Image>();
    }
    public bool getDeleteMode() { return deleteMode;}
    public void setDeleteMode() {
        if (deleteMode) {
            deleteMode = false;
            deleteBtn.color = Color.white;
        } else {
            deleteMode = true;
            deleteBtn.color = highlightedCol;
        }
    }
}
