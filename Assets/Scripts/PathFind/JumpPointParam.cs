using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JumpPointParam 
{
    public Node StartNode;
    public Node EndNode;
    public List<Node> openList;

    public JumpPointParam(Node m_startNode, Node m_endNode, List<Node> m_openList)
    {
        this.StartNode = m_startNode;
        this.EndNode = m_endNode;
        this.openList = m_openList;
    }

    public JumpPointParam()
    {
        StartNode = null;
        EndNode = null;
        openList = new List<Node>() ;
    }

    public void Reset(Node iStartPos, Node iEndPos)
    {
        StartNode = iStartPos;
        EndNode = iEndPos;
        openList.Clear();
    }

}
