using UnityEngine;
using System.Collections;

public class UnitLayer : MonoBehaviour {

    public bool have_child = false;
    
	void Update () {
        if (have_child && gameObject.transform.childCount ==0)
        {
            Destroy(gameObject);
        }
	}
}
