using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PersentUnit : MonoBehaviour {
    
    int propercent;
    Vector2 posC;

  

    void OnGUI()
    { 
        GUI.Label(new Rect(posC.x -10, Screen.height - posC.y -10, 100, 20), propercent.ToString());
    }
    void Start() {
      
       
        posC = Camera.main.WorldToScreenPoint(transform.position);
        if (gameObject.name == "100")
        {propercent = 100;}
        if (gameObject.name == "50")
        { propercent = 50; }
        if (gameObject.name == "20")
        {propercent = 20;}
        Buffer.Instance.ChangePersent(100);
    }

    void OnMouseDown()
    {
        if (gameObject.name == "100")
        { propercent = 100; }
        if (gameObject.name == "50")
        { propercent = 50; }
        if (gameObject.name == "20")
        { propercent = 20; }
        Buffer.Instance.ChangePersent(propercent);       
    }
}
