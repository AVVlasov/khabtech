using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Movement : MonoBehaviour
{
    public float cnUB, ostcnUb;

    public int player, TileX, TileY;

    public GameObject Unit,GoTo, lcolor, targetBase, global, obj;

    bool colOn = false;
    public List<Node> currentPath = null;   
    public TileMap map;

    public Material mat; 

    [HideInInspector]
    public int countUnit, ostcn, hoU, nGl;

    [HideInInspector]
    public GameObject clone_global;

    [HideInInspector]
    public Vector2 posC, posC2,mbpos;
    
    [HideInInspector]
    public float fx;

    [HideInInspector]
    public Material hoUC;

   

    void OnGUI()
    {
        //выравнивание для счетчика
        if (countUnit < 10) { fx = 3; }
        if (countUnit >= 10) { fx = 7; }
        if (countUnit >= 100) { fx = 11; }
        if (countUnit >= 1000) { fx = 15; }

        GUI.contentColor = Color.black;
        //рисуем счетчик
        GUI.Label(new Rect(posC.x - fx, Screen.height - posC.y - 10, 100, 20), countUnit.ToString());
    }
    void Start()
    {        

        ostcnUb = cnUB;
        lcolor.GetComponent<SpriteRenderer>().material = mat;
        posC2 = transform.position;
        posC2.x += 0.75f;
        posC2.y += 0.8f;
        posC = Camera.main.WorldToScreenPoint(posC2);

       
      
    }

    void FixedUpdate() 
    {
        if (TileX == 0 && TileY == 0)
        {
            foreach (Collider2D coll in Physics2D.OverlapPointAll(transform.position))
            {
                if (coll.gameObject != gameObject && coll.gameObject != null)
                {
                    GameObject obj = coll.gameObject;
                    if (obj.layer == LayerMask.NameToLayer("cloneTile"))
                    {
                        TileX = obj.GetComponent<Tile>().tileX;
                        TileY = obj.GetComponent<Tile>().tileY;
                        obj.GetComponent<SpriteRenderer>().material = Buffer.Instance.base5;
                    }

                }


            }
        }

    }

    public void CreateUnit(GameObject UnitE)
    {
        if (Input.GetMouseButton(0) && !Input.GetMouseButtonUp(0))
        {
            currentPath = null;

            Vector2 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (GetComponent<Collider2D>() == Physics2D.OverlapPoint(wp))
            {              
                colOn = true;                
                if (Buffer.Instance.mainBase == null)
                {
                    Buffer.Instance.mainBase = gameObject;
                 }
            }
        }

        else if (colOn == true && Buffer.Instance.mainBase == gameObject)
        {
            colOn = false;
            

            if (countUnit > 0)
            {
                Vector2 wp2 = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                foreach (Collider2D coll in Physics2D.OverlapPointAll(wp2))
                {                   

                    if (coll != null && coll.gameObject != gameObject && coll.tag == "base")
                    {
                        nGl += 1;
                        obj = coll.gameObject;
                        clone_global = Instantiate(GoTo, transform.position, Quaternion.identity) as GameObject;
                        clone_global.name = "GoTo_" + gameObject.name + "_To_" + obj.name+"_("+ nGl+")";
                        StartCoroutine(clone_global.GetComponent<GoToBase>().BeginGo(obj, gameObject, Unit, this, Buffer.Instance.percent / 100));
                    }
                }               
            }
            Buffer.Instance.mainBase = null;
        }
    }

    void Update()
    {
        if (currentPath != null)
        {

            int currNode = 0;

            while (currNode < currentPath.Count - 1)
            {

                Vector2 start = map.TileCoordToWorldCoord(currentPath[currNode].x, currentPath[currNode].y);
                Vector2 end = map.TileCoordToWorldCoord(currentPath[currNode + 1].x, currentPath[currNode + 1].y);

                Debug.DrawLine(start, end, Color.red);

                currNode++;
            }

        }
 
        if (cnUB < 0)
        {            
            cnUB = Mathf.Abs(cnUB);
        }
        if (ostcnUb < 0)
        { 
            player = hoU;
            lcolor.GetComponent<SpriteRenderer>().material = hoUC;
            mat = hoUC;
            ostcnUb = Mathf.Abs(ostcnUb); 
        }

        countUnit = Mathf.Abs((int)cnUB / 100);
        ostcn = Mathf.Abs((int)ostcnUb / 100);

        if (ostcn < 1 && player != 0)
        {
            cnUB++;
            ostcnUb++;
        }
       
        if (player == 1)
        {         
            CreateUnit(Unit);
        }
    }
}
