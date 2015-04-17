using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



public class Node : IComparable
{
    public List<Node> neighbours;
    public float x;
    public float y;
    public int xz;
    public int yz;

    public bool walkable;
    public float heuristicStartToEndLen; // which passes current node
    public float startToCurNodeLen;
    public float? heuristicCurNodeToEndLen;
    public bool isOpened;
    public bool isClosed;
    public Node parent;


    public Node()
    {
        neighbours = new List<Node>();
//        parent = new Node();
    }
    public int CompareTo(object iObj)
    {
        Node tOtherNode = (Node)iObj;
        float result = this.heuristicStartToEndLen - tOtherNode.heuristicStartToEndLen;
        if (result > 0.0f)
            return 1;
        else if (result == 0.0f)
            return 0;
        return -1;
    }
   
    public static List<Node> Backtrace(Node iNode)
    {
        List<Node> path = new List<Node>();
        path.Add(iNode);
        while (iNode.parent != null)
        {
            iNode = (Node)iNode.parent;
            path.Add(iNode);
        }
        path.Reverse();
        return path;
    }
    public void Reset()
    {        
        this.heuristicStartToEndLen = 0;
        this.startToCurNodeLen = 0;
        this.heuristicCurNodeToEndLen = null;
        this.isOpened = false;
        this.isClosed = false;
        this.parent = null;
    }

}
