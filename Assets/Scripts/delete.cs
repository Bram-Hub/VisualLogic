using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class delete : MonoBehaviour {
	public bool deleteMode, copyMode;
    Image deleteBtn, copyBtn;
    Color highlightedCol, copyHighlight;
	// Use this for initialization
	void Start () {
        deleteMode = false;
        highlightedCol = new Color32(255,80,80,255);
		ColorUtility.TryParseHtmlString ("#4286f4", out copyHighlight);
        deleteBtn = gameObject.GetComponent<Image>();
		copyBtn = GameObject.Find ("CopyButton").GetComponent<Image> ();
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
	public void setCopyMode(){
		if (copyMode) {
			copyMode = false;
			copyBtn.color = Color.white;
		} else {
			copyMode = true;
			copyBtn.color = copyHighlight;
		}
	}
}
