using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathFind : MonoBehaviour
{
    [HideInInspector]
    public bool go2 = false, go3 = false, go4 = false;       
    [HideInInspector]
    public Vector2 pos0, pos1, pos2, pos3, pos4, scale;
    float speed = 0.025f;     
    public int i = 0, kfb = 1, playerU;  
    public bool left;
    
    public GameObject targetU;
    public Material matU;
    public int cg ;
    public float dust, cA;
    public List<UnitSpeed_Position> arr_V;
    public List<Vector2> arr_all; 

    void Start()
    {  
        GetComponent<SpriteRenderer>().material = matU; 
        pos1 = transform.position;
        go2 = true;
        if (left)
        {
            scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;          
        }
    }

    void Update()
    {       
        pos0 = transform.position;
        if (go2)
        {
            pos0 = Vector2.MoveTowards(transform.position, pos2, speed);
            if (pos0 == pos2)
            {
                go3 = true;
                go2 = false;
            }
        }
        if (go3)
        {           
                pos0 = Vector2.MoveTowards(transform.position, pos3, speed);
                if (pos0 == pos3)
                {
                    go4 = true;
                    go3 = false;
                }
            
        
        }
        if (go4)
        {
            pos0 = Vector2.MoveTowards(transform.position, pos4, speed);
        }
      

        kfb = (targetU.GetComponent<Movement>().player != playerU) ? -1 : 1;

        if (pos0 == pos4)
        {
            targetU.GetComponent<Movement>().cnUB += 100 * (kfb);
            targetU.GetComponent<Movement>().ostcnUb += 100 * (kfb);
            targetU.GetComponent<Movement>().hoU = playerU;
            targetU.GetComponent<Movement>().hoUC = matU;
            Destroy(gameObject);
        }
        transform.position = pos0;
        
        #region   рисуем траекторию юнита цветными линиями
        Debug.DrawLine(pos1, pos2, Color.blue);     
        Debug.DrawLine(pos2, pos3, Color.green);       
        Debug.DrawLine(pos3, pos4, Color.black);
        #endregion 
        

      
    }
}
