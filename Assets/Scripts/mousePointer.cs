using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mousePointer : MonoBehaviour {
    static Texture2D pointerIcon, scaleIcon;
    CursorMode cursorMode;
    Vector2 hotSpot;
	Ray ray;
	RaycastHit2D hit;
	public List<GameObject> selectedObjects;
    // Use this for initialization
    void Start () {
        pointerIcon = Resources.Load("pointer") as Texture2D;
        scaleIcon = Resources.Load("Square") as Texture2D;
        cursorMode = CursorMode.Auto;
        hotSpot = Vector2.zero;
		selectedObjects = new List<GameObject> ();
    }
	void Update(){
		if (Input.GetMouseButtonDown (0)) {
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
			if (!hit.collider)
				selectedObjects.Clear ();
		}

		foreach (GameObject i in selectedObjects) {
			if (i.GetComponent<innerCut> ())
				i.GetComponent<innerCut> ().parent.GetComponent<cut> ().showHighlight (true);
			else
				i.GetComponent<dragable> ().showHighlight (true);
		}
	}
    public void resizeCutCursor() {
        Cursor.SetCursor(scaleIcon, hotSpot, cursorMode);
    }
	public void resetCursor() {
        Cursor.SetCursor(null, hotSpot, cursorMode);
    }
    public void mouseOverCursor() {
        Cursor.SetCursor(pointerIcon, hotSpot, cursorMode);
    }

	public void addSelectedObject(GameObject g){
		if (selectedObjects.Contains (g)) {
			selectedObjects.Remove (g);
			g.transform.SetParent (null);
		} else {
			selectedObjects.Add (g);
		}
	}
}
