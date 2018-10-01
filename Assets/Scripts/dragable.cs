using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class dragable : MonoBehaviour{
    Vector3 inputPos, offset;
    SpriteRenderer rend;
    mousePointer pointer;
    nodeManager nmanager;
    delete deleteMode;
    Vector2 start, end;
    public UIdrag uiDrag;
    public bool hit, isDragging, isMouseOver,clicked;
    void Start() {
        rend = gameObject.GetComponent<SpriteRenderer>();
        pointer = Camera.main.GetComponent<mousePointer>();
        deleteMode = GameObject.Find("DeleteButton").GetComponent<delete>();
        nmanager = GameObject.Find("lev_0").GetComponent<nodeManager>();
        hit = isDragging = isMouseOver = clicked= false;
    }
    private void Update() {
        if (hit) {
            rend.color = Color.red;
        }
    }
    private void OnMouseDown() {
        clicked = true;
        if (deleteMode.getDeleteMode()){
            nmanager.delete(gameObject);
            pointer.resetCursor();
            isDragging = hit = isMouseOver = false;
            return;
        }
        start = transform.position;
        isDragging = true;
        offset = transform.position - GetHitPoint();
    }
    private void OnMouseUp() {
        end = transform.position;
        nmanager.moved(start, end, gameObject);
        isDragging = clicked = false;
        nmanager.currentItem = null;
    }
    private void OnMouseDrag() {
        clicked = true;
        nmanager.currentItem = gameObject;
        if(!deleteMode.getDeleteMode()){
            isDragging = true;
            transform.position = GetHitPoint() + offset;
        }
    }
    private void OnMouseEnter() {
        nmanager.currentItem = gameObject;
        pointer.mouseOverCursor();
        isMouseOver = true;
        rend.color = Color.blue;
    }
    private void OnMouseExit() {
        nmanager.currentItem = null;
        pointer.resetCursor();
        isMouseOver = false;
        rend.color = Color.white;        
    }
    private Vector3 GetHitPoint() {
        Plane plane = new Plane(Camera.main.transform.forward, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float dist;
        plane.Raycast(ray, out dist);
        return ray.GetPoint(dist);
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.tag != "background" && collision.name != "cut" && collision.name != "titleCut" && collision.name != "titleCut2"){
            hit = true;
        }
        if(collision.name == "cut"){
            float z = transform.position.z - 1.2f;
            uiDrag.setNewPos(z);
            gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, z);
        }
    }
    private void OnTriggerExit2D(Collider2D collision) {
        hit = false;
        rend.color = Color.white;
        uiDrag.resetUpdate();
        if (collision.name == "cut") {
            float z = transform.position.z + 1.2f;
            gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, z);
        }
    }
    public bool getDragStatus() {
        return isDragging;
    }
    public bool getMouseOverStatus() {
        return isMouseOver;
    }
}
