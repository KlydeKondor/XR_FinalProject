using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRoverAStar : MonoBehaviour
{
    // PRIVATE DATA MEMBERS
    private bool autoMove = true; // Set to false if user should decide when to show the path traversal
    private int index = 0;
    private GameObject curNode;

    // Struct to facilitate the priority queue
    private struct NodeCost
    {
        // Public constructor (only visible within this class due to the outer private designation
        public NodeCost(int node, float cost)
        {
            Node = node;
            Cost = cost;
        }

        public int Node { get; }
        public float Cost { get; }
    }
    
    // Data structures for waypoints/edges
    private GameObject[] waypoints;
    private List<int>[] edges;
    private List<int> optimalPath = new List<int>();
    
    // Arrays for actual and heuristic costs and the path taken
    private float[] gFunc;
    private float[] hFunc;
    private int[] origins;


    // PUBLIC ACCESSORS
    public GameObject[] Waypoints
    {
        get { return waypoints; }
        set { waypoints = value; }
    }
    
    public List<int>[] Edges
    {
        get { return edges; }
        set { edges = value; }
    }

    public bool AutoMove
    {
        get { return autoMove; }
        set { autoMove = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Define gFunc (path cost) and initialize hFunc (array of heuristic costs)
        gFunc = new float[waypoints.Length];
        GetActualCosts();
        hFunc = new float[waypoints.Length];
        GetHeuristicCosts();

        // Define origin (array of indices showing the path taken)
        origins = new int[waypoints.Length];

        // Call the A* algorithm
        AStar();
    }

    // Update is called once per frame
    void Update()
    {
        // If Shift + A + * is pressed, set autoMove to true (currently not being used; button generates map and moves rover)
        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.Alpha8))
        {
            autoMove = true;
        }

        // If the path produced by A* should be traversed, move the SEV
        if (autoMove && index < optimalPath.Count)
        {
            int curNodeIndex = optimalPath[index];
            curNode = waypoints[curNodeIndex];
            this.transform.position = Vector3.MoveTowards(this.transform.position, curNode.transform.position, 0.5f);
        }
    }
    
    bool AStar()
    {
        // Instantiate a priority queue for tracking the nodes to explore and a stack for tracking explored nodes
        List<NodeCost> priorityQueue = new List<NodeCost>();

        // Instantiate NodeCost struct curNode initialized to the first node and a cost of zero; add to queue
        priorityQueue.Add(new NodeCost(0, 0f));
        gFunc[0] = 0f;

        // Define variables for checking the actual and heuristic costs of a node
        float gCost;
        float hCost;

        // Remember the previous index
        int prev = -1;
        origins[0] = prev;

        // Until there are no more nodes in the queue
        while (priorityQueue.Count > 0)
        {
            // Assign to curNode and dequeue
            NodeCost curNode = priorityQueue[0];
            priorityQueue.Remove(curNode);

            // Check if this is the destination node
            if (curNode.Node == waypoints.Length - 1)
            {
                Debug.Log("------FINISHED------");
                
                // Create an output list and add the final node to it
                int pathElem = curNode.Node;
                optimalPath.Add(pathElem);

                // Traverse backwards and append each predecessor node to the front of finalPath
                while (origins[pathElem] != -1)
                {
                    pathElem = origins[pathElem];
                    optimalPath.Insert(0, pathElem);
                }

                // DEBUG: Print the final path
                foreach (int i in optimalPath)
                {
                    Debug.Log(i);
                }

                // Return true on success
                return true;
            }

            // Expand curNode and determine the total cost for each neighbor; place in priorityQueue accordingly
            foreach (int endp in edges[curNode.Node])
            {
                // Get the actual cost (i.e. g(n), which is along the path being explored)
                gCost = gFunc[curNode.Node] + Vector3.Distance(waypoints[curNode.Node].transform.position,
                    waypoints[endp].transform.position);

                // Get the heuristic cost (i.e. h(n), which is the straight-line distance to the end)
                hCost = hFunc[endp];

                // If cost is less than the previous gFunc for endp, update
                if (gCost < gFunc[endp])
                {
                    // Set curNode as this node's origin
                    origins[endp] = curNode.Node;
                    
                    // Set gFunc[endp] to the updated path cost
                    gFunc[endp] = gCost;

                    // Temporarily remove the endp node from the queue
                    UpdateQueue(endp, ref priorityQueue);

                    // Create a new NodeCost struct and add to the priority queue
                    NodeCost neighborNode = new NodeCost(endp, gCost + hCost);
                    PriorityEnqueue(ref priorityQueue, ref neighborNode);
                }

                // Update prev
                prev = curNode.Node;
            }
        }

        // If the queue is consumed without reaching the end node, return null
        return false;
    }

    void GetActualCosts()
    {
        // Intialize the actual cost for each node to a large number
        for (int i = 0; i < gFunc.Length; i++)
        {
            gFunc[i] = 100000f;
        }
    }

    void GetHeuristicCosts()
    {
        // Get the end node
        GameObject endNode = waypoints[waypoints.Length - 1];
        
        // Use the straight-line distance from the current node to endNode as the heuristic cost
        for (int i = 0; i < hFunc.Length; i++)
        {
            // Set the heuristic cost at node i
            hFunc[i] = Vector3.Distance(waypoints[i].transform.position, endNode.transform.position);
        }
    }

    void PriorityEnqueue(ref List<NodeCost> q, ref NodeCost elem)
    {
        // Indexing variables
        int start = 0, end = q.Count - 1, med = (start + end) / 2;

        // Direction to insert (1 is right, -1 is left)
        int dir = 1;

        // Insert via binary search
        while (start <= end)
        {
            // Get the current median
            med = (start + end) / 2;

            // Check if the element to be inserted is of lower or greater cost than the current median element
            if (elem.Cost > q[med].Cost)
            {
                // Priority of elem is lower than the element at med; search the current right half
                start = med + 1;
                dir = 1;
            }
            else if (elem.Cost < q[med].Cost)
            {
                // Priority of elem is higher than the element at med; search the current left half
                end = med - 1;
                dir = -1;
            }
            else
            {
                // Priority is the same as the current median; insert to the right (give priority to existing element)
                dir = 1;
                break;
            }
        }

        // Insert elem left or right of med
        if (med + dir >= q.Count)
        {
            // Edge case; at end of list, so append normally
            q.Add(elem);
        }
        else if (dir > 0)
        {
            // Insert at med + dir (just right of med; will shift rest of list right)
            q.Insert(med + dir, elem);
        }
        else
        {
            // Insert at med (will shift med and rest of list right)
            q.Insert(med, elem);
        }
        
        // DEBUG: Print q
        foreach (NodeCost c in q)
        {
            Debug.Log(c.Cost);
        }
    }

    void UpdateQueue(int index, ref List<NodeCost> q)
    {
        // Remove the old element
        for (int i = 0; i < q.Count; i++)
        {
            // Revisiting a node would be suboptimal by definition; Node values are unique
            if (q[i].Node == index)
            {
                // Remove the element and exit
                q.Remove(q[i]);
                break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the auto-movement procedure is occurring and other is a waypoint, remove other
        if (autoMove && other.gameObject == curNode)
        {
            Destroy(other.gameObject);
            index++;
        }
    }

    void TestEnqueue()
    {
        List<NodeCost> nodes = new List<NodeCost>();
        NodeCost nodeCost = new NodeCost(0, 10f);
        PriorityEnqueue(ref nodes, ref nodeCost);
        nodeCost = new NodeCost(0, 10f);
        PriorityEnqueue(ref nodes, ref nodeCost);
        nodeCost = new NodeCost(0, 20f);
        PriorityEnqueue(ref nodes, ref nodeCost);
        nodeCost = new NodeCost(0, 5f);
        PriorityEnqueue(ref nodes, ref nodeCost);
        nodeCost = new NodeCost(0, 7f);
        PriorityEnqueue(ref nodes, ref nodeCost);
    }
}
