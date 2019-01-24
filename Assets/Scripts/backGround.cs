using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class backGround : MonoBehaviour {
    public Transform background;
    public int width, height, xStart, yStart;
    private float scale;
    public float speed = 5f;
    Transform copy, par;
    nodeManager nManager;
	// Use this for initialization
	void Start () {
        par = GameObject.Find("backGround").transform;
        nManager = GameObject.Find("lev_0").GetComponent<nodeManager>();
        scale = 1.7f;
        width = 16;
        height = 11;
        for (int i = -10; i < width;i++) {
            for (int j = -10; j < height;j++) {
                copy = Instantiate(background,new Vector3(i*scale,j*scale,0.0f),Quaternion.identity);
                copy.parent = par;
            }
        }
    }
    private void Update() {
        if (nManager.currentItem != null) return;
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f && Camera.main.orthographicSize <= 10f) {
            Camera.main.orthographicSize += scroll;
        }
        if (scroll < 0f && Camera.main.orthographicSize >= 1f) {
            Camera.main.orthographicSize += scroll;
        }
        if (Input.GetKey(KeyCode.LeftArrow)) transform.Translate(Vector2.left * Time.deltaTime * 5);
        if (Input.GetKey(KeyCode.RightArrow)) transform.Translate(Vector2.right * Time.deltaTime * 5);
        if (Input.GetKey(KeyCode.UpArrow)) transform.Translate(Vector2.up * Time.deltaTime * 5);
        if (Input.GetKey(KeyCode.DownArrow)) transform.Translate(Vector2.down * Time.deltaTime * 5);
    }
}
