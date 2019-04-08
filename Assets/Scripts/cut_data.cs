using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cut_data : MonoBehaviour {
	PolygonCollider2D pColl;
	// Use this for initialization
	void Start () {
		pColl = GetComponent<PolygonCollider2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log(getCenter());
		//Debug.Log(getWidth());
	}

	public Vector2 getCenter(){
		return new Vector2 (transform.position.x, transform.position.y);
	}
	public float getWidth(){
		return pColl.bounds.size.x;
	}
	public float getHeight(){
		return pColl.bounds.size.y;
	}
}
