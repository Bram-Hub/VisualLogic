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
	bool mouseOver, mouseDown;
	nodeManager nManager;
	// Use this for initialization
	delete deleteMode;
	SpriteRenderer renderer;

	Color tru, fal;
	void Start () {
		//innerCut_
		string parName = gameObject.name.Substring(gameObject.name.IndexOf ('_') + 1);
		parent = GameObject.Find ("cut_" + parName);
		deleteMode = GameObject.Find ("DeleteButton").GetComponent<delete> ();
		nManager = GameObject.Find ("lev_0").GetComponent<nodeManager>();
		renderer = GetComponent<SpriteRenderer> ();

		ColorUtility.TryParseHtmlString ("#FFFFFFBD", out tru);
		ColorUtility.TryParseHtmlString ("#A3A3A3BD", out fal);
	}
	
	// Update is called once per frame
	void Update () {
		if (!parent)
			return;
		if (!mouseDown || !mouseOver)
			transform.position = parent.transform.position;

		transform.position = new Vector3 (transform.position.x, transform.position.y, 0.0f);

		renderer.color = (renderer.sortingOrder % 2 != 0) ? tru : fal;
	
	}

	void OnMouseOver(){
		mouseOver = true;
		nManager.currentItem = gameObject;
		float scroll = Input.GetAxis ("Mouse ScrollWheel");
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
	}
	void OnMouseDown(){
		mouseDown = true;
		if (deleteMode.getDeleteMode ()){
			Destroy (parent);
			Destroy (gameObject);
		}
		offset = transform.position - GetHitPoint ();
	}
	void OnMouseDrag(){
		transform.position = GetHitPoint () + offset;
		parent.transform.position = transform.position;

		PolygonCollider2D coll = gameObject.GetComponent<PolygonCollider2D> ();
		Collider2D[] overlap = Physics2D.OverlapAreaAll (coll.bounds.min, coll.bounds.max);
		foreach (Collider2D i in overlap) {
			if (i.gameObject.name.Contains ("cut")) 
				continue;
			if (i.gameObject.name.Contains ("innerCut") && !isGreater (i)) {
				i.transform.SetParent (transform);
				continue;
			}
			i.transform.SetParent (transform);
		}
	}
	void OnMouseUp(){
		PolygonCollider2D coll = gameObject.GetComponent<PolygonCollider2D> ();
		Collider2D[] overlap = Physics2D.OverlapAreaAll (coll.bounds.min, coll.bounds.max);
		foreach (Collider2D i in overlap) {
			if (i.gameObject.name.Contains ("cut"))
				continue;
			i.transform.SetParent (null);
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
		if (collision.GetComponent<SpriteRenderer>().sortingLayerName == "Variables") {
			collision.GetComponent<SpriteRenderer> ().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 1;
			return;
		}
		if (!collision.name.Contains ("innerCut"))
			return;

		//the smaller cut should be the child
		if(!isGreater(collision)){
			collision.GetComponent<SpriteRenderer> ().sortingOrder += 1;
		}
	}
	void OnTriggerExit2D(Collider2D collision){
		if (collision.GetComponent<SpriteRenderer> ().sortingLayerName == "Variables") {
			collision.GetComponent<SpriteRenderer> ().sortingOrder = GetComponent<SpriteRenderer> ().sortingOrder - 1;
			return;
		}

		if (!collision.name.Contains ("innerCut"))
			return;

		//the smaller cut should be the child
		if(!isGreater(collision) ){
			collision.GetComponent<SpriteRenderer> ().sortingOrder -= 1;
		}
	}
	bool isGreater( Collider2D b){
		PolygonCollider2D thisCollider = GetComponent<PolygonCollider2D> ();
		PolygonCollider2D otherCollider = b.GetComponent<PolygonCollider2D> ();
		Vector3 thisSize = thisCollider.bounds.size;
		Vector3 otherSize = otherCollider.bounds.size;

		return (thisSize.x < otherSize.x && thisSize.y < otherSize.y);
	}
}
