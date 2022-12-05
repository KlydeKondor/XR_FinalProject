using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMap : MonoBehaviour
{
    // PUBLIC DATA MEMBERS
    public GameObject waypoint;
    public GameObject edge;
    public Terrain ground;
    public Material lineMaterial;

    // PRIVATE DATA MEMBERS
    // Bounds for waypoints/paths
    private const int NUM_WAYPOINTS = 14; // Includes the start and end points
    private int numPaths = 3; // Number of paths from start to end (may overlap)

    // Bounds for coordinates
    private float minX = -100f, maxX = 100f, minZ = 25f, maxZ = 225f, defaultY = -20f;

    // Data structures for waypoints/edges
    private GameObject[] waypoints = new GameObject[NUM_WAYPOINTS];
    //private Dictionary<int, int> edges = new Dictionary<int, int>();
    private List<int>[] edges = new List<int>[NUM_WAYPOINTS];

    // LineRenderer object for drawing edges
    private LineRenderer lineRenderer;
    private int lineVertices = 2;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the lists in edges
        for (int i = 0; i < NUM_WAYPOINTS; i++)
        {
            edges[i] = new List<int>();
        }
        
        // Get randomized waypoints and edges
        GetWaypoints();
        GetEdges();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GetWaypoints()
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
        waypoints[NUM_WAYPOINTS - 1] = curWaypoint;

        // Randomly select coordinates for 12 waypoints, within a range of coordinates
        for (int i = 1; i < NUM_WAYPOINTS - 1; i++)
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

    void GetEdges()
    {
        // Create numPaths routes from the start node to the end node, visiting waypoints semi-randomly
        for (int i = 0; i < numPaths; i++)
        {
            // Create indices for start/end waypoints
            int a;
            int b = 0;

            // Loop until the end node is reached
            while (b < NUM_WAYPOINTS - 1)
            {
                // Set the current start index to the previous end index
                a = b;

                // Get random indices for the edge's end waypoint
                while (a == b)
                {
                    // Minimum is a to prevent backtracking; if the random index is out of bounds, go to the end node
                    b = Mathf.Min(Random.Range(a, a + NUM_WAYPOINTS / 2), NUM_WAYPOINTS - 1);
                }

                // Add b to the list of endpoints for index a
                edges[a].Add(b);
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
        for (int i = 0; i < NUM_WAYPOINTS; i++)
        {
            Debug.Log("\n----Node " + i.ToString() + "----");
            foreach (int endp in edges[i])
            {
                Debug.Log(endp);
            }
        }
    }
}
