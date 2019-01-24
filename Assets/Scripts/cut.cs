using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


/*
 * This Class represents a 'cut' 
 * A cut is similar to a free variable but has some special conditions
 * for example if it is true or false
 */

public class cut : MonoBehaviour{
    Vector3 s, currentPoint, offset;
    mousePointer pointer;
    delete deleteMode;
    nodeManager nmanager;
    dragable drag;
    Vector3 start, end, originalScale, finalScale;
    List<child> children;
    public int level;
	void Start () {
        children = new List<child>();
        currentPoint = Vector3.zero;
        pointer = Camera.main.GetComponent<mousePointer>();
        deleteMode = GameObject.Find("DeleteButton").GetComponent<delete>();
        nmanager = GameObject.Find("lev_0").GetComponent<nodeManager>();
        drag = GameObject.Find("A").GetComponent<dragable>();
        level = 1;
	}
    private void Update() {
		//check to display the inside of the cut as either dark or light gray to indicate negation
        //FFFFFFBD
        //A3A3A3BD
        if (level == 0) level = 1;
        Color tru, fal;
        ColorUtility.TryParseHtmlString("#FFFFFFBD", out tru);
        ColorUtility.TryParseHtmlString("#A3A3A3BD", out fal);
        GameObject inner = gameObject.transform.GetChild(0).gameObject;
        inner.GetComponent<SpriteRenderer>().color = (level % 2 == 0) ? tru : fal;

		//the level should decide the z position so the highest level can always be selected first
        float z = (level == 1) ? 0  : -1 * level;
        gameObject.transform.position = new Vector3(transform.position.x,transform.position.y,z);
    }
    private void OnMouseDown() {
        if (deleteMode.getDeleteMode()){
			//check if deleting
            nmanager.delete(gameObject);
            pointer.resetCursor();
            nmanager.currentItem = null;
            return;
        }
		//handle touch inputs and scaling
		if (Input.touchCount == 2 ) {
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

			ResizeCut (deltaMagnitudeDiff);

		}else {
			//otherwise a normal move
			start = transform.position;
			offset = transform.position - GetHitPoint ();
			//get all of the children (smaller cuts and variables) and correct their position
			foreach (child i in children) {
				i.setStartPos ();
				i.getObj ().transform.SetParent (gameObject.transform);
			}
		}
    }
	//code to help record the movements of the user
	//TODO better recording to eventually play a 'movie'
    private void OnMouseUp() {
		if (deleteMode.getDeleteMode ())
			return;
        nmanager.moved(start, end, gameObject);
        nmanager.currentItem = null;
        foreach (child i in children) {
            i.getObj().transform.SetParent(null);
            i.getObj().transform.position = i.getPos();
            i.setFinalPos();
            nmanager.moved(i.start, i.end, i.getObj());
        }
    }
	//change position when dragging
    private void OnMouseDrag() {
		float deltaMagnitudeDiff = 0;
		Vector2 diff = new Vector2(0,0);
		RectTransform rt = (RectTransform)gameObject.transform;
		Vector3 normalizedPointer = gameObject.transform.position - GetHitPoint ();
		double radius = Mathf.Sqrt (normalizedPointer.x * normalizedPointer.x + normalizedPointer.y * normalizedPointer.y);

        nmanager.currentItem = gameObject;
        transform.position = GetHitPoint() + offset;
        foreach (child i in children) {
            i.updatePos(i.getObj().transform.position);
        }
    }
	//handle scrolling with mousewhile and update cursor
    private void OnMouseOver() {
        nmanager.currentItem = gameObject;
        Vector3 screenSpace = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y)) - transform.position;
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        pointer.mouseOverCursor();
        ResizeCut(scroll);
    }
    private void OnMouseExit() {
        nmanager.currentItem = null;
        end = transform.position;
        pointer.resetCursor();
    }
	private void ResizeCut(float scroll, Vector2 direction = new Vector2()) {
        if (scroll != 0f) {
            foreach (child i in children) {
                i.getObj().transform.SetParent(null);
                i.getObj().transform.position = new Vector3(i.getObj().transform.position.x, i.getObj().transform.position.y, i.getObj().transform.position.z);
                //i.getObj().transform.position = i.getPos();
            }
        }
        if (scroll > 0f) {
            transform.localScale += new Vector3(scroll, scroll, 0);
        } else if (scroll != 0 && transform.localScale.x >= 0.3f) {
            transform.localScale -= new Vector3(-scroll, -scroll, 0);
        }
    }
    private Vector3 GetHitPoint() {
        Plane plane = new Plane(Camera.main.transform.forward, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float dist;
        plane.Raycast(ray, out dist);
        return ray.GetPoint(dist);
    }
	//when another cut is placed into a cut decide which one should be the higher level cut
	//TODO: auto-resize cuts in the future?
    private void OnTriggerEnter2D(Collider2D collision) {
        child c = new child(collision.gameObject, collision.transform.localPosition);
        if(collision.name != "cut"){
            children.Add(c);
        }
        else {
            //the smaller cut should be the child
            if (collision.gameObject.GetComponent<SpriteRenderer>().bounds.size.x ==
                this.gameObject.GetComponent<SpriteRenderer>().bounds.size.x) return;
            if (collision.gameObject.GetComponent<SpriteRenderer>().bounds.size.x >= 
                this.gameObject.GetComponent<SpriteRenderer>().bounds.size.x) {
                collision.gameObject.GetComponent<cut>().children.Add(new child(gameObject,gameObject.transform.position));
                level++;            
            }
            else{
               //handle same size? 
				//TODO: add auto-resize code
            }
        }
    }
	//update level reverse of above function
    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.name == "cut") {
            if (collision.gameObject.GetComponent<SpriteRenderer>().bounds.size.x >=
                this.gameObject.GetComponent<SpriteRenderer>().bounds.size.x) {
                level--;
            } else {
             
            }
        }
        for (int i = 0; i < children.Count;i++) {
            if(collision.gameObject == children[i].getObj()) {
                children.Remove(children[i]);
            }
        }
    }
}
//helper class to handle variables (this should be removed in the future and handled
//only with the nodeManager class)
class child {
    public child(GameObject g, Vector3 pos) {
        gObject = g;
        localPos = pos;
    }

    public Vector3 getPos() {
        return localPos;
    }
    public GameObject getObj(){
        return gObject;
    }
    public void updatePos(Vector3 newPos) {
        localPos = newPos;
    }
    public void setFinalPos() {
        end = gObject.transform.position;
    }
    public void setStartPos() {
        start = gObject.transform.position;
    }
    public Vector3 start, end;
    private Vector3 localPos;
    private GameObject gObject;
}