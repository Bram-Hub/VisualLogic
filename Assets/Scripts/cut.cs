using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


/*
 * This Class represents the outer ring of a 'cut' 
 * A cut is similar to a free variable but has some special conditions
 * for example scaling a cut is controlled through the edge
 */

public class cut : MonoBehaviour{
	public GameObject child;
	//what is the mouse doing?
	bool mouseOver, mouseDown;
	//are we in select or delete mode?
	delete deleteMode;
	public GameObject highlight;
	Vector3 offset;
	Vector3 mouseDownPos;
	Vector3 center = new Vector3(8,5,0);

	void Start(){
		string childName = gameObject.name.Substring(gameObject.name.IndexOf ('_') + 1);
		child = GameObject.Find ("innerCut_" + childName);
		deleteMode = GameObject.Find ("DeleteButton").GetComponent<delete> ();
		highlight = transform.GetChild (0).gameObject;
		showHighlight (false);
	}

	void Update(){
		transform.position = new Vector3 (transform.position.x, transform.position.y, 0.0f);
		if (!child)
			return;
		GetComponent<SpriteRenderer> ().sortingOrder = child.GetComponent<SpriteRenderer> ().sortingOrder;
		transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder =  GetComponent<SpriteRenderer> ().sortingOrder;

		if (transform.localScale.x <= 0.15f) {
			transform.localScale = new Vector3 (0.16f, transform.localScale.y, 0);
			child.transform.localScale = transform.localScale;
		}

		if (transform.localScale.y <= 0.15f) {
			transform.localScale = new Vector3 (transform.position.x, 0.16f, 0);
			child.transform.localScale = transform.localScale;
		}
	}

	void OnMouseOver(){
		mouseOver = true;
		if(child.GetComponent<innerCut>().mouseOver)
			showHighlight (true);
	}
	void OnMouseDown(){
		mouseDown = true;
		if (deleteMode.getDeleteMode ()){
			Destroy (child);
			Destroy (gameObject);
		}
		offset = transform.position - GetHitPoint ();
		mouseDownPos = (GetHitPoint() - transform.position);
	}
	void OnMouseExit(){
		mouseOver = false;
		showHighlight (false);
	}
	void OnMouseUp(){
		mouseDown = false;
	}
	void OnMouseDrag(){
		float speed= 15f;

		Vector3 diffVect = ABS((transform.position - ( GetHitPoint() )) * 0.65f );
		float mouseDiff = (mouseDownPos - (GetHitPoint () - transform.position)).magnitude;
		if (mouseDiff <= 0.1)
			return;

		//change the scale unless the difference is too close to 0 (this would cause undefined behavior)
		if(diffVect.y >= 0.1 && diffVect.x >= 0.1)
			transform.localScale = Vector3.Lerp(transform.localScale, diffVect, speed * Time.deltaTime);
		child.transform.localScale = transform.localScale;
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

	Vector3 ABS(Vector3 v){
		return new Vector3 (Mathf.Abs( v.x) , Mathf.Abs (v.y) , Mathf.Abs (v.z));
	}

	public void showHighlight(bool show){
		highlight.GetComponent<SpriteRenderer> ().enabled = show;
	}
}