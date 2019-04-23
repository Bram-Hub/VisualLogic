using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class copy : MonoBehaviour {

	GameObject copyInto;

	// Use this for initialization
	void Start () {
		
	}

	//is a larger than b
	bool lessThan(Vector3 a, Vector3 b){
		var dx = a.x - b.x;

		if (a.x > b.x) {
			return false;
		}

		if (a.y > a.y) {
			return false;
		}

		return true;
	}

	Vector3 add(Vector3 a, Vector3 b){
		return new Vector3 (a.x + b.x, a.y + b.y, 0f);
	}

	public void copyIntoCut(GameObject child, GameObject parent = null){

		if (!child || !parent)
			return;

		if (!child.name.Contains ("inner") || !parent.name.Contains ("cut"))
			return;

		if (!copyInto.GetComponent<innerCut> ()) {
			copyInto = copyInto.GetComponent<cut> ().child;
		}


		if (!parent)
			parent = copyInto;

		Vector3 parSize = parent.GetComponent<innerCut> ().parent.GetComponent<PolygonCollider2D> ().bounds.size;
		Vector3 childSize = child.GetComponent<innerCut> ().parent.GetComponent<PolygonCollider2D> ().bounds.size;

		Vector3 scale = add (parent.transform.localScale, child.transform.localScale);

		if (lessThan (parSize, childSize)) {
			parent.GetComponent<innerCut> ().parent.transform.localScale = scale;
			parent.transform.localScale = scale;
		}
	}

	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D collision){
		copyInto = collision.gameObject;
	}
}
