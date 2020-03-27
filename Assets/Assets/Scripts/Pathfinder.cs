using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pathfinder : MonoBehaviour
{
    //define some nodes 
    Node m_startNode;
    Node m_goalNode;
    Graph m_graph;
    GraphView m_graphView;

    //// queue instead of list
    PriorityQueue<Node> m_frontierNodes;
    //those nodes that have explored
    List<Node> m_exploredNodes;
    //those nodes that make up final path
    List<Node> m_pathNodes;

    //color used to color nodes respectively
    public Color startColor = Color.green;
    public Color goalColor = Color.red;
    public Color frontierColor = Color.magenta;
    public Color exploredColor = Color.gray;
    public Color pathColor = Color.cyan;
    public Color arrowColor = new Color(0.85f, 0.85f, 0.85f, 1f);
    public Color highlightColor = new Color(1f, 1f, 0.5f, 1f);

    public bool showIterations = true;
    public bool showColors = true;
    public bool exitOnGoal = true;

    public bool isComplete = false;
    int m_iterations = 0;

    //two algorithm method 
    public enum Mode
    {
        Dijkstra = 0,
        AStar =1
    }

    //default mode 
    public Mode mode = Mode.Dijkstra;

    //initialize pathfinder
    public void Initialze(Graph graph, GraphView graphView, Node start, Node goal)
    {
        if (start == null || goal == null || graph == null || graphView == null)
        {
            Debug.LogWarning("missing component(s)!");
            return;
        }

        if (start.nodeType == NodeType.Blocked || goal.nodeType == NodeType.Blocked)
        {
            Debug.LogWarning("Start and goal nodes must be unblocked!");
            return;
        }

        m_graph = graph;
        m_graphView = graphView;
        m_startNode = start;
        m_goalNode = goal;

        //color corresponding nodes
        ShowColors(graphView, start, goal); 

        m_frontierNodes = new PriorityQueue<Node>();

        //the first nodes to start
        m_frontierNodes.Enqueue(start);
        m_exploredNodes = new List<Node>();
        m_pathNodes = new List<Node>();

        //clear out previous path finding data
        for (int x = 0; x < m_graph.Width; x++)
        {
            for (int y = 0; y < m_graph.Height; y++)
            {
                m_graph.nodes[x, y].Reset();
            }
        }
        //setup starting values
        isComplete = false;
        m_iterations = 0;
        m_startNode.distanceTraveled = 0;
    }

    void ShowColors()
    {
        ShowColors(m_graphView, m_startNode, m_goalNode);
    }

    void ShowColors(GraphView graphView, Node start, Node goal)
    {
        if (graphView == null || start == null || goal == null)
        {
            return;
        }

        if (m_frontierNodes != null)
        {
            graphView.ColorNodes(m_frontierNodes.ToList(), frontierColor);
        }

        if (m_exploredNodes != null)
        {
            graphView.ColorNodes(m_exploredNodes, exploredColor);
        }

        if (m_pathNodes != null && m_pathNodes.Count > 0)
        {
            graphView.ColorNodes(m_pathNodes, pathColor);
        }

        NodeView startNodeView = graphView.nodeViews[start.xIndex, start.yIndex];

        if (startNodeView != null)
        {
            startNodeView.ColorNode(startColor); //color start node
        }

        NodeView goalNodeView = graphView.nodeViews[goal.xIndex, goal.yIndex];

        if (goalNodeView != null)
        {
            goalNodeView.ColorNode(goalColor); //color end node
        }
    }

    //main graph search rooutine
    public IEnumerator SearchRoutine(float timeStep = 0.1f)
    {
        float timeStart = Time.time;

        yield return null;

        while (!isComplete)
        {
            if (m_frontierNodes.Count > 0)
            {
                Node currentNode = m_frontierNodes.Dequeue();
                m_iterations++;

                //marked this node as explored
                if (!m_exploredNodes.Contains(currentNode))
                {
                    m_exploredNodes.Add(currentNode);
                }

                if (mode == Mode.Dijkstra)
                {
                    ExpandFrontierDijkstra(currentNode);
                }
                else if (mode == Mode.AStar)
                {
                    ExpandFrontierAStar(currentNode);
                }

                // if the goal node is in the frontier 
                if (m_frontierNodes.Contains(m_goalNode))
                {
                    //set the path node
                    m_pathNodes = GetPathNodes(m_goalNode);

                    if (exitOnGoal)
                    {
                        //if exitongoal is checked, finishing the search
                        isComplete = true;
                        Debug.Log("PATHFINDER mode: " + mode.ToString() + 
                                  "    path length = " + m_goalNode.distanceTraveled.ToString());
                    }
                }

                if (showIterations)
                {
                    ShowDiagnostics();

                    yield return new WaitForSeconds(timeStep);
                }
            }
            else
            {
                isComplete = true;
            }
        }

        ShowDiagnostics();
        Debug.Log("PATHFINDER SearchRoutine: elapse time = " + (Time.time - timeStart).ToString() +
                  " seconds");

    }

    private void ShowDiagnostics()
    {
        if (showColors)
        {
            ShowColors();
        }
    }

    void ExpandFrontierDijkstra(Node node)
    {
        if (node != null)
        {
            //loop through the neighbors
            for (int i = 0; i < node.neighbors.Count; i++)
            {
                //if the current neighbor has not been explored
                if (!m_exploredNodes.Contains(node.neighbors[i]))
                {
                    float distanceToNeighbor = m_graph.GetNodeDistance(node, node.neighbors[i]);
                    float newDistanceTraveled = distanceToNeighbor + node.distanceTraveled;

                    //if a shorter path exists to the neighbor via this node, re-route
                    if (float.IsPositiveInfinity(node.neighbors[i].distanceTraveled) ||
                        newDistanceTraveled < node.neighbors[i].distanceTraveled)
                    {
                        node.neighbors[i].previous = node;
                        node.neighbors[i].distanceTraveled = newDistanceTraveled;
                    }

                    // if the current neighbor is not already part of the frontier
                    if (!m_frontierNodes.Contains(node.neighbors[i]))
                    {
                        // set the priority based on distance traveled from start Node and add to frontier
                        node.neighbors[i].priority = (int)node.neighbors[i].distanceTraveled;
                        m_frontierNodes.Enqueue(node.neighbors[i]);
                    }
                }
            }
        }
    }
    private void ExpandFrontierAStar(Node node)
    {
        if (node != null)
        {
            // loop through the neighbors
            for (int i = 0; i < node.neighbors.Count; i++)
            {
                // if the current neighbor has not been explored
                if (!m_exploredNodes.Contains(node.neighbors[i]))
                {
                    // calculate distance to current neighbor
                    float distanceToNeighbor = m_graph.GetNodeDistance(node, node.neighbors[i]);

                    // calculate cumulative distance if we travel to neighbor via the current node
                    float newDistanceTraveled = distanceToNeighbor + node.distanceTraveled + (int)node.nodeType;

                    // if a shorter path exists to the neighbor via this node, re-route
                    if (float.IsPositiveInfinity(node.neighbors[i].distanceTraveled) || newDistanceTraveled < node.neighbors[i].distanceTraveled)
                    {
                        node.neighbors[i].previous = node;
                        node.neighbors[i].distanceTraveled = newDistanceTraveled;
                    }

                    // if the neighbor is not part of the frontier, add this to the priority queue
                    if (!m_frontierNodes.Contains(node.neighbors[i]) && m_graph != null)
                    {
                        // base priority, F score,  on G score (distance from start) + H score (estimated distance to goal)
                        float distanceToGoal = m_graph.GetNodeDistance(node.neighbors[i], m_goalNode);
                        node.neighbors[i].priority = (int)(node.neighbors[i].distanceTraveled + distanceToGoal);

                        // add to priority queue using the F score
                        m_frontierNodes.Enqueue(node.neighbors[i]);
                    }
                }
            }
        }
    }
    // generate a list of nodes from an end node
    List<Node> GetPathNodes(Node endNode)
    {
        List<Node> path = new List<Node>();
        if (endNode == null)
        {
            return path;
        }
        path.Add(endNode);

        Node currentNode = endNode.previous;

        while (currentNode != null)
        {
            path.Insert(0, currentNode);
            currentNode = currentNode.previous;
        }

        return path;
    }

}
