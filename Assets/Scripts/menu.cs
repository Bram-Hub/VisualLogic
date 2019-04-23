using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class menu : MonoBehaviour, IPointerDownHandler {
    public bool showMenu;
	bool showInputField;
    GameObject menuPanel;
	public GameObject timeLineButton;
	public GameObject expression_field;
	nodeManager nManager;
	// Use this for initialization
    private void Start() {
        showMenu = false;
        gameObject.SetActive(showMenu);
		expression_field = GameObject.Find ("InputField");
		nManager = GameObject.Find ("lev_0").GetComponent<nodeManager> ();
		expression_field.SetActive (false);
    }
    // Update is called once per frame
    private void Update() {
        
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

        if (showInputField)
        {
            enterExpression();
        }
    }
    public void reload() {
        SceneManager.LoadScene("s1");
    }

	//if the user clicks outside the modal menu close it, also close the input field if its open
    public void OnPointerDown (PointerEventData eventData) {
        menueScreen();
		if(showInputField)
			enterExpression ();
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
		Vector2 cursor = new Vector3(8,5);
		//List<string> ret = parseExpressionHelper (expr);
		string output = "";

		int index = 0;
		char v1, v2;
		if (expr.Contains ("|")) {
			if (expr [0] != '~') {
				v1 = expr [index];
				GameObject cut = nManager.createAtPos ("cut", cursor);
				cut.GetComponent<innerCut>().parent.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
				cut.transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
				nManager.createAtPos (v1.ToString().ToUpper(), cursor);
				cursor = new Vector2 (12, 5);
			} else {
				index++;
				v1 = expr [index];
				nManager.createAtPos (v1.ToString ().ToUpper (), cursor);
				cursor = new Vector2 (12, 5);
			}
			index += 2;
			if (expr [index] != '~') {
				v2 = expr [index];

				GameObject cut = nManager.createAtPos ("cut", cursor);
				cut.GetComponent<innerCut>().parent.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
				cut.transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
				nManager.createAtPos (v2.ToString ().ToUpper (), cursor);
			} else {
				index++;
				v2 = expr [index];
				nManager.createAtPos (v2.ToString ().ToUpper (), cursor);
			}
			cursor = new Vector2 (10, 5);
			GameObject overall_cut = nManager.createAtPos("cut", cursor);
			overall_cut.transform.localScale = new Vector3(2,1,0);
			overall_cut.GetComponent<innerCut>().parent.transform.localScale = new Vector3(2,1,0);
		} else if (expr.Contains ("&")) {
			if (expr [0] == '~') {
				index++;
				v1 = expr [index];
				GameObject cut = nManager.createAtPos ("cut", cursor);
				cut.GetComponent<innerCut>().parent.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
				cut.transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
				nManager.createAtPos (v1.ToString().ToUpper(), cursor);
				cursor = new Vector3 (12, 5);
			} else {
				nManager.createAtPos (expr [0].ToString().ToUpper(), cursor);
				cursor = new Vector2 (12, 5);
			}
			index += 2;
			if (expr [index] == '~') {
				index++;
				v2 = expr [index];
				GameObject cut = nManager.createAtPos ("cut", cursor);
				cut.GetComponent<innerCut>().parent.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
				cut.transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
				nManager.createAtPos (v2.ToString().ToUpper(), cursor);
			} else {
				nManager.createAtPos (expr [index].ToString().ToUpper(), cursor);
			}
		} else if (expr.Contains (">")) {
			if (expr [0] == '~') {
				index++;
				nManager.createAtPos (expr [index].ToString().ToUpper(), cursor);
				cursor = new Vector2 (12, 5);
			} else {
				GameObject cut = nManager.createAtPos ("cut", cursor);
				cut.GetComponent<innerCut>().parent.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
				cut.transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
				nManager.createAtPos (expr[0].ToString().ToUpper(), cursor);
				cursor = new Vector2 (12, 5);

			}
			index += 2;
			if (expr [index] == '~') {
				index++;
				GameObject cut = nManager.createAtPos ("cut", cursor);
				cut.GetComponent<innerCut>().parent.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
				cut.transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
				nManager.createAtPos (expr[index].ToString().ToUpper(), cursor);
			} else {
				v2 = expr [index];
				nManager.createAtPos (expr [index].ToString().ToUpper(), cursor);
			}
			cursor = new Vector2 (10, 5);
			GameObject overall_cut = nManager.createAtPos("cut", cursor);
			overall_cut.transform.localScale = new Vector3(2,1,0);
			overall_cut.GetComponent<innerCut>().parent.transform.localScale = new Vector3(2,1,0);
		}

		menueScreen ();
		if(showInputField)
			enterExpression ();
	}

	private bool isVar(char x){
		return x == 'a' || x == 'b' || x == 'c' || x == 'd' || x == 'e' || x == 'f' || x == 'p' || x == 'q';
	}

	//TODO: move me to my own file!

	public List<string> parseExpressionHelper(string x){
		List<string> tokenized = new List<string>();
		for (int i = 0; i < x.Length; ++i) {
			//not
			if (isVar (x [i])) {
				tokenized.Add ( "" + x [i]);
			} else if (x [i] == '~') { 
				tokenized.Add ("NOT");
			} else if (x [i] == '&') {
				tokenized.Add ("AND");
			} else if (x [i] == '|') {
				tokenized.Add ("OR");
			} else if (x [i] == '(') {
				int open = 0;
				int closed = 0;
				bool found_closed = false;
				string tmp = "";
				tokenized.Add ("BEG");
				while (!found_closed) {
					if (i >= x.Length)
						break;

					if (x [i] == '(') {
						open++;
						i++;
					} else if (x [i] == ')') {
						closed++;
						i++;
					} else {
						i++;
					}

					if (i < x.Length && x[i] != ')' && x[i]  != '(') {
						tmp += x[i];
					}

					if (open == closed) {
						found_closed = true;
						//Debug.Log (tmp);
						tokenized.AddRange(parseExpressionHelper (tmp));
						tokenized.Add ("END");
						open = closed = 0;
						break;
					}
				}
			}
		}
		return tokenized;
	}
	//platform specific compilation!
	//on desktop and other platforms, we can open a new window normally
	//on webgl we have to open a new browser window!
	public void showHelp(){
		#if UNITY_EDITOR
			Application.OpenURL("https://bram-hub.github.io/VisualLogic/");
			return;
		#elif UNITY_WEBGL
			Application.ExternalEval("window.open(\"https://bram-hub.github.io/VisualLogic/\")");
		#endif
	}
}
