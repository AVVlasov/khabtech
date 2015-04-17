using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ai : MonoBehaviour {


    public int typep = 2, nGl; //тип игрока
    public GameObject[] baseall; //масив имен всех баз
    public GameObject global;
    GameObject firstBase = null; //приоритетная база
    [HideInInspector]
    public GameObject clone_global;
   
    void Start () {
        baseall = GameObject.FindGameObjectsWithTag("base"); //ищем по тэгу  
        InvokeRepeating("Think", 0.5f, 0.5F); //запускаем ИИ для каждого типа отдельно
    }
  
   
    //Поиск ближайшей базы
    GameObject Closer(GameObject myb, List<GameObject> noMybArr)
    {
        //устанавливаем по умолчанию значениями первого элемента масива
        GameObject target_cl = noMybArr[0]; //переменная содержащая ближнюю базу 
        float min_dist = Vector3.Distance(myb.transform.position, noMybArr[0].transform.position); //дистанция до него

        foreach (GameObject noMy in noMybArr)
        {
            //если дистанция меньше то перезапишем начальные переменые
            if (Vector3.Distance(myb.transform.position, noMy.transform.position) < min_dist)
            {
                min_dist = Vector3.Distance(myb.transform.position, noMy.transform.position);
                target_cl = noMy;
            }   
        }
        return target_cl;
    }

    //Поиск слабой базы
    GameObject Easy(GameObject myb, List<GameObject> noMybArr)
    {
        //устанавливаем по умолчанию значениями первого элемента масива
        GameObject target_ea = noMybArr[0]; //переменная содержащая слабую базу 
        float min_count = noMybArr[0].GetComponent<Movement>().countUnit; //количество юнитов

        foreach (GameObject noMy in noMybArr)
        {
            //если количество юнитов меньше то перезапишем начальные переменные
            if (noMy.GetComponent<Movement>().countUnit < min_count)
            {
                min_count = noMy.GetComponent<Movement>().countUnit;
                target_ea = noMy;
            }
        }
        return target_ea;
    }
    //Поиск слабых баз
    List<GameObject> Easy_closer(GameObject myb, List<GameObject> noMybArr)
    {
        //устанавливаем по умолчанию значениями первого элемента масива
        List<GameObject> target_eacl = new List<GameObject>(); //переменная содержащая слабую базу        

        foreach (GameObject noMy in noMybArr)
        {
            //если количество юнитов меньше то перезапишем начальные переменные
            if (myb.GetComponent<Movement>().countUnit > (noMy.GetComponent<Movement>().countUnit +5))
            {
                target_eacl.Add(noMy);
            }
        }
        return target_eacl;
    }

    // думалка ИИ
    void Think()
    {
        List<GameObject> ai_base = new List<GameObject>(); //базы ИИ
        List<GameObject> enemies = new List<GameObject>(); // базы не ИИ        
        int cn_all = 0; //общее число юнитов     
        //перебираем и распределяем их по масивам
        foreach (GameObject basein in baseall)
        {
            if (basein.GetComponent<Movement>().player == typep)
            {
                cn_all += basein.GetComponent<Movement>().countUnit; //считаем сколько у нас всего юнитов
                ai_base.Add(basein);
                if (firstBase == basein)
                 firstBase = null;  
            }
            else
             enemies.Add(basein);            
        }
      
        if (enemies.Count > 0)
        {
          foreach(GameObject my in ai_base){
            GameObject targetC = null;
            GameObject target = my; //база куда направляем юнитов
            List<GameObject> targetYС = Easy_closer(my, enemies); //слабые базы
            if (targetYС.Count >0)
            {               
                 targetC = Closer(my, targetYС); //ближайшая база
            }         
          
            
            if (my.GetComponent<Movement>().countUnit > 20 || (firstBase != null && my.GetComponent<Movement>().countUnit > 15))
            {
                Movement Movement = my.GetComponent<Movement>();            
             
                if (targetC != null)
                    target = targetC;                
                else
                {
                    targetC = Closer(my, enemies); 
                    if (targetC.GetComponent<Movement>().countUnit < cn_all)
                        firstBase = targetC;                  
                }
                if (firstBase != null)
                {

                    clone_global = Instantiate(global, transform.position, Quaternion.identity) as GameObject;
                    clone_global.name = "global_" + gameObject.name + "_" + nGl;
                    clone_global.GetComponent<Global>().baseall = GameObject.FindGameObjectsWithTag("base"); //ищем по тэгу  
                    StartCoroutine(clone_global.GetComponent<Global>().goWaits(firstBase, my, Movement.Unit, Movement, 1,null));

                }
                else if (target != my)
                {
                    clone_global = Instantiate(global, transform.position, Quaternion.identity) as GameObject;
                    clone_global.name = "global_" + gameObject.name + "_" + nGl;
                    clone_global.GetComponent<Global>().baseall = GameObject.FindGameObjectsWithTag("base"); //ищем по тэгу  
                    StartCoroutine(clone_global.GetComponent<Global>().goWaits(target, my, Movement.Unit, Movement, 1,null));
                   
                }             
            }
          }
        }
        ai_base.Clear();
        enemies.Clear();         
    }
    void Update ()
    {
       
    }
    
}
