using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode 
{
    private Grid<PathNode> grid;
    public int x;
    public int y;
    public int gCost;//cost from startPoint
    public int hCost;//cost to endPoint without hindernisse
    public int fCost;//gCost+hCost
    public bool isWalkable;
    public PathNode cameFromNote;
    public PathNode(Grid<PathNode> grid, int x, int y, bool isWalkable)
    {
        this.grid=grid;
        this.x=x;
        this.y=y;
        this.isWalkable=isWalkable;
    }
    public void calculateFCost()
    {
        fCost=gCost+hCost;
    }
}
