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
	delete deleteBtn;
	public List<GameObject> selectedObjects;
	public delete dMode;
	public nodeManager nManager;
    // Use this for initialization
    void Start () {
        pointerIcon = Resources.Load("pointer") as Texture2D;
        scaleIcon = Resources.Load("Square") as Texture2D;
		deleteBtn = GameObject.Find ("DeleteButton").GetComponent<delete> ();
		selectionBox = GameObject.Find ("selection-square");
        cursorMode = CursorMode.Auto;
        hotSpot = Vector2.zero;
		selectedObjects = new List<GameObject> ();
    }

	void merge(){
		Vector3 parent_pos = selectedObjects [0].transform.position;
		List<GameObject> children = selectedObjects [1].GetComponent<innerCut> ().getAllChildCuts ();
		foreach (GameObject i in children) {
			i.transform.position = parent_pos;
		}
		selectedObjects [1].transform.position = selectedObjects [0].transform.position;
			
		selectedObjects.Clear ();
	}

	private Dictionary<GameObject, Vector2>  getChildVarHelper(List<GameObject> childVars){
		Dictionary<GameObject, Vector2> vars;
		Vector3 child_pos = selectedObjects [0].transform.position;
		if (childVars.Count == 0) {
			vars = null;
		} else {
			vars = new Dictionary<GameObject, Vector2> ();
			foreach (GameObject i in childVars) {
				Vector2 tmp = new Vector2( i.transform.position.x -  child_pos.x , i.transform.position.y - child_pos.y   );
				vars.Add (i, tmp);
			}
		}
		return vars;
	}

	private bool isIn(Collider2D[] list, Collider2D obj){
		for (int i = 0; i < list.Length; ++i) {
			if (list [i] == obj) {
				return true;
			}
		}
		return false;
	}

	public cut_data getCutData(GameObject g){
		if (g.GetComponent<innerCut> ()) {
			return g.GetComponent<innerCut> ().parent.GetComponent<cut_data> ();
		} else {
			return g.GetComponent<cut_data> ();
		}
	}

	//copy one cut into another should be called from a GUI button click
	public void copy(){
		if (!deleteBtn.copyMode)
			return;
	
		//both selected objects must be cuts
		if (!selectedObjects [1].name.ToLower ().Contains ("cut") || !selectedObjects [0].name.ToLower ().Contains ("cut"))
			return;

		Vector3 par_pos = selectedObjects [1].transform.position;
		List<GameObject> childVars =  selectedObjects[0].GetComponent<innerCut> ().getChildVars ();

		Dictionary<GameObject, Vector2> vars = getChildVarHelper (childVars);

		GameObject copy  = nManager.createCutFromCopy(selectedObjects[0],par_pos, vars);



		selectedObjects [0].transform.SetParent (null);
		selectedObjects [1].transform.SetParent (null);

		GameObject copy_to = selectedObjects [1];
		GameObject copy_from = selectedObjects [0];
	

		foreach (GameObject i in selectedObjects[0].GetComponent<innerCut>().getAllChildCuts()) {
			childVars = i.GetComponent<innerCut> ().getChildVars ();
			vars = getChildVarHelper (childVars);
			nManager.createCutFromCopy (i, par_pos, vars);
		}

		//FIXME: this should be broken into multiple functions
		if(copy_to.GetComponent<innerCut>().isGreater(copy_from.GetComponent<Collider2D>())){
			Collider2D[] before_overlap = copy_to.GetComponent<innerCut>().getOverLap ();
			selectedObjects [1].transform.localScale += selectedObjects [0].transform.localScale;
			selectedObjects [1].GetComponent<innerCut> ().parent.transform.localScale += selectedObjects [0].transform.localScale;

			Collider2D[] after_overlap = copy_to.GetComponent<innerCut>().getOverLap ();

			List<Collider2D> overlap = new List<Collider2D> ();
			for (int j = 0; j < after_overlap.Length; ++j) {
				if(! isIn(before_overlap,  after_overlap[j] )){
					overlap.Add(after_overlap[j]);
				}
			}

			foreach (Collider2D i in overlap) {
				if (copy_to.transform.position.x <= i.transform.position.x && copy_to.layer == i.gameObject.layer) {
					cut_data cd = getCutData (copy_to);
					Vector3 offset = new Vector3 (copy_from.transform.position.x + cd.getWidth () / 4, copy_from.transform.position.y, 0f);
					copy_from.transform.position = offset;
					foreach (GameObject j in copy_from.GetComponent<innerCut>().getChildVars()) {
						j.transform.position = offset;
					}
				} else {
					cut_data cd = getCutData (copy_to);
					Vector3 offset = new Vector3 (copy_from.transform.position.x - cd.getWidth () / 4, copy_from.transform.position.y, 0f);
					copy_from.transform.position = offset;
					foreach (GameObject j in copy_from.GetComponent<innerCut>().getChildVars()) {
						j.transform.position = offset;
					}
				}

				//y

				if (copy_to.transform.position.y <= i.transform.position.y && copy_to.layer == i.gameObject.layer) {
					cut_data cd = getCutData (copy_to);
					Vector3 offset = new Vector3 (copy_from.transform.position.x, copy_from.transform.position.y + cd.getHeight()/4, 0f);
					copy_from.transform.position = offset;
					foreach (GameObject j in copy_from.GetComponent<innerCut>().getChildVars()) {
						j.transform.position = offset;
					}
				} else {
					cut_data cd = getCutData (copy_to);
					Vector3 offset = new Vector3 (copy_from.transform.position.x, copy_from.transform.position.y - cd.getHeight()/4, 0f);
					copy_from.transform.position = offset;
					foreach (GameObject j in copy_from.GetComponent<innerCut>().getChildVars()) {
						j.transform.position = offset;
					}
				}
			}
		}

		selectedObjects.Clear ();
		deleteBtn.setCopyMode ();
	}

	void Update(){
		//keyboard shortcuts
		if (Input.GetKeyDown (KeyCode.Delete))
			dMode.setDeleteMode ();

		if (Input.GetKeyDown (KeyCode.M))
			merge ();


		if (deleteBtn.copyMode && selectedObjects.Count == 2) {
			copy ();
		}

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
				//box = Instantiate (selectionBox, GetHitPoint(), Quaternion.identity);
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
