using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMap : MonoBehaviour
{
    // PUBLIC DATA MEMBERS
    public MoveRoverAStar roverCtrl;
    public GameObject waypoint;
    public GameObject edge;
    public Terrain ground;
    public Material lineMaterial;

    // PRIVATE DATA MEMBERS
    // Debug flag
    bool isDebug = true;
    
    // Bounds for waypoints/paths
    private const int NUM_WAYPOINTS_TEST = 9;
    private const int NUM_WAYPOINTS = 14; // Includes the start and end points
    private int numPaths = 3; // Number of paths from start to end (may overlap)

    // Bounds for coordinates
    private float minX = -100f, maxX = 100f, minZ = 25f, maxZ = 80f, defaultY = -20f;
    //private float minX = -100f, maxX = 100f, minZ = 25f, maxZ = 225f, defaultY = -20f;

    // Data structures for waypoints/edges
    private GameObject[] waypoints;
    private List<int>[] edges;

    // LineRenderer object for drawing edges
    private LineRenderer lineRenderer;
    private int lineVertices = 2;

    // Start is called before the first frame update
    void Start()
    {
        // Use NUM_WAYPOINTS for random, NUM_WAYPOINTS_TEST for debug
        if (!isDebug)
        {
            waypoints = new GameObject[NUM_WAYPOINTS];
            edges = new List<int>[NUM_WAYPOINTS];
        }
        else
        {
            waypoints = new GameObject[NUM_WAYPOINTS_TEST];
            edges = new List<int>[NUM_WAYPOINTS_TEST];
        }
        
        // Initialize the lists in edges
        for (int i = 0; i < edges.Length; i++)
        {
            edges[i] = new List<int>();
        }
        
        // Get randomized waypoints and edges (send true for debug mode)
        GetMap(isDebug);

        // Pass the map's references over to the A-Star controller
        roverCtrl.Waypoints = waypoints;
        roverCtrl.Edges = edges;
        roverCtrl.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GetMap(bool isDebug = false)
    {
        // Get a deterministic map if testing; otherwise, get a randomized map
        if (isDebug)
        {
            GetWaypoints_Test();
            GetEdges_Test();
        }
        else
        {
            GetWaypoints_Random();
            GetEdges_Random();
        }
    }

    void GetWaypoints_Random()
    {
        // Variables for instantiating waypoints and determining heights above-ground
        GameObject curWaypoint;
        float heightBuff;

        // Set the start as a fixed point
        curWaypoint = Instantiate(waypoint);
        heightBuff = ground.SampleHeight(new Vector3(0f, 0f, 0f));
        curWaypoint.transform.position = new Vector3(0f, defaultY + heightBuff, 0f);
        waypoints[0] = curWaypoint;
        
        // Set the end as a fixed point
        curWaypoint = Instantiate(waypoint);
        heightBuff = ground.SampleHeight(new Vector3(0f, 0f, maxZ + 25f));
        curWaypoint.transform.position = new Vector3(0f, defaultY + heightBuff, maxZ + 25f);
        waypoints[waypoints.Length - 1] = curWaypoint;

        // Randomly select coordinates for 12 waypoints, within a range of coordinates
        for (int i = 1; i < waypoints.Length - 1; i++)
        {
            // Get random X and Y coordinates within the range
            float randX = Random.Range(minX, maxX);
            float randZ = Random.Range(minZ, maxZ);

            // Get the coordinates and the corresponding height
            heightBuff = ground.SampleHeight(new Vector3(randX, 0f, randZ));

            // Create a clone of the waypoint object and assign the random coordinates as its location
            curWaypoint = Instantiate(waypoint);
            curWaypoint.transform.position = new Vector3(randX, defaultY + heightBuff, randZ);

            // Add curWaypoint to the array
            waypoints[i] = curWaypoint;
        }
    }

    void GetEdges_Random()
    {
        // Create numPaths routes from the start node to the end node, visiting waypoints semi-randomly
        for (int i = 0; i < numPaths; i++)
        {
            // Create indices for start/end waypoints
            int a;
            int b = 0;

            // Loop until the end node is reached
            while (b < waypoints.Length - 1)
            {
                // Set the current start index to the previous end index
                a = b;

                // Get random indices for the edge's end waypoint
                while (a == b)
                {
                    // Minimum is a to prevent backtracking; if the random index is out of bounds, go to the end node
                    b = Mathf.Min(Random.Range(a, a + waypoints.Length / 2), waypoints.Length - 1);
                }

                // Add b to the list of endpoints for index a, and vice versa (bidirectional)
                edges[a].Add(b);
                edges[b].Add(a);
                Debug.Log("Adding " + a.ToString() + ", " + b.ToString());

                // Get the GameObjects at a and b
                GameObject startNode = waypoints[a];
                GameObject endNode = waypoints[b];

                // Initialize lineRenderer (https://docs.unity3d.com/ScriptReference/LineRenderer.SetPosition.html)
                GameObject newChild = new GameObject();
                newChild.transform.parent = startNode.transform;
                lineRenderer = newChild.AddComponent<LineRenderer>();
                lineRenderer.material = lineMaterial;
                lineRenderer.widthMultiplier = 0.2f;
                lineRenderer.positionCount = lineVertices;

                // Get a vector from startNode to endNode
                Vector3 curLine = endNode.transform.position - startNode.transform.position;

                // Render the line
                for (int j = 0; j < lineVertices; j++)
                {
                    // Set the position for each vertex
                    lineRenderer.SetPosition(j, startNode.transform.position + j * curLine);
                }
            }
        }

        // DEBUG: Print edge array
        for (int i = 0; i < waypoints.Length; i++)
        {
            Debug.Log("\n----Node " + i.ToString() + "----");
            foreach (int endp in edges[i])
            {
                Debug.Log(endp);
            }
        }
    }

    void GetWaypoints_Test()
    {
        // Variables for instantiating waypoints and determining heights above-ground
        GameObject curWaypoint;
        float heightBuff;

        // Set the start as a fixed point
        curWaypoint = Instantiate(waypoint);
        heightBuff = ground.SampleHeight(new Vector3(0f, 0f, 0f));
        curWaypoint.transform.position = new Vector3(0f, defaultY + heightBuff, 0f);
        waypoints[0] = curWaypoint;

        // Set the end as a fixed point
        curWaypoint = Instantiate(waypoint);
        heightBuff = ground.SampleHeight(new Vector3(0f, 0f, maxZ + 25f));
        curWaypoint.transform.position = new Vector3(0f, defaultY + heightBuff, maxZ);
        waypoints[waypoints.Length - 1] = curWaypoint;

        // Create array for intermediate nodes
        Vector3[] vecArray = new Vector3[7];
        vecArray[0] = new Vector3(-40f, 0f, 20f);
        vecArray[1] = new Vector3(0f, 0f, 45f);
        vecArray[2] = new Vector3(30f, 0f, 30f);
        vecArray[3] = new Vector3(70f, 0f, 20f);
        vecArray[4] = new Vector3(-20f, 0f, 60f);
        vecArray[5] = new Vector3(20f, 0f, 60f);
        vecArray[6] = new Vector3(40f, 0f, 60f);

        // Randomly select coordinates for 7 waypoints, within a range of coordinates
        for (int i = 1; i < waypoints.Length - 1; i++)
        {
            // Get the coordinates and the corresponding height
            //heightBuff = ground.SampleHeight(new Vector3(vecArray[i].x, 0f, vecArray[i].z));
            vecArray[i - 1].y = defaultY + ground.SampleHeight(new Vector3(vecArray[i - 1].x, 0f, vecArray[i - 1].z));

            // Create a clone of the waypoint object and assign the random coordinates as its location
            curWaypoint = Instantiate(waypoint);
            curWaypoint.transform.position = vecArray[i - 1];

            // Add curWaypoint to the array
            waypoints[i] = curWaypoint;
        }
    }

    void GetEdges_Test()
    {
        // Create the edges (represented unidirectionally here)
        edges[0].Add(1);
        edges[0].Add(2);
        edges[0].Add(3);
        edges[0].Add(4);
        edges[1].Add(0);
        edges[1].Add(5);
        edges[2].Add(0);
        edges[2].Add(5);
        edges[3].Add(0);
        edges[3].Add(4);
        edges[3].Add(7);
        edges[4].Add(0);
        edges[4].Add(7);
        edges[5].Add(1);
        edges[5].Add(2);
        edges[5].Add(6);
        edges[6].Add(5);
        edges[6].Add(8);
        edges[7].Add(3);
        edges[7].Add(4);
        edges[7].Add(8);
        edges[8].Add(6);
        edges[8].Add(7);

        // Create edges
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            foreach (int endp in edges[i])
            {
                // Get the GameObjects at a and b
                GameObject startNode = waypoints[i];
                GameObject endNode = waypoints[endp];

                // Initialize lineRenderer (https://docs.unity3d.com/ScriptReference/LineRenderer.SetPosition.html)
                GameObject newChild = new GameObject();
                newChild.transform.parent = startNode.transform;
                lineRenderer = newChild.AddComponent<LineRenderer>();
                lineRenderer.material = lineMaterial;
                lineRenderer.widthMultiplier = 0.2f;
                lineRenderer.positionCount = lineVertices;

                // Get a vector from startNode to endNode
                Vector3 curLine = endNode.transform.position - startNode.transform.position;

                // Render the line
                for (int j = 0; j < lineVertices; j++)
                {
                    // Set the position for each vertex
                    lineRenderer.SetPosition(j, startNode.transform.position + j * curLine);
                }
            }
        }

        // DEBUG: Print edge array
        for (int i = 0; i < waypoints.Length; i++)
        {
            Debug.Log("\n----Node " + i.ToString() + "----");
            foreach (int endp in edges[i])
            {
                Debug.Log(endp);
            }
        }
    }
}
