using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    public int x;
    public int y;
    public int gCost;
    public int hCost;
    public int fCost;
    public PathNode cameFromNode;

    public PathNode(int _x, int _y)
    {
        this.x = _x;
        this.y = _y;
    }

    public void calculateFCost()
    {
        fCost = gCost + hCost;
    }
}

public class PathFinding
{
    List<List<bool>> obstaclesGrid;
    List<List<PathNode>> grid;
    private List<PathNode> openList;
    private List<PathNode> closeList;
    private int mapWidth;
    private int mapLength;

    public PathFinding(int _mapWidth, int _mapLength)
    {
        this.mapWidth = _mapWidth;
        this.mapLength = _mapLength;
        initGrid();
    }

    public List<PathNode> FindPath(List<List<bool>> _obstaclesGrid, int startX, int startY, int endX, int endY)
    {
        this.obstaclesGrid = _obstaclesGrid;
        PathNode startNode = grid[startY][startX];
        PathNode endNode = grid[endY][endX];

        openList = new List<PathNode> { startNode };
        closeList = new List<PathNode>();

        startNode.gCost = 0;
        startNode.hCost = calculateDistance(startNode, endNode);
        startNode.calculateFCost();

        while (openList.Count > 0) {
            PathNode currentNode = getLowestFCostNode(openList);
            if (currentNode == endNode)
                return CalculatePath(endNode);
            openList.Remove(currentNode);
            closeList.Add(currentNode);

            foreach (PathNode neighbourNode in getNeighboursList(currentNode)) {
                if (closeList.Contains(neighbourNode) == false ) {
                    int tentativeGCost = currentNode.gCost + calculateDistance(currentNode, neighbourNode);
                    if (tentativeGCost < neighbourNode.gCost || neighbourNode.gCost == 0) {
                        neighbourNode.cameFromNode = currentNode;
                        neighbourNode.gCost = tentativeGCost;
                        neighbourNode.hCost = calculateDistance(neighbourNode, endNode);
                        neighbourNode.calculateFCost();

                        if (openList.Contains(neighbourNode) == false) {
                            openList.Add(neighbourNode);
                        }
                    }
                }
            }
        }
        return (null);
    }

    private void initGrid()
    {
        grid = new List<List<PathNode>>();
        for (int y = 0; y < mapLength; y++) {
            grid.Add(new List<PathNode>());
            for (int x = 0; x < mapWidth; x++) {
                grid[y].Add(new PathNode(x, y));
                grid[y][x].gCost = int.MaxValue;
                grid[y][x].calculateFCost();
                grid[y][x].cameFromNode = null;
            }
        }
    }

    private List<PathNode> getNeighboursList(PathNode node)
    {
        List<PathNode> neighboursList = new List<PathNode>();

        if (node.x - 1 >= 0 && obstaclesGrid[node.y][node.x - 1] == false)
            neighboursList.Add(grid[node.y][node.x - 1]);
        if (node.x + 1 < mapWidth && obstaclesGrid[node.y][node.x + 1] == false)
            neighboursList.Add(grid[node.y][node.x + 1]);
        if (node.y - 1 >= 0 && obstaclesGrid[node.y - 1][node.x] == false)
            neighboursList.Add(grid[node.y - 1][node.x]);
        if (node.y + 1 < mapLength && obstaclesGrid[node.y + 1][node.x] == false)
            neighboursList.Add(grid[node.y + 1][node.x]);
        return (neighboursList);
    }

    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        PathNode currentNode = endNode;

        path.Add(endNode);
        while (currentNode.cameFromNode != null) {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        return path;
    }

    private int calculateDistance(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        
        return (xDistance + yDistance);
    }

    private PathNode getLowestFCostNode(List<PathNode> pathNodeList)
    {
        PathNode LowestFCostNode = pathNodeList[0];

        for (int i = 0; i < pathNodeList.Count; i++) {
            if (pathNodeList[i].fCost < LowestFCostNode.fCost)
                LowestFCostNode = pathNodeList[i];
        }
        return (LowestFCostNode);
    }
}
