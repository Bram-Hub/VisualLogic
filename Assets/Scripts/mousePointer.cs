using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mousePointer : MonoBehaviour {
    static Texture2D pointerIcon, scaleIcon;
    CursorMode cursorMode;
    Vector2 hotSpot;
	Vector3 initial;
	Ray ray;
	RaycastHit2D hit;
	GameObject selectionBox, box = null;
	public List<GameObject> selectedObjects;
	public delete dMode;
    // Use this for initialization
    void Start () {
        pointerIcon = Resources.Load("pointer") as Texture2D;
        scaleIcon = Resources.Load("Square") as Texture2D;

		selectionBox = GameObject.Find ("selection-square");
        cursorMode = CursorMode.Auto;
        hotSpot = Vector2.zero;
		selectedObjects = new List<GameObject> ();
    }
	void Update(){
		//keyboard shortcuts
		if (Input.GetKeyDown (KeyCode.Delete))
			dMode.setDeleteMode ();

		bool beginDrag = false;
		if (Input.GetMouseButtonDown (0)) {
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
			if (!hit.collider) {
				beginDrag = true;

				foreach (GameObject i in selectedObjects) {
					if (i.GetComponent<innerCut> ())
						i.GetComponent<innerCut> ().parent.GetComponent<cut> ().showHighlight (false);
					else
						i.GetComponent<dragable> ().showHighlight (false);
				}
				selectedObjects.Clear ();
				box = Instantiate (selectionBox, GetHitPoint(), Quaternion.identity);
			}
		}

		if (Input.GetMouseButton (0)) {
			float diff = (initial - GetHitPoint()).magnitude;
			diff *= 1.25f;
			if(diff > 0){
				//box.transform.localScale = new Vector3(box.transform.localScale.x + diff, box.transform.localScale.y + diff, 0) ;
				//box.transform.position = new Vector3 (box.transform.position.x, box.transform.position.y, 0);
			}
		}

		if (Input.GetMouseButtonUp (0)) {
			Destroy (box);
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

	private Vector3 GetHitPoint(){
		Plane plane = new Plane (Camera.main.transform.forward, transform.position);
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		float dist;
		plane.Raycast (ray, out dist);
		return ray.GetPoint(dist);
	}
}
