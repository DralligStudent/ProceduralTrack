﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGrid : MonoBehaviour
{
    public Vector2 gridSize;
    Node[,] grid;
    public int cornerNum = 15;
    int cornerCounter = 0;
    public List<Node> cornerPos;
    Node endOfStraight, startOfStraight;
    public Node[] cornerOrder;

    int distanceValue = 100;



    // Use this for initialization
    void Start()
    {
        cornerPos = new List<Node>();
        cornerOrder = new Node[cornerNum + 2];
        setCorners();
        setStartFinishStraight();

        grid = new Node[(int)gridSize.x, (int)gridSize.y];
        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                Vector2 nPos = new Vector2(x, y);
                grid[x, y] = new Node(nPos, true);
            }
        }
        cornerNodeOrder();

















        //retracePath()
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridSize.x, 1, gridSize.y));
        Gizmos.color = Color.red;
        for (int i = 1; i < cornerNum + 2; i++)
        {
            Gizmos.DrawSphere(cornerOrder[i - 1].Position, 1);
            Gizmos.DrawLine(cornerOrder[i - 1].Position, cornerOrder[i].Position);
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    List<Node> getNeighbours(Node node)
    {
        int xVal, yVal;

        List<Node> neighbours = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                xVal = (int)node.Position.x + x;
                yVal = (int)node.Position.y + y;

                if (xVal >= 0 && xVal < gridSize.x && yVal >= 0 && yVal < gridSize.y)
                {
                    neighbours.Add(grid[xVal, yVal]);
                }
            }
        }

        return neighbours;
    }

    void setCorners()
    {
        for (int i = 0; i < cornerNum; i++)
        {
            int _x = Random.Range(0, 200);
            int _y = Random.Range(0, 200);

            if (_y < 100)
            {
                int diff = 100 - _y;

                _y += Random.Range(diff+20, diff + 55);
            }

            if (_y == 100)
            {
                _y += Random.Range(30, 60);
            }
            Vector2 nPos = new Vector2(_x, _y);
            Node n = new Node(nPos, true);
            Debug.Log(_x + ",  "+ _y);
            cornerPos.Add(n);
        }
        

    }

    void setStartFinishStraight() //sets the start finish straight in the center of the map, traveling right to left
    {
        int finishX = Random.Range(50, 75);
        int startX = Random.Range(125, 150);
        startOfStraight = new Node(new Vector2(startX, 100), true);
        endOfStraight = new Node(new Vector2(finishX, 100), true);
    }

    void findPath(Vector2 startPos, Vector2 endPos)
    {
        Node initialNode = grid[(int)startPos.x, (int)startPos.y];
        Node targetNode = grid[(int)endPos.x, (int)endPos.y];

        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();
        openList.Add(initialNode);

        while (openList.Count > 0)
        {
            Node currentNode = openList[0];
            for (int i = 0; i < openList.Count; i++)
            {
                if (openList[i].fCost < currentNode.fCost || openList[i].fCost == currentNode.fCost && openList[i].hCost < currentNode.hCost)
                {
                    currentNode = openList[i];
                    
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode == targetNode)
            {

                return;
            }

            List<Node> nList = getNeighbours(currentNode);
            foreach (Node neighbour in nList)
            {
                if (!neighbour.isWalkable || closedList.Contains(neighbour))
                {
                    continue;
                }

                int newMovCost = currentNode.gCost + getDist(currentNode, neighbour);
                if (newMovCost < neighbour.gCost || !openList.Contains(neighbour))
                {
                    neighbour.gCost = newMovCost;
                    neighbour.hCost = getDist(neighbour, targetNode);

                    neighbour.Parent = currentNode;

                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                }
            }
        }
    }

    int getDist(Node n1, Node n2)
    {
        int distX = Mathf.Abs((int)n1.Position.x - (int)n2.Position.x);
        int distY = Mathf.Abs((int)n1.Position.y - (int)n2.Position.y);

        if (distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
        else
        {
            return 14 * distX + 10 * (distY - distX);
        }
            
    }

    void retracePath(Node start, Node end)
    {
        List<Node> path = new List<Node>();

        Node current = end;
        while (current != start)
        {
            path.Add(current);
            current = current.Parent;
        }
        path.Reverse();


        //Alright you furr burger heres what you are going to do:
        //create a list path from point to point on the track.
        //create a list. any time you traverse to a new node, add the old node to this list. (prevent overlap)
        //alter algorithm that if node is in "trackposition" list(see previous create list line), it can not use it and must move on to another neighbor
    }

    void cornerNodeOrder() //creates an ordered list of nodes that are then to be pathfound between.
    {
        cornerCounter = 0;
        cornerOrder[cornerCounter] = startOfStraight;
        cornerCounter++;
        cornerOrder[cornerCounter] = endOfStraight;
        cornerCounter++;


        List<Node> firstCorner = new List<Node>();
        List<Node> lastCorner = new List<Node>();
        //Debug.Log("fc");
        List<Node> tempList = new List<Node>();
        List<Node> orderedLastCorner = new List<Node>();


        //Following part contains algorithm for defining the first corners
        {
            for (int n = 0; n < cornerPos.Count; n++)
            {
                //Debug.Log(n);
                if (cornerPos[n].Position.x < endOfStraight.Position.x + 5 || (cornerPos[n].Position.y < 100 && cornerPos[n].Position.x < ((startOfStraight.Position.x - endOfStraight.Position.x) / 2)))
                {
                    //Debug.Log(cornerPos[n].Position);
                    tempList.Add(cornerPos[n]);
                    //cornerPos.Remove(cornerPos[n]);
                }
            }

            foreach (Node toRemove in tempList)
            {
                cornerPos.Remove(toRemove);
                //cornerOrder[cornerCounter] = toRemove;
                firstCorner.Add(toRemove);
                //cornerCounter++;
            }
            tempList.Clear();

            if (firstCorner.Count > 0)
            {
                //int turnOne = (int)endOfStraight.Position.x;
                if (firstCorner.Count < 2)
                {
                    Debug.Log(firstCorner.Count);
                    Debug.Log("im always in here when i shouldnt be");
                    cornerOrder[cornerCounter] = firstCorner[0];
                    cornerCounter++;
                }
                else
                {

                    while (firstCorner.Count > 0)
                    {
                        //Debug.Log(firstCorner.Count);
                        int currentChoice = 0;
                        int lowestChoice = 0;
                        //int lowestValue = 0;
                        distanceValue = 10000;
                        //if (firstCorner.Count >= 2)
                        {
                            foreach (Node n in firstCorner)
                            {
                                int distX;
                                if (cornerOrder[cornerCounter - 1] == endOfStraight)
                                {
                                    distX = getDist(endOfStraight, n);
                                }
                                else
                                {
                                    distX = getDist(cornerOrder[cornerCounter - 1], n);
                                    //Debug.Log("distX = " + distX);
                                }
                                //
                                //
                                if (distX < distanceValue)
                                {
                                    distanceValue = distX;
                                    //lowestValue = currentChoice;
                                    lowestChoice = currentChoice;
                                }
                                currentChoice++;
                            }
                            //Debug.Log(cornerCounter);
                            cornerOrder[cornerCounter] = firstCorner[lowestChoice];
                            cornerCounter++;
                            firstCorner.Remove(firstCorner[lowestChoice]);
                        }
                        //else
                        {
                            //Debug.Log("im breaking lmao");
                            //Debug.Log(firstCorner[0]);
                            //cornerOrder[cornerCounter] = firstCorner.;
                            //cornerCounter++;
                        }
                    }
                }

                //cornerOrder[2] = FirstCorner.
            }
        }

        foreach (Node toRemove in firstCorner)
        {
            Debug.Log(toRemove.Position + " got cucked");
            cornerPos.Remove(toRemove);
        }

        //following part contains algorithm for defining the final corners
        {
            for (int n = 0; n < cornerPos.Count; n++)
            {
                //Debug.Log(n);
                if (cornerPos[n].Position.x > startOfStraight.Position.x - 10)
                {
                    //Debug.Log(cornerPos[n].Position);
                    tempList.Add(cornerPos[n]);
                    //cornerPos.Remove(cornerPos[n]);
                }
            }

            foreach (Node toRemove in tempList)
            {
                lastCorner.Add(toRemove);
                cornerPos.Remove(toRemove);
                //cornerOrder[cornerCounter] = toRemove;
                //cornerCounter++;
            }

            int lastCornerAcc = 0;

            Debug.Log(lastCorner.Count);
            while (lastCorner.Count > 0)
            {
                //Debug.Log(firstCorner.Count);
                int currentChoice = 0;
                int lowestChoice = 0;
                //int lowestValue = 0;
                distanceValue = 10000;
                //if (lastCorner.Count >= 2)
                foreach (Node n in lastCorner)
                {
                    int distX = 0;
                    if (lastCornerAcc < 1)
                    {
                        distX = getDist(startOfStraight, n);
                    }
                    else
                    {
                        Debug.Log(lastCornerAcc);
                        distX = getDist(orderedLastCorner[lastCornerAcc -1], n);
                        //Debug.Log("distX = " + distX);
                    }
                    //
                    //
                    if (distX < distanceValue)
                    {
                        distanceValue = distX;
                        //lowestValue = currentChoice;
                        lowestChoice = currentChoice;
                    }
                    currentChoice++;

                }
                lastCornerAcc++;
                orderedLastCorner.Add(lastCorner[lowestChoice]);
                lastCorner.Remove(lastCorner[lowestChoice]);
                //cornerPos.Remove(orderedLastCorner[lastCornerAcc]);
                //Debug.Log(cornerCounter);
                //cornerOrder[cornerCounter] = firstCorner[lowestChoice];
                //cornerCounter++;
                //firstCorner.Remove(firstCorner[lowestChoice]);
            }

            orderedLastCorner.Reverse();
            orderedLastCorner.Remove(startOfStraight);
            //Debug.Log("hopefully " + orderedLastCorner.Count + " "+ cornerPos.Count + "makes");
        }

        //finds the closest point from the end of the straight, and adds that to the order.
        { 
            /*
            int distConsider = 10000;
            int listAcc = 0;
            int listPosAcc = 0;
            foreach (Node N in cornerPos)
            {
                int nDist = getDist(N, endOfStraight);
                if (nDist < distConsider)
                {
                    distConsider = nDist;
                    listPosAcc = listAcc;
                }
                listAcc++;
            }
            
            //if()
            cornerOrder[cornerCounter] = cornerPos[listPosAcc];
            cornerCounter++;
            cornerPos.Remove(cornerPos[listPosAcc]);
            */
        }


        int listPosOfShortest = 0;
        distanceValue = 1000;
        int cpCount = cornerPos.Count;
        Debug.Log(cpCount);

        for (int i = 0; i < cpCount; i++)
        {
            //int startVal = i;
            int listPos = 0;
            listPosOfShortest = 0;
            distanceValue = 1000;
            int newDistEnd = getDist(cornerOrder[cornerCounter - 1], startOfStraight);
            foreach (Node n in cornerPos)
            {
                //if(i != startVal)
                {
                    int newDistCorn = getDist(cornerOrder[cornerCounter-1], n); //its cornercounter -1 since 

                    if (newDistCorn < distanceValue && newDistCorn < newDistEnd)
                    {
                        distanceValue = newDistCorn;
                        listPosOfShortest = listPos;
                    }
                    else
                    {
                        
                        Debug.Log("ALGORITHM SHOULD GO HERE AND WERE AT " + cornerCounter);
                    }
                    
                    listPos++;

                }
            }

            if (cornerPos.Count > 0)
            {
                Debug.Log("we looping bois");
                cornerOrder[cornerCounter] = cornerPos[listPosOfShortest];
                cornerPos.Remove(cornerPos[listPosOfShortest]);
                cornerCounter++;
            }
            else
            {
                Debug.Log("BROKEN HERE");
            }
            //Debug.Log(cornerOrder[cornerCounter]);
            Debug.Log(i);
            //Debug.Log(cornerPos);
            Debug.Log(listPosOfShortest);
            Debug.Log(cornerPos.Count);
            //Debug.Log(cornerPos[listPosOfShortest]);
            //Debug.Log()


        }


        /*
        Debug.Log("we here");
        int lol = 0;
        foreach (Node Complete in cornerOrder)
        {
            Debug.Log("Position " + lol +" = " + Complete.Position);
            lol++;
        }*/

        foreach (Node n in orderedLastCorner)
        {
            Debug.Log("do we make it?");
            Debug.Log("corner counter = " + cornerCounter);
            cornerOrder[cornerCounter] = n;
            cornerCounter++;
        }

        orderedLastCorner.Clear();

        Debug.Log(cornerOrder.Length);

        int lol = 0;
        foreach (Node Complete in cornerOrder)
        {
            Debug.Log("Position " + lol + " = " + Complete.Position);
            lol++;
        }
    }

    Vector3[] getMeshPath(Node[] nArray)
    {
        Vector3[] toReturn = new Vector3[nArray.Length];

        for (int i = 0; i < nArray.Length; i++)
        {
            Vector3 nextPoint = new Vector3(nArray[i].Position.x, 0.0f, nArray[i].Position.y);

            toReturn[i] = nextPoint;
        }

        return toReturn;
    }
}
