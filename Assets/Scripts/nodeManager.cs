﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nodeManager : MonoBehaviour {
    public moveStack redoStack, undoStack;
    mousePointer mPointer;
    public timelineController timeLine;
    public GameObject currentItem;
	public int cut_count = 0;
	// Use this for initialization
	void Start () {
        mPointer = Camera.main.GetComponent<mousePointer>();
        redoStack = new moveStack();
        undoStack = new moveStack();
    }

	/**
	A valid type can be one of these strings: 
		cut, A, B, C, D, E, F, P, Q
	**/
	//given a type, this function creates a new one of them at the ceneter of the screen (8,5)
    public void create(string type) {
		//find and copy the original type
        GameObject original = GameObject.Find(type);
        GameObject copy;
        copy = Instantiate(original,new Vector2(8,5), Quaternion.identity);
        copy.tag = "draggable";

		if (type != "cut") {
			//correct the variables size so its not too big and fix its z position
			copy.transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
			copy.transform.position = new Vector3 (copy.transform.position.x, copy.transform.position.y, 0f);
		} else {
			//increment number of cuts created so far and fix this ones name
			copy.name = type + "_" + cut_count;
			GameObject inner = GameObject.Find ("innerCut");
			GameObject innerCopy = Instantiate(inner,new Vector2(8,5), Quaternion.identity);

			innerCopy.name = "innerCut_" + cut_count;
			innerCopy.tag = "draggable";

			innerCopy.transform.localScale = new Vector3(1, 1, 1);
			++cut_count;
		}
    }

	//given a type and a position create an object at a location and return an instance of it
	//this should be used for instantiating and dragging objects from the var box
    public GameObject createAtUI(string type, Vector3 pos) {
        GameObject original = GameObject.Find(type);
		GameObject copy, innerCopy = null;
        copy = Instantiate(original, pos, Quaternion.identity);
        copy.name = type;
        copy.tag = "draggable";
        if (type != "cut") {
            copy.transform.position = new Vector3(copy.transform.position.x, copy.transform.position.y, 0f);
            copy.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		}else {
			copy.name = type + "_" + cut_count;
			GameObject inner = GameObject.Find ("innerCut");
			innerCopy = Instantiate(inner,pos, Quaternion.identity);
            innerCopy.transform.position = new Vector3(innerCopy.transform.position.x, innerCopy.transform.position.y, 0f);
            innerCopy.name = "innerCut_" + cut_count;
			innerCopy.tag = "draggable";
			innerCopy.transform.localScale = new Vector3(1, 1, 1);
			++cut_count;

			copy.GetComponent<cut> ().child = innerCopy;
			innerCopy.GetComponent<innerCut> ().parent = copy;
		}
		return (type != "cut") ? copy : innerCopy ;
    }

	//given a type and a position create an object at a location and return an instance of it
	//driver function for when having a Vector2 position
	public GameObject createAtPos(string type, Vector2 pos) {
		Vector3 pos_ = new Vector3 (pos.x, pos.y, 0.0f);
        GameObject copy = createAtUI(type, pos_);
        return copy;
    }

	//given an original cut, position, and optional map of variables create a copy of that object at that position
	//the optional param maps gamobjects to their offsets to other cuts
	//this prevents variables from moving to the center when copying over
	//returns an instance of the new object
	public GameObject createCutFromCopy(GameObject original, Vector3 pos, Dictionary<GameObject, Vector2> vars = null){
		GameObject copy, innerCopy = null;
		copy = Instantiate (GameObject.Find("cut"), pos, Quaternion.identity);
		copy.name = "cut_" + cut_count;
		copy.tag = "draggable";
		copy.transform.localScale = original.GetComponent<innerCut>().parent.transform.localScale;
		copy.transform.SetParent (null);

		innerCopy = Instantiate (GameObject.Find ("innerCut"), pos, Quaternion.identity);
		innerCopy.transform.SetParent (null);

		innerCopy.tag = "draggable";
		innerCopy.name = "innerCut_" + cut_count;

		copy.GetComponent<cut> ().child = innerCopy;
		innerCopy.GetComponent<innerCut> ().parent = copy;

		original.transform.SetParent (null);
		innerCopy.transform.localScale = original.transform.localScale;

		++cut_count;

		if (vars != null) {
			foreach ( KeyValuePair<GameObject, Vector2> i in vars) {
				Vector3 var_pos = new Vector3 ( innerCopy.transform.position.x + i.Value.x, innerCopy.transform.position.y + i.Value.y, 0f );
				GameObject tmp = Instantiate (i.Key, var_pos, Quaternion.identity);
				tmp.name = i.Key.name;
			}
		}

		return innerCopy;
	}

    public void moved(Vector2 origin, Vector2 newPos, GameObject g) {
        undoStack.push(new move(g, move.action.moved,origin,newPos) );
    }
    public void scaled(Vector2 originalSize, Vector2 finalSize, GameObject g) {
        throw new System.Exception("not implemented");
    }
    public void inserted(GameObject child, GameObject parent) {
        throw new System.Exception("not implemented");
    }
    public void delete(GameObject g) {
        undoStack.push(new move(g,move.action.deleted) );
        mPointer.resetCursor();
        Destroy(g);
    }
    public void deleteWithoutNewMove(GameObject g) {
        Destroy(g);
    }
    public void undo() {
        if (undoStack.size() == 0) return;
        move next = undoStack.pop();
        if(next.getAction() == move.action.moved) {
            //move the object to old position
            next.resetPos();
            next.swapPositions();
            redoStack.push(next);
        }
        else if(next.getAction() == move.action.created) {
            //the object was last created, now delete it
            GameObject copy = next.getObject();
            deleteWithoutNewMove(next.getObject());
            redoStack.push(new move(copy, move.action.deleted));
        }
        else if(next.getAction() == move.action.deleted) {
            //the object was last deleted, now create it
            GameObject copy = createAtPos(next.getName(), next.getPosition());
            redoStack.push(new move(copy, move.action.created) );
            undoStack.updateNullObjs(copy);
        }
    }
    public void redo() {
        if (redoStack.size() == 0) return;
        move next = redoStack.pop();
        if(next.getAction() == move.action.moved) {
            next.resetPos();
            next.swapPositions();
            undoStack.push(next);
        }
        else if (next.getAction() == move.action.deleted) {
            //the object was last delete, now create it
            GameObject copy = createAtPos(next.getName(),next.getPosition());
            redoStack.updateNullObjs(copy);
        } 
        else if (next.getAction() == move.action.created) {
            //the object was last created, now delete it
            GameObject copy = next.getObject();
            deleteWithoutNewMove(next.getObject());
            undoStack.push(new move(copy, move.action.deleted));
        }
    }
    public void addUndoMove(GameObject g, move.action a) {
        undoStack.push(new move(g, a));
    }
    
}

public class move{
    public move (move c) {
        this.commandObject = c.commandObject;
        this.name = c.name;
        this.act = c.act;
        this.currentPos = c.currentPos;
        this.oldPos = c.oldPos;
    }
    public move(GameObject g, action a) {
        commandObject = g;
        name = g.name;
        act = a;
        currentPos = g.transform.position;
    }
    public move(GameObject g, action a, Vector2 o, Vector2 n) {
        commandObject = g;
        name = g.name;
        act = a;
        oldPos = o;
        newPos = n;
    }
    public void setPos(Vector2 pos) {
        currentPos = pos;
    }
    public void setAction(action a) {
        act = a;
    }
    public void resetPos() {
        commandObject.transform.position = oldPos;
    }
    public void swapPositions() {
        Vector2 tmp = oldPos;
        oldPos = newPos;
        newPos = tmp;
    }
    public void setGameObject(GameObject g) {
        commandObject = g;
    }

    public string getName(){ return name; }
    public GameObject getObject(){return commandObject;}
    public Vector2 getPosition() { return currentPos;}
    public Vector2 getOldPos() { return oldPos; }
    public Vector2 getNewPos() { return newPos; }
    public action getAction(){ return act; }

    public enum action { created, deleted, moved, scaled };

    private GameObject commandObject;
    private string name;
    private action act;
    private Vector2 currentPos, oldPos, newPos;
}
public class moveStack {
    public moveStack() {
        moves = new List<move>();
    }

    public move pop() {
        move m = moves[moves.Count-1];
        moves.Remove(m);
        return m;
    }
    public move pop_front() {
        move m = moves[0];
        moves.Remove(m);
        return m;
    }
    public void push(move m) {
        moves.Add(m);
    }
    public int size() {
        return moves.Count;
    }
    public void updateNullObjs(GameObject newObj) {
        foreach (move i in moves) {
            if (i.getObject() == null){
                i.setGameObject(newObj);
            }
        }
    }
    public void updateGameObject(GameObject oldObj, GameObject newObj) {
        foreach (move i in moves) {
            if (i.getObject() == oldObj) {
                i.setGameObject(newObj);
            }
        }
    }
    public void clear() { moves.Clear(); }
    private List<move> moves;
}