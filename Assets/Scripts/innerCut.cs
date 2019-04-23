using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
/*
 * This Class represents the inner part of a 'cut' 
 * A cut is similar to a free variable but has some special conditions
 * for example if it is true or false which is indicated by its color on a graph
 */

public class innerCut : MonoBehaviour {
	public GameObject parent;
	Vector3 offset;
	public bool mouseOver, mouseDown, mouseDragging;
	nodeManager nManager;
	mousePointer mPointer;
	// Use this for initialization
	delete deleteMode;
	SpriteRenderer renderer;
	Color tru, fal;
	GameObject dot_copy = null;
	List<GameObject> children;
	void Start () {
		//innerCut_
		string parName = gameObject.name.Substring(gameObject.name.IndexOf ('_') + 1);
		parent = GameObject.Find ("cut_" + parName);
		deleteMode = GameObject.Find ("DeleteButton").GetComponent<delete> ();
		nManager = GameObject.Find ("lev_0").GetComponent<nodeManager>();
		mPointer = Camera.main.GetComponent<mousePointer> ();
		renderer = GetComponent<SpriteRenderer> ();

		ColorUtility.TryParseHtmlString ("#FFFFFFBD", out tru);
		ColorUtility.TryParseHtmlString ("#A3A3A3BD", out fal);

		children = new List<GameObject> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!parent)
			return;

		renderer.color = (renderer.sortingOrder % 2 != 0) ? tru : fal;
		parent.transform.position = transform.position;
	}

	void OnMouseEnter(){
		nManager.currentItem = gameObject;
	}

	void OnMouseOver(){
		mouseOver = true;

		if(nManager.currentItem == gameObject)
			parent.GetComponent<cut> ().showHighlight (true);
		float scroll = Input.GetAxis ("Mouse ScrollWheel");

		//on scroll scale the innercut and its border up or down
		if (scroll == 0f)
			return;
		if (scroll > 0f) {
			transform.localScale += new Vector3 (scroll, scroll, 0);
		} else if (scroll < 0f) {
			transform.localScale -= new Vector3 (-scroll, -scroll, 0);
		}
		parent.transform.localScale = transform.localScale;
	}

	void OnMouseExit(){
		mouseOver = false;
		nManager.currentItem = null;
		parent.GetComponent<cut> ().showHighlight (false);
	}

	void OnMouseDown(){
		mouseDown = true;
		if (mouseOver)
			nManager.currentItem = gameObject;

		//since deleting a gameobject doesn't trigger 'onTriggerExit2D' we make the object invisible and
		//manually move it outside, then delete it
		if (deleteMode.getDeleteMode ()){
			List<GameObject> children = getAllChildCuts ();
			GetComponent<SpriteRenderer>().enabled = false;
			parent.GetComponent<SpriteRenderer> ().enabled = false;
			transform.position = new Vector3 (-200, -200, -200);
			parent.transform.position = transform.position;
			Destroy (gameObject);
			Destroy (parent);

			//update all the nested cuts now that the base level has changed
			foreach (GameObject i in children) {
				cascadeUpdateLevel (false, i);
			}
			return;
		}

		//if shift click then add this object to list of select objects
		if (Input.GetKey (KeyCode.LeftShift)) {
			mPointer.addSelectedObject (gameObject);
		}

		//handle touch inputs and scaling
		if (Input.touchCount == 2) {
			//hanlde zoom out here
			Touch touchZero = Input.GetTouch (0);
			Touch touchOne = Input.GetTouch (1);

			// Find the position in the previous frame of each touch.
			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

			// Find the magnitude of the vector (the distance) between the touches in each frame.
			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

			// Find the difference in the distances between each frame.
			float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

			if (deltaMagnitudeDiff == 0f)
				return;
			if (deltaMagnitudeDiff > 0f) {
				transform.localScale += new Vector3 (deltaMagnitudeDiff, deltaMagnitudeDiff, 0);
			} else if (deltaMagnitudeDiff < 0f) {
				transform.localScale -= new Vector3 (-deltaMagnitudeDiff, -deltaMagnitudeDiff, 0);
			}
			parent.transform.localScale = transform.localScale;
		} else {
			offset = transform.position - GetHitPoint ();
		}
		//if there are any selected objects move them with this one
		if (!mPointer.selectedObjects.Contains (gameObject))
			return;
		foreach (GameObject j in mPointer.selectedObjects){
			j.transform.SetParent (transform);
		}
	}

	void copy(GameObject dot){
		dot.transform.position = GetHitPoint ();
	}

	private Dictionary<GameObject, Vector2>  getChildVarHelper(List<GameObject> childVars){
		Dictionary<GameObject, Vector2> vars;
		Vector3 child_pos = gameObject.transform.position;
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

	void copyRelease(GameObject dot){

		dot.GetComponent<copy> ().copyIntoCut (gameObject);

		nManager.createCutFromCopy (gameObject, GetHitPoint (), getChildVarHelper(getChildVars()));
		foreach (GameObject i in children) {
			nManager.createCutFromCopy (i, GetHitPoint(), getChildVarHelper(i.GetComponent<innerCut>().getChildVars()    ) );
		}

		Destroy (dot);
	}

	void OnMouseDrag(){
		mouseDragging = true;
		GameObject dot = GameObject.Find ("Dot");
		if (deleteMode.copyMode) {
			if(!dot_copy)
				dot_copy = Instantiate (dot, GetHitPoint (), Quaternion.identity);

			children = getAllChildCuts ();
			copy (dot_copy);
			return;
		}



		nManager.currentItem = gameObject;
		transform.position = GetHitPoint () + offset;
		parent.transform.position = transform.position;


		//get everything this cut collides with thats smaller than it and parent them for smooth movement
		PolygonCollider2D coll = gameObject.GetComponent<PolygonCollider2D> ();
		Collider2D[] overlap = Physics2D.OverlapAreaAll (coll.bounds.min, coll.bounds.max);
		foreach (Collider2D i in overlap) {
			if(!isCoveringObject(i))
				continue;
			if (i == GetComponent<Collider2D> ())
				continue;
			if (i.gameObject.name.Contains ("cut")) 
				continue;
			if (i.gameObject.name.Contains ("innerCut") && !isGreater (i) && !isEqual(i) ) {
				i.transform.SetParent (transform);
			} else if(i.GetComponent<SpriteRenderer>().sortingLayerName == "Variables") {
				i.transform.SetParent (transform);
			}
		}
		//if there are any selected objects move them with this one
		if (!mPointer.selectedObjects.Contains (gameObject))
			return;
		foreach (GameObject j in mPointer.selectedObjects){
			j.transform.SetParent (transform);
		}
	}

	void OnMouseUp(){
		mouseDragging = false;

		if (deleteMode.copyMode) {
			copyRelease(dot_copy);
			return;
		}


		PolygonCollider2D coll = gameObject.GetComponent<PolygonCollider2D> ();
		Collider2D[] overlap = Physics2D.OverlapAreaAll (coll.bounds.min, coll.bounds.max);
		foreach (Collider2D i in overlap) {
			i.transform.SetParent (null);
		}
		if (mPointer.selectedObjects.Contains (gameObject)) {
			foreach (GameObject i in mPointer.selectedObjects) {
				i.transform.SetParent (null);
			}
		}
		transform.localPosition = new Vector3 (transform.position.x, transform.position.y, 0f);
		mouseDown = false;
	}

	//normalize the mouseposition from a 3d perspective to a 2d one,
	//this makes moving elements smoother
	private Vector3 GetHitPoint(){
		Plane plane = new Plane (Camera.main.transform.forward, transform.position);
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		float dist;
		plane.Raycast (ray, out dist);
		return ray.GetPoint(dist);
	}

	void OnTriggerEnter2D(Collider2D collision){
		if (deleteMode.copyMode && collision.tag == "draggable" && collision.name.Contains("cut")) {
			//are my borders colliding
			if(collision.gameObject != parent){
				//Debug.Log (gameObject.name + " is colliding with " + collision.name);
				//Debug.Log (isGreater (collision));
				//Debug.Log("game object " + GetComponent<PolygonCollider2D>().bounds);
				//Debug.Log(collision.GetComponent<PolygonCollider2D>().bounds);
			}
		}

		if (collision.GetComponent<SpriteRenderer>().sortingLayerName == "Variables") {
			collision.GetComponent<SpriteRenderer> ().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 1;
			return;
		}
		if (!collision.name.Contains ("innerCut"))
			return;

		if (!isGreater (collision) && !isEqual (collision)) {
			//the other cut is smaller than this one
			updateLevel (true, collision.gameObject);
		} else if (isGreater (collision)) {
			//the other cut is larger than this one
		} else {
			//the other cut is the same size as this one
		}
	}

	void OnTriggerExit2D(Collider2D collision){
		if (collision.GetComponent<SpriteRenderer> ().sortingLayerName == "Variables") {
			collision.GetComponent<SpriteRenderer> ().sortingOrder = GetComponent<SpriteRenderer> ().sortingOrder - 1;
			return;
		}

		if (!collision.name.Contains ("innerCut"))
			return;

		if (!isGreater (collision) && !isEqual (collision)) {
			//the other cut is smaller than this one
			updateLevel (false, collision.gameObject);
		} else if (isGreater (collision)) {
			//the other cut is larger than this one
		} else {
			//the other cut is the same size as this one
		}
	}

	public cut_data getCutData(GameObject g){
		if (g.GetComponent<innerCut> ()) {
			return g.GetComponent<innerCut> ().parent.GetComponent<cut_data> ();
		} else {
			return g.GetComponent<cut_data> ();
		}
	}

	public innerCut getInnerCut(GameObject g){
		if (g.GetComponent<innerCut> ()) {
			return g.GetComponent<innerCut> ();
		} else {
			return g.GetComponent<cut> ().child.GetComponent<innerCut>();
		}
	}

	private bool isIn(Collider2D[] list, Collider2D obj){
		for (int i = 0; i < list.Length; ++i) {
			if (list [i] == obj) {
				return true;
			}
		}
		return false;
	}

	public Collider2D[] getOverLap(){
		PolygonCollider2D coll = gameObject.GetComponent<PolygonCollider2D> ();
		return Physics2D.OverlapAreaAll (coll.bounds.min, coll.bounds.max);
	}

	//is cut b greater than this cut
	public bool isGreater( Collider2D b){
		PolygonCollider2D thisCollider = GetComponent<PolygonCollider2D> ();
		PolygonCollider2D otherCollider = b.GetComponent<PolygonCollider2D> ();
		Vector3 thisSize = thisCollider.bounds.size;
		Vector3 otherSize = otherCollider.bounds.size;

		return (thisSize.x < otherSize.x && thisSize.y < otherSize.y);
	}

	//is this cut equal to cut b
	bool isEqual(Collider2D b){
		PolygonCollider2D thisCollider = GetComponent<PolygonCollider2D> ();
		PolygonCollider2D otherCollider = b.GetComponent<PolygonCollider2D> ();
		return thisCollider.bounds.size == otherCollider.bounds.size;
	}

	public void updateLevel(bool increase, GameObject obj){
		obj.GetComponent<SpriteRenderer> ().sortingOrder += (increase) ? 1 : -1;
	}
		
	public void cascadeUpdateLevel(bool increase, GameObject obj){
		obj.GetComponent<SpriteRenderer> ().sortingOrder += (increase) ? 1 : -1;
		foreach (GameObject i in getAllChildCuts()) {
			i.GetComponent<SpriteRenderer> ().sortingOrder += (increase) ? 1 : -1;
		}
	}

	public List<GameObject> getAllChildren(){
		List<GameObject> ret = new List<GameObject> ();
		ret.AddRange (getAllChildCuts ());
		ret.AddRange (getAllChildCuts ());
		return ret;
	}

	//get every cut nested within this one
	public List<GameObject> getAllChildCuts(){
		PolygonCollider2D coll = gameObject.GetComponent<PolygonCollider2D> ();
		Collider2D[] overlap = Physics2D.OverlapAreaAll (coll.bounds.min, coll.bounds.max);
		List<GameObject> result = new List<GameObject> ();
		foreach (Collider2D i in overlap) {
			if (!i.name.Contains ("innerCut"))
				continue;
			if (i.GetComponent<SpriteRenderer> ().sortingOrder > GetComponent<SpriteRenderer> ().sortingOrder )
				result.Add (i.gameObject);
		}
		return result;
	}

	//returns a list of gameobjects that are all the variables on the same layer as this cut
	public List<GameObject> getChildVars(){
		PolygonCollider2D coll = gameObject.GetComponent<PolygonCollider2D> ();
		Collider2D[] overlap = Physics2D.OverlapAreaAll (coll.bounds.min, coll.bounds.max);
		List<GameObject> result = new List<GameObject> ();
		foreach (Collider2D i in overlap) {
			if (i.name.Contains ("innerCut") || i.name.Contains("cut"))
				continue;
			if (i.GetComponent<SpriteRenderer> ().sortingOrder - 1 == GetComponent<SpriteRenderer> ().sortingOrder )
				result.Add (i.gameObject);
		}
		return result;
	}

	bool isCoveringObject(Collider2D other){
		return GetComponent<PolygonCollider2D> ().bounds.Contains (other.bounds.max) &&
		GetComponent<PolygonCollider2D> ().bounds.Contains (other.bounds.min);
	}
}
