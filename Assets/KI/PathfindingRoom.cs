using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingRoom 
{
    private const int MOVE_STRAIGHT_COST=10;
    private const int MOVE_DIAGONAL_COST=14;
    private Grid<PathNode>grid;
    private List<PathNode> openList;
    private List<PathNode> closedList;
    public PathfindingRoom( Grid<bool> roomGrid)
    {
        grid=new Grid<PathNode>(roomGrid.widht, roomGrid.height, roomGrid.cellSize, roomGrid.originPosition);
        for(int x=0;x<grid.widht;x++)
        {
            for(int y=0;y<grid.height;y++)
            {
                grid.setValue(x,y,new PathNode(grid,x,y, roomGrid.getValue(x,y)));
            }
        }
    }
    public List<PathNode> FindPath(Vector3 startPoint, Vector3 endPoint)
    {
        int[] startGrid=grid.getXY(startPoint);
        int[] endGrid=grid.getXY(endPoint);
        return FindPath(startGrid[0], startGrid[1], endGrid[0], endGrid[1]);
    }
    public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        PathNode startPoint=grid.getValue(startX,startY);
        PathNode endPoint=grid.getValue(endX, endY);
        openList=new List<PathNode>(){startPoint};
        closedList=new List<PathNode>();
        for(int x=0;x<grid.widht;x++)
        {
            for(int y=0;y<grid.height;y++)
            {
                PathNode pathNode=grid.getValue(x,y);
                pathNode.gCost=int.MaxValue;
                pathNode.hCost=CalculateDistance(pathNode,endPoint);
                pathNode.calculateFCost();
                pathNode.cameFromNote=null;
            }
        }
        startPoint.hCost=0;
        startPoint.gCost=CalculateDistance(startPoint, endPoint);
        startPoint.calculateFCost();
        while(openList.Count>0)
        {
            PathNode currentNode=GetLowestFCostNode(openList);
            if(currentNode==endPoint)
            {
                //way found
                return CalculatePath(endPoint);
            }
            openList.Remove(currentNode);
            closedList.Add(currentNode);
            foreach (PathNode neighborNode in GetNeighborList(currentNode))
            {
                if(closedList.Contains(neighborNode))continue;
                if(!neighborNode.isWalkable){
                    closedList.Add(neighborNode);
                    continue;
                }
                int tentativeGCost=currentNode.gCost+CalculateDistance(currentNode, neighborNode);
                if(tentativeGCost<neighborNode.gCost)
                {
                    neighborNode.gCost=tentativeGCost;
                    neighborNode.cameFromNote=currentNode;
                    neighborNode.calculateFCost();
                    if(!openList.Contains(neighborNode))
                    {
                        openList.Add(neighborNode);
                    }
                }
            }
        }
        // no way found
        return null;
    }
    private int CalculateDistance(PathNode a, PathNode b)
    {
        int xDistance=Mathf.Abs(a.x-b.y);
        int yDistance=Mathf.Abs(a.y-b.y);
        int remaining=Mathf.Abs(xDistance-yDistance);
        return MOVE_DIAGONAL_COST*Mathf.Min(xDistance,yDistance)+MOVE_STRAIGHT_COST*remaining;
    }
    private PathNode GetLowestFCostNode(List<PathNode> pathNodes)
    {
        PathNode result=pathNodes[0];
        for(int i=1;i<pathNodes.Count;i++)
        {
            if(pathNodes[i].fCost<result.fCost)
            {
                result=pathNodes[i];
            }
        }
        return result;
    }
    private List<PathNode>CalculatePath(PathNode endPoint)
    {
        PathNode currentNode=endPoint;
        List<PathNode>result=new List<PathNode>(){endPoint};
        while(currentNode.cameFromNote!=null)
        {
            currentNode=currentNode.cameFromNote;
            result.Add(currentNode);
        }
        result.Reverse();//dreht liste um
        return result;
    }
    private List<PathNode> GetNeighborList(PathNode currentNode)
    {
        List<PathNode> neighborList=new List<PathNode>();
        if(currentNode.x-1>=0)
        {
            neighborList.Add(GetNode(currentNode.x-1, currentNode.y));
            if(currentNode.y-1>=0)neighborList.Add(GetNode(currentNode.x-1, currentNode.y-1));
            if(currentNode.y+1>=0)neighborList.Add(GetNode(currentNode.x-1, currentNode.y+1));
        }
        if(currentNode.x+1<grid.widht)
        {
            neighborList.Add(GetNode(currentNode.x+1, currentNode.y));
            if(currentNode.y-1>=0)neighborList.Add(GetNode(currentNode.x+1, currentNode.y-1));
            if(currentNode.y+1>=0)neighborList.Add(GetNode(currentNode.x+1, currentNode.y+1));
        
        }
        if(currentNode.y-1>=0)neighborList.Add(GetNode(currentNode.x, currentNode.y-1));
        if(currentNode.y+1>=0)neighborList.Add(GetNode(currentNode.x, currentNode.y+1));
        return neighborList;
    }
    private PathNode GetNode(int x, int y)
    {
        return grid.getValue(x,y);
    }
}
