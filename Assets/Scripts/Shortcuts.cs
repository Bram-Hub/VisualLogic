using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shortcuts : MonoBehaviour
{
    nodeManager nManage;
    delete dMode;
    public menu Mn;
    backGround bgrnd;

    // Start is called before the first frame update
    void Start()
    {
        nManage = GameObject.Find("lev_0").GetComponent<nodeManager>();
        dMode = GameObject.Find("DeleteButton").GetComponent<delete>();
        bgrnd = GameObject.Find("Main Camera").GetComponent<backGround>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!(Mn.showMenu)) { 
            if (Input.GetKeyDown(KeyCode.A))
            {
                nManage.createAtUI("A", GetHitPoint());
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                nManage.createAtUI("B", GetHitPoint());
            }

            if (Input.GetKeyDown(KeyCode.C) && !(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
            {
                nManage.createAtUI("C", GetHitPoint());
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                nManage.createAtUI("D", GetHitPoint());
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                nManage.createAtUI("E", GetHitPoint());
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                nManage.createAtUI("F", GetHitPoint());
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                nManage.createAtUI("P", GetHitPoint());
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                nManage.createAtUI("Q", GetHitPoint());
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                nManage.createAtUI("cut", GetHitPoint());
            }

            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.C))
            {
                dMode.setCopyMode();
            }

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                dMode.setDeleteMode();
            }

            if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                bgrnd.zoom(false);
            }

            if (Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.Plus))
            {
                bgrnd.zoom(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Mn.menueScreen();
        }
    }

    private Vector3 GetHitPoint()
    {
        Plane plane = new Plane(Camera.main.transform.forward, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float dist;
        plane.Raycast(ray, out dist);
        return ray.GetPoint(dist);
    }
}
