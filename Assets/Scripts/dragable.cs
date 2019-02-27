using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class dragable : MonoBehaviour{
    Vector3 inputPos, offset;
    SpriteRenderer rend;
    mousePointer mPointer;
    nodeManager nmanager;
    delete deleteMode;
    Vector2 start, end;
    public UIdrag uiDrag;
    public bool hit, isDragging, isMouseOver,clicked;

	Color highlight, normal;
    void Start() {
        rend = gameObject.GetComponent<SpriteRenderer>();
        mPointer = Camera.main.GetComponent<mousePointer>();
        deleteMode = GameObject.Find("DeleteButton").GetComponent<delete>();
        nmanager = GameObject.Find("lev_0").GetComponent<nodeManager>();
        hit = isDragging = isMouseOver = clicked= false;
		normal = GetComponent<SpriteRenderer> ().color;
		ColorUtility.TryParseHtmlString ("#78A1FFBA", out highlight);
    }
    private void Update() {
		if (!isMouseOver && hit)
			rend.color = Color.red;
    }
    private void OnMouseDown() {
        clicked = true;

        if (deleteMode.getDeleteMode()){
            nmanager.delete(gameObject);
            mPointer.resetCursor();
            isDragging = hit = isMouseOver = false;
            return;
        }

		if (Input.GetKey (KeyCode.LeftShift)) {
			mPointer.addSelectedObject (gameObject);
		}

        start = transform.position;
        isDragging = true;
        offset = transform.position - GetHitPoint();

		//if there are any selected objects move them with this one
		if (!mPointer.selectedObjects.Contains (gameObject))
			return;
		foreach (GameObject j in mPointer.selectedObjects){
			j.transform.SetParent (transform);
		}
    }
    private void OnMouseUp() {
        end = transform.position;
        nmanager.moved(start, end, gameObject);
        isDragging = clicked = false;
        nmanager.currentItem = null;

		if (mPointer.selectedObjects.Contains (gameObject)) {
			foreach (GameObject i in mPointer.selectedObjects) {
				i.transform.SetParent (null);
			}
		}
    }
    private void OnMouseDrag() {
        clicked = true;
        nmanager.currentItem = gameObject;
        if(!deleteMode.getDeleteMode()){
            isDragging = true;
            transform.position = GetHitPoint() + offset;
        }

		//if there are any selected objects move them with this one
		if (!mPointer.selectedObjects.Contains (gameObject))
			return;
		foreach (GameObject j in mPointer.selectedObjects){
			j.transform.SetParent (transform);
		}
    }
    private void OnMouseEnter() {
        nmanager.currentItem = gameObject;
		showHighlight (true);
        isMouseOver = true;
    }
	private void OnMouseOver(){
		nmanager.currentItem = gameObject;
		isMouseOver = true;
		showHighlight (true);
	}
    private void OnMouseExit() {
        nmanager.currentItem = null;
        isMouseOver = false;
		showHighlight (false);       
    }
    private Vector3 GetHitPoint() {
        Plane plane = new Plane(Camera.main.transform.forward, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float dist;
        plane.Raycast(ray, out dist);
        return ray.GetPoint(dist);
    }
    private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.GetComponent<SpriteRenderer> ().sortingLayerName == "Cuts")
			return;

		hit = true;
    }
	private void OnTriggerExit2D(Collider2D collision){
		if (collision.GetComponent<SpriteRenderer> ().sortingLayerName == "Cuts")
			return;

		hit = false;
	}
    public bool getDragStatus() {
        return isDragging;
    }
    public bool getMouseOverStatus() {
        return isMouseOver;
    }

	public void showHighlight(bool x){
		rend.color = (x) ? highlight : normal;
	}
}
