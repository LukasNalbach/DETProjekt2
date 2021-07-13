using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    public LinkedList<Vector3> calculatedPoints;//points from the actuall Room grid calculated with A* or next checkpoint
    public LinkedList<PathPoint> nextPoints;//checkpoints and goal, way to this point is not jet calculated with A*
    public GameObject myPlayer;
    public Pathfinding(GameObject player)
    {
        myPlayer=player;
    }
    public void calculateNextGoalForTasks()
    {
        //nextPoints=dijstra(myPlayer.GetComponent<CrewMate>().taskToDo);//needs graph
        calculateNextPoints();
    }
    public void calculateNextPoints()
    {
        /*
        PathPoint nextGoal=nextPoints.First.Value;
        nextPoints.RemoveFirst();
        if(nextGoal.checkpoint)
        {
            //?
            calculatedPoints.AddLast(nextGoal.position);
        }
        else
        {
            Vector3 start=myPlayer.transform.position;
            Room nextRoom=nextGoal.nextRoom;
            PathfindingRoom pathfindingRoom=new PathfindingRoom(nextRoom.roomGrid);
            List<PathNode> pathToGoal=pathfindingRoom.FindPath(start, nextGoal.position);
            foreach(PathNode pn in pathToGoal)
            {
                calculatedPoints.AddLast(nextRoom.roomGrid.getWorldPosition(pn.x,pn.y));
            }
        }*/
    }
    public bool hasNextPosition()
    {
        return calculatedPoints.Count>0||nextPoints.Count>0;
    }
    public Vector3 getNextPosition()
    {
        if(calculatedPoints.Count>0)
        {
            return calculatedPoints.First.Value;
        }
        if(nextPoints.Count>0)
        {
            calculateNextPoints();
            return calculatedPoints.First.Value;         
        }
        throw new System.NotSupportedException("First check with hasNextPosition!");
    }
}
