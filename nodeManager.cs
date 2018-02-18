using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nodeManager : MonoBehaviour {
    Stack<move> timeLine, redoStack, undoStack;
    public GameObject currentItem;
	// Use this for initialization
	void Start () {
        timeLine = new Stack<move>();
        redoStack = new Stack<move>();
        undoStack = new Stack<move>();
    }
	// Update is called once per frame
	void Update () {
		
	}
    public void create(string type) {
        GameObject original = GameObject.Find(type);
        GameObject copy;
        copy = Instantiate(original,new Vector2(8,5), Quaternion.identity);
        copy.name = type;
        float scale = 0.6f;
        if(type != "cut"){
            copy.transform.localScale -= new Vector3(scale, scale, scale);
            copy.transform.position = new Vector3(copy.transform.position.x,copy.transform.position.y,-0.5f); ;
        }
        undoStack.Push(new move(copy, move.action.created));
    }
    public GameObject createAtUI(string type, Vector2 pos) {
        GameObject original = GameObject.Find(type);
        GameObject copy;
        copy = Instantiate(original, pos, Quaternion.identity);
        copy.name = type;
        float scale = 0.6f;
        if (type != "cut") {
            copy.transform.localScale -= new Vector3(scale, scale, scale);
            copy.transform.position = new Vector3(copy.transform.position.x, copy.transform.position.y, -0.5f); ;
        }
        return copy;
    }
    public GameObject createAtPos(string type, Vector2 pos) {
        GameObject original = GameObject.Find(type);
        GameObject copy;
        copy = Instantiate(original, pos, Quaternion.identity);
        copy.name = type;
        float scale = 0.6f;
        if (type != "cut") {
            copy.transform.localScale -= new Vector3(scale, scale, scale);
            copy.transform.position = new Vector3(copy.transform.position.x, copy.transform.position.y, -0.5f); ;
        }
        undoStack.Push(new move(copy, move.action.created));
        return copy;
    }
    public void moved(Vector2 origin, Vector2 newPos, GameObject g) {
        undoStack.Push(new move(g, move.action.moved,origin,newPos) );
    }
    public void delete(GameObject g) {
        Destroy(g);
    }
    public void undo() {
        if (undoStack.Count == 0) return;
        move next = undoStack.Pop();
        Debug.Log(next.getAction());
        if (next.getAction() == move.action.created) {
            next.setAction(move.action.deleted);
            redoStack.Push(next);
            delete(next.getObject());
        }
        else if (next.getAction() == move.action.moved) {
            next.resetPos();
            next.changePos();
            redoStack.Push(next);
        }
        else if (next.getAction() == move.action.deleted) {
            next.setAction(move.action.created);
            GameObject copy = createAtPos(next.getName(), next.getPosition());
            redoStack.Push(new move(copy,move.action.created ) );
        }
    }
    public void redo() {
        if (redoStack.Count == 0) return;
        move next = redoStack.Pop();
        Debug.Log(next.getAction());
        if (next.getAction() == move.action.created) {
            next.setAction(move.action.deleted);
            undoStack.Push(new move(next));
            delete(next.getObject());
        }
        else if (next.getAction() == move.action.moved) {
            next.resetPos();
            next.changePos();
            undoStack.Push(next);
        }
        else if(next.getAction() == move.action.deleted) {
            next.setAction(move.action.created);
            GameObject copy = createAtPos(next.getName(), next.getPosition());
            undoStack.Push(new move(copy, move.action.created));
        }
        
    }
    public void addUndoMove(GameObject g, move.action a) {
        undoStack.Push(new move(g, a));
    }
}

public class move{
    public move (move c) {
        this.commandObject = c.commandObject;
        this.name = c.name;
        this.act = c.act;
        this.x = c.x;
        this.y = c.y;
    }
    public move(GameObject g, action a) {
        commandObject = g;
        name = g.name;
        act = a;
        x = g.transform.position.x;
        y = g.transform.position.y;
    }
    public move(GameObject g, action a, Vector2 o, Vector2 n) {
        commandObject = g;
        name = g.name;
        act = a;
        x = g.transform.position.x;
        y = g.transform.position.y;
        origin = o;
        newPos = n;
        posDiff = n - o;
    }
    public void setPos(Vector2 pos) {
        x = pos.x;
        y = pos.y;
    }
    public void setAction(action a) {
        act = a;
    }
    public void resetPos() {
        commandObject.transform.position = origin;
    }
    public void changePos() {
        Vector2 tmp = origin;
        origin = newPos;
        newPos = tmp;
        posDiff = newPos - origin;
    }
    public Vector2 getOrigin(){ return origin; }
    public Vector2 getNewPos(){ return newPos; }
    public string getName(){ return name; }
    public GameObject getObject(){
        if (!commandObject) throw new System.Exception("Timeline trying to access null object");
        return commandObject;
    }
    public Vector2 getPosition() { return new Vector2(x, y);}
    public action getAction(){ return act; }

    public enum action { created, deleted, moved };

    private GameObject commandObject;
    private string name;
    private action act;
    private float x, y;
    private Vector2 origin, newPos, posDiff;
}
