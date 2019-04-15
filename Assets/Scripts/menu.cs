using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class menu : MonoBehaviour, IPointerDownHandler {
    bool showMenu;
	bool showInputField;
    GameObject menuPanel;
	public GameObject timeLineButton;
	public GameObject expression_field;
	// Use this for initialization
    private void Start() {
        showMenu = false;
        gameObject.SetActive(showMenu);
		expression_field = GameObject.Find ("InputField");
		expression_field.SetActive (false);
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
			//timeLineButton.SetActive (true);
        } else {
            showMenu = true;
            gameObject.SetActive(showMenu);
            foreach (GameObject g in vars) {
                g.GetComponent<SpriteRenderer>().enabled = false;
            }
			//timeLineButton.SetActive (false);
        }
    }
    public void reload() {
        SceneManager.LoadScene("s1");
    }
    public void OnPointerDown (PointerEventData eventData) {
        menueScreen();
    }

	public void enterExpression(){
		if (showInputField) {
			showInputField = false;
		} else {
			showInputField = true;
		}
		expression_field.SetActive(showInputField);
	}

	public void parseExpression(){
		//p
		//!p
		//(!p) && q
		string expr = expression_field.GetComponent<InputField>().text;
		Debug.Log (expr);
		Vector3 cursor = new Vector3(8,5,0);
	}

	public void parseExpressionHelper(){

	}
}
