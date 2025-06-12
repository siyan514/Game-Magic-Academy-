using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A* Pathfinding algorithm implementation class
/// </summary>
public static class AStarPathfinding
{
    /// <summary>
    /// Pathfinding node class
    /// </summary>
    private class PathNode
    {
        public Vector2Int position;     // node position
        public float gCost;            // The actual cost from the starting point to the current node
        public float hCost;            // The heuristic cost from the current node to the destination
        public float fCost => gCost + hCost;  // total cost
        public PathNode parent;        // The parent node is used to reconstruct the path
        public PathNode(Vector2Int pos)
        {
            position = pos;
            gCost = 0;
            hCost = 0;
            parent = null;
        }
    }

    /// <summary>
    /// Use the A* algorithm to find the optimal path from the starting point to the destination
    /// </summary>
    /// <param name="startPos">initial position</param>
    /// <param name="targetPos">target positon</param>
    /// <returns>List of path points. Return null if no path is found</returns>
    public static List<Vector2Int> FindPath(Vector2Int startPos, Vector2Int targetPos)
    {
        // Open list: Nodes to be evaluated
        List<PathNode> openList = new List<PathNode>();
        // Close list: Evaluated nodes
        HashSet<Vector2Int> closedList = new HashSet<Vector2Int>();

        // Create the starting node
        PathNode startNode = new PathNode(startPos);
        startNode.gCost = 0;
        startNode.hCost = CalculateHeuristic(startPos, targetPos);
        openList.Add(startNode);

        // Main loop: Keep searching until a path is found or the open list is empty
        while (openList.Count > 0)
        {
            // Select the node with the lowest F cost from the open list
            PathNode currentNode = GetLowestFCostNode(openList);

            // Move the current node from the open list to the closed list
            openList.Remove(currentNode);
            closedList.Add(currentNode.position);

            // If the target position is reached, rebuild and return the path
            if (currentNode.position == targetPos)
            {
                return ReconstructPath(currentNode);
            }

            // Check all the neighbors of the current node
            foreach (Vector2Int neighborPos in GetNeighbors(currentNode.position))
            {
                // Skip the neighbors that are already in the close list
                if (closedList.Contains(neighborPos))
                    continue;

                // Jump over impassable neighbors (walls, etc.)
                if (!IsWalkable(neighborPos))
                    continue;

                // Calculate the new G cost to the neighboring nodes
                float tentativeGCost = currentNode.gCost + GetMovementCost(currentNode.position, neighborPos);

                // Find out if the neighbor is already in the open list
                PathNode neighborNode = openList.FirstOrDefault(n => n.position == neighborPos);

                if (neighborNode == null)
                {
                    // The neighbor is not in the open list. Create a new node
                    neighborNode = new PathNode(neighborPos);
                    neighborNode.gCost = tentativeGCost;
                    neighborNode.hCost = CalculateHeuristic(neighborPos, targetPos);
                    neighborNode.parent = currentNode;
                    openList.Add(neighborNode);
                }
                else if (tentativeGCost < neighborNode.gCost)
                {
                    // Find the shorter path to the neighbor and update the neighbor node
                    neighborNode.gCost = tentativeGCost;
                    neighborNode.parent = currentNode;
                }
            }
        }

        // The path was not found.
        return null;
    }

    /// <summary>
    /// Obtain the node with the minimum cost of F from the open list
    /// </summary>
    private static PathNode GetLowestFCostNode(List<PathNode> openList)
    {
        PathNode lowestNode = openList[0];
        for (int i = 1; i < openList.Count; i++)
        {
            if (openList[i].fCost < lowestNode.fCost)
            {
                lowestNode = openList[i];
            }
        }
        return lowestNode;
    }

    /// <summary>
    /// Calculate the heuristic cost (Manhattan distance)
    /// </summary>
    private static float CalculateHeuristic(Vector2Int posA, Vector2Int posB)
    {
        return Mathf.Abs(posA.x - posB.x) + Mathf.Abs(posA.y - posB.y);
    }

    /// <summary>
    /// Obtain the locations of all neighbors at the specified location (in the four directions of up, down, left and right)
    /// </summary>
    private static Vector2Int[] GetNeighbors(Vector2Int position)
    {
        return new Vector2Int[]
        {
            position + Vector2Int.up,     
            position + Vector2Int.down,    
            position + Vector2Int.left,   
            position + Vector2Int.right   
        };
    }

    /// <summary>
    /// Check whether the designated position is passable
    /// </summary>
    private static bool IsWalkable(Vector2Int position)
    {
        Vector2 worldPos = new Vector2(position.x, position.y);

        // Check for any wall colliders
        Collider2D wallCollider = Physics2D.OverlapPoint(worldPos, LayerMask.GetMask("wall"));
        if (wallCollider != null)
            return false;

        // Inspect the super wall
        if (GameController.instance != null && GameController.instance.IsSuperWall(worldPos))
            return false;

        return true;
    }

    /// <summary>
    /// Calculate the cost of moving from one location to the neighboring location
    /// </summary>
    private static float GetMovementCost(Vector2Int from, Vector2Int to)
    {
        // In the grid game, the cost of moving adjacent grids is 1
        return 1f;
    }

    /// <summary>
    /// Reconstruct the complete path from the end node
    /// </summary>
    private static List<Vector2Int> ReconstructPath(PathNode endNode)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        PathNode currentNode = endNode;

        // Trace back from the end point to the beginning point and construct the path
        while (currentNode != null)
        {
            path.Add(currentNode.position);
            currentNode = currentNode.parent;
        }

        // Reverse the path to make it go from the starting point to the end point
        path.Reverse();
        return path;
    }
}