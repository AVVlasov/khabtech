using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class TileMap : MonoBehaviour
{
    int[,] tiles;
    public int mapSizeX;
    public int mapSizeY;
    public GameObject background, Tiles;
    public float width, height, widthTile, heightTile, back2W, back2H;

    public Node[,] graph;

    public JumpPointParam iParam = new JumpPointParam();

    void Start()
    {
        GenerateMapData();
        GenerateMapVisual();
        GeneratePathfindingGraph();

    }
    void GenerateMapData()
    {
        var renderer = background.GetComponent<Renderer>();
        width = renderer.bounds.size.x;
        height = renderer.bounds.size.y;

        var rendererTile = Tiles.GetComponent<Renderer>();
        widthTile = rendererTile.bounds.size.x;
        heightTile = rendererTile.bounds.size.y;

        mapSizeX = (int)(width / widthTile) + 1;
        mapSizeY = (int)(height / heightTile) + 1;

        back2W = width / 2;
        back2H = height / 2;
    }

    public float CostToEnterTile(int sourceX, int sourceY, int targetX, int targetY)
    {

        if (tiles[targetX, targetY] == 1)
            return Mathf.Infinity;
        float cost = 1;

        if (sourceX != targetX && sourceY != targetY)
        {
            cost += 0.001f;
        }

        return cost;

    }

    public Vector2 TileCoordToWorldCoord(float x, float y)
    {
        return new Vector2(x, y);
    }

    public bool IsWalkableAt(int x, int y)
    {
        if (x < 0 || y < 0 || x > mapSizeX - 1 || y > mapSizeY - 1)
        {
            return false;
        }
                
            if (tiles[x, y] == 1)
            {
                return false;
            }
        
     
        return true;
    }
    

    void GeneratePathfindingGraph()
    {
        float posX, posY;
        posX = -back2W + (widthTile / 2);
        posY = -back2H + (heightTile / 2);

        graph = new Node[mapSizeX, mapSizeY];

        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                graph[x, y] = new Node();
                graph[x, y].x = posX;
                graph[x, y].y = posY;
                graph[x, y].xz = x;
                graph[x, y].yz = y;
                posY += heightTile;
            }
            posX += widthTile;
            posY = -back2H + (heightTile / 2);
        }


        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {

                // ищем с лева соседей
                if (x > 0)
                {
                    if (IsWalkableAt(x - 1, y))
                    {
                        graph[x, y].neighbours.Add(graph[x - 1, y]);
                    }

                    if (y > 0)
                    {
                        if (IsWalkableAt(x - 1, y - 1))
                        {
                            graph[x, y].neighbours.Add(graph[x - 1, y - 1]);
                        }

                    }

                    if (y < mapSizeY - 1)
                    {
                        if (IsWalkableAt(x - 1, y + 1))
                        {
                            graph[x, y].neighbours.Add(graph[x - 1, y + 1]);
                        }

                    }

                }

                // ищем справа соседей
                if (x < mapSizeX - 1)
                {
                    if (IsWalkableAt(x + 1, y))
                    {
                        graph[x, y].neighbours.Add(graph[x + 1, y]);
                    }

                    if (y > 0)
                    {
                        if (IsWalkableAt(x + 1, y - 1))
                        {
                            graph[x, y].neighbours.Add(graph[x + 1, y - 1]);
                        }
                    }

                    if (y < mapSizeY - 1)
                    {
                        if (IsWalkableAt(x + 1, y + 1))
                        {
                            graph[x, y].neighbours.Add(graph[x + 1, y + 1]);
                        }
                    }
                }

                // вверх и вниз
                if (y > 0)
                {
                    if (IsWalkableAt(x, y - 1))
                    {
                        graph[x, y].neighbours.Add(graph[x, y - 1]);
                    }
                }

                if (y < mapSizeY - 1)
                {
                    if (IsWalkableAt(x, y + 1))
                    {
                        graph[x, y].neighbours.Add(graph[x, y + 1]);
                    }
                }



            }
        }

    }

    public void clear_graph()
    {      
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                graph[x, y].Reset() ;
              
            }
        }
    }


    public List<Node> FindPath(GameObject source, GameObject target)
    {
        source.GetComponent<Movement>().currentPath = null;
        clear_graph();       
        List<Node> tOpenList = iParam.openList; //открытый список
        
        Node tNode =null;

        Node tStartNode = graph[
                         source.GetComponent<Movement>().TileX,
                         source.GetComponent<Movement>().TileY
                         ];

        Node tEndNode = graph[
                            target.GetComponent<Movement>().TileX,
                            target.GetComponent<Movement>().TileY
                            ];

        iParam.Reset(tStartNode, tEndNode);
        tOpenList.Add(tStartNode);
        tStartNode.isOpened = true;   
        
        while (tOpenList.Count > 0)
        {
           
            tOpenList.Sort();
           
            tNode = (Node)tOpenList[0];
            tOpenList.RemoveAt(0);
            tNode.isClosed = true;

            if (tNode.xz == tEndNode.xz && tNode.yz == tEndNode.yz)
            {
                return Node.Backtrace(tNode); // перестраиваем путь
            }

           
                identifySuccessors(iParam, tNode);          

        }

        return new List<Node>();
    }
    private class JumpSnapshot
    {
        public int iX;
        public int iY;
        public int iPx;
        public int iPy;
        public int tDx;
        public int tDy;
        public GridPos? jx;
        public GridPos? jy;
        public int stage;
        public JumpSnapshot()
        {

            iX = 0;
            iY = 0;
            iPx = 0;
            iPy = 0;
            tDx = 0;
            tDy = 0;
            jx = null;
            jy = null;
            stage = 0;
        }
    }

    public GridPos? jumpLoop(JumpPointParam iParam, int iX, int iY, int iPx, int iPy)
    {
        GridPos? retVal = null;
        Stack<JumpSnapshot> stack = new Stack<JumpSnapshot>();

        JumpSnapshot currentSnapshot = new JumpSnapshot();
        JumpSnapshot newSnapshot = null;
        currentSnapshot.iX = iX;
        currentSnapshot.iY = iY;
        currentSnapshot.iPx = iPx;
        currentSnapshot.iPy = iPy;
        currentSnapshot.stage = 0;

        stack.Push(currentSnapshot);
        while (stack.Count != 0)
        {
            currentSnapshot = stack.Pop();
            switch (currentSnapshot.stage)
            {
                case 0:
                    if (!IsWalkableAt(currentSnapshot.iX, currentSnapshot.iY))
                    {
                        retVal = null;
                        continue;
                    }
                    else if (graph[currentSnapshot.iX, currentSnapshot.iY] == iParam.EndNode)
                    {
                        retVal = new GridPos(currentSnapshot.iX, currentSnapshot.iY);
                        continue;
                    }
                    currentSnapshot.tDx = currentSnapshot.iX - currentSnapshot.iPx;
                    currentSnapshot.tDy = currentSnapshot.iY - currentSnapshot.iPy;
                    currentSnapshot.jx = null;
                    currentSnapshot.jy = null;

                    // check for forced neighbors
                    // along the diagonal
                    if (currentSnapshot.tDx != 0 && currentSnapshot.tDy != 0)
                    {
                        if ((IsWalkableAt(currentSnapshot.iX - currentSnapshot.tDx, currentSnapshot.iY + currentSnapshot.tDy) && !IsWalkableAt(currentSnapshot.iX - currentSnapshot.tDx, currentSnapshot.iY)) ||
                            (IsWalkableAt(currentSnapshot.iX + currentSnapshot.tDx, currentSnapshot.iY - currentSnapshot.tDy) && !IsWalkableAt(currentSnapshot.iX, currentSnapshot.iY - currentSnapshot.tDy)))
                        {
                            retVal = new GridPos(currentSnapshot.iX, currentSnapshot.iY);
                            continue;
                        }
                    }
                    // horizontally/vertically
                    else
                    {
                        if (currentSnapshot.tDx != 0)
                        {
                            // moving along x
                            if ((IsWalkableAt(currentSnapshot.iX + currentSnapshot.tDx, currentSnapshot.iY + 1) && !IsWalkableAt(currentSnapshot.iX, currentSnapshot.iY + 1)) ||
                                (IsWalkableAt(currentSnapshot.iX + currentSnapshot.tDx, currentSnapshot.iY - 1) && !IsWalkableAt(currentSnapshot.iX, currentSnapshot.iY - 1)))
                            {
                                retVal = new GridPos(currentSnapshot.iX, currentSnapshot.iY);
                                continue;
                            }
                        }
                        else
                        {
                            if ((IsWalkableAt(currentSnapshot.iX + 1, currentSnapshot.iY + currentSnapshot.tDy) && !IsWalkableAt(currentSnapshot.iX + 1, currentSnapshot.iY)) ||
                                (IsWalkableAt(currentSnapshot.iX - 1, currentSnapshot.iY + currentSnapshot.tDy) && !IsWalkableAt(currentSnapshot.iX - 1, currentSnapshot.iY)))
                            {
                                retVal = new GridPos(currentSnapshot.iX, currentSnapshot.iY);
                                continue;
                            }
                        }
                    }
                    // when moving diagonally, must check for vertical/horizontal jump points
                    if (currentSnapshot.tDx != 0 && currentSnapshot.tDy != 0)
                    {
                        currentSnapshot.stage = 1;
                        stack.Push(currentSnapshot);

                        newSnapshot = new JumpSnapshot();
                        newSnapshot.iX = currentSnapshot.iX + currentSnapshot.tDx;
                        newSnapshot.iY = currentSnapshot.iY;
                        newSnapshot.iPx = currentSnapshot.iX;
                        newSnapshot.iPy = currentSnapshot.iY;
                        newSnapshot.stage = 0;
                        stack.Push(newSnapshot);
                        continue;
                    }

                    // moving diagonally, must make sure one of the vertical/horizontal
                    // neighbors is open to allow the path

                    // moving diagonally, must make sure one of the vertical/horizontal
                    // neighbors is open to allow the path
                    if (IsWalkableAt(currentSnapshot.iX + currentSnapshot.tDx, currentSnapshot.iY) || IsWalkableAt(currentSnapshot.iX, currentSnapshot.iY + currentSnapshot.tDy))
                    {
                        newSnapshot = new JumpSnapshot();
                        newSnapshot.iX = currentSnapshot.iX + currentSnapshot.tDx;
                        newSnapshot.iY = currentSnapshot.iY + currentSnapshot.tDy;
                        newSnapshot.iPx = currentSnapshot.iX;
                        newSnapshot.iPy = currentSnapshot.iY;
                        newSnapshot.stage = 0;
                        stack.Push(newSnapshot);
                        continue;
                    }
                    retVal = null;
                    break;
                case 1:
                    currentSnapshot.jx = retVal;

                    currentSnapshot.stage = 2;
                    stack.Push(currentSnapshot);

                    newSnapshot = new JumpSnapshot();
                    newSnapshot.iX = currentSnapshot.iX;
                    newSnapshot.iY = currentSnapshot.iY + currentSnapshot.tDy;
                    newSnapshot.iPx = currentSnapshot.iX;
                    newSnapshot.iPy = currentSnapshot.iY;
                    newSnapshot.stage = 0;
                    stack.Push(newSnapshot);
                    break;
                case 2:
                    currentSnapshot.jy = retVal;
                    if (currentSnapshot.jx != null || currentSnapshot.jy != null)
                    {
                        retVal = new GridPos(currentSnapshot.iX, currentSnapshot.iY);
                        continue;
                    }

                    // moving diagonally, must make sure one of the vertical/horizontal
                    // neighbors is open to allow the path
                    if (IsWalkableAt(currentSnapshot.iX + currentSnapshot.tDx, currentSnapshot.iY) || IsWalkableAt(currentSnapshot.iX, currentSnapshot.iY + currentSnapshot.tDy))
                    {
                        newSnapshot = new JumpSnapshot();
                        newSnapshot.iX = currentSnapshot.iX + currentSnapshot.tDx;
                        newSnapshot.iY = currentSnapshot.iY + currentSnapshot.tDy;
                        newSnapshot.iPx = currentSnapshot.iX;
                        newSnapshot.iPy = currentSnapshot.iY;
                        newSnapshot.stage = 0;
                        stack.Push(newSnapshot);
                        continue;
                    }

                    retVal = null;
                    break;
                case 3:
                    currentSnapshot.jx = retVal;

                    currentSnapshot.stage = 4;
                    stack.Push(currentSnapshot);

                    newSnapshot = new JumpSnapshot();
                    newSnapshot.iX = currentSnapshot.iX;
                    newSnapshot.iY = currentSnapshot.iY + currentSnapshot.tDy;
                    newSnapshot.iPx = currentSnapshot.iX;
                    newSnapshot.iPy = currentSnapshot.iY;
                    newSnapshot.stage = 0;
                    stack.Push(newSnapshot);
                    break;
                case 4:
                    currentSnapshot.jy = retVal;
                    if (currentSnapshot.jx != null || currentSnapshot.jy != null)
                    {
                        retVal = new GridPos(currentSnapshot.iX, currentSnapshot.iY);
                        continue;
                    }

                    // moving diagonally, must make sure both of the vertical/horizontal
                    // neighbors is open to allow the path
                    if (IsWalkableAt(currentSnapshot.iX + currentSnapshot.tDx, currentSnapshot.iY) && IsWalkableAt(currentSnapshot.iX, currentSnapshot.iY + currentSnapshot.tDy))
                    {
                        newSnapshot = new JumpSnapshot();
                        newSnapshot.iX = currentSnapshot.iX + currentSnapshot.tDx;
                        newSnapshot.iY = currentSnapshot.iY + currentSnapshot.tDy;
                        newSnapshot.iPx = currentSnapshot.iX;
                        newSnapshot.iPy = currentSnapshot.iY;
                        newSnapshot.stage = 0;
                        stack.Push(newSnapshot);
                        continue;
                    }
                    retVal = null;
                    break;

            }
        }
        return retVal;
    }

  


    public float Manhattan(int iDx, int iDy)
    {
        return (float)iDx + iDy;
    }
    public void identifySuccessors(JumpPointParam iParam, Node iNode)
    {
        List<Node> tOpenList = iParam.openList;
        int tEndX = iParam.EndNode.xz;
        int tEndY = iParam.EndNode.yz;
        Node tNeighbor;
        GridPos? tJumpPoint;
        Node tJumpNode;
        List<Node> tNeighbors = iNode.neighbours;
        for (int i = 0; i < iNode.neighbours.Count; i++)
        {
            tNeighbor = tNeighbors[i];

            tJumpPoint = jumpLoop(iParam, tNeighbor.xz, tNeighbor.yz, iNode.xz, iNode.yz);

            if (tJumpPoint != null)
            {
                tJumpNode = graph[tJumpPoint.Value.x, tJumpPoint.Value.y];
                if (tJumpNode == null)
                {
                    if (iParam.EndNode.xz == tJumpPoint.Value.x && iParam.EndNode.yz == tJumpPoint.Value.y)
                        tJumpNode = graph[tJumpPoint.Value.x, tJumpPoint.Value.y];
                }
                if (tJumpNode.isClosed)
                {
                    continue;
                }
         
                float tCurNodeToJumpNodeLen = Manhattan(Mathf.Abs(tJumpPoint.Value.x - iNode.xz), Mathf.Abs(tJumpPoint.Value.y - iNode.yz));
                float tStartToJumpNodeLen = iNode.startToCurNodeLen + tCurNodeToJumpNodeLen; 
                if (!tJumpNode.isOpened || tStartToJumpNodeLen < tJumpNode.startToCurNodeLen)
                {
                    tJumpNode.startToCurNodeLen = tStartToJumpNodeLen;
                    tJumpNode.heuristicCurNodeToEndLen = (tJumpNode.heuristicCurNodeToEndLen == null ? Manhattan(Mathf.Abs(tJumpPoint.Value.x - tEndX), Mathf.Abs(tJumpPoint.Value.y - tEndY)) : tJumpNode.heuristicCurNodeToEndLen);
                    tJumpNode.heuristicStartToEndLen = tJumpNode.startToCurNodeLen + tJumpNode.heuristicCurNodeToEndLen.Value;
                    tJumpNode.parent = iNode;

                    if (!tJumpNode.isOpened)
                    {
                        tOpenList.Add(tJumpNode);
                        tJumpNode.isOpened = true;
                    }
                }
            }
        }

    }

    public Vector2 GetPos(Vector2 pos, int type)
    {
        Vector2 posNew = pos;
        if(type ==1){
        posNew.x = pos.x + widthTile/2;
        posNew.y = pos.y + heightTile/2;

        }else if(type ==2){
            posNew.x = pos.x - widthTile/2;
            posNew.y = pos.y - heightTile/2;
        }
        
        return posNew;
    }


    void GenerateMapVisual()
    {
        float posX, posY;
        posX = -back2W + (widthTile / 2);
        posY = -back2H + (heightTile / 2);
        int x, y;
        tiles = new int[mapSizeX, mapSizeY];
        for (x = 0; x < mapSizeX; x++)
        {
            for (y = 0; y < mapSizeY; y++)
            {
                GameObject go = (GameObject)Instantiate(Tiles, new Vector3(posX, posY, 1), Quaternion.identity);
                go.name = "tile_(" + x + " " + y + ")";
               
                if ((Physics2D.OverlapArea(GetPos(go.transform.position, 2), GetPos(go.transform.position, 1)) != null && Physics2D.OverlapArea(GetPos(go.transform.position, 2), GetPos(go.transform.position, 1)).tag == "wall"))
                {
                    go.GetComponent<SpriteRenderer>().material = Buffer.Instance.base5;
                    go.GetComponent<Tile>().isWalkable = false;
                    tiles[x, y] = 1;

                }
                go.GetComponent<Tile>().tileX = x;
                go.GetComponent<Tile>().tileY = y;

                posY += heightTile;
            }
            posX += widthTile;
            posY = -back2H + (heightTile / 2);

        }

    }


}
