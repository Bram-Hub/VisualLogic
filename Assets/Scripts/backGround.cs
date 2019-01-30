using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class backGround : MonoBehaviour {
    public Transform background;
    public int width, height, xStart, yStart;
    private float scale;
    public float speed = 5f;
	public float zoomLevel = 0.75f;
    Transform copy, par;
    nodeManager nManager;
	GameObject selection;
	// Use this for initialization
	void Start () {
		selection = GameObject.Find ("Selection");
        par = GameObject.Find("backGround").transform;
        nManager = GameObject.Find("lev_0").GetComponent<nodeManager>();
        scale = 1.7f;
        width = 16;
        height = 11;
        for (int i = -20; i < width*2;i++) {
            for (int j = -20; j < height*2;j++) {
                copy = Instantiate(background,new Vector3(i*scale,j*scale,0.0f),Quaternion.identity);
                copy.parent = par;
            }
        }
    }
    private void Update() {
		if (Input.GetKey(KeyCode.LeftArrow)) transform.Translate(Vector2.left * Time.deltaTime * 5);
		if (Input.GetKey(KeyCode.RightArrow)) transform.Translate(Vector2.right * Time.deltaTime * 5);
		if (Input.GetKey(KeyCode.UpArrow)) transform.Translate(Vector2.up * Time.deltaTime * 5);
		if (Input.GetKey(KeyCode.DownArrow)) transform.Translate(Vector2.down * Time.deltaTime * 5);

        if (nManager.currentItem != null) 
			return;
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f && Camera.main.orthographicSize <= 10f) {
            Camera.main.orthographicSize += scroll;
        }
        if (scroll < 0f && Camera.main.orthographicSize >= 1f) {
            Camera.main.orthographicSize += scroll;
        }
			
    }
	public void zoom(bool inward){
		Camera.main.orthographicSize += (inward) ? -zoomLevel : zoomLevel;
	}
}
