using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoToBase : MonoBehaviour
{
    public GameObject UnitLayer;
    int cnUnit;
    bool reverse = false;
    public List<Vector2> arrPos1 = new List<Vector2>(); //массив с позициями центр от источника
    public List<Vector2> arrPos2 = new List<Vector2>(); //массив с позициями полукругом от источника
    public List<Vector2> arrPos3 = new List<Vector2>(); //массив с позициями полукругом к цели
    public List<Vector2> arrPos4 = new List<Vector2>(); //массив с позициями центр цели   



    public List<PoinTouch> pointTouch = new List<PoinTouch>();

    public void calcPointToMove(Vector2 PointFrom, Vector2 PointTo, float percentUnit, Movement ParamsBaseFrom)
    {
        float Angle = -35; //угол отклонения юнита
        cnUnit = (int)(ParamsBaseFrom.ostcn * percentUnit); //количество юнитов для генерации                        
        int cnFirstLine = (cnUnit < 8) ? cnUnit : 8;//счетчик для расчета позиций        
        //цикл для расчета позиций       
        for (int i = 0; i < cnFirstLine; i++)
        {
            arrPos1.Add(PointFrom);
            arrPos2.Add(calc_pos(PointFrom, PointTo, 2, Angle, 1));
            arrPos3.Add(calc_pos(PointTo, PointFrom, 3, Angle, 1));
            arrPos4.Add(PointTo);
            Angle += 10;
        }
        FindPointColl();

    }


    void FindPointColl()
    {
        //бегаем по маршрутам юнитов
        foreach (Vector2 Pos2 in arrPos2)
        {
            List<Vector2> arr = new List<Vector2>();
            int i = arrPos2.IndexOf(Pos2);
            int distance = (int)Vector2.Distance(arrPos2[i], arrPos3[i]);
            float segment = 0.5f;
            int index = 0;
            while (distance > segment)
            {
                Vector2 nextPoint = arrPos2[i] + ((arrPos3[i] - arrPos2[i]) * segment) / distance;               
                foreach (Collider2D coll in Physics2D.OverlapPointAll(nextPoint))
                {
                    if (coll.gameObject != gameObject && coll != null )
                    {
                        //останавливаемся после первого попадания точки на базу   
                        pointTouch.Add(new PoinTouch
                        {
                            IndexCol = index,
                            NumUnit = (i + 1),
                            ObjTouch = coll.gameObject,
                            Point = nextPoint
                        });
                        index++;
                       
                    }
                   
                }
                 segment += 0.5f;
            }

           

        }
       
    }


    public Vector2 calc_pos(Vector2 from, Vector2 to, int type, float cAngle, float radius)
    {
        Vector2 cV = to - from, calc;
        float _x, _y, angleU, c_angle;
        int kfbase = (type == 3) ? -1 : 1;

        c_angle = Mathf.Atan2(cV.y, cV.x) * Mathf.Rad2Deg;

        if (Mathf.Abs(c_angle) < 90 && type == 2) { reverse = true; }
        angleU = (c_angle - cAngle * kfbase) * Mathf.Deg2Rad;

        _x = from.x + Mathf.Cos(angleU) * radius;
        _y = from.y + Mathf.Sin(angleU) * radius;

        calc.x = _x; calc.y = _y;
        return calc;
    }

    public IEnumerator BeginGo(GameObject targets, GameObject BaseFrom, GameObject Unit, Movement Movements, float prce)
    {
        calcPointToMove(BaseFrom.transform.position, targets.transform.position, prce, Movements);
        GameObject clone, unitLayer; //объект юнита             
        int nUnit = 0; //номер юнита
        Movements.ostcnUb -= cnUnit * 100;//забираем генерируемое количество юнитов
        int ci = 0;
        int cr = (reverse) ? 0 : 8;
        unitLayer = Instantiate(UnitLayer, arrPos1[0], Quaternion.identity) as GameObject;
        unitLayer.name = "from_"+BaseFrom.name+"_to_"+targets.name;
        for (int i = 1; i <= cnUnit; i++)
        {
            Movements.cnUB -= 100;
            nUnit += 1;
            clone = Instantiate(Unit, arrPos1[ci], Quaternion.identity) as GameObject;
            clone.name = "unite_" + Movements.name + "_" + nUnit;
            clone.GetComponent<PathFind>().left = reverse;
            clone.GetComponent<SpriteRenderer>().sortingOrder = cr;
            clone.GetComponent<PathFind>().pos2 = arrPos2[ci];
            clone.GetComponent<PathFind>().pos3 = arrPos3[ci];
            clone.GetComponent<PathFind>().pos4 = arrPos4[ci];
            clone.GetComponent<PathFind>().playerU = Movements.player;
            clone.GetComponent<PathFind>().matU = Movements.mat;
            clone.GetComponent<PathFind>().targetU = targets;
            ci++;

            clone.transform.parent = unitLayer.transform;
            unitLayer.GetComponent<UnitLayer>().have_child = true;
            if (reverse) { cr++; } else { cr--; }
            int dig = i % 8;
            if (dig == 0)
            {
                ci = 0;
                cr = (reverse) ? 0 : 8;
                yield return new WaitForSeconds(0.5f);
            }
        }
        arrPos1.Clear();
        arrPos2.Clear();
        arrPos3.Clear();
        arrPos4.Clear();

        reverse = false;
        Destroy(gameObject);
    }
}
//кандидаты на расчет обхода пути
public class PoinTouch
{
    public int IndexCol { get; set; }
    public int NumUnit { get; set; }
    public GameObject ObjTouch { get; set; }   
    public Vector2 Point { get; set; }
}