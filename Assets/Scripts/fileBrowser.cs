using System.IO;
using UnityEngine;

using UnityEngine.SceneManagement;
#if UNITY_EDITOR
// Editor specific code here
using UnityEditor;
#endif
public class fileBrowser : MonoBehaviour {
    public string text;
    nodeManager nManager;
    public menu menuPanel;
    GameObject goal, assume;
    private void Start() {
        nManager = GameObject.Find("lev_0").GetComponent<nodeManager>();
        goal = GameObject.Find("Goal");
        assume = GameObject.Find("Assume");
    }
#if UNITY_EDITOR
    [MenuItem("Example/Overwrite Texture")]
    public void Apply() {
        string path = EditorUtility.OpenFilePanel("Open a new visual logic file", "", "txt");
        if (path.Length != 0) {
            StreamReader reader = new StreamReader(path);
            string text = reader.ReadToEnd();
            string[] words = text.Split('\n');
            Read(words);
        }
    }
    private void Read(string[] words) {
        for (int i = 0; i < words.Length; i++) {
            string[] subStr = words[i].Split(' ');
            for(int j = 0; j < subStr.Length; j++) {
                if ( subStr[j].Equals("Goal")) {
                    if (subStr[j + 1].Equals("not")) {
                        GameObject par = nManager.createAtUI("titleCut", new Vector3(2, 5, 0));
                        string varName = getVar(subStr[j + 2]);
                        GameObject g = nManager.createAtUI(varName, new Vector3(2, 5, -0.5f));
                        g.transform.SetParent(par.transform);
                    } else {
                        string varName = getVar(subStr[j + 1]);
                        nManager.createAtUI(varName, new Vector3(3, 4, -0.5f));
                    }
                }
                if (subStr[j].Equals("Assume")) {
                    if (subStr[j + 1].Equals("not")) {
                        GameObject par = nManager.createAtUI("titleCut2", new Vector3(12, 5, 0));
                        string varName = getVar(subStr[j + 2]);
                        GameObject g = nManager.createAtUI(varName, new Vector3(12, 5, -0.5f));
                        g.transform.SetParent(par.transform);
                    } else {
                        string varName = getVar(subStr[j + 1]);
                        nManager.createAtUI(varName, new Vector3(7, 2, -0.5f));
                    }
                }
            }
        }
        menuPanel.menueScreen();
    }
    public string getVar(string t) {
        if (t.StartsWith("A")) t = "A";
        if (t.StartsWith("B")) t = "B";
        if(t.StartsWith("C")) t ="C";
        if (t.StartsWith("D")) t = "D"; 
        return t;
    }
#endif
}
