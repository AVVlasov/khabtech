using UnityEngine;
using System.Collections;

public class Buffer : MonoBehaviour {

    public static Buffer Instance { get; private set; }   
    public GameObject mainBase, o100, o50, o20;
    public float percent;

    public Object[] baseMat;
    public Material base2;
    public Material base5;
    



    public void ChangePersent(int procent)
    {
        percent = procent;
        if (procent == 100)
        {
           
            o100.GetComponent<SpriteRenderer>().material = base2;
            o50.GetComponent<SpriteRenderer>().material = base5;
            o20.GetComponent<SpriteRenderer>().material = base5;           
        }
        if (procent == 50)
        {
            o100.GetComponent<SpriteRenderer>().material = base5;
            o50.GetComponent<SpriteRenderer>().material = base2;
            o20.GetComponent<SpriteRenderer>().material = base5;        
        }
        if (procent == 20)
        {
            o100.GetComponent<SpriteRenderer>().material = base5;
            o50.GetComponent<SpriteRenderer>().material = base5;
            o20.GetComponent<SpriteRenderer>().material = base2;         
          
        }
    }

    public void Awake()
    {
        Instance = this;
    }
    void Start () {   
        percent = 100f;
    }
   

}
