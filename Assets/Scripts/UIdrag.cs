using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIdrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler{
    public string charName;
    nodeManager nManager;
    GameObject newVariable;
    static float pos;
    static bool needsUpdating;
    private void Start() {
        needsUpdating = false;
        nManager = GameObject.Find("lev_0").GetComponent<nodeManager>();
    }
    public void OnBeginDrag(PointerEventData eventData) {
        newVariable = nManager.createAtUI(charName, new Vector3(transform.position.x, transform.position.y, 0) );
    }
    public void OnDrag(PointerEventData eventData) {
        if (newVariable) {
            newVariable.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        nManager.addUndoMove(newVariable, move.action.created);
        updateNewPos();
    }
    //sets the position but does not apply it
    public void setNewPos(float z) {
        pos = z;
        needsUpdating = true;
    }
    //applies newpos
    public void updateNewPos() {
        if (newVariable && needsUpdating) {
            newVariable.transform.position = new Vector3(newVariable.transform.position.x,newVariable.transform.position.y,pos);
            needsUpdating = false;
        }
    }
    public void resetUpdate() {
        needsUpdating = (needsUpdating) ? false : true;
    }

    // Use this for initialization

}
