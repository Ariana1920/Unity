using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingController : MonoBehaviour
{
    public MapData mapData;
    public Graph graph;

    public Pathfinder pathfinder;
    public int startX = 0;
    public int startY = 0;
    public int goalX = 15;
    public int goalY = 10;

    public float timeStep = 0.1f;

    void Start()
    {
        if (mapData != null && graph != null)
        {
            // generate the map from text file or texture map
            int[,] mapInstance = mapData.MakeMap();
            // initialize the graph
            graph.Initialze(mapInstance);

            GraphView graphView = graph.gameObject.GetComponent<GraphView>();

            if (graphView != null)
            {
                graphView.Initialize(graph);
            }

            if (graph.IsWithinBounds(startX, startY) && graph.IsWithinBounds(goalX, goalY)
                && pathfinder != null) //within the boundary
            {
                Node startNode = graph.nodes[startX, startY];
                Node goalNode = graph.nodes[goalX, goalY];

                pathfinder.Initialze(graph, graphView, startNode, goalNode);
                StartCoroutine(pathfinder.SearchRoutine(timeStep));
            }
        }
    }

}

