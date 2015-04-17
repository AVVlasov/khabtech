using UnityEngine;
using System.Collections;

public class KeyEvent : MonoBehaviour {

	void Update () {
		if (Input.GetKey("escape"))
			Application.Quit();
	}

	public void Save()
	{
		Debug.Log("Button was pressed");
	}
 
}
