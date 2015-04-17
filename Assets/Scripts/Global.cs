using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Global : MonoBehaviour
{
    public GameObject[] baseall; //масив имен всех баз 
    public GameObject UnitLayer;
    bool reverse = false;
    public List<Vector2> arrPos1 = new List<Vector2>(); //массив с позициями центр от источника
    public List<Vector2> arrPos2 = new List<Vector2>(); //массив с позициями полукругом от источника
    public List<Vector2> arrPos3 = new List<Vector2>(); //массив с позициями полукругом к цели
    public List<Vector2> arrPos4 = new List<Vector2>(); //массив с позициями центр цели    
    public List<List<UnitSpeed_Position>> arrPos2to3 = new List<List<UnitSpeed_Position>>(); //массив позиций между 2 и 3 вектором

    public List<PoinCollider> pointColl = new List<PoinCollider>();
    public List<PoinColliderCalc> pointCollcalc = new List<PoinColliderCalc>();

    public List<PosAll> pPall = new List<PosAll>();
    public List<Vector2> arrPosJPS = new List<Vector2>();

    int cnUnit;
    [HideInInspector]
    List<List<Vector2>> arrPosAll = new List<List<Vector2>>();

    List<Vector2> arrb = new List<Vector2>();

    List<MassArrPos> tiu = new List<MassArrPos>();
    //расчет точек для перемещения юнитов
  /*  public void calcPointToMove(Vector2 PointFrom, Vector2 PointTo, float percentUnit, Movement ParamsBaseFrom)
    {
        float Angle = -35; //угол отклонения юнита
        cnUnit = (int)(ParamsBaseFrom.ostcn * percentUnit); //количество юнитов для генерации                        
        int cnFirstLine = (cnUnit < 8) ? cnUnit : 8;//счетчик для расчета позиций        
        //цикл для расчета позиций       
        for (int i = 0; i < cnFirstLine; i++)
        {
            arrPos2.Add(calc_pos(PointFrom, PointTo, 2, Angle, 1));
            arrPos3.Add(calc_pos(PointTo, PointFrom, 3, Angle, 1));
            arrPos1.Add(PointFrom);
            arrPos4.Add(PointTo);
            Angle += 10;
        }
        FindPointColl();
        CalcPointColl();
        SortResult();
    }*/

    
    public void calcPointToMove(List<Vector2> arrPath, float percentUnit, Movement ParamsBaseFrom)
    {
        float Angle = -35; //угол отклонения юнита
        cnUnit = (int)(ParamsBaseFrom.ostcn * percentUnit); //количество юнитов для генерации                        
        int cnFirstLine = (cnUnit < 8) ? cnUnit : 8;//счетчик для расчета позиций   
        Vector2 PointFrom = arrPath[0];
        Vector2 PointTo = arrPath[arrPath.Count - 1];
        Vector2 endPoint2 = PointTo;
        Vector2 startPoint3 = PointFrom;
        if(arrPath.Count >2){
            endPoint2 = arrPath[1];
            startPoint3 = arrPath[arrPath.Count - 2];
        }
       
         
        List<UnitSpeed_Position> redyCalc = new List<UnitSpeed_Position>();
        arrPos2to3.Add(redyCalc);

        
        //цикл для расчета позиций       
        for (int i = 0; i < cnFirstLine; i++)
        {
            
           // arrPos1.Add(PointFrom);
           // arrb.Add(PointFrom);
            

         int currNode = 0;

         tiu.Add(new MassArrPos { numU = i, iter = currNode, pis = PointFrom });
        while (currNode < arrPath.Count - 1 && arrPath.Count > 2 && arrPath.Count > currNode + 2)
        {
            //arrPos2.Add(calc_pos(arrPath[currNode], arrPath[currNode+1], 2, Angle, 1));
            //arrPos3.Add(calc_pos(arrPath[currNode + 1], arrPath[currNode + 2], 3, Angle, 1));


           // arrb.Add(calc_pos(arrPath[currNode], arrPath[currNode + 1], 2, Angle, 1));
            tiu.Add(new MassArrPos { numU = i, iter = currNode + 1, pis = calc_pos(arrPath[currNode], arrPath[currNode + 1], 2, Angle, 1) });
          //  arrb.Add(calc_pos(arrPath[currNode + 1], arrPath[currNode + 2], 3, Angle, 1));
            tiu.Add(new MassArrPos { numU = i, iter = currNode + 1, pis = calc_pos(arrPath[currNode + 1], arrPath[currNode + 2], 2, Angle, 1) });


            // arrPos2.Clear();
            // arrPos3.Clear();
            currNode++;
        }


            //arrPos4.Add(PointTo);
            //arrb.Add(PointTo);
            tiu.Add(new MassArrPos { numU = i, iter = currNode + 1, pis = PointTo });
            //arrPosAll.Add(arrPos4);


            //arrPosAll.Add(arrb);
           
            //arrb.Clear();
           
            Angle += 10;
        }
        for (int i = 0; i < cnFirstLine; i++)
        {
            List<Vector2> rr = new List<Vector2>();

            var maxQuery = from prod in tiu
                           where prod.numU == i
                           orderby prod.iter ascending                           
                           select prod.pis
                           ;
            foreach (var trerwe in maxQuery)
            {
                rr.Add(trerwe);
            }

            pPall.Add(new PosAll { poss = rr });
        }

       


        //Debug.Log(arrPosAll);
        //FindPointColl();
        //CalcPointColl();
        //SortResult();
    }

    void FindPointColl()
    {
        //бегаем по маршрутам юнитов
        foreach (Vector2 Pos2 in arrPos2)
        {
            int i = arrPos2.IndexOf(Pos2);
            int distance = (int)Vector2.Distance(arrPos2[i], arrPos3[i]);
            float segment = 0.5f;
            string nameBase = "";
            int index = 0;
            while (distance > segment)
            {
                Vector2 nextPoint = arrPos2[i] + ((arrPos3[i] - arrPos2[i]) * segment) / distance;

                Vector2 previousPoint = arrPos2[i] + ((arrPos3[i] - arrPos2[i]) * (segment - (0.5f * 3))) / distance;

                foreach (Collider2D coll in Physics2D.OverlapPointAll(nextPoint))
                {
                    if (coll.gameObject != gameObject && coll.gameObject != null && coll.tag == "base")
                    {
                        //останавливаемся после первого попадания точки на базу                
                        nameBase = coll.name;
                        pointColl.Add(new PoinCollider
                        {
                            ObjColl = coll.gameObject,
                            IndexCol = index,
                            NumUnit = (i + 1),
                            PointColl = previousPoint
                        });
                        index++;
                        //  print(" numunit: " + (i + 1) + " baseNum: " + baseNum + " basein: " + basein);
                    }

                    segment += 0.5f;
                }
            }
        }
    }



    //метод помойка надо разобрать
    void CalcPointColl()
    {
        //выборка от 1-4 юнита сортировка от большего к меньшему
        var maxQuery = from prod in pointColl
                       where prod.NumUnit <= 4
                       orderby prod.NumUnit descending
                       group prod by prod.ObjColl into grouping
                       select new
                       {
                           grouping.Key,
                           UnitData = from prod2 in grouping
                                      where prod2.NumUnit == grouping.Max(prod3 => prod3.NumUnit)
                                      select prod2
                       };

        //выборка от 5-8 юнита сортировка от меньшего к большему
        var minQuery = from prod in pointColl
                       where prod.NumUnit > 4
                       orderby prod.NumUnit descending
                       group prod by prod.ObjColl into grouping
                       select new
                       {
                           grouping.Key,
                           UnitData = from prod2 in grouping
                                      where prod2.NumUnit == grouping.Min(prod3 => prod3.NumUnit)
                                      select prod2
                       };

        foreach (var unitC in maxQuery)
        {
            foreach (PoinCollider uC in unitC.UnitData)
            {
                dataToMove(uC, 1, 1);
            }
        }

        foreach (var unitC2 in minQuery)
        {
            foreach (PoinCollider uC2 in unitC2.UnitData)
            {
                dataToMove(uC2, -1f, 2);

            }
        }
    }



    void dataToMove(PoinCollider data, float kfa, int type)
    {
        // print("unit: " + uC.NumUnit + " baseCol: " + uC.ObjColl + " NumBase: " + uC.NumBase + " PointCol: " + uC.PointColl);
        int t = data.NumUnit;
        float fd = 0.3f;
        Vector2 tmpVn = Vector2.zero;
        GameObject tmpObj = null;
        int rule1, rule2;     

        if (type == 1)
        {
            rule1 = t;            
            rule2 = 0;
        }
        else
        {
            rule1 = arrPos2.Count + 1;
            rule2 = t;            
        }

        while (rule1 > rule2)
        {
            int r = t - 1;
            List<UnitSpeed_Position> usp = new List<UnitSpeed_Position>();

            GameObject Objtmp = (tmpObj != null) ? tmpObj : data.ObjColl;

            Vector2 cV0, cV1, cV2, cV3, cV4, pos1, pos2, pos3, pos4;
            float a1, a2, a3, a4, a5; // наши углы
            float _x, _y, dist, kfspeed;

            Vector2 VnTmp = (tmpVn != Vector2.zero) ? tmpVn : data.PointColl;
            usp.Add(new UnitSpeed_Position { kfSpeed = 1, PointOut = VnTmp });

            cV0 = arrPos3[r] - arrPos2[r];
            cV1 = Objtmp.transform.position;
            cV2 = cV1 - VnTmp;
            dist = Vector2.Distance(cV1, VnTmp);

            a1 = calc_angle(cV0);

            a2 = calc_angle(cV2);

            a3 = ((a1 - a2) + a1) * Mathf.Deg2Rad;

            pos1 = posCalc(a1, cV1, kfa, fd);

            cV3 = pos1 - VnTmp;

            a4 = calc_angle(cV3);

            pos3 = posCalc(a4, cV1, kfa, fd);

            _x = cV1.x + Mathf.Cos(a3) * dist;
            _y = cV1.y + Mathf.Sin(a3) * dist;
            pos2.x = _x; pos2.y = _y;

            cV4 = pos2 - pos1;
            a5 = calc_angle(cV4);

            pos4 = posCalc(a5, cV1, kfa, fd);

            tmpVn = posCalc(a1, VnTmp, kfa, (-0.5f));

            kfspeed = (Vector2.Distance(VnTmp, pos3)
                + Vector2.Distance(pos3, pos1)
                + Vector2.Distance(pos1, pos4)
                + Vector2.Distance(pos4, pos2)
                ) / Vector2.Distance(VnTmp, pos2);
            usp.Add(new UnitSpeed_Position { kfSpeed = kfspeed, PointOut = pos3 });
            usp.Add(new UnitSpeed_Position { kfSpeed = kfspeed, PointOut = pos1 });
            usp.Add(new UnitSpeed_Position { kfSpeed = kfspeed, PointOut = pos4 });
            usp.Add(new UnitSpeed_Position { kfSpeed = kfspeed, PointOut = pos2 });
            pointCollcalc.Add(new PoinColliderCalc { ObjColl = data.ObjColl, IndexCol = data.IndexCol, NumUnit = t, ListPointColl = usp });
            fd += 0.2f;
            if (type == 1)
            {
                rule1--;
                t--;
            }
            else
            {
                t++;
                rule2++;
            }


        }

    }

    void SortResult() //сортировка по номеру юнита
    {
        foreach (Vector2 ar in arrPos2)
        {
            int i = arrPos2.IndexOf(ar);
            var UnitColliderCalc = (from pC in pointCollcalc
                                    where pC.NumUnit == i + 1
                                    orderby pC.IndexCol ascending
                                    group pC by pC.NumUnit into nU
                                    orderby nU.Key
                                    select nU);
            List<UnitSpeed_Position> redyCalc = new List<UnitSpeed_Position>();
            foreach (var unitCalc in UnitColliderCalc)
            {
                foreach (var baseCalc in unitCalc)
                {
                    foreach (var vectorCalc in baseCalc.ListPointColl)
                    {
                        redyCalc.Add(vectorCalc);
                    }
                }

            }
            arrPos2to3.Add(redyCalc);
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

    public Vector2 posCalc(float angle, Vector2 vector, float kf, float kf2)
    {
        Vector2 pos;
        float angle_calc, x, y;
        angle_calc = (angle + (90 * kf)) * Mathf.Deg2Rad;
        x = vector.x + Mathf.Cos(angle_calc) * (.7f + kf2);
        y = vector.y + Mathf.Sin(angle_calc) * (.7f + kf2);
        pos.x = x; pos.y = y;
        return pos;

    }

    public float calc_angle(Vector2 data)
    {
        float calc_angle;
        calc_angle = Mathf.Atan2(data.y, data.x) * Mathf.Rad2Deg;
        return calc_angle;
    }

    public IEnumerator goWaits(GameObject targets, GameObject BaseFrom, GameObject Unit, Movement Movements, float prce, TileMap maps)
    {
             

        BaseFrom.GetComponent<Movement>().currentPath = maps.FindPath(BaseFrom, targets);
        List<Node> currentPathtt = BaseFrom.GetComponent<Movement>().currentPath;

        if (currentPathtt != null)
        {
            int currNode = 0;
            while (currNode < currentPathtt.Count)
            {
                Vector2 start = maps.TileCoordToWorldCoord(currentPathtt[currNode].x, currentPathtt[currNode].y);
                arrPosJPS.Add(start);
                currNode++;
            }

        }

        calcPointToMove(arrPosJPS, prce, Movements);       

        GameObject clone, unitLayer; //объект юнита             
        int nUnit = 0; //номер юнита
        Movements.ostcnUb -= cnUnit * 100;//забираем генерируемое количество юнитов
        int ci = 0;
        int cr = (reverse) ? 0 : 8;
        unitLayer = Instantiate(UnitLayer, pPall[ci].poss[0], Quaternion.identity) as GameObject;
        unitLayer.name = "test";
        for (int i = 1; i <= cnUnit; i++)
        {
            Movements.cnUB -= 100;
            nUnit += 1;
           
            clone = Instantiate(Unit, pPall[ci].poss[0], Quaternion.identity) as GameObject;
            clone.transform.parent = UnitLayer.transform;
            clone.name = "unite_" + Movements.name + "_" + nUnit;
            clone.GetComponent<PathFind>().left = reverse;
            clone.GetComponent<SpriteRenderer>().sortingOrder = cr;        
            clone.GetComponent<PathFind>().arr_all = pPall[ci].poss;           
            clone.GetComponent<PathFind>().playerU = Movements.player;
            clone.GetComponent<PathFind>().matU = Movements.mat;
            clone.GetComponent<PathFind>().targetU = targets;


            ci++;
            
            
            if (reverse) { cr++; } else { cr--; }
            int dig = i % 8;
            if (dig == 0)
            {
                ci = 0;
                cr = (reverse) ? 0 : 8;
                yield return new WaitForSeconds(0.5f);
            }
        }
 
        pPall.Clear();
        reverse = false;
        pointColl.Clear();
        pointCollcalc.Clear();
        Destroy(gameObject);
    }

}



//кандидаты на расчет обхода пути
public class PoinCollider
{
    public int IndexCol { get; set; }
    public GameObject ObjColl { get; set; }

    public int NumUnit { get; set; }
    public Vector2 PointColl { get; set; }
}
//юниты с расчитанным обходом препятствия
public class PoinColliderCalc
{
    public int IndexCol { get; set; }
    public GameObject ObjColl { get; set; }
    public int NumBase { get; set; }
    public int NumUnit { get; set; }
    public List<UnitSpeed_Position> ListPointColl { get; set; }
}
//  точка обхода плюс скорость
public class UnitSpeed_Position
{
    public float kfSpeed { get; set; }
    public Vector2 PointOut { get; set; }
}

public class PosAll
{
    public List<Vector2> poss { get; set; }
}
public class MassArrPos
{
    public int numU { get; set; }
    public int iter { get; set; }

    public Vector2 pis { get; set; } 
}