using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class timelineController : MonoBehaviour {
    GameObject timeLinePanel;
    public GameObject node;
    GameObject currentNode;
    NodeList nodeList;
    public GameObject display, content;
    nodeManager nManager;
    private float currentX, currentY;
    void Start() {
        currentNode = node;
        currentX = 0f;
        currentY = 2.75f;
        gameObject.SetActive(false);
        nodeList = new NodeList();
        nodeList.pushBack(new logicNode(currentNode));
        nManager = GameObject.Find("lev_0").GetComponent<nodeManager>();
    }
    public void show() {
        bool active = (gameObject.activeSelf) ? false : true;
        gameObject.SetActive(active);
    }
    public void creatNewNode(string rule) {
        if(nodeList.find(currentNode).next == null){
            gameObject.GetComponent<CanvasRenderer>().SetColor(Color.green);
            currentY = currentNode.transform.position.y;
            currentX = currentNode.transform.position.x;
            currentX += 1f;
            currentNode = Instantiate(node, new Vector3(currentX,currentY, node.transform.position.z), Quaternion.identity, content.transform);
            gameObject.GetComponent<CanvasRenderer>().SetColor(Color.red);
            currentNode.transform.GetChild(0).GetComponent<Text>().text = rule;
            nodeList.pushBack(new logicNode(currentNode));
            nodeList.find(currentNode).allObjs = getElements();
        } else {
            GameObject prev = currentNode;
            gameObject.GetComponent<CanvasRenderer>().SetColor(Color.green);
            currentY -= 1f;
            currentNode = Instantiate(node, new Vector3(currentX, currentY, node.transform.position.z), Quaternion.identity, content.transform);
            gameObject.GetComponent<CanvasRenderer>().SetColor(Color.red);
            nodeList.insert(prev, currentNode);
        }
    }
    public void setCurrentNode(GameObject g) {
        if (isCurrent(g)) return;
        nManager.undoStack.clear();
        nManager.redoStack.clear();
        List<GameObject> selectedObjects = new List<GameObject>();
        selectedObjects = getElements();

        nodeList.find(currentNode).allObjs = selectedObjects;
        currentNode = g;

        foreach (GameObject i in selectedObjects) {
            i.SetActive(false);
        }
        foreach (GameObject i in nodeList.find(g).allObjs) {
            i.SetActive(true);
        }
    }
    public bool isCurrent(GameObject g) { return g == currentNode; }

    List<GameObject> getElements() {
        GameObject[] allObjects = GameObject.FindGameObjectsWithTag("draggable");
        List<GameObject> selectedObjects = new List<GameObject>();
        foreach (GameObject i in allObjects) {
            var pos = i.transform.position;
            if (pos.x >= -1f && pos.x <= 25f && pos.y >= -9 && pos.y <= 10f && i.activeSelf == true) selectedObjects.Add(i);
        }
        return selectedObjects;
    }

}
class logicNode{
    public logicNode next, prev;
    public GameObject gNode;
    public List<GameObject> allObjs;

    public logicNode(GameObject g){
        next = null;
        prev = null;
        gNode = g;
        allObjs = new List<GameObject>();
        allObjs = getElements();
    }
    private List<GameObject> getElements() {
        GameObject[] allObjects = GameObject.FindGameObjectsWithTag("draggable");
        List<GameObject> selectedObjects = new List<GameObject>();
        foreach (GameObject i in allObjects) {
            var pos = i.transform.position;
            if (pos.x >= 4.5f && pos.x <= 25f && pos.y >= -9 && pos.y <= 3) selectedObjects.Add(i);
        }
        return selectedObjects;
    }
}
class NodeList {
    private List<logicNode> nlist;
    public NodeList() {nlist = new List<logicNode>();}
    public GameObject getLast() { return nlist[nlist.Count-1].gNode;}
    public void pushBack(logicNode n) {
        if(nlist.Count == 0) {
            nlist.Add(n);
            n.prev = n.next = null;
            return;
        }else{
            nlist[nlist.Count-1].next = n;
            n.prev = nlist[nlist.Count-1];
            n.next = null;
            nlist.Add(n);
        }
    }
    public void insert(GameObject prev,GameObject other) {
        //new logicNode(other);
        logicNode t = new logicNode(other);
        nlist.Add(t);
        find(prev).next = t;
        t.prev = find(prev);
    }
    public logicNode find(GameObject g) {
        foreach(logicNode i in nlist) if (g == i.gNode) return i;
        return null;
    }
}