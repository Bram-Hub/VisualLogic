using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour, IPointerDownHandler {
    bool showMenu;
    GameObject menuPanel;
	// Use this for initialization
    private void Start() {
        showMenu = false;
        gameObject.SetActive(showMenu);
    }
    // Update is called once per frame
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            menueScreen();
        }
    }
    public void menueScreen() {
        GameObject[] vars = GameObject.FindGameObjectsWithTag("draggable");
        if (showMenu) {
            showMenu = false;
            gameObject.SetActive(showMenu);
            foreach (GameObject g in vars) {
                g.GetComponent<SpriteRenderer>().enabled = true;
            }
        } else {
            showMenu = true;
            gameObject.SetActive(showMenu);
            foreach (GameObject g in vars) {
                g.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }
    public void reload() {
        SceneManager.LoadScene("s1");
    }
    public void OnPointerDown (PointerEventData eventData) {
        menueScreen();
    }
}
