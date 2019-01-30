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
	}

	void OnMouseOver(){
		mouseOver = true;
		showHighlight (true);
	}
	void OnMouseDown(){
		mouseDown = true;
		if (deleteMode.getDeleteMode ()){
			Destroy (child);
			Destroy (gameObject);
		}
		offset = transform.position - GetHitPoint ();
	}
	void OnMouseExit(){
		mouseOver = false;
		showHighlight (false);
	}
	void OnMouseUp(){
		mouseDown = false;
	}
	void OnMouseDrag(){
		transform.localScale = ABS((transform.position - ( GetHitPoint() )) * 0.65f );
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