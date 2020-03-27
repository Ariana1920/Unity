using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//define nodetype
public enum NodeType
{
    Open = 0,
    Blocked = 1
}
//// Node class implements the IComparable interface
public class Node: IComparable<Node>
{
    public NodeType nodeType = NodeType.Open;

    // 2d array to track the node position
    public int xIndex = -1;
    public int yIndex = -1;

    public Vector3 position;

    //list of neighbor nodes
    public List<Node> neighbors = new List<Node>();

    //total distance traveled from the start node
    public float distanceTraveled = Mathf.Infinity;
    public Node previous = null;

    //priority used to set place in queue
    public int priority;

    //constructor
    public Node(int xIndex, int yIndex, NodeType nodeType)
    {
        this.xIndex = xIndex;
        this.yIndex = yIndex;
        this.nodeType = nodeType;
    }
    // used to compare current node with another node based on priority
    public int CompareTo(Node other)
    {
        if (this.priority < other.priority)
        {
            return -1;
        }
        else if (this.priority > other.priority)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
    //  clear the previous nodes
    public void Reset()
    {
        previous = null;
    }


}
